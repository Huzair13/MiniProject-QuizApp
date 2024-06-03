using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.ResponseDTO;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAttemptController : ControllerBase
    {
        //INITIALIZATION
        private readonly IQuizResponseServices _quizResponseServices;
        private readonly ILogger<QuizController> _logger;

        //DEPENDENCY INJECTION
        [ExcludeFromCodeCoverage]
        public QuizAttemptController(IQuizResponseServices quizResponseServices, ILogger<QuizController> logger)
        {
            _quizResponseServices = quizResponseServices;
            _logger = logger;
        }

        //START QUIZ
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("StartQuiz")]
        [ProducesResponseType(typeof(StartQuizResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StartQuizResponseDTO>> StartQuiz([FromBody] int QuizId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                var quizDetails = await _quizResponseServices.StartQuizAsync(Convert.ToInt32(userId), QuizId);
                return Ok(quizDetails);
            }
            catch (QuizAlreadyStartedException ex)
            {
                _logger.LogError(ex, "Quiz already started.");
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while starting the quiz.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //SUBMIT ANSWER
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("submitAnswer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SubmitAnswer([FromBody] SubmitAnswerDTO submitAnswerDTO)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);
                    submitAnswerDTO.UserId = Convert.ToInt32(userId);

                    string result = await _quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);
                    return Ok(result);
                }
                catch (NoSuchQuizException ex)
                {
                    _logger.LogError(ex, "Quiz Not found while Submitting the answer.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while Submitting the answer.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UserAlreadyAnsweredTheQuestionException ex)
                {
                    _logger.LogError(ex, "User already answered the question.");
                    return Conflict(new ErrorModel(409, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while Submitting the answer.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details are not provided");
        }

        //SUBMIT ALL ANSWER
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("submitAllAnswer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status408RequestTimeout)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SubmitAllAnswer([FromBody] SubmitAllAnswersDTO submitAllAnswersDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                submitAllAnswersDTO.UserId = Convert.ToInt32(userId);

                var result= await _quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO);
                return Ok(result);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while Submitting the answer.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (QuizNotStartedException ex)
            {
                _logger.LogError(ex, "Quiz has not started.");
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while Submitting the answer.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UserAlreadyAnsweredTheQuestionException ex)
            {
                _logger.LogError(ex, "Already Answered the question.");
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (QuizTimeLimitExceededException ex)
            {
                _logger.LogError(ex, "Quiz time limit exceeded.");
                return StatusCode(408, new ErrorModel(408, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Submitting the answer.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //CHECK RESULT
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpPost("checkResult")]
        [ProducesResponseType(typeof(QuizResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizResultDTO>> GetQuizResult([FromBody]int quizId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _quizResponseServices.GetQuizResultAsync(userId, quizId);
                return Ok(result);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the result");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the result.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //LEADERBOARD
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("LeaderBoard")]
        [ProducesResponseType(typeof(List<LeaderboardDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<LeaderboardDTO>>> GetLeaderboard(int quizId)
        {
            try
            {
                var leaderboard = await _quizResponseServices.GetQuizLeaderboardAsync(quizId);
                return Ok(leaderboard);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the leaderboard");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the leaderboard.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //STUDENT LEADERBOARD
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("StudentLeaderBoard")]
        [ProducesResponseType(typeof(List<LeaderboardDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<LeaderboardDTO>>> GetStudentLeaderboard(int quizId)
        {
            try
            {
                var leaderboard = await _quizResponseServices.GetStudentQuizLeaderboardAsync(quizId);
                return Ok(leaderboard);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the student Leaderboard");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the student Leaderboard.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //STUDENT POSITION
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("GetStudentPosition")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetStudentPosition(int quizId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var leaderboard = await _quizResponseServices.GetStudentPositionInLeaderboardAsync(userId, quizId);
                return Ok(leaderboard);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found while getting the student position in the quiz");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the student position in the quiz.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //GET ALL RESPONSES
        [ExcludeFromCodeCoverage]
        [Authorize]
        [HttpGet("GetAllUserResponses")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizResponseDTO>> GetAllUserResponses()
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var responses = await _quizResponseServices.GetAllUseresponsesAsync(userId);
                return Ok(responses);
            }
            catch(NoSuchUserException ex)
            {
                _logger.LogError(ex, "user not found while getting user responses");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user responses.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }
    }
}
