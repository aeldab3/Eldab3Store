import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-pager',
  templateUrl: './pager.component.html',
  styleUrls: ['./pager.component.scss']
})
export class PagerComponent implements OnInit {
@Input() pageSize: number = 0;
@Input() totalCount: number = 0;
@Input() pageNumber: number = 0;
// Output property to emit an event from the child component to the parent component when the page changes
@Output() pageChanged = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
  }

   // Method to handle page change event (example usage)
  onPagerChange(event: any){
     // Emit the new page to the parent component
    this.pageChanged.emit(event.page);
  }

}
