import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/product-categories';
import { ProductDto, ProductInListDto, ProductService } from '@proxy/products';
import { Subject, takeUntil } from 'rxjs';
import { NotificationService } from '../shared/services/notification.service';
import { DialogService } from 'primeng/dynamicdialog';
import {ProductDetailComponent  } from './product-detail.component';
import { ProductType } from '@proxy/tedu-ecommerce/products';
import { ConfirmationService } from 'primeng/api';
import { ProductAttributeComponent } from './product-attribute.component';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ProductInListDto[] = [];
  public selectedItems: ProductInListDto[] = [];

  //Paging variable
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;

  //Filter
  productCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  constructor(private productService: ProductService, 
              private productCategoryService: ProductCategoriesService, 
              private notificationService: NotificationService, 
              private dialogService: DialogService,
              private confirmationService: ConfirmationService) { }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.loadProductCategories();
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.productService
      .getListFilter({
        keyword: this.keyword,
        categoryId: this.categoryId,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<ProductInListDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadProductCategories() {
    this.productCategoryService.getListAll().subscribe((response: ProductCategoryInListDto[]) => {
      this.productCategories.push({
        value: '',
        label: 'Chọn danh mục'
      });
        response.forEach(element => {
          this.productCategories.push({
            value: element.id,
            label: element.name
          })
        })
      });
  }

  pageChange(event: any): void {
    this.skipCount = (event.pageCount - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  manageProductAttribute(id:string){
    const ref = this.dialogService.open(ProductAttributeComponent, {
      data: {
        id: id
      },
      header: 'Quản lý thuộc tính sản phẩm',
      width: '70%',
      height: '50%'
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.notificationService.showSuccess('Cập nhật thuộc tính sản phẩm thành công');
      }
    });
  }

  showAddModal() {
    const ref = this.dialogService.open(ProductDetailComponent, {
      header: 'Thêm mới sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Thêm sản phẩm thành công');
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
    const ref = this.dialogService.open(ProductDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập nhật sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.notificationService.showSuccess('Cập nhật sản phẩm thành công');
      }
    });
  }

  deleteItems(){
    if(this.selectedItems.length == 0){
      this.notificationService.showError('Phải chọn ít nhất 1 bản ghi');
      return;
    }
    var ids = [];
    this.selectedItems.forEach(items => {
      ids.push(items.id);
    });
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa bản ghi này?',
      accept: ()=> {
        this.deleteItemsConfirm(ids);
      }
    })
  }

  deleteItemsConfirm(ids: string[]){
    this.toggleBlockUI(true);
    this.productService.deleteMutiple(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: ()=> {
        this.notificationService.showSuccess('Xóa thành công');
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
