using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deleveryMethodId, string basketId, Address shippingAddress)
        {
            //Fetches the basket using the basket repository
            var basket = await _basketRepo.GetBasketAsync(basketId);
            var items = new List<OrderItem>(); //Creates an empty list to hold OrderItem objects.
            foreach (var item in basket.Items)
            {
                // fetching the corresponding product details from the product repository.
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (productItem == null)
                {
                    throw new Exception($"Product with ID {item.Id} not found");
                }

                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);

                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deleveryMethodId);
            if (deliveryMethod == null)
            {
                throw new Exception($"Delivery method with ID {deliveryMethod} not found");
            }


            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // Check to see if order exists
            var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (existingOrder != null)
            {
                // If such an order exists, delete it from the repository
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                // Create or update the payment intent associated with the basket's PaymentIntentId.
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);

            }

            // Create order
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, basket.PaymentIntentId);
            _unitOfWork.Repository<Order>().Add(order);

            // Save to database
            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                throw new Exception("Failed to create order");
            }

            //// Delete basket
            //await _basketRepo.DeleteBasketAsync(basketId);

            return order;

        }

        // Method to get all delivery methods asynchronously
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        // Method to get a specific order by its ID and buyer's email asynchronously
        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        // Method to get all orders for a specific user asynchronously
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }

}
