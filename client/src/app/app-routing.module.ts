import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { TestErrorComponent } from './core/test-error/test-error.component';
import { ServerErrorComponent } from './core/server-error/server-error.component';
import { NotFoundComponent } from './core/not-found/not-found.component';

const routes: Routes = [
  {path: '', component: HomeComponent, data: {breadcrumb: 'Home'}},
  {path: 'test-error', component: TestErrorComponent, data: {breadcrumb: 'Test Error'}},
  {path: 'server-error', component: ServerErrorComponent, data: {breadcrumb: 'Server Error'}},
  {path: 'not-found', component: NotFoundComponent, data: {breadcrumb: 'Not Found'}},
  {path: 'shop', loadChildren: () => import('./shop/shop.module').then(mod => mod.ShopModule), data: {breadcrumb: 'Shop'}}, //It uses lazy loading to load the ShopModule only when the user navigates to the 'shop' path.
  {path: 'basket', loadChildren: () => import('./basket/basket.module').then(mod => mod.BasketModule), data: {breadcrumb: 'Basket'}},
  {path: 'checkout', loadChildren: () => import('./checkout/checkout.module').then(mod => mod.CheckoutModule), data: {breadcrumb: 'Checkout'}},
  {path: '**', redirectTo: 'Not-Found', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)], //The forRoot method is used to create a routing configuration that is shared across the entire application.
  exports: [RouterModule]
})
export class AppRoutingModule { }
