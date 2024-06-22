namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserId { get; set; }
    }
}
