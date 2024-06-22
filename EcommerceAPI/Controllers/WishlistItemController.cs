using AutoMapper;
using EcommerceAPI.DTO.ControllersDTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Unit_Of_Work;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistItemController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public WishlistItemController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all wishlistItems.
        /// </summary>
        /// <returns>A list of wishlistItem DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WishlistItemDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            try
            {
                var WishlistItems = await unitOfWork.WishlistItemGenericRepository.GetAll();
                var WLDTOs = WishlistItems.Select(p => mapper.Map<WishlistItemDTO>(p));
                return Ok(WLDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a wishlistItem by ID.
        /// </summary>
        /// <param name="id">The ID of the wishlistItem to retrieve.</param>
        /// <returns>The wishlistItem DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WishlistItemDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWishlistItemByID(int id)
        {
            try
            {
                var WishlistItem = await unitOfWork.WishlistItemGenericRepository.GetById(id);
                if (WishlistItem == null)
                {
                    return BadRequest("No such WishlistItem with the provided id");
                }
                var WLDTO = mapper.Map<WishlistItemDTO>(WishlistItem);
                return Ok(WLDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates a wishlistItem.
        /// </summary>
        /// <param name="id">The ID of the wishlistItem to update.</param>
        /// <param name="wishlistItem">The updated wishlistItem object.</param>
        /// <returns>The updated wishlistItem object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WishlistItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutWishlistItem(int id, WishlistItem wishlistItem)
        {
            if (wishlistItem == null)
            {
                return BadRequest("Please provide student data");
            }
            WishlistItem existedWishlistItem = await unitOfWork.WishlistItemGenericRepository.GetById(id);
            if (existedWishlistItem != null && existedWishlistItem.Id != wishlistItem.Id)
            {
                return NotFound($"There is a WishlistItem with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.WishlistItemGenericRepository.Update(id, wishlistItem);
                await unitOfWork.Save();
                return Ok(wishlistItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new wishlistItem.
        /// </summary>
        /// <param name="wishlistItem">The wishlistItem object to add.</param>
        /// <returns>The added wishlistItem object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WishlistItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddWishlistItem(WishlistItem wishlistItem)
        {
            if (wishlistItem == null)
            {
                return BadRequest("Please, enter WishlistItem's data");
            }

            try
            {
                wishlistItem = await unitOfWork.WishlistItemGenericRepository.Add(wishlistItem);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetWishlistItemByID), new { id = wishlistItem.Id }, wishlistItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a wishlistItem by ID.
        /// </summary>
        /// <param name="id">The ID of the wishlistItem to delete.</param>
        /// <returns>The deleted wishlistItem object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WishlistItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            WishlistItem wishlistItem = await unitOfWork.WishlistItemGenericRepository.GetById(id);
            if (wishlistItem == null)
            {
                return NotFound("WishlistItem doesn't exist");
            }
            await unitOfWork.WishlistItemGenericRepository.Delete(wishlistItem);
            await unitOfWork.Save();
            return Ok(wishlistItem);
        }
    }
}