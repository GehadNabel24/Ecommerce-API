namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class OrderProductDTO
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}
