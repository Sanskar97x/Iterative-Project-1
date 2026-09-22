using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using IterativeProject1.Services;
using IterativeProject1.DTOs;
using Microsoft.AspNetCore.Authorization;



namespace IterativeProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        private string GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "Client";

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();

            try
            {
                var result = await _bookingService.GetById(id, userId, userRole);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpPost("CreateBooking")]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            var clientId = GetUserId();
            try
            {
                var result = await _bookingService.Create(clientId, dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var clientId = GetUserId();
            var bookings = await _bookingService.GetByClient(clientId);
            return Ok(bookings);
        }

        [HttpGet] 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAll();
            return Ok(bookings);
        }



        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();

            try
            {
                var success = await _bookingService.Cancel(id, userId, userRole);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
 

            [HttpPatch("{id}/reschedule")]
            public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleBookingDto dto)
            {
                var userId = GetUserId();

                try
                {
                    var result = await _bookingService.Reschedule(id, userId, dto.NewStart, dto.NewEnd);
                    return Ok(result);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
                catch (InvalidOperationException ex)
                {
                    return Conflict(new { message = ex.Message });
                }
                
            }
           

            [HttpPatch("{id}/status")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto dto)
            {
                try
                {
                    var success = await _bookingService.UpdateStatus(id, dto.Status);
                    if (!success) return NotFound();
                    return NoContent();
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
        }
 }

