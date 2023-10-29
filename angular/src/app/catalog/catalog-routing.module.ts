import { RouterModule, Routes } from "@angular/router";
import { ProductComponent } from "./product/product.component";
import { AttributeComponent } from "./attribute/attribute.component";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {path: 'product', component: ProductComponent},
    {path: 'attribute', component: AttributeComponent}
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class CatalogRoutingModule {}