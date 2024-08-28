import { CdkStepper } from '@angular/cdk/stepper';
import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent implements OnInit {
  @Input() appStepper!: CdkStepper;
  basket$!: Observable<IBasket | null>;
    constructor(private basketService : BasketService, private toastr : ToastrService) { }

  ngOnInit(): void {
    this.basket$ = this.basketService.basket$;
  }
  createPaymentIntent(){
    return this.basketService.createPaymentIntent().subscribe({
      next: (response : any) => {
        this.appStepper.next();
      },
      error: (error) =>{
        console.log(error);
      }
    })
  }
}
