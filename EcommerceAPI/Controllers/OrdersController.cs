using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using EcommerceAPI.Unit_Of_Work;
using AutoMapper;
using EcommerceAPI.DTO.ControllersDTOs;

namespace EcommerceAPI.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public OrdersController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of order DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var Orders = await unitOfWork.OrderGenericRepository.GetAll();
                var ODTOs = Orders.Select(p => mapper.Map<OrderDTO>(p));
                return Ok(ODTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves an order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetorderByID(int id)
        {
            try
            {
                var order = await unitOfWork.OrderGenericRepository.GetById(id);
                if (order == null)
                {
                    return BadRequest("No such order with the provided id");
                }
                var CODTO = mapper.Map<OrderDTO>(order);
                return Ok(CODTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates an order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="order">The updated order object.</param>
        /// <returns>The updated order object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Putorder(int id, Order order)
        {
            if (order == null)
            {
                return BadRequest("Please provide student data");
            }
            Order existedorder = await unitOfWork.OrderGenericRepository.GetById(id);
            if (existedorder != null && existedorder.Id != order.Id)
            {
                return NotFound($"There is a order with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.OrderGenericRepository.Update(id, order);
                await unitOfWork.Save();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new order.
        /// </summary>
        /// <param name="order">The order object to add.</param>
        /// <returns>The added order object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Addorder(Order order)
        {
            if (order == null)
            {
                return BadRequest("Please, enter order's data");
            }

            try
            {
                order = await unitOfWork.OrderGenericRepository.Add(order);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetorderByID), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>The deleted order object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deleteorder(int id)
        {
            Order order = await unitOfWork.OrderGenericRepository.GetById(id);
            if (order == null)
            {
                return NotFound("order doesn't exist");
            }
            await unitOfWork.OrderGenericRepository.Delete(order);
            await unitOfWork.Save();
            return Ok(order);
        }
    }
}