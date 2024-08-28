using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    // This class is a specification used to retrieve orders by their PaymentIntentId
    public class OrderByPaymentIntentIdSpecification : BaseSpecification<Order>
    {
        public OrderByPaymentIntentIdSpecification(string paymentIntentId)
            : base(o=> o.PaymentIntentId == paymentIntentId) // The condition checks if the PaymentIntentId of the order matches the provided paymentIntentId.
        {
        }
    }
}
