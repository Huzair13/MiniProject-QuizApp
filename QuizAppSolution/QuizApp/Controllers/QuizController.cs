using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using QuizApp.Models;

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

        [Authorize]
        [HttpPost("AddQuiz")]
        [ProducesResponseType(typeof(Quiz), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Quiz>> AddQuiz([FromBody] QuizInputDTO quizInputDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                if (userId == null)
                {
                    return Unauthorized();
                }

                QuizDTO quizDTO = new QuizDTO
                {
                    QuizName = quizInputDTO.QuizName,
                    QuizDescription = quizInputDTO.QuizDescription,
                    QuizType = quizInputDTO.QuizType,
                    QuizCreatedBy = Convert.ToInt32(userId),
                    QuestionIds = quizInputDTO.QuestionIds
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
    }
}
