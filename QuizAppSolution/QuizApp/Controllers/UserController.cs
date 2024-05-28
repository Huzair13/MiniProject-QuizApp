using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Services;
using QuizApp.Models.DTOs.UserDTOs;

namespace QuizApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController :ControllerBase
    {
        private readonly IUserLoginAndRegisterServices _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserLoginAndRegisterServices userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userService.Login(userLoginDTO);
                    _logger.LogInformation("Login successful for user: {UserID}", userLoginDTO.UserId);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login failed for user: {UserID}", userLoginDTO.UserId);
                    return Unauthorized(new ErrorModel(401, ex.Message));
                }
            }

            return BadRequest("Details are Missing . Please check the input object");

        }


        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegisterReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Register([FromBody] UserRegisterInputDTO userInputDTO)
        {
            try
            {
                object result = await _userService.Register(userInputDTO);
                //_logger.LogInformation($"Registration successful for user: {((Teacher)result).Id}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for user");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }


    
}
