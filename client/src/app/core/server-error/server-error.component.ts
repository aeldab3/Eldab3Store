import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.scss']
})
export class ServerErrorComponent implements OnInit {
  error: any;

  //The Router service is used to manage navigation and routing within the application.
  constructor(private router: Router) {
  const navigation  = this.router.getCurrentNavigation();  //This line retrieves the current navigation information
  //This line assigns the error details to the error property of the component.
  this.error = navigation?.extras?.state?.['error'];
  }
  ngOnInit(): void {
   }
}
