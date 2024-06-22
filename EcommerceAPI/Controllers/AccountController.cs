using AutoMapper;
using System.Text;
using System.Diagnostics;
using EcommerceAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using EcommerceAPI.DTO.AuthenticationDTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EcommerceAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper mapper;
        private IConfiguration configuration;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;

        public AccountController(IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        // for testing
        [AllowAnonymous]
        [HttpGet("Hello")]
        public IActionResult Hello(string name)
        {
            name = $"Welcome back {name} :)";
            return Ok(name);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // var user = mapper.Map<ApplicationUser>(registerDTO);
            var user = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Address = registerDTO.Address,
                Gender = registerDTO.Gender,
                PhoneNumber = registerDTO.PhoneNumber,
                ProfileImage = registerDTO.ProfileImage,
            };

            // check needed?
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return BadRequest("Passwords aren't matched ...");
            }

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (registerDTO.Roles is null)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                foreach (var role in registerDTO.Roles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            return Ok(new AuthResponseDTO
            {
                isSuccess = true,
                Message = "User has been created successfully :)",
            });
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ApplicationUser? user = await userManager.FindByEmailAsync(loginDTO.Email);

                if (user is null)
                {
                    return Unauthorized(new AuthResponseDTO
                    {
                        isSuccess = false,
                        Message = "No user with the provided credentials :("
                    });
                }

                var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (!result)
                {
                    return Unauthorized(new AuthResponseDTO
                    {
                        isSuccess = false,
                        Message = "No user with the provided credentials :("
                    });
                }

                var token = GenerateToken(user);

                return Ok(new AuthResponseDTO
                {
                    Token = token,
                    isSuccess = true,
                    Message = $"Welcome {user.FirstName}",
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return StatusCode(502);
            }
        }

        #region helper function
        private string GenerateToken(ApplicationUser applicationUser)
        {
            List<Claim> claims = [
                new Claim (JwtRegisteredClaimNames.NameId, applicationUser.Id??""),
                new Claim (JwtRegisteredClaimNames.Email, applicationUser.Email??""),
                new Claim (JwtRegisteredClaimNames.Name, applicationUser.FirstName??""),
                new Claim (JwtRegisteredClaimNames.FamilyName, applicationUser.LastName??""),
                new Claim (JwtRegisteredClaimNames.Iss, configuration.GetSection("JwtSettings").GetSection("ValidIssuer").Value!??""),
                new Claim (JwtRegisteredClaimNames.Aud, configuration.GetSection("JwtSettings").GetSection("ValidAudience").Value!??""),
                new Claim("address", applicationUser.Address ?? ""),
                new Claim ("phoneNumber", applicationUser.PhoneNumber??""),
                new Claim ("profileImage", applicationUser.ProfileImage??""),
                new Claim("phoneNumberConfirmed", applicationUser.PhoneNumberConfirmed.ToString()??""),
                new Claim("twoFactorEnabled", applicationUser.TwoFactorEnabled.ToString()??""),
                new Claim("accessFailedCount", applicationUser.AccessFailedCount.ToString()??""),
            ];

            var roles = userManager.GetRolesAsync(applicationUser).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // signature
            var key = Encoding.ASCII.GetBytes(configuration.GetSection("JwtSettings").GetSection("securityKey").Value!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // header: token type and hashing algorithm
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512),

                // payload: extra data as [expire date, claims, audience, issure]
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return token;
        }
        #endregion

        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails()
        {
            var AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var AppUser = await userManager.FindByIdAsync(AppUserId!);
            if (AppUser is null)
            {
                return NotFound(new AuthResponseDTO
                {
                    isSuccess = false,
                    Message = "User not found"
                });
            }

            return Ok(new UserDetailsDTO
            {
                Id = AppUser.Id,
                Email = AppUser.Email,
                Address = AppUser.Address,
                LastName = AppUser.LastName,
                FirstName = AppUser.FirstName,
                PhoneNumber = AppUser.PhoneNumber,
                ProfileImage = AppUser.ProfileImage,
                TwoFactorEnabled = AppUser.TwoFactorEnabled,
                AccessFailedCount = AppUser.AccessFailedCount,
                PhoneNumberConfirmed = AppUser.PhoneNumberConfirmed,
                Roles = [.. await userManager.GetRolesAsync(AppUser)],
            });
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            var DTOs = users.Select(u => new UserDetailsDTO
            {
                Id = u.Id,
                Email = u.Email,
                Address = u.Address,
                LastName = u.LastName,
                FirstName = u.FirstName,
                PhoneNumber = u.PhoneNumber,
                ProfileImage = u.ProfileImage,
                TwoFactorEnabled = u.TwoFactorEnabled,
                AccessFailedCount = u.AccessFailedCount,
                PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                Roles = userManager.GetRolesAsync(u).Result.ToArray()
            }).ToList();
            return Ok(DTOs);
        }
    }
}