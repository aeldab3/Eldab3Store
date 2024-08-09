using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer redis)
        {

            _database = redis.GetDatabase(); //Initializes the _database field by getting a Redis database instance
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId); //Uses the KeyDeleteAsync method of the Redis database to delete the basket.

        }

        public async Task<CustomerBasket> GetBasketAsync(string baskedId)
        {
            var data = await _database.StringGetAsync(baskedId); //Uses the StringGetAsync method of the Redis database to get the basket data.
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data); //it deserializes the data into a CustomerBasket object and returns it.
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            //Uses the StringSetAsync method of the Redis database to set the basket data with an expiration time of 30 days
            var created = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket)
                , TimeSpan.FromDays(30)); //Serializes the CustomerBasket object to a JSON string.
            if (!created) return null;

            return await GetBasketAsync(basket.Id); //to retrieve the updated basket and returns it.
        }
    }
  
}
