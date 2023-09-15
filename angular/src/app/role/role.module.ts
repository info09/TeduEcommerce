import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { RoleRoutingModule } from './role-routing.module';
import { RoleComponent } from './role.component';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { RoleDetailComponent } from './role-detail.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { EditorModule } from 'primeng/editor';
import { TeduSharedModule } from '../shared/modules/tedu-shared.module';
import { BadgeModule } from 'primeng/badge';
import { ImageModule } from 'primeng/image';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
@NgModule({
  declarations: [RoleComponent, RoleDetailComponent],
  imports: [
    SharedModule,
    RoleRoutingModule,
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    DropdownModule,
    InputTextModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    InputNumberModule,
    CheckboxModule,
    InputTextareaModule,
    EditorModule,
    TeduSharedModule,
    BadgeModule,
    ImageModule,
    ConfirmDialogModule
  ],
  entryComponents: [
    RoleDetailComponent
  ]
})
export class RoleModule { }