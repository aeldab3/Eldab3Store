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
    //public class OrderService : IOrderService
    //{
    //    private readonly IBasketRepository _basketRepo;
    //    private readonly IUnitOfWork _unitOfWork;

    //    public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _basketRepo = basketRepo;
    //    }

    //    public async Task<Order> CreateOrderAsync(string buyerEmail, int deleveryMethodId, string basketId, Address shippingAddress)
    //    {
    //            //Fetches the basket using the basket repository
    //            var basket = await _basketRepo.GetBasketAsync(basketId);
    //            var items = new List<OrderItem>(); //Creates an empty list to hold OrderItem objects.
    //            foreach (var item in basket.Items)
    //            {
    //                // fetching the corresponding product details from the product repository.
    //                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
    //                if (productItem == null)
    //                {
    //                    throw new Exception($"Product with ID {item.Id} not found");
    //                }

    //                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);

    //                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
    //                items.Add(orderItem);
    //            }

    //            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deleveryMethodId);
    //            if (deliveryMethod == null)
    //            {
    //                throw new Exception($"Delivery method with ID {deliveryMethod} not found");
    //            }


    //            var subtotal = items.Sum(item => item.Price * item.Quantity);

    //            // Create order
    //            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
    //            _unitOfWork.Repository<Order>().Add(order);

    //            // Save to database
    //            var result = await _unitOfWork.Complete();

    //            if (result <= 0)
    //            {
    //                throw new Exception("Failed to create order");
    //            }

    //            // Delete basket
    //            await _basketRepo.DeleteBasketAsync(basketId);

    //            return order;

    //    } 

    //    // Method to get all delivery methods asynchronously
    //    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    //    {
    //        return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
    //    }

    //    // Method to get a specific order by its ID and buyer's email asynchronously
    //    public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    //    {
    //        var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
    //        return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
    //    }

    //    // Method to get all orders for a specific user asynchronously
    //    public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
    //    {
    //        var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
    //        return await _unitOfWork.Repository<Order>().ListAsync(spec);
    //    }
    //}
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // get basket from the repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // get items from the product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            // get delivery method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);


            // create order
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
            _unitOfWork.Repository<Order>().Add(order);

            // save to db
            var result = await _unitOfWork.Complete();

            if (result <= 0) return null;

            // return order
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}
