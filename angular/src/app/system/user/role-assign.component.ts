import { Component, EventEmitter, OnDestroy, OnInit } from "@angular/core";
import { RoleDto, RoleService } from "@proxy/system/roles";
import { UserDto, UserService } from "@proxy/system/users";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { Subject, forkJoin, takeUntil } from "rxjs";

@Component({
    templateUrl: './role-assign.component.html'
})
export class RoleAssignComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject<void>();

    //Default
    public blockedPanelDetail: boolean = false;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public closeBtnName: string;
    public availableRoles: string[] = [];
    public seletedRoles: string[] = [];
    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private userService: UserService,
        private roleService: RoleService) {

    }

    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    ngOnInit(): void {
        var roles = this.roleService.getListAll();

        forkJoin({ roles }).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: any) => {
                var roles = res.roles as RoleDto[];
                roles.forEach(element => {
                    this.availableRoles.push(element.name);
                });
                this.loadDetail(this.config.data.id);
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })

        this.saveBtnName = 'Cập nhật';
        this.closeBtnName = 'Hủy';
    }

    loadDetail(id: any) {
        this.toggleBlockUI(true);
        this.userService.get(id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (response: UserDto) => {
                this.seletedRoles = response.roles;
                this.availableRoles = this.availableRoles.filter(i => !this.seletedRoles.includes(i));
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    saveChange() {
        this.toggleBlockUI(true);
        this.saveData();
    }

    private saveData() {
        this.userService.assignRoles(this.config.data.id, this.seletedRoles).pipe(takeUntil(this.ngUnsubscribe)).subscribe(() => {
            this.toggleBlockUI(false);
            this.ref.close();
        })
    }

    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.btnDisabled = true;
            this.blockedPanelDetail = true;
        } else {
            setTimeout(() => {
                this.btnDisabled = false;
                this.blockedPanelDetail = false;
            }, 1000);
        }
    }
}