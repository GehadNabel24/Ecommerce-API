namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class WishlistItemDTO
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
    }
}
