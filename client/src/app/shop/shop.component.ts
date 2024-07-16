import { shopParams } from './../shared/models/shopParams';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ShopService } from './shop.service';
import { IProduct } from '../shared/models/product';
import { IBrand } from '../shared/models/brand';
import { IType } from '../shared/models/productType';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
// Define class properties
@ViewChild('search', {static: false}) searchTerm!: ElementRef; //The @ViewChild decorator is used to access a template reference variable (#search) in component template. //ElementRef provides a way to interact directly with a native DOM element //The searchTerm property will hold a reference to the input element identified by #search.
products: IProduct[] = []; // Array to hold the products
brands: IBrand[] = [];
types: IType[] = [];
shopParams: shopParams = new shopParams();
totalCount: number = 0;
sortOptions = [ // Array of sorting options
  {name: 'Name: (a - z)', value: 'name'},
  {name: 'Price: Low To High', value: 'priceAsc'},
  {name: 'Price: High To Low', value: 'priceDesc'}
]

  constructor(private shopService: ShopService) { }

  ngOnInit(): void{


// Call the methods to get products, brands, and types
this.getProducts();
this.getBrands();
this.getTypes();
}

  // Method to get products based on selected brand, type, and sort option
  getProducts(){
  this.shopService.getProducts(this.shopParams).subscribe({
    next: (products) => { //The 'next' handler is called when new data arrives
      this.products = products?.data || []; //Assign the fetched products to the products array
      this.shopParams.pageNumber = products?.pageIndex || 1; //Set the pageNumber from the returned products data
      this.shopParams.pageSize = products?.pageSize || 6; //Set the pageSize from the returned products data
      this.totalCount = products?.count || 0; //Set the totalCount from the returned products data
      },
    error: (response) => {
      console.log(response);
      }
    });
  }

  // Method to get the list of brands
  getBrands(){
  this.shopService.getBrands().subscribe({
    next: (brands) => {
      this.brands = [{id: 0, name: 'All'}, ...brands]; // Add 'All' option and assign the fetched brands to the brands array
      },
    error: (response) => {
      console.log(response);
      }
    });
  }

  // Method to get the list of types
getTypes(){
  this.shopService.getTypes().subscribe({
    next: (types) => {
      this.types = [{id: 0, name: 'All'}, ...types];
      },
    error: (response) => {
      console.log(response);
      }
    });
  }

  // Method to handle brand selection
  onBrandSelected(brandId: number){
    this.shopParams.brandId = brandId; // Set the selected brand ID in the shopParams object
    this.shopParams.pageNumber = 1; //Reset the page number to 1 whenever a new brand is selected
    this.getProducts(); // Fetch the products based on the updated shopParams
  };

  onTypeSelected(typeId: number){
    this.shopParams.typeId = typeId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  };

  onSortSelected(sort: string){
    this.shopParams.sort = sort;
    this.getProducts();
  }

  onPageChanged(event: any){
    //Check if the new page number is different from the current page number
    if (this.shopParams.pageNumber !== event){
      // Update the page number in shopParams with the new page number from the event
      this.shopParams.pageNumber = event;
      this.getProducts();
    }
  }

  onSearch(){
    //Update the search term in shopParams with the value from the search input field
    this.shopParams.search = this.searchTerm.nativeElement.value;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onReset(){
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new shopParams(); //Reset the shopParams object to its default state
    this.getProducts();
  }
}
