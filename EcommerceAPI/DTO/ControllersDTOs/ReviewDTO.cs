namespace EcommerceAPI.DTO.ControllersDTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        public decimal Rating { get; set; }

        public string Comment { get; set; }

        public int ProductId { get; set; }

        public string UserId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
