using AutoMapper;
using EcommerceAPI.DTO.AuthenticationDTOs;
using EcommerceAPI.DTO.ControllersDTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity;


namespace EcommerceAPI.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest =>  dest.Image, opt => opt.MapFrom<ProductResolver>())
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap();

            // note: full name : trim(fname) + trim(lname)
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<OrderProduct, OrderProductDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ReverseMap();

            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ReverseMap();

        CreateMap<Cart, CartDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<CartItem, CartItemDTO>()
                    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                    .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                    .ReverseMap();

            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ReverseMap();

           CreateMap<Coupon, CouponDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.ValidFrom))
                .ForMember(dest => dest.ValidTo, opt => opt.MapFrom(src => src.ValidTo))
                .ReverseMap();

            CreateMap<Wishlist, WishlistDTO>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                 .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                 .ReverseMap();

            CreateMap<WishlistItem, WishlistItemDTO>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.WishlistId, opt => opt.MapFrom(src => src.WishlistId))
                 .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                 .ReverseMap();

            // todo in frontend: register contains username -> change concat firstname + lastname to username property
            // else if register contains first name and last name leave everything as it now.
            //CreateMap<ApplicationUser, RegisterDTO>()
            //        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => $"{src.FirstName.Trim()}"))
            //        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => $"{src.LastName.Trim()}"))
            //        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            //        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName))
            //        .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            //        .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.ProfileImage))
            //        .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            //        .ReverseMap();

            //CreateMap<ApplicationUser, UserDetailsDTO>()
            //        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            //        .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            //        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => $"{src.LastName.Trim()}"))
            //        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => $"{src.FirstName.Trim()}"))
            //        .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
            //        .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
            //        .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
            //        .ReverseMap();
        }
    }
}
