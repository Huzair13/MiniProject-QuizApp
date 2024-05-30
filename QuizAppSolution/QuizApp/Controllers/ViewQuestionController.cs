using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Models;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Services;
using System.Security.Claims;

namespace QuizApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewQuestionController :ControllerBase
    {
        private readonly IQuestionViewServices _questionServices;
        private readonly ILogger<ViewQuestionController> _logger;

        public ViewQuestionController(IQuestionViewServices questionServices, ILogger<ViewQuestionController> logger)
        {
            _questionServices = questionServices;
            _logger = logger;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllQuestions")]
        [ProducesResponseType(typeof(IList<QuestionReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<Question>>> GetAllQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the question");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllMCQQuestions")]
        [ProducesResponseType(typeof(IList<QuestionReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<MultipleChoice>>> GetAllMCQQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllMCQQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the MCQ question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the MCQ Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllFillUpsQuestions")]
        [ProducesResponseType(typeof(IList<FillUpsReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<MultipleChoice>>> GetAllFillUpsQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllFillUpsQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the Fillups question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Fillups Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllSoftDeletedQuestion")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllSoftDeletedQuestion()
        {
            try
            {
                var result = await _questionServices.GetAllSoftDeletedQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the sof deleted question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the soft deleted Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllHardQuestions")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllHardQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllHardQuestions();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the Hard question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Hard Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllMediumQuestions")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllMediumQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllMediumQuestions();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the Medium question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Medium Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllEasyQuestions")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllEasyQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllEasyQuestions();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the Easy question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Easy Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllCreatedQuestion")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> GetAllCreatedQuestion()
        {
            try
            {
                int userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _questionServices.GetAllQuestionsCreatedByLoggedInTeacherAsync(userID);
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while getting the created question");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the created Question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }
    }
}
