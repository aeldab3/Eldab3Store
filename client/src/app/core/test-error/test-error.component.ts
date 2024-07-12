import { HttpClient } from '@angular/common/http';
import { environment } from './../../../environments/environment';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss']
})
export class TestErrorComponent implements OnInit {
baseUrl = environment.apiUrl;
validationErrors: any;
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }
  get404Error(){
    this.http.get(this.baseUrl + 'products/42').subscribe({
      next: response => {
        console.log(response);
      },
      error: error => {
        console.log(error);
      }
    });
    }
  get500Error(){
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: response => {
        console.log(response);
      },
      error: error => {
        console.log(error);
      }
    });
    }
  get400Error(){
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: response => {
        console.log(response);
      },
      error: error => {
        console.log(error);
      }
    });
    }
  get400ValidationError(){
    this.http.get(this.baseUrl + 'products/fortytwo').subscribe({
      next: response => {
        console.log(response);
      },
      error: error => {
        console.log(error);
        this.validationErrors = error.errors;
      }
    });
    }
}
