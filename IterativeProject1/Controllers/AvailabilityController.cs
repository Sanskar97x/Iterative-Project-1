using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using IterativeProject1.DTOs;


namespace IterativeProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService) 
        {
            _availabilityService = availabilityService;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        // Defines an API endpoint named Create that accepts availability data
        // and returns an HTTP response.
        public async Task<IActionResult>Create([FromBody] CreateAvailabilityDto dto)
        {
            // Gets the logged-in user's ID from their authentication claim.
            // FindFirst() looks for the NameIdentifier claim.
            // Value gets the ID as a string.
            // int.Parse() converts that string into an integer.
            var providerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Calls the availability service to create a new availability
            // using the provider's ID and the data received from the client.
            // The returned value is the ID of the newly created availability.
            var id = await _availabilityService.Create(providerId, dto);
            //providerId and dto here are not being created. 
            //They are variables that already contain the values you're passing into the service.

            // Returns HTTP 201 (Created) and sends the newly created ID
            // back to the client.
            return CreatedAtAction(nameof(Create), new { id }, new { id });
        }


        [HttpGet("slots")]
        public async Task<IActionResult> GetOpenSlots([FromQuery] int providerId, [FromQuery] DateTime date, [FromQuery] int slotDurationMinutes = 30)
        {
            var slots = await _availabilityService.GetOpenSlots(providerId, date, slotDurationMinutes);
            return Ok(slots);

        }

        //This endpoint lets an Admin delete an availability by ID, returning 404 if it doesn't exist and 204 if deletion succeeds.
        [HttpDelete("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _availabilityService.Delete(id);
            if (!success) return NotFound(); // 404 : The requested resource was not found
            return NoContent(); // 204 : the operation succeed but there is no data to return
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMine()
        {
            var providerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var availabilities = await _availabilityService.GetMine(providerId);
            return Ok(availabilities);
        }

    }
}
