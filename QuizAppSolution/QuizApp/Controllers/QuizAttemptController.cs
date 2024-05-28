using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.ResponseDTO;
using System.Security.Claims;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAttemptController : ControllerBase
    {
        private readonly IQuizResponseServices _quizResponseServices;

        public QuizAttemptController(IQuizResponseServices quizResponseServices)
        {
            _quizResponseServices = quizResponseServices;
        }

        [Authorize]
        [HttpPost("StartQuiz")]
        [ProducesResponseType(typeof(StartQuizResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StartQuizResponseDTO>> StartQuiz([FromBody] int QuizId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                var quizDetails = await _quizResponseServices.StartQuizAsync(Convert.ToInt32(userId), QuizId);
                return Ok(quizDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("submitAnswer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SubmitAnswer([FromBody] SubmitAnswerDTO submitAnswerDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                submitAnswerDTO.UserId = Convert.ToInt32(userId);

                string result= await _quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("submitAllAnswer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
