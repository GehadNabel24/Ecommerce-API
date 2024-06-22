using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTO.Roles
{
    public class RoleDTO
    {
        [Required(ErrorMessage = "Role Name is Required ...")]
        public string RoleName { get; set; }
    }
}
