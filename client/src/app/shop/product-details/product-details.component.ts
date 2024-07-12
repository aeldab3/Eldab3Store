import { IProduct } from 'src/app/shared/models/product';
import { ShopService } from './../shop.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
product!: IProduct;
  constructor(private shopService: ShopService, private activateRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadProduct();
  }

  // This method fetches the product details based on the route parameter id.
  loadProduct(){
    this.shopService.getProduct(+this.activateRoute.snapshot.paramMap.get('id')!).subscribe({ //snapshot: It provides the current state of the route at the time of the component's initialization.
      // product => {
      //   this.product = product;
      // },
      // error =>{
      //   console.error(error);
      // }
      next:(product) => {
        this.product = product;
      },
      error:(error) =>{
        console.log(error)
      },
    })
  }

}
