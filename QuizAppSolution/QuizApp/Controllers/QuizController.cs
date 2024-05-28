using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using QuizApp.Models;
using QuizApp.Models.DTOs.QuizDTOs;

namespace QuizApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizServices _quizServices;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizServices quizServices, ILogger<QuizController> logger)
        {
            _quizServices = quizServices;
            _logger = logger;
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("AddQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> AddQuiz([FromBody] QuizInputDTO quizInputDTO)
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
                    IsMultpleAttemptAllowed = quizInputDTO.IsMultipleAttemptAllowed

                };

                var result = await _quizServices.AddQuizAsync(quizDTO);
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }


        [Authorize(Roles = "Teacher")]
        [HttpPut("EditQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> EditQuiz([FromBody] QuizUpdateDTO updateQuizDTO)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _quizServices.EditQuizByIDAsync(updateQuizDTO,userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(NoSuchQuizException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(QuizDeletedException ex)
            {
                return StatusCode(410, new ErrorModel(410, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("DeleteQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> DeleteQuiz([FromBody] int QuizId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _quizServices.DeleteQuizByIDAsync(QuizId,userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (NoSuchQuizException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(UnauthorizedToDeleteException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }


        [Authorize(Roles = "Teacher")]
        [HttpDelete("SoftDeleteQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> SoftDeleteQuiz([FromBody] int QuizId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _quizServices.SoftDeleteQuizByIDAsync(QuizId, userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (NoSuchQuizException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(UnauthorizedToDeleteException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("UndoSoftDelete")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> UndoSoftDeleteByID([FromBody] int QuizId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _quizServices.UndoSoftDeleteQuizByIDAsync(QuizId, userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (NoSuchQuizException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(UnauthorizedToEditException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("CreateQuizByExistingQuiz")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> CreateQuizByExistingQuiz(int QuizId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _quizServices.CreateQuizFromExistingQuiz(QuizId, userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (NoSuchQuizException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

    }
}
