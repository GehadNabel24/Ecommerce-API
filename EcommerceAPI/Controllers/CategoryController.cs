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
    public class CategoryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public CategoryController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A list of category DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var Categories = await unitOfWork.CategoryGenericRepository.GetAll();
                var CDTOs = Categories.Select(p => mapper.Map<CategoryDTO>(p));
                return Ok(CDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a category by ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The category DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryByID(int id)
        {
            try
            {
                var category = await unitOfWork.CategoryGenericRepository.GetById(id);
                if (category == null)
                {
                    return BadRequest("No such category with the provided id");
                }
                var CDTO = mapper.Map<CategoryDTO>(category);
                return Ok(CDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        // PUT: api/Category/5
        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="category">The updated category object.</param>
        /// <returns>The updated category object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (category == null)
            {
                return BadRequest("Please provide student data");
            }
            Category existedCategory = await unitOfWork.CategoryGenericRepository.GetById(id);
            if (existedCategory != null && existedCategory.Id != category.Id)
            {
                return NotFound($"There is a category with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.CategoryGenericRepository.Update(id, category);
                await unitOfWork.Save();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="category">The category object to add.</param>
        /// <returns>The added category object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddCategory(Category category)
        {
            if (category == null)
            {
                return BadRequest("Please, enter category's data");
            }

            try
            {
                category = await unitOfWork.CategoryGenericRepository.Add(category);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetCategoryByID), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a category by ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>The deleted category object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = await unitOfWork.CategoryGenericRepository.GetById(id);
            if (category == null)
            {
                return NotFound("Category doesn't exist");
            }
            await unitOfWork.CategoryGenericRepository.Delete(category);
            await unitOfWork.Save();
            return Ok(category);
        }
    }
}