using AutoMapper;
using EcommerceAPI.DTO.ControllersDTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Unit_Of_Work;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public WishlistController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all wishlists.
        /// </summary>
        /// <returns>A list of wishlist DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WishlistDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWishlists()
        {
            try
            {
                var Wishlists = await unitOfWork.WishlistGenericRepository.GetAll();
                var WDTOs = Wishlists.Select(p => mapper.Map<WishlistDTO>(p));
                return Ok(WDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a wishlist by ID.
        /// </summary>
        /// <param name="id">The ID of the wishlist to retrieve.</param>
        /// <returns>The wishlist DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WishlistDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWishlistByID(int id)
        {
            try
            {
                var Wishlist = await unitOfWork.WishlistGenericRepository.GetById(id);
                if (Wishlist == null)
                {
                    return BadRequest("No such Wishlist with the provided id");
                }
                var WDTO = mapper.Map<WishlistDTO>(Wishlist);
                return Ok(WDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates a wishlist.
        /// </summary>
        /// <param name="id">The ID of the wishlist to update.</param>
        /// <param name="wishlist">The updated wishlist object.</param>
        /// <returns>The updated wishlist object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Wishlist))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutWishlist(int id, Wishlist wishlist)
        {
            if (wishlist == null)
            {
                return BadRequest("Please provide student data");
            }
            Wishlist existedWishlist = await unitOfWork.WishlistGenericRepository.GetById(id);
            if (existedWishlist != null && existedWishlist.Id != wishlist.Id)
            {
                return NotFound($"There is a Wishlist with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.WishlistGenericRepository.Update(id, wishlist);
                await unitOfWork.Save();
                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new wishlist.
        /// </summary>
        /// <param name="wishlist">The wishlist object to add.</param>
        /// <returns>The added wishlist object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Wishlist))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddWishlist(Wishlist wishlist)
        {
            if (wishlist == null)
            {
                return BadRequest("Please, enter Wishlist's data");
            }

            try
            {
                wishlist = await unitOfWork.WishlistGenericRepository.Add(wishlist);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetWishlistByID), new { id = wishlist.Id }, wishlist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a wishlist by ID.
        /// </summary>
        /// <param name="id">The ID of the wishlist to delete.</param>
        /// <returns>The deleted wishlist object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Wishlist))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            Wishlist wishlist = await unitOfWork.WishlistGenericRepository.GetById(id);
            if (wishlist == null)
            {
                return NotFound("Wishlist doesn't exist");
            }
            await unitOfWork.WishlistGenericRepository.Delete(wishlist);
            await unitOfWork.Save();
            return Ok(wishlist);
        }
    }
}