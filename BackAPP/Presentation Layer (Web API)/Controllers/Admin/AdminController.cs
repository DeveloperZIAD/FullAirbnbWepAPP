using AirbnbClone.Api.Controllers.Authentication;
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Service.Admin;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirbnbClone.Api.Controllers.Admin
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController()
        {
            _adminService = new AdminService();
        }

        [HttpGet("dashboard-stats")]
        
        public IActionResult GetStats()
        {
            var stats = _adminService.GetDashboardStats();
            return Ok(stats);
        }
        // GetDashboardStats new one too with the data
        [HttpGet("users")]
        
        public IActionResult GetAllUsers()
        {
            var users = _adminService.GetAllUsersDetailed(); // make sure this send all the data he shud send all of it
            return Ok(users);
        }

        [HttpPatch("users/{userId}/block")]
       
        public IActionResult BlockUser(Guid userId)
        {
            var result = _adminService.ToggleUserBlockStatus(userId, true);
            if (!result) return BadRequest(new { Message = "Failed to block user." });
            return Ok(new { Message = "User has been blocked." });
        }

        [HttpGet("activity-logs")]
        
        public IActionResult GetActivityLogs()
        {
            var logs = _adminService.GetSystemLogs();
            return Ok(logs);
        }

        [HttpGet("listings")]
        
        public IActionResult GetAllListings()
        {
            var listings = _adminService.GetAllListingsDetailed();
            if (listings == null) return NotFound(new { Message = "No listings found." });

            return Ok(listings);
        }

        [HttpGet("reservations")]
        
        public IActionResult GetAllReservations()
        {
            var reservations = _adminService.GetAllReservationsDetailed();
            if (reservations == null) return NotFound(new { Message = "No reservations found." });

            return Ok(reservations);
        }

    }
}