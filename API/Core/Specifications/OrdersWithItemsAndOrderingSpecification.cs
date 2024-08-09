using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    // This class is used to create specifications for querying Order entities 
    //public class OrdersWithItemsAndOrderingSpecification : BaseSpecification<Order>
    //{
    //    public OrdersWithItemsAndOrderingSpecification(string email) : base (o => o.BuyerEmail == email)
    //    {
    //        AddInclude(o => o.OrderItems);
    //        AddInclude(o => o.DeliveryMethod);
    //        AddOrderByDescending(o => o.OrderDate);
    //    }

    //    public OrdersWithItemsAndOrderingSpecification(int id, string email) : base(o => o.Id == id && o.BuyerEmail == email)
    //    {
    //        AddInclude(o => o.OrderItems);
    //        AddInclude(o => o.DeliveryMethod);
    //    }
    //}
    public class OrdersWithItemsAndOrderingSpecification : BaseSpecification<Order>
    {
        public OrdersWithItemsAndOrderingSpecification(string email) : base(o => o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);
        }

        public OrdersWithItemsAndOrderingSpecification(int id, string email)
            : base(o => o.Id == id && o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}
