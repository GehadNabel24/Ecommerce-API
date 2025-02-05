﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Image { get; set; }

    public decimal Discount { get; set; }

    public int CategoryId { get; set; }
    public decimal Rating { get; set; }


    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    

    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}