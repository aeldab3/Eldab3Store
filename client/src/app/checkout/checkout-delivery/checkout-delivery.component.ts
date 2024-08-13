import { Component, Input, OnInit } from '@angular/core';
import { CheckoutService } from '../checkout.service';
import { FormGroup } from '@angular/forms';
import { IDeliveryMethod } from 'src/app/shared/models/deliveryMethod';
import { BasketService } from 'src/app/basket/basket.service';

@Component({
  selector: 'app-checkout-delivery',
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss']
})
export class CheckoutDeliveryComponent implements OnInit {
  @Input() checkoutForm!: FormGroup
  deliveryMethods!: IDeliveryMethod [];
  constructor(private checkoutService : CheckoutService, private basketSerive : BasketService) { }

  ngOnInit(): void {
    this.checkoutService.getDeliveryMethod().subscribe({
      next : (dm : IDeliveryMethod[]) => {
        this.deliveryMethods = dm;
      },
      error : (error) => {
        console.log(error);
      }
    });
  }

  setShippingPrice(deliveryMethod : IDeliveryMethod){
    this.basketSerive.setShippingPrice(deliveryMethod);
  }

}
