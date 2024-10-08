import { Component, OnInit } from '@angular/core';
import { OrdersService } from './orders.service';
import { IOrder } from '../shared/models/order';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  orders!: IOrder[];
  constructor(private orderService : OrdersService) { }

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders(){
  this.orderService.getOrdersForUser().subscribe({
    next: (orders: IOrder[]) => {
      this.orders = orders;
    },
    error: (error: any) => console.log(error)
  });
}

}
