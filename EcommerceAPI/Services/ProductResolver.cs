using AutoMapper;
using EcommerceAPI.DTO.ControllersDTOs;
using EcommerceAPI.Models;
using Microsoft.Extensions.Configuration;

namespace EcommerceAPI.Services
{
    public class ProductResolver : IValueResolver<Product, ProductDTO, string>
    {
       
             private readonly IConfiguration _configuration;

        public ProductResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Product source, ProductDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Image))
                return $"{_configuration["APIURL"]}{source.Image}";
            return null;
        }
    
    }
}
