import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { RoleService } from '@proxy/roles';
import {
  GetPermissionListResultDto,
  PermissionGrantInfoDto,
  PermissionGroupDto,
  UpdatePermissionDto,
  UpdatePermissionsDto,
} from '@proxy/volo/abp/permission-management';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';

@Component({
    templateUrl: 'permission-grant.component.html'
})
export class PermissionGrantComponent implements OnInit, OnDestroy{

    private ngUnsubcribe  =new Subject<void>();

    public blockedPanelDetail: boolean = false;
    public form: FormGroup;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public closeBtnName: string;
    public groups: PermissionGroupDto[] = [];
    public permissions: PermissionGrantInfoDto[] = [];
    public selectedPermissions: string[] = [];
    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(public ref: DynamicDialogRef,
                public config: DynamicDialogConfig,
                public roleService: RoleService,
                private fb: FormBuilder){}

    ngOnDestroy(): void {
        if(this.ref){
            this.ref.close();
        }
        this.ngUnsubcribe.next();
        this.ngUnsubcribe.complete();
    }

    ngOnInit(): void {
        this.buildForm();
        this.loadDetail(this.config.data.name, 'R');
        this.saveBtnName = 'Cập nhật';
        this.closeBtnName = 'Hủy';
    }

    loadDetail(providerKey: string, providerName: string){
        this.toggleBlockUI(true);
        this.roleService.getPermission(providerName, providerKey).pipe(takeUntil(this.ngUnsubcribe)).subscribe({
            next: (response: GetPermissionListResultDto) => {
                this.groups = response.groups;
                this.groups.forEach(element => {
                    element.permissions.forEach(permission => {
                        this.permissions.push(permission);
                    });
                });
                this.buildForm();
                this.toggleBlockUI(false);
            },
            error: ()=> {
                this.toggleBlockUI(false);
            }
        })
    }

    saveChange(){
        this.toggleBlockUI(true);
        this.saveData();
    }

    private saveData(){
        var permissions: UpdatePermissionDto[] = [];
        for (let index = 0; index < this.permissions.length; index++) {
            const isGranted = this.selectedPermissions.filter(i => i == this.permissions[index].name).length > 0;
            permissions.push({name: this.permissions[index].name, isGranted: isGranted});
        }
        var updateValues: UpdatePermissionsDto = {
            permissions: permissions
        };
        this.roleService.updatePermissions('R', this.config.data.name, updateValues).pipe(takeUntil(this.ngUnsubcribe)).subscribe(() => {
            this.toggleBlockUI(false);
            this.ref.close(this.form.value);
        })
    }

    buildForm(){
        this.form = this.fb.group({});
        for(let index = 0; index < this.groups.length; index++){
            const group = this.groups[index];
            for (let jIndex = 0; jIndex < group.permissions.length; jIndex++) {
                const permission = group.permissions[jIndex];
                if(permission.isGranted){
                    this.selectedPermissions.push(permission.name);
                }
            }
        }
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