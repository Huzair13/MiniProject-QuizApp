using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using QuizApp.Models;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        //INITIALIZATION
        private readonly IQuizServices _quizServices;
        private readonly ILogger<QuizController> _logger;

        //DEPENDENCY INJECTION
        [ExcludeFromCodeCoverage]
        public QuizController(IQuizServices quizServices, ILogger<QuizController> logger)
        {
            _quizServices = quizServices;
            _logger = logger;
        }

        //ADD QUIZ
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> AddQuiz([FromBody] QuizInputDTO quizInputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);

                    QuizDTO quizDTO = new QuizDTO
                    {
                        QuizName = quizInputDTO.QuizName,
                        QuizDescription = quizInputDTO.QuizDescription,
                        QuizType = quizInputDTO.QuizType,
                        QuizCreatedBy = Convert.ToInt32(userId),
                        QuestionIds = quizInputDTO.QuestionIds,
                        TimeLimit = quizInputDTO.TimeLimit,
                        IsMultpleAttemptAllowed = quizInputDTO.IsMultipleAttemptAllowed

                    };

                    var result = await _quizServices.AddQuizAsync(quizDTO);
                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while adding the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while adding the quiz.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");
        }

        //EDIT QUIZ
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpPut("EditQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> EditQuiz([FromBody] QuizUpdateDTO updateQuizDTO)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                    var result = await _quizServices.EditQuizByIDAsync(updateQuizDTO, userId);

                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while adding the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while updating the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (QuizDeletedException ex)
                {
                    _logger.LogError(ex, "Quiz Deleted while updating the quiz.");
                    return StatusCode(410, new ErrorModel(410, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the quiz ");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");

        }

        //DELETE QUIZ
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpDelete("DeleteQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> DeleteQuiz(QuizIDDTO QuizId)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                    var result = await _quizServices.DeleteQuizByIDAsync(QuizId.QuizId, userId);

                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while deleting the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while deleting the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UnauthorizedToDeleteException ex)
                {
                    _logger.LogError(ex, "Unauthorized to delete the quiz.");
                    return Unauthorized(new ErrorModel(403, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the quiz.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");
        }

        //SOFT DELETE QUIZ
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpDelete("SoftDeleteQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> SoftDeleteQuiz(QuizIDDTO QuizId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                    var result = await _quizServices.SoftDeleteQuizByIDAsync(QuizId.QuizId, userId);

                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while soft deleting the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while soft deleting the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UnauthorizedToDeleteException ex)
                {
                    _logger.LogError(ex, "Unauthorized to soft delete the quiz.");
                    return Unauthorized(new ErrorModel(403, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the quiz.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");

        }

        //UNDO SOFT DELETE
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpPost("UndoSoftDelete")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> UndoSoftDeleteByID(QuizIDDTO QuizId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                    var result = await _quizServices.UndoSoftDeleteQuizByIDAsync(QuizId.QuizId, userId);

                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while unoding soft delete.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while undoing soft delete.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UnauthorizedToEditException ex)
                {
                    _logger.LogError(ex, "Unauthorized to undo soft delete.");
                    return Unauthorized(new ErrorModel(403, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while undoing the quiz soft delete.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");
        }

        //CREATE QUIZ FROM EXISTING QUIZ
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpPost("CreateQuizByExistingQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> CreateQuizByExistingQuiz(QuizIDDTO QuizId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                    var result = await _quizServices.CreateQuizFromExistingQuiz(QuizId.QuizId, userId);

                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while creating the quiz from existing quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while creating the quiz from existing quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the quiz from existing quiz.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");

        }

        //ADD QUESTIONS
        [ExcludeFromCodeCoverage]
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddQuestions")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> AddQuestionsToQuiz([FromBody] AddQuestionsToQuizDTO inputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                    var result = await _quizServices.AddQuestionsToQuizAsync(inputDTO, userId);
                    return Ok(result);
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while adding questions to the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while adding questions to the quiz.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while adding questions the quiz.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");
        }

    }
}
