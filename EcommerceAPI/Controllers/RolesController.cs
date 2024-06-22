using EcommerceAPI.DTO.Roles;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpPost("CreateNewRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO roleDTO)
        {
            if (string.IsNullOrEmpty(roleDTO.RoleName))
            {
                return BadRequest("Role Name is Required ...");
            }
            var roleExists = await roleManager.RoleExistsAsync(roleDTO.RoleName);
            if (roleExists)
            {
                return BadRequest("Role already Exists !!");
            }

            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleDTO.RoleName));
            if (roleResult.Succeeded)
            {
                return Ok(new { Message = $"Role: {roleDTO.RoleName}, Created Successfully" });
            }
            return BadRequest($"Creating Role: {roleDTO.RoleName}, Failed ...");

        }

        [AllowAnonymous]
        [HttpGet("GetUsers'Roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetRoles()
        {
            var allRoles = await roleManager.Roles.ToListAsync();
            var roleDTOs = allRoles.Select(role => new RoleResponseDTO {
                Id = role.Id,
                Name = role.Name,
                TotalUsers = userManager.GetUsersInRoleAsync(role.Name!).Result.Count
            }).ToList();
            return Ok(roleDTOs);
        }

        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role is null)
            {
                return NotFound("Role doesn't Exist !!!");
            }
            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok(new { Message = $"Role: {role.Name}, has been deleted successfully" });
            }
            return BadRequest(new { Message = $"Deleting role: {role.Name}, failed !!!" });
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO assignRoleDTO)
        {
            var user = await userManager.FindByIdAsync(assignRoleDTO.UserId);
            if(user is null)
            {
                return NotFound("User doesn't exist !!!");
            }
            var role = await roleManager.FindByIdAsync(assignRoleDTO.RoleId);
            if (role is null)
            {
                return NotFound("Role doesn't exist !!!");
            }

            var result = await userManager.AddToRoleAsync(user, role.Name!);
            if (result.Succeeded)
            {
                return Ok(new { Status = $"{user.FirstName} becomes {role.Name}" });
            }
            
            return BadRequest(new { 
                Status = $"Failed to make {user.FirstName} become {role.Name}", 
                Problems = result.Errors.FirstOrDefault()!.Description 
            });
        }
    }
}