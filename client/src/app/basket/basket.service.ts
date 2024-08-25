import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, throwError , map, Observable } from 'rxjs';
import { Basket, IBasket, IBasketItem, IBasketTotals } from '../shared/models/basket';
import { IProduct } from '../shared/models/product';
import { environment } from 'src/environments/environment';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
baseUrl = environment.apiUrl;
private basketSource = new BehaviorSubject<IBasket | null>(null); //BehaviorSubject is a type of subject that requires an initial value and emits its current value to new subscribers.
basket$ = this.basketSource.asObservable(); //This is an observable of the basketSource that can be subscribed to by other components or services to get the current state of the basket.
private basketTotalSource = new BehaviorSubject<IBasketTotals | null>(null);
basketTotal$ = this.basketTotalSource.asObservable();
shipping = 0;
  constructor(private http: HttpClient) { }

  createPaymentIntent(){
    return this.http.post<IBasket>(this.baseUrl + 'payments/' + this.getCurrentBasketValue()?.id, {}).pipe(
      map((basket : IBasket) => {
        this.basketSource.next(basket);
        console.log(this.getCurrentBasketValue());
      })
    )
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.shipping = deliveryMethod.price;
    const basket = this.getCurrentBasketValue();
      basket!.deliveryMethodId = deliveryMethod.id;
      basket!.shippingPrice = deliveryMethod.price;
      this.calculateTotals();
      this.setBasket(basket!);

  }
  // Fetches the basket by ID
  getBasket(id: string) {
    return this.http.get<IBasket>(`${this.baseUrl}basket?id=${id}`)
    // The response is piped through the map operator, which sets the fetched basket as the current basket in basketSource.
    .pipe(
      map((basket: IBasket) => {
        this.basketSource.next(basket);
        this.shipping = basket.shippingPrice ?? 0;
        this.calculateTotals();
      }),
      catchError(this.handleError)
    );
  }

//// Updates the basket
setBasket(basket: IBasket){
  return this.http.post<IBasket>(`${this.baseUrl}basket`, basket) //The basket parameter is sent as the payload of the POST request.
  .subscribe((response: IBasket) => {
      this.basketSource.next(response); //updates the BehaviorSubject with the new basket data
      this.calculateTotals();
    }),
    catchError(this.handleError)
};

//Designed to return the current value of the basket
getCurrentBasketValue() {
  return this.basketSource.value;
}

// Adds an item to the basket and updates it on the server
addItemToBasket(item: IProduct, quantity = 1){
  const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(item, quantity); // which takes the item and quantity and returns an IBasketItem object
  let basket = this.getCurrentBasketValue();
  if(basket === null){
    basket = this.createBasket();
  }
  basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity); //This method either adds the new item to the basket or updates the quantity of an existing item if it's already in the basket.
  this.setBasket(basket); // sends the basket to the server and updates the local state
}

//Converts a product to a basket item with necessary properties.
private mapProductItemToBasketItem(item: IProduct, quantity: number): IBasketItem {
  return{
    id: item.id,
    productName: item.name,
    price: item.price,
    quantity,
    pictureUrl: item.pictureUrl,
    brand: item.productBrand,
    type: item.productType
  }
}

//Creates a new basket and stores its ID in local storage.
private createBasket(): IBasket {
  const basket = new Basket(); // Create a new instance of the Basket class
  localStorage.setItem('basket_id', basket.id); //Store the basket's id in the local storage with the key 'basket_id'
  return basket;
}

//Adds a new item to the basket or updates the quantity of an existing item.
private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
  //Find the index of the item in the basket that matches the item to add by its id
  const Index = items.findIndex(i => i.id === itemToAdd.id);
  if (Index === -1){ // If the item is not found in the basket
    itemToAdd.quantity = quantity;
    items.push(itemToAdd);
  }
  else{
    items[Index].quantity += quantity; //If the item is already in the basket, update the quantity
  }
  return items;
}

private calculateTotals (){
  const basket = this.getCurrentBasketValue();
  if (basket && basket.items) { //If the basket exists and has items
    const shipping = this.shipping;
    const subtotal = basket.items.reduce((a, b) => (b.price * b.quantity) + a, 0); //Calculate the subtotal by summing up the total price of each item
    const total = subtotal + shipping;
    this.basketTotalSource.next({shipping, total, subtotal}); //Update the basketTotalSource with the new totals
  }
}

incrementItemQuantity(item: IBasketItem){
  const basket = this.getCurrentBasketValue();
  if (basket) {
    // Find the index of the item in the basket that matches the given item's id.
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }
}

decrementItemQuantity(item: IBasketItem){
  const basket = this.getCurrentBasketValue();
  if (basket) {
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    if (basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }
}
  removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    //Check if the basket exists and contains the item to be removed.
    if (basket?.items.some(x => x.id === item.id)){
      //Filter out the item to be removed from the basket's items.
      basket.items = basket.items.filter(i => i.id !== item.id);
      if (basket.items.length > 0){
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteLocalBasket(id: string) {
    this.basketSource.next(null);
    this.basketTotalSource.next(null);
    localStorage.removeItem('basket_id');
  }

  deleteBasket(basket: IBasket) {
    return this.http.delete(`${this.baseUrl}basket?id=${basket.id}`).subscribe({
      next: () => {
        this.basketSource.next(null);
        this.basketTotalSource.next(null);
        localStorage.removeItem('basket_id');
      },
      error: error => {
        console.log(error);
      }
    })
  }

 // Handles any HTTP errors
  private handleError(error: any): Observable<never> {
  console.error('An error occurred', error);
  return throwError(error);
  }

}
