namespace EcommerceAPI.DTO.AuthenticationDTOs
{
    public class AuthResponseDTO
    {  
        public bool isSuccess { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; } = string.Empty;
    }
}