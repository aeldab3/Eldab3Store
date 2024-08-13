import { IOrder } from './../../shared/models/order';
import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BasketService } from 'src/app/basket/basket.service';
import { CheckoutService } from '../checkout.service';
import { ToastrService } from 'ngx-toastr';
import { IBasket } from 'src/app/shared/models/basket';
import { NavigationExtras, Router } from '@angular/router';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements OnInit {
  @Input() checkoutForm!: FormGroup
  constructor(private basketService : BasketService, private checkoutService : CheckoutService, private toaster : ToastrService, private router: Router) { }

  ngOnInit(): void {
  }

  //This method handles the process of submitting an order
  submitOrder(){
    const basket = this.basketService.getCurrentBasketValue();
    if(basket) {
      const orderToCreate = this.getOrderToCreate(basket) // Prepare the order object using the basket data
      // Call the service to create the order, subscribing to the response
      this.checkoutService.createOrder(orderToCreate).subscribe({
        next : (order : IOrder) => {
          this.toaster.success('Order created successfully');
          this.basketService.deleteLocalBasket(basket.id);
          //It stores the order object in the state property. This will be passed as additional data when navigating to a different route.
          const navigationExtras : NavigationExtras = {state: order};
          this.router.navigate(['checkout/success'], navigationExtras);
          console.log(order);
        },
        error : (error) => {
          this.toaster.error(error.message);
          console.log(error);
        }
      });
    }
  }

  //This method generates an order object based on the current basket and the user's selections
  getOrderToCreate(basket: IBasket) {
    return {
      // Include the basket ID (unique identifier for the basket)
      basketId: basket.id,
      // Include the delivery method ID, converting it to a number
      deliveryMethodId: +this.checkoutForm.get('deliveryForm')?.get('deliveryMethod')?.value,
      shipToAddress: this.checkoutForm.get('addressForm')?.value
    }
  }
}
