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
    public class CartItemController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public CartItemController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A list of cartItem DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartItemDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var Categories = await unitOfWork.CartItemGenericRepository.GetAll();
                var CDTOs = Categories.Select(p => mapper.Map<CartItemDTO>(p));
                return Ok(CDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a cartItem by ID.
        /// </summary>
        /// <param name="id">The ID of the cartItem to retrieve.</param>
        /// <returns>The cartItem DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartItemDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCartItemByID(int id)
        {
            try
            {
                var cartItem = await unitOfWork.CartItemGenericRepository.GetById(id);
                if (cartItem == null)
                {
                    return BadRequest("No such cartItem with the provided id");
                }
                var CDTO = mapper.Map<CartItemDTO>(cartItem);
                return Ok(CDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        // PUT: api/CartItem/5
        /// <summary>
        /// Updates a cartItem.
        /// </summary>
        /// <param name="id">The ID of the cartItem to update.</param>
        /// <param name="cartItem">The updated cartItem object.</param>
        /// <returns>The updated cartItem object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCartItem(int id, CartItem cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Please provide student data");
            }
            CartItem existedCartItem = await unitOfWork.CartItemGenericRepository.GetById(id);
            if (existedCartItem != null && existedCartItem.Id != cartItem.Id)
            {
                return NotFound($"There is a cartItem with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.CartItemGenericRepository.Update(id, cartItem);
                await unitOfWork.Save();
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new cartItem.
        /// </summary>
        /// <param name="cartItem">The cartItem object to add.</param>
        /// <returns>The added cartItem object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CartItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddCartItem(CartItem cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Please, enter cartItem's data");
            }

            try
            {
                cartItem = await unitOfWork.CartItemGenericRepository.Add(cartItem);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetCartItemByID), new { id = cartItem.Id }, cartItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a cartItem by ID.
        /// </summary>
        /// <param name="id">The ID of the cartItem to delete.</param>
        /// <returns>The deleted cartItem object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            CartItem cartItem = await unitOfWork.CartItemGenericRepository.GetById(id);
            if (cartItem == null)
            {
                return NotFound("CartItem doesn't exist");
            }
            await unitOfWork.CartItemGenericRepository.Delete(cartItem);
            await unitOfWork.Save();
            return Ok(cartItem);
        }
        [HttpGet("byProduct/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartItemDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCartItemByProductId(int productId)
        {
            try
            {
                var cartItem = await unitOfWork.CartItemGenericRepository.GetByProductId(productId);
                if (cartItem == null)
                {
                    return BadRequest("No cart item found with the provided product ID.");
                }
                var cartItemDTO = mapper.Map<CartItemDTO>(cartItem);
                return Ok(cartItemDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

    }
}
    