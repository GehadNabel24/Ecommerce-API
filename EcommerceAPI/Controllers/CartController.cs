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
    public class CartController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public CartController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A list of cart DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CartDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var Categories = await unitOfWork.CartGenericRepository.GetAll();
                var CDTOs = Categories.Select(p => mapper.Map<CartDTO>(p));
                return Ok(CDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a cart by ID.
        /// </summary>
        /// <param name="id">The ID of the cart to retrieve.</param>
        /// <returns>The cart DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCartByID(int id)
        {
            try
            {
                var cart = await unitOfWork.CartGenericRepository.GetById(id);
                if (cart == null)
                {
                    return BadRequest("No such cart with the provided id");
                }
                var CDTO = mapper.Map<CartDTO>(cart);
                return Ok(CDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        // PUT: api/Cart/5
        /// <summary>
        /// Updates a cart.
        /// </summary>
        /// <param name="id">The ID of the cart to update.</param>
        /// <param name="cart">The updated cart object.</param>
        /// <returns>The updated cart object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Cart))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (cart == null)
            {
                return BadRequest("Please provide student data");
            }
            Cart existedCart = await unitOfWork.CartGenericRepository.GetById(id);
            if (existedCart != null && existedCart.Id != cart.Id)
            {
                return NotFound($"There is a cart with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.CartGenericRepository.Update(id, cart);
                await unitOfWork.Save();
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new cart.
        /// </summary>
        /// <param name="cart">The cart object to add.</param>
        /// <returns>The added cart object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Cart))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddCart(Cart cart)
        {
            if (cart == null)
            {
                return BadRequest("Please, enter cart's data");
            }

            try
            {
                cart = await unitOfWork.CartGenericRepository.Add(cart);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetCartByID), new { id = cart.Id }, cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a cart by ID.
        /// </summary>
        /// <param name="id">The ID of the cart to delete.</param>
        /// <returns>The deleted cart object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Cart))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCart(int id)
        {
            Cart cart = await unitOfWork.CartGenericRepository.GetById(id);
            if (cart == null)
            {
                return NotFound("Cart doesn't exist");
            }
            await unitOfWork.CartGenericRepository.Delete(cart);
            await unitOfWork.Save();
            return Ok(cart);
        }
    }
}
