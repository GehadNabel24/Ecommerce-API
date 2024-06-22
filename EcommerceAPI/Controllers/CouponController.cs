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
    public class CouponController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly unitOfWork unitOfWork;

        public CouponController(unitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all coupons.
        /// </summary>
        /// <returns>A list of coupon DTOs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CouponDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCoupons()
        {
            try
            {
                var Coupons = await unitOfWork.CouponGenericRepository.GetAll();
                var CDTOs = Coupons.Select(p => mapper.Map<CouponDTO>(p));
                return Ok(CDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Retrieves a coupon by ID.
        /// </summary>
        /// <param name="id">The ID of the coupon to retrieve.</param>
        /// <returns>The coupon DTO.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CouponDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCouponByID(int id)
        {
            try
            {
                var coupon = await unitOfWork.CouponGenericRepository.GetById(id);
                if (coupon == null)
                {
                    return BadRequest("No such coupon with the provided id");
                }
                var CDTOs = mapper.Map<CouponDTO>(coupon);
                return Ok(CDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Updates a coupon.
        /// </summary>
        /// <param name="id">The ID of the coupon to update.</param>
        /// <param name="coupon">The updated coupon object.</param>
        /// <returns>The updated coupon object.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCoupon(int id, Coupon coupon)
        {
            if (coupon == null)
            {
                return BadRequest("Please provide student data");
            }
            Coupon existedCoupon = await unitOfWork.CouponGenericRepository.GetById(id);
            if (existedCoupon != null && existedCoupon.Id != coupon.Id)
            {
                return NotFound($"There is a coupon with this ID : {id}, Please, Change it");
            }

            try
            {
                unitOfWork.CouponGenericRepository.Update(id, coupon);
                await unitOfWork.Save(); 
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error !!");
            }
        }

        /// <summary>
        /// Adds a new coupon.
        /// </summary>
        /// <param name="coupon">The coupon object to add.</param>
        /// <returns>The added coupon object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Coupon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddCoupon(Coupon coupon)
        {
            if (coupon == null)
            {
                return BadRequest("Please, enter coupon's data");
            }

            try
            {
                coupon = await unitOfWork.CouponGenericRepository.Add(coupon);
                await unitOfWork.Save();
                return CreatedAtAction(nameof(GetCouponByID), new { id = coupon.Id }, coupon);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a coupon by ID.
        /// </summary>
        /// <param name="id">The ID of the coupon to delete.</param>
        /// <returns>The deleted coupon object.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            Coupon coupon = await unitOfWork.CouponGenericRepository.GetById(id);
            if (coupon == null)
            {
                return NotFound("Coupon doesn't exist");
            }
            await unitOfWork.CouponGenericRepository.Delete(coupon);
            await unitOfWork.Save();
            return Ok(coupon);
        }
    }
}