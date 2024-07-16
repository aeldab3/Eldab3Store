using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync (string baskedId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket); //Takes a CustomerBasket object as a parameter.
        Task<bool>  DeleteBasketAsync(string basketId);
    }
}
