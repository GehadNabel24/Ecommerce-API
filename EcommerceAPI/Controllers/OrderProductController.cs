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
    public class OrderProductController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public OrderProductController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all orderProduct.
        /// </summary>
        /// <returns>A list of orderProduct DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderProductDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrderProduct()
        {
            try
            {
                var OrderProduct = await unitOfWork.OrderProductGenericRepository.GetAll();
                var ODTOs = OrderProduct.Select(p => mapper.Map<OrderProductDTO>(p));
                return Ok(ODTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves an orderProduct by ID.
        /// </summary>
        /// <param name="id">The ID of the orderProduct to retrieve.</param>
        /// <returns>The orderProduct DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetorderProductByID(int id)
        {
            try
            {
                var orderProduct = await unitOfWork.OrderProductGenericRepository.GetById(id);
                if (orderProduct == null)
                {
                    return BadRequest("No such orderProduct with the provided id");
                }
                var CODTO = mapper.Map<OrderProductDTO>(orderProduct);
                return Ok(CODTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates an orderProduct.
        /// </summary>
        /// <param name="id">The ID of the orderProduct to update.</param>
        /// <param name="orderProduct">The updated orderProduct object.</param>
        /// <returns>The updated orderProduct object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderProduct))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutorderProduct(int id, OrderProduct orderProduct)
        {
            if (orderProduct == null)
            {
                return BadRequest("Please provide student data");
            }
            OrderProduct existedorderProduct = await unitOfWork.OrderProductGenericRepository.GetById(id);
            if (existedorderProduct != null && existedorderProduct.Id != orderProduct.Id)
            {
                return NotFound($"There is a orderProduct with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.OrderProductGenericRepository.Update(id, orderProduct);
                await unitOfWork.Save();
                return Ok(orderProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new orderProduct.
        /// </summary>
        /// <param name="orderProduct">The orderProduct object to add.</param>
        /// <returns>The added orderProduct object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderProduct))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddorderProduct(OrderProduct orderProduct)
        {
            if (orderProduct == null)
            {
                return BadRequest("Please, enter orderProduct's data");
            }

            try
            {
                orderProduct = await unitOfWork.OrderProductGenericRepository.Add(orderProduct);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetorderProductByID), new { id = orderProduct.Id }, orderProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an orderProduct by ID.
        /// </summary>
        /// <param name="id">The ID of the orderProduct to delete.</param>
        /// <returns>The deleted orderProduct object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderProduct))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteorderProduct(int id)
        {
            OrderProduct orderProduct = await unitOfWork.OrderProductGenericRepository.GetById(id);
            if (orderProduct == null)
            {
                return NotFound("orderProduct doesn't exist");
            }
            await unitOfWork.OrderProductGenericRepository.Delete(orderProduct);
            await unitOfWork.Save();
            return Ok(orderProduct);
        }
    }
}