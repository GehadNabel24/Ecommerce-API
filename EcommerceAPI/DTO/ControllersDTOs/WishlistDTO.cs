namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class WishlistDTO
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }

    }
}
