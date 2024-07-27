import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { map, of, switchMap, timer } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

registerForm! : FormGroup;
errors! : string[];

  constructor(private fb: FormBuilder, private accountService : AccountService, private router : Router) { }

  ngOnInit(): void {
    this.createRegisterForm();
  }
  createRegisterForm(){
    this.registerForm = this.fb.group({
      displayName: [null, [Validators.required]],
      email : [null, [Validators.required, Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')], [this.validateEmailNotTaken()]],
      password : [null, Validators.required]
    });
  }

  onSubmit(){
    this.accountService.register(this.registerForm.value).subscribe({
      next : () =>{
        this.router.navigateByUrl('/shop');
      },
      error: error => {
        console.log(error);
        this.errors = error.errors;
      }
    });
  };

  //returns an AsyncValidatorFn which is a function that returns an observable. This function can be used to validate form controls asynchronously.
  validateEmailNotTaken() : AsyncValidatorFn{
    return control => { //This parameter represents the form control that needs validation.
      return timer(500).pipe(
        switchMap(() => { //This operator maps the value from the timer observable to a new observable
          if(!control.value){
            return of(null);
          }
          return this.accountService.checkEmailExists(control.value).pipe(
            map(res => { //This operator maps the response (res) to a validation error object if the email exists
              return res ? {emailExists : true} : null;
            })
          )
        })
      )
    }
  }

}
