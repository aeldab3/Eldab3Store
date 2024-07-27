import { Component, ElementRef, Input, OnInit, Self, ViewChild } from '@angular/core';
import { ControlValueAccessor, NgControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})
export class TextInputComponent implements OnInit, ControlValueAccessor {
// Access the input element in the template
@ViewChild('input', {static : true}) input! : ElementRef;

// Define input properties to set the type and label of the input field
@Input() type = 'text';
@Input() label! : string;

// Inject NgControl to manage form control
  constructor(@Self() public controlDir : NgControl ) {
    // Set this component as the value accessor for the control
    this.controlDir.valueAccessor = this;
  }

  ngOnInit(): void {
    // Access the control linked with this component
    const control = this.controlDir.control;
     // Extract the validators (both synchronous and asynchronous) from the control
    const Validators = control?.validator ? [control.validator] : [];
    const asyncValidators = control?.asyncValidator ? [control.asyncValidator] : [];

    // Set the validators to the control
    control?.setValidators(Validators);
    control?.setAsyncValidators(asyncValidators);
    // Update the validity status of the control
    control?.updateValueAndValidity();
  }
  // Placeholder method for handling value changes
  onChange(event : any) {}
  // Placeholder method for handling touch events
  onTouched(){}

  // Implementing the ControlValueAccessor interface methods

  // Method to write the value to the input element
  writeValue(obj: any): void {
    this.input.nativeElement.value = obj || '';
  }

  // Method to register the function to call when the input value changes
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }


}
