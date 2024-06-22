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
    public class ReviewController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public ReviewController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all reviews.
        /// </summary>
        /// <returns>A list of review DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var Reviews = await unitOfWork.ReviewGenericRepository.GetAll();
                var RDTOs = Reviews.Select(p => mapper.Map<ReviewDTO>(p));
                return Ok(RDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a review by ID.
        /// </summary>
        /// <param name="id">The ID of the review to retrieve.</param>
        /// <returns>The review DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewByID(int id)
        {
            try
            {
                var review = await unitOfWork.ReviewGenericRepository.GetById(id);
                if (review == null)
                {
                    return BadRequest("No such Review with the provided id");
                }
                var RDTO = mapper.Map<ReviewDTO>(review);
                return Ok(RDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates a review.
        /// </summary>
        /// <param name="id">The ID of the review to update.</param>
        /// <param name="review">The updated review object.</param>
        /// <returns>The updated review object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Review))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (review == null)
            {
                return BadRequest("Please provide student data");
            }
            Review existedReview = await unitOfWork.ReviewGenericRepository.GetById(id);
            if (existedReview != null && existedReview.Id != review.Id)
            {
                return NotFound($"There is a Review with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.ReviewGenericRepository.Update(id, review);
                await unitOfWork.Save();
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new review.
        /// </summary>
        /// <param name="review">The review object to add.</param>
        /// <returns>The added review object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Review))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddReview(Review review)
        {
            if (review == null)
            {
                return BadRequest("Please, enter Review's data");
            }

            try
            {
                review = await unitOfWork.ReviewGenericRepository.Add(review);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetReviewByID), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a review by ID.
        /// </summary>
        /// <param name="id">The ID of the review to delete.</param>
        /// <returns>The deleted review object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Review))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            Review review = await unitOfWork.ReviewGenericRepository.GetById(id);
            if (review == null)
            {
                return NotFound("Review doesn't exist");
            }
            await unitOfWork.ReviewGenericRepository.Delete(review);
            await unitOfWork.Save();
            return Ok(review);
        }
    }
}