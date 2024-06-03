using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Services;
using QuizApp.Models.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController :ControllerBase
    {
        //INITIALIZATION
        private readonly IUserLoginAndRegisterServices _userLoginService;
        private readonly ILogger<QuizController> _logger;

        //DEPENDNCY INJECTION
        [ExcludeFromCodeCoverage]
        public UserController(IUserLoginAndRegisterServices userLoginService, ILogger<QuizController> logger)
        {
            _userLoginService = userLoginService;
            _logger = logger;
        }


        //LOGIN
        [ExcludeFromCodeCoverage]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userLoginService.Login(userLoginDTO);
                    _logger.LogInformation("Login successful for user: {UserID}", userLoginDTO.UserId);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login failed for user: {UserID}", userLoginDTO.UserId);
                    return Unauthorized(new ErrorModel(401, ex.Message));
                }
            }

            return BadRequest("Details are Missing");

        }

        //REGISTER
        [ExcludeFromCodeCoverage]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegisterReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterReturnDTO>> Register([FromBody] UserRegisterInputDTO userInputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    object result = await _userLoginService.Register(userInputDTO);
                    _logger.LogInformation($"Registration successful for user");
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Registration failed for user");
                    return BadRequest(new ErrorModel(501, ex.Message));
                }  
            }
            return BadRequest("Details are Missing");

        }

    }
    
}
