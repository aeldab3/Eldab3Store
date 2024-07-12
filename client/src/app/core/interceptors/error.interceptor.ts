import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs';
import { ToastrService} from 'ngx-toastr';

@Injectable()
// It is used to handle HTTP errors globally by intercepting HTTP requests and navigating to specific error pages based on the error status code.
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService ) {}

  // Implement the intercept method required by HttpInterceptor
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Handle the HTTP request and catch errors
    return next.handle(request).pipe(
      catchError((error) => {
        if(error) {
          if(error.status === 400) {
            if(error.error.errors) { //Checks if the error object contains an errors property
              throw error.error;
            }
            this.toastr.error(error.error.message, error.error.statusCode);
          }
          if(error.status === 401) {
            this.toastr.error(error.error.message, error.error.statusCode);
          }
          if(error.status === 404) {
            this.router.navigateByUrl('/not-found')
          }
          if(error.status === 500) {
            const navigationExtras: NavigationExtras = {state: {error: error.error}}; //NavigationExtras is an interface provided by Angular that allows you to pass additional information during navigation. state: it captures the error details to be passed along with the navigation.
            this.router.navigateByUrl('/server-error', navigationExtras)
          }
        }
        //Re-throw the error to be handled by other parts of the application
        return throwError(error);
      })
    );
  }
}
