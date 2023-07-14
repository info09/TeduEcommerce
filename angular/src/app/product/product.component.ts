import { AuthService, PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/product-categories';
import { ProductInListDto, ProductService } from '@proxy/products';
import { OAuthService } from 'angular-oauth2-oidc';
import { Subject, takeUntil } from 'rxjs';

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

  constructor(private productService: ProductService, private productCategoryService: ProductCategoriesService) { }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.loadProductCategories();
    this.loadData();
  }

  loadData() {
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
        },
        error: () => { },
      });
  }

  loadProductCategories(){
    this.productCategoryService.getListAll()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((response: ProductCategoryInListDto[]) => {
        response.forEach(element => {
          this.productCategories.push({
            value: element.id,
            name: element.name
          })
        })
      });
  }

  pageChange(event: any): void {
    this.skipCount = (event - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }
}
