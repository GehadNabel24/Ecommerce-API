namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public int OrderStatus { get; set; }
        public int PaymentMethod { get; set; }
        public string UserId { get; set; }
    }
}
