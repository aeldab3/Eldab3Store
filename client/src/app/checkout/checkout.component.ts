import { AccountService } from 'src/app/account/account.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BasketService } from '../basket/basket.service';
import { IBasketTotals } from '../shared/models/basket';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  basketTotals$!: Observable<IBasketTotals | null>;
  checkoutForm!: FormGroup;

  constructor(private fb: FormBuilder, private accountService: AccountService, private basketService: BasketService) { }

  ngOnInit() {
    this.createCheckoutForm();
    this.getAddressFormValues();
    this.basketTotals$ = this.basketService.basketTotal$;
  }

  createCheckoutForm() {
    this.checkoutForm = this.fb.group({
      addressForm: this.fb.group({
        firstName: [null, Validators.required],
        lastName: [null, Validators.required],
        street: [null, Validators.required],
        city: [null, Validators.required],
        state: [null, Validators.required],
        zipcode: [null, Validators.required],
      }),
      deliveryForm: this.fb.group({
        deliveryMethod: [null, Validators.required]
      }),
      paymentForm: this.fb.group({
        nameOnCard: [null, Validators.required]
      })
    });
  }

  getAddressFormValues(){
    this.accountService.getUserAddress().subscribe({
      next: (address) => {
        if(address) {
          return this.checkoutForm.get('addressForm')?.patchValue(address)
        }
      },
      error: error => console.log(error)
    })
  }
}
