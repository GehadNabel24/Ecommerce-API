namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        public int CartId { get; set; }
        public int ProductId { get; set; }
    }
}
