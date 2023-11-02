import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { NotificationService } from '../../shared/services/notification.service';
import { DialogService } from 'primeng/dynamicdialog';
import { ProductType } from '@proxy/tedu-ecommerce/products';
import { ConfirmationService } from 'primeng/api';
import { MessageConstants } from '../../shared/constants/messages.constant';
import { RoleDto, RoleInListDto, RoleService } from '@proxy/system/roles';
import { UserDetailComponent } from './user-detail.component';
import { UserDto, UserInListDto, UserService } from '@proxy/system/users';
import { RoleAssignComponent } from './role-assign.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
})
export class UserComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  

  //Paging variable
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;

  //Filter
  items: UserInListDto[] = [];
  public selectedItems: UserInListDto[] = [];
  keyword: string = '';

  constructor(private roleService: RoleService,
              private userService: UserService,
              private notificationService: NotificationService, 
              private dialogService: DialogService,
              private confirmationService: ConfirmationService) { }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(selectionId = null) {
    this.toggleBlockUI(true);
    this.userService
      .getListWithFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<UserInListDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;

          if (selectionId != null && this.items.length > 0) {
            this.selectedItems = this.items.filter(x => x.id == selectionId);
          }

          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  pageChange(event: any): void {
    this.skipCount = (event.pageCount - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(UserDetailComponent, {
      header: 'Thêm mới quyền',
      width: '70%',
    });

    ref.onClose.subscribe((data: RoleDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Thêm quyền thành công');
        this.selectedItems = [];
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError('Bạn phải chọn một bản ghi');
      return;
    }
    const id = this.selectedItems[0].id;
    const ref = this.dialogService.open(UserDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập nhật quyền',
      width: '70%',
    });

    ref.onClose.subscribe((data: RoleDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.notificationService.showSuccess('Cập nhật quyền thành công');
      }
    });
  }

  assignRole(id: string){
    const ref = this.dialogService.open(RoleAssignComponent, {
      data: {
        id: id
      },
      header: 'Gán quyền',
      width: '70%'
    });

    ref.onClose.subscribe((result: boolean) => {
      if(result){
        this.notificationService.showSuccess(MessageConstants.ROLE_ASSIGN_SUCCESS_MSG);
        this.loadData();
      }
    })
  }

  setPassword(id: string){
    const ref = this.dialogService.open(SetPasswordComponent, {
      data: {
        id: id
      },
      header: 'Đặt lại mật khẩu',
      with: '70%'
    });

    ref.onClose.subscribe((result: boolean) => {
      if(result){
        this.notificationService.showSuccess(MessageConstants.CHANGE_PASSWORD_SUCCCESS_MSG);
        this.loadData();
      }
    })
  }

  deleteItems(){
    if(this.selectedItems.length == 0){
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var ids = [];
    this.selectedItems.forEach(items => {
      ids.push(items.id);
    });
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: ()=> {
        this.deleteItemsConfirm(ids);
      }
    })
  }

  deleteItemsConfirm(ids: string[]){
    this.toggleBlockUI(true);
    this.userService.deleteMultiple(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: ()=> {
        this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
        this.loadData();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: ()=> {
        this.toggleBlockUI(false);
      }
    })
  }

  getProductTypeName(value: number){
    return ProductType[value];
  }

  private toggleBlockUI(enable: boolean) {
    if (enable == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
