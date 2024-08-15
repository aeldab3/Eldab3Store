import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderDetailedComponent } from './order-detailed/order-detailed.component';
import { SharedModule } from "../shared/shared.module";
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';



@NgModule({
  declarations: [
    OrderDetailedComponent,
    OrdersComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    OrdersRoutingModule
]
})
export class OrdersModule { }
