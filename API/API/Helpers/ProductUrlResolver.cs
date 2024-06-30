using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Helpers
{
    //It is used to resolve the URL for a product's picture
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDto, string> //This interface is used to define custom value
    {
        private readonly IConfiguration _config;

        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
                {
                  return _config["ApiUrl"] + source.PictureUrl;       
                }
            return null;
        }
    }
}
