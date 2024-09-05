import { ShopParams} from './../shared/models/shopParams';
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
shopParams!: ShopParams;
totalCount: number = 0;
sortOptions = [ // Array of sorting options
  {name: 'Name: (a - z)', value: 'name'},
  {name: 'Price: Low To High', value: 'priceAsc'},
  {name: 'Price: High To Low', value: 'priceDesc'}
]

  constructor(private shopService: ShopService) {
    this.shopParams = this.shopService.getShopParams();
  }

  ngOnInit(): void{


// Call the methods to get products, brands, and types
this.getProducts(true);
this.getBrands();
this.getTypes();
}

  // Method to get products based on selected brand, type, and sort option
  getProducts(useCache = false){
  this.shopService.getProducts(useCache).subscribe({
    next: (products) => { //The 'next' handler is called when new data arrives
      this.products = products?.data || []; //Assign the fetched products to the products array
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
    const params = this.shopService.getShopParams();
    params.brandId = brandId; // Set the selected brand ID in the shopParams object
    params.pageNumber = 1; //Reset the page number to 1 whenever a new brand is selected
    this.shopService.setShopParams(params);
    this.getProducts(); // Fetch the products based on the updated shopParams
  };

  onTypeSelected(typeId: number){
    const params = this.shopService.getShopParams();
    params.typeId = typeId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.getProducts();
  };

  onSortSelected(sort: string){
    const params = this.shopService.getShopParams();
    params.sort = sort;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onPageChanged(event: any){
    const params = this.shopService.getShopParams();
    //Check if the new page number is different from the current page number
    if (params.pageNumber !== event){
      // Update the page number in shopParams with the new page number from the event
      params.pageNumber = event;
      this.shopService.setShopParams(params);
      this.getProducts(true);
    }
  }

  onSearch(){
    const params = this.shopService.getShopParams();
    //Update the search term in shopParams with the value from the search input field
    params.search = this.searchTerm.nativeElement.value;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onReset(){
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams(); //Reset the shopParams object to its default state
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }
}
