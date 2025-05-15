using EmployeesPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vacation_Service.DTOs;
using Vacation_Service.Interfaces;

namespace Vacation_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacationController : ControllerBase
    {
        private readonly IVacationService _vacationService;
        private readonly IRabbitMqIntgration _rabbitMqIntgration;

        public VacationController(IVacationService vacationService, IRabbitMqIntgration rabbitMqIntgration)
        {
            _vacationService = vacationService;
            _rabbitMqIntgration = rabbitMqIntgration;
        }
        [Authorize(Roles = "Employee")]
        [HttpPost("CreateVacationRequest")]
        public async Task<IActionResult> CreateVacationRequest([FromBody] VacationRequestCreateDTO request)
        {
            var result = await _vacationService.CreateVacationRequestAsync(request);
            return Ok("You have succefully Created Request Vacation");
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("GetVacationRequestById/{id}")]
        public async Task<IActionResult> GetVacationRequestById(int id)
        {
            var result = await _vacationService.GetVacationRequestByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Manager")]

        [HttpGet("GetAllVacationRequessOfManagerId/{id}")]
        public async Task<IActionResult> GetAllVacationRequessOfManagerId(int id)
        {
            var result = await _vacationService.GetVacationRequestsByManagerIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Employee")]

        [HttpPut("UpdateVacationRequest")]
        public async Task<IActionResult> UpdateVacationRequest([FromBody] VacationRequestUpdateDTO request)
        {
            var result = await _vacationService.UpdateVacationRequestDTOAsync(request);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Employee")]

        [HttpDelete("DeleteVacationRequest/{id}")]
        public async Task<IActionResult> DeleteVacationRequest(int id)
        {
            var success = await _vacationService.DeleteVacationRequestAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Manager")]

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveVacationRequest(int id)
        {
            var success = await _vacationService.ApproveVacationRequestAsync(id);
            if (!success) return NotFound();
            //broad cast vaction
            var v = await _vacationService.GetVacationRequestByIdAsync(id);
            _rabbitMqIntgration.Publish(v);
            return Ok();
        }
        [Authorize(Roles = "Manager")]

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectVacationRequest(int id)
        {
            var success = await _vacationService.RejectVacationRequestAsync(id);
            if (!success) return NotFound();
            return Ok();
        }
    }

}
