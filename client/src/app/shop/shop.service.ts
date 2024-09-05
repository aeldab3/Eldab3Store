import { IProduct } from './../shared/models/product';
import { ShopParams } from './../shared/models/shopParams';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination, Pagination } from '../shared/models/pagination';
import { IBrand } from '../shared/models/brand';
import { IType } from '../shared/models/productType';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  products : IProduct[] = [];
  brands : IBrand[] = [];
  types : IType[] = [];
  pagination = new Pagination();
  shopParams = new ShopParams();
  productCache = new Map();

  constructor(private http: HttpClient) { }

  //Fetches products from the API.
  getProducts(useCache : boolean){
    if (useCache === false) {
      this.productCache = new Map();
    }

    if (this.productCache.size > 0 && useCache === true) {
      // Check if the cache contains a key that matches the current shop parameters
      if (this.productCache.has(Object.values(this.shopParams).join("-"))) {
        // If the cache contains the key, retrieve the cached data and assign it to this.pagination.data
        this.pagination.data = this.productCache.get(Object.values(this.shopParams).join("-"));
        // Return an Observable of the pagination object, using the 'of' function to wrap it
        return of(this.pagination);
      }
    }

    let params = new HttpParams(); //Uses HttpParams to append query parameters based on the provided arguments.

    if (this.shopParams.brandId !== 0){ // Check if brandId is not zero and append it as a query parameter if true.

      params = params.append('brandId', this.shopParams.brandId.toString());
    }

    if (this.shopParams.typeId !== 0) {
      params = params.append('typeId', this.shopParams.typeId.toString());
    }

    if(this.shopParams.search){
      params = params.append('search', this.shopParams.search)
    }

      params = params.append('sort', this.shopParams.sort); // Append the sort parameter to the query parameters.
      params = params.append('pageIndex', this.shopParams.pageNumber.toString()); //Append the pageIndex parameter to the query parameters
      params = params.append('pageIndex', this.shopParams.pageSize.toString());

    return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params}) //Uses observe: 'response' to get the full HTTP response.
    .pipe( //pipe is used to apply the map operator to the observable returned by this.http.get.
      map(response => { //In this case, map is used to transform the full HTTP response into just the body of the response.
          this.productCache.set(Object.values(this.shopParams).join("-"), response.body?.data)
          if (response.body) {
            this.pagination = response.body;
        }
        return this.pagination;
      })
    )
  }

  getProduct(id: number){
    let product : IProduct | undefined;
    this.productCache.forEach((products : IProduct[]) => {
      product = products.find(p => p.id === id);
    })
    if (product) {
      return of(product);
    }
    // If the product is not found in the cache, make an HTTP GET request to fetch it from the server.
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  setShopParams(params : ShopParams){
    this.shopParams = params ;
  }

  getShopParams(){
    return this.shopParams;
  }
  //Fetches product brands from the API
  getBrands(){
    if (this.brands.length > 0) {
      return of(this.brands);
    }
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands').pipe(
      map(response => {
        this.brands = response;
        return response;
      })
    ) //Returns an observable of an array of brands (IBrand[]).
  }
  getTypes(){
    if (this.types.length > 0) {
      return of(this.types);
    }
    return this.http.get<IType[]>(this.baseUrl + 'products/types').pipe(
      map(response => {
        this.types = response;
        return response;
      })
    )
  }
}
