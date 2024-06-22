using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using AutoMapper;
using EcommerceAPI.Unit_Of_Work;
using EcommerceAPI.DTO.ControllersDTOs;

namespace EcommerceAPI.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public ProductsController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list of product DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var Products = await unitOfWork.ProductGenericRepository.GetAll();
                var PDTOs = Products.Select(p => mapper.Map<ProductDTO>(p));
                return Ok(PDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a product by ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The product DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductByID(int id)
        {
            try
            {
                var product = await unitOfWork.ProductGenericRepository.GetById(id);
                if (product == null)
                {
                    return BadRequest("No such Product with the provided id");
                }
                var PDTOs = mapper.Map<ProductDTO>(product);
                return Ok(PDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="product">The updated product object.</param>
        /// <returns>The updated product object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (product == null)
            {
                return BadRequest("Please provide student data");
            }
            Product existedProduct = await unitOfWork.ProductGenericRepository.GetById(id);
            if (existedProduct != null && existedProduct.Id != product.Id)
            {
                return NotFound($"There is a Product with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.ProductGenericRepository.Update(id, product);
                await unitOfWork.Save();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product object to add.</param>
        /// <returns>The added product object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Please, enter Product's data");
            }

            try
            {
                product = await unitOfWork.ProductGenericRepository.Add(product);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetProductByID), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>The deleted product object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product product = await unitOfWork.ProductGenericRepository.GetById(id);
            if (product == null)
            {
                return NotFound("Product doesn't exist");
            }
            await unitOfWork.ProductGenericRepository.Delete(product);
            await unitOfWork.Save();
            return Ok(product);
        }
    }
}