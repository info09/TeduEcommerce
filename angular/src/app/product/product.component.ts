import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/product-categories';
import { ProductDto, ProductInListDto, ProductService } from '@proxy/products';
import { Subject, takeUntil } from 'rxjs';
import { NotificationService } from '../shared/services/notification.service';
import { DialogService } from 'primeng/dynamicdialog';
import {ProductDetailComponent  } from './product-detail.component';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ProductInListDto[] = [];

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
              private notificationSevice: NotificationService, 
              private dialogService: DialogService) { }
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
        response.forEach(element => {
          this.productCategories.push({
            value: element.id,
            label: element.name
          })
        })
      });
  }

  pageChange(event: any): void {
    this.skipCount = (event - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(ProductDetailComponent, {
      header: 'Thêm mới sản phẩm',
      width: '70%'
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationSevice.showSuccess('Thêm sản phẩm thành công');
      }
    })
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
