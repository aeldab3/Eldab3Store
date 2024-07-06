import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { IProduct } from './models/product';
import { IPagination } from './models/pagination';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Eldab3 Store';
  products: IProduct[] = [];
  constructor(private http: HttpClient){}
  ngOnInit(): void {
    this.http.get<IPagination>('https://localhost:7099/api/products?pageSize=50')
    .subscribe(
      (response: IPagination ) =>{
      this.products = response.data;
    },
    error =>
    {
      console.log(error);
    });
  }
}
