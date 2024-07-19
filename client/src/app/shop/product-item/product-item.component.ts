import { Component, OnInit, Input } from '@angular/core';
import { BasketService } from 'src/app/basket/basket.service';
import { IProduct } from 'src/app/shared/models/product';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent implements OnInit {
@Input() product!:IProduct;
  constructor(private baskService:BasketService) { }

  ngOnInit(): void {
  }

  addItemToBasket(){
   this.product && this.baskService.addItemToBasket(this.product);
  }
}
