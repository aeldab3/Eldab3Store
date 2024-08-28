import { IOrder } from './../../shared/models/order';
import { AfterViewInit, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BasketService } from 'src/app/basket/basket.service';
import { CheckoutService } from '../checkout.service';
import { ToastrService } from 'ngx-toastr';
import { IBasket } from 'src/app/shared/models/basket';
import { NavigationExtras, Router } from '@angular/router';
import { lastValueFrom } from 'rxjs';



// Declare Stripe as a global variable to avoid TypeScript errors since Stripe.js is loaded from an external source.
declare var Stripe: any;
@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements AfterViewInit, OnDestroy {

  // The checkout form is passed as an input to this component.
  @Input() checkoutForm!: FormGroup;

  // ViewChild is used to get references to the HTML elements where the Stripe card inputs will be mounted.
  // The {static: true} option indicates that the view queries are resolved before change detection runs.
  @ViewChild('cardNumber', {static: true}) cardNumberElement?: ElementRef;
  @ViewChild('cardExpiry', {static: true}) cardExpiryElement?: ElementRef;
  @ViewChild('cardCvc', {static: true}) cardCvcElement?: ElementRef;

  // Stripe-related variables to hold the Stripe object and individual elements.
  stripe: any;
  cardNumber: any;
  cardExpiry: any;
  cardCvc: any;
  cardHandler = this.onChange.bind(this); //Ensures that the onChange method has the correct this context when it is used as an event handler for changes in the Stripe elements.
  cardErrors: any;
  loading = false;
  cardNumberValid = false;
  cardExpiryValid = false;
  cardCvcValid = false;

  constructor(private basketService : BasketService, private checkoutService : CheckoutService, private toaster : ToastrService, private router: Router) { }

   // Lifecycle hook that runs after the component's view has been fully initialized.
  ngAfterViewInit (): void {
    // Initialize the Stripe object with your public key
    this.stripe = Stripe('pk_test_51PqHbUDrtUOzjiis9ytugDHtjP2HjmuLuDMGOdi8rkbUISEnhSrcjSE6LEDZjhY7lp9MpvYFYSoZile8pGJVpgCU007Lz6UP8P');
    // Create an instance of Stripe Elements.
    const elements = this.stripe.elements();

    // Create and mount the card number input field.
    this.cardNumber = elements.create('cardNumber');
    //mount is Used to attach a Stripe element to the DOM so that it becomes visible and functional on your page.
    this.cardNumber.mount(this.cardNumberElement?.nativeElement);
    this.cardNumber.addEventListener('change', this.cardHandler);

    this.cardExpiry = elements.create('cardExpiry');
    this.cardExpiry.mount(this.cardExpiryElement?.nativeElement);
    this.cardExpiry.addEventListener('change', this.cardHandler);

    this.cardCvc = elements.create('cardCvc');
    this.cardCvc.mount(this.cardCvcElement?.nativeElement);
    this.cardCvc.addEventListener('change', this.cardHandler);
  }

  ngOnDestroy(): void{
    // Clean up Stripe elements to prevent memory leaks.
    this.cardNumber.destroy();
    this.cardExpiry.destroy();
    this.cardCvc.destroy();
  }

  onChange(event : any) {
    if(event.error) {
      this.cardErrors = event.error.message;
    }
    else {
      this.cardErrors = null;
    }

    // A switch statement is used to determine which input field triggered the event.
    switch (event.elementType) {
      case 'cardNumber':
      this.cardNumberValid = event.complete;
      break;
      case 'cardExpiry':
        this.cardExpiryValid = event.complete;
        break;
      case 'cardCvc':
        this.cardCvcValid = event.complete;
        break;
    }
  }

  //This method handles the process of submitting an order
  async submitOrder(){
    this.loading = true;
    const basket = this.basketService.getCurrentBasketValue();
    try{
      const createdOrder = await this.createOrder(basket);
      const paymentResult = await this.confirmPaymentWithStripe(basket);
      if (paymentResult.paymentIntent) {
        this.basketService.deleteBasket(basket!);
        //It stores the order object in the state property. This will be passed as additional data when navigating to a different route.
        const navigationExtras : NavigationExtras = {state: createdOrder};
        this.router.navigate(['checkout/success'], navigationExtras);
      }
      else {
        this.toaster.error(paymentResult.error.message);
      }
      this.loading = false;
    } catch (error) {
      console.log(error);
      this.loading = false
    }
  }
  private async confirmPaymentWithStripe(basket: IBasket | null) {
      // Use Stripe to confirm the card payment using the client secret and payment method details
      return this.stripe.confirmCardPayment(basket?.clientSecret, {
      payment_method: {
        card: this.cardNumber,
        billing_details: {
          name: this.checkoutForm.get('paymentForm')?.get('nameOnCard')?.value
        }
      }
    });
  }
  private createOrder(basket: IBasket | null): Promise<any> {
    if(basket){
    const orderToCreate = this.getOrderToCreate(basket) // Prepare the order object using the basket data
    // Convert the observable returned by createOrder into a Promise and return it
    // return this.checkoutService.createOrder(orderToCreate).toPromise();
    // lastValueFrom function will take the last value emitted by the observable and convert it into a promise it do the same for the toPromise() but in this version
    return lastValueFrom(this.checkoutService.createOrder(orderToCreate));
    } else {
      // Return a default value or throw an error
      return Promise.reject("Basket is null or undefined");
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
