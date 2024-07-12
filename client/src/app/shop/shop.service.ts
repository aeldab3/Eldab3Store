import { IProduct } from './../shared/models/product';
import { shopParams } from './../shared/models/shopParams';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination } from '../shared/models/pagination';
import { IBrand } from '../shared/models/brand';
import { IType } from '../shared/models/productType';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = "https://localhost:7099/api/"

  constructor(private http: HttpClient) { }

  //Fetches products from the API.
  getProducts(shopParams: shopParams){
    let params = new HttpParams(); //Uses HttpParams to append query parameters based on the provided arguments.

    if (shopParams.brandId !== 0){ // Check if brandId is not zero and append it as a query parameter if true.

      params = params.append('brandId', shopParams.brandId.toString());
    }

    if (shopParams.typeId !== 0) {
      params = params.append('typeId', shopParams.typeId.toString());
    }

    if(shopParams.search){
      params = params.append('search', shopParams.search)
    }

      params = params.append('sort', shopParams.sort); // Append the sort parameter to the query parameters.
      params = params.append('pageIndex', shopParams.pageNumber.toString()); //Append the pageIndex parameter to the query parameters
      params = params.append('pageIndex', shopParams.pageSize.toString());

    return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params}) //Uses observe: 'response' to get the full HTTP response.
    .pipe( //pipe is used to apply the map operator to the observable returned by this.http.get.
      map(response => { //In this case, map is used to transform the full HTTP response into just the body of the response.
        return response.body;
      })
    )
  }

  getProduct(id: number){
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  //Fetches product brands from the API
  getBrands(){
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands'); //Returns an observable of an array of brands (IBrand[]).
  }
  getTypes(){
    return this.http.get<IType[]>(this.baseUrl + 'products/types');
  }
}
