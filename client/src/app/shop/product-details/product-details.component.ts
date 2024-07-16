import { IProduct } from 'src/app/shared/models/product';
import { ShopService } from './../shop.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
product!: IProduct;
  constructor(private shopService: ShopService, private activateRoute: ActivatedRoute, private bcService: BreadcrumbService) {
    this.bcService.set('@productDetails', ' ')
  }

  ngOnInit(): void {
    this.loadProduct();
  }

  // This method fetches the product details based on the route parameter id.
  loadProduct(){
    this.shopService.getProduct(+this.activateRoute.snapshot.paramMap.get('id')!).subscribe({ //snapshot: It provides the current state of the route at the time of the component's initialization.
      next:(product) => {
        this.product = product;
        this.bcService.set('@productDetails', product.name);
      },
      error:(error) =>{
        console.log(error)
      },
    })
  }

}
