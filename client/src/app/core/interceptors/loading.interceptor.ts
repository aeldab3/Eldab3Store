import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { delay, finalize, Observable } from 'rxjs';
import { BusyService } from '../services/busy.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if( request.method ==='POST' && request.url.includes('orders')) {
      return next.handle(request);
    }
    if(request.url.includes('emailexists')){
      return next.handle(request);
    }
    // If the conditions above are not met, call the busy() method on the busyService to indicate loading state
    this.busyService.busy();
    return next.handle(request).pipe(
      finalize(() => this.busyService.idle())
    )
  }
}
