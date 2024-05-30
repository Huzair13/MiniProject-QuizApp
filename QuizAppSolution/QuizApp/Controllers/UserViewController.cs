using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Interfaces;
using QuizApp.Models.DTOs.UserDTOs;
using QuizApp.Models;
using System.Security.Claims;

namespace QuizApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserViewController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<UserViewController> _logger;

        public UserViewController(IUserServices userServices ,ILogger<UserViewController> logger) 
        {
            _userServices = userServices;
            _logger = logger;
        }


        [Authorize(Roles = "Student")]
        [HttpGet("ViewStudentProfile")]
        [ProducesResponseType(typeof(StudentReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentReturnDTO>> ViewStudentProfile()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                StudentReturnDTO result = await _userServices.ViewStudentProfile(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Occured while Viewing Student Profile");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("ViewTeacherProfile")]
        [ProducesResponseType(typeof(TeacherReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentReturnDTO>> ViewTeacherProfile()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                TeacherReturnDTO result = await _userServices.ViewTeacherProfile(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Occured while Viewing Teacher Profile");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}
