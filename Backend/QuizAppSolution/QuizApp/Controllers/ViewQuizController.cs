using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models;
using System.Security.Claims;
using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewQuizController : ControllerBase
    {
        //INITIALIZATION
        private readonly IQuizServices _quizServices;
        private readonly ILogger<QuizController> _logger;

        //DEPENDENCY INJECTION
        [ExcludeFromCodeCoverage]
        public ViewQuizController(IQuizServices quizServices, ILogger<QuizController> logger)
        {
            _quizServices = quizServices;
            _logger = logger;
        }

        //GET ALL QUIZZES
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllQuizzes")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllQuizzes()
        {
            try
            {
                var result = await _quizServices.GetAllQuizzesAsync();
                return Ok(result);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the quiz.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //GET ALL SOFT DELETED QUIZZES
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllSoftDeletedQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllSoftDeletedQuiz()
        {
            try
            {
                var result = await _quizServices.GetAllSoftDeletedQuizzesAsync();
                return Ok(result);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the soft deleted quiz.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while gettings the soft deleted quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //GET ALL QUIZZES CREATED BY THE LOGGED IN TEACHER
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetQuizzesBySpecificTeacher")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetQuizzesBySpecificTeacher()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _quizServices.GetAllQuizzesCreatedByLoggedInTeacherAsync(userId);
                return Ok(result);
            }
            catch(NoSuchUserException ex)
            {
                _logger.LogError(ex, "User Not found while getting the created quiz.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the created quiz.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the created quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }
    }
}
