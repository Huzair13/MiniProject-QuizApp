using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace QuizApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController :ControllerBase
    {
        private readonly IQuestionServices _questionServices;
        private readonly ILogger<UserController> _logger;

        public QuestionController(IQuestionServices questionServices, ILogger<UserController> logger)
        {
            _questionServices = questionServices;
            _logger = logger;
        }

        //ADD MCQ CONTROLLER
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddMCQQuestion")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> AddMCQQuestion([FromBody] MCQInputDTO inputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);

                    MCQDTO mCQDTO = MapInputDTOToMCQDTO(inputDTO);

                    mCQDTO.CreatedBy = Convert.ToInt32(userId);
                    mCQDTO.CreatedDate = DateTime.Now;

                    var result = await _questionServices.AddMCQQuestion(mCQDTO);

                    return Ok(result);
                }
                catch (NoSuchUserException ex)
                {
                    _logger.LogError(ex, "User Not found while adding the MCQ Question.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while adding the MCQ Question.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details not provided");
        }

        //ADD FILL UPS
        [Authorize(Roles = "Teacher")]
        [HttpPost("AddFillUpsQuestion")]
        [ProducesResponseType(typeof(FillUpsReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> AddFillUpsQuestion([FromBody] FillUpsInputDTO inputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.Name);

                    FillUpsDTO fillUpsDTO = MapInputDTOToFiilUpsDTO(inputDTO);

                    fillUpsDTO.CreatedBy = Convert.ToInt32(userId);
                    fillUpsDTO.CreatedDate = DateTime.Now;

                    var result = await _questionServices.AddFillUpsQuestion(fillUpsDTO);

                    return Ok(result);
                }
                catch (NoSuchUserException ex)
                {
                    _logger.LogError(ex, "User Not found while adding the FillUps Question.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while adding the FillUps Question.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details not provided");
        }

        //EDIT FILL UPS
        [Authorize(Roles = "Teacher")]
        [HttpPut("EditFillUps")]
        [ProducesResponseType(typeof(FillUpsReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FillUpsReturnDTO>> EditFillUps([FromBody] FillUpsUpdateDTO inputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                    var result = await _questionServices.EditFillUpsQuestionById(inputDTO, userId);
                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while editing the fillups.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UnauthorizedToEditException ex)
                {
                    _logger.LogError(ex, "An error occurred while editing the fillups.");
                    return Unauthorized(new ErrorModel(403, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while editing the fillups.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details not provided");
        }

        //EDIT MCQ
        [Authorize(Roles = "Teacher")]
        [HttpPut("EditMCQ")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionReturnDTO>> EditMCQ([FromBody] MCQUpdateDTO inputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                    var result = await _questionServices.EditMCQByQuestionID(inputDTO, userId);
                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while editing the MCQ.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UnauthorizedToEditException ex)
                {
                    _logger.LogError(ex, "An error occurred while editing the MCQ.");
                    return Unauthorized(new ErrorModel(403, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while editing the MCQ.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details not provided");
        }

        //EDIT QUESTION
        [Authorize(Roles = "Teacher")]
        [HttpPut("EditQuestionByID")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionReturnDTO>> EditQuestionByID([FromBody] UpdateQuestionDTO inputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                    var result = await _questionServices.EditQuestionByID(inputDTO, userId);
                    return Ok(result);
                }
                catch (NoSuchQuestionException ex)
                {
                    _logger.LogError(ex, "Question Not found while editing the Question By ID.");
                    return NotFound(new ErrorModel(404, ex.Message));
                }
                catch (UnauthorizedToEditException ex)
                {
                    _logger.LogError(ex, "An error occurred while editing the Question By ID.");
                    return Unauthorized(new ErrorModel(403, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while editing the Question By ID.");
                    return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
                }
            }
            return BadRequest("All Details not provided");
        }

        //DELETE QUESTION
        [Authorize(Roles = "Teacher")]
        [HttpDelete("DeleteQuestionByID")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDTO>> DeleteQuestionById(int questionId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _questionServices.DeleteQuestionByID(questionId, userId);
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while Deleting the Question By ID.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UnauthorizedToDeleteException ex)
            {
                _logger.LogError(ex, "Unathorized while Deleting the Question By ID.");
                return Unauthorized(new ErrorModel(403, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Deleting the Question By ID.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //SOFT DELETE QUESTION
        [Authorize(Roles = "Teacher")]
        [HttpDelete("SoftDeleteQuestion")]
        [ProducesResponseType(typeof(QuestionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuestionDTO>> SoftDeleteQuestion(int QuestionID)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _questionServices.SoftDeleteQuestionByIDAsync(QuestionID, userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while Soft Deleting the Question By ID.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UnauthorizedToDeleteException ex)
            {
                _logger.LogError(ex, "Unathorized while soft Deleting the Question By ID.");
                return Unauthorized(new ErrorModel(403, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while soft deleting the question.");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //UNDO SOFT DELETE
        [Authorize(Roles = "Teacher")]
        [HttpPost("UndoSoftDelete")]
        [ProducesResponseType(typeof(QuizReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizReturnDTO>> UndoSoftDeleteByID(int QuestionID)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));

                var result = await _questionServices.UndoSoftDeleteQuestionByIDAsync(QuestionID, userId);

                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found while undoing Soft Delete.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UnauthorizedToEditException ex)
            {
                _logger.LogError(ex, "Unathorized while undoing the soft Delete");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred undoing the soft Delete");
                return StatusCode(500, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        //MAP INPUT DTO TO FILL UPS DTO
        private FillUpsDTO MapInputDTOToFiilUpsDTO(FillUpsInputDTO inputDTO)
        {
            FillUpsDTO fillUpsDTO = new FillUpsDTO();
            fillUpsDTO.Category = inputDTO.Category;
            fillUpsDTO.QuestionText = inputDTO.QuestionText;
            fillUpsDTO.DifficultyLevel = inputDTO.DifficultyLevel;
            fillUpsDTO.Points = inputDTO.Points;
            fillUpsDTO.CorrectAnswer = inputDTO.CorrectAnswer;
            return fillUpsDTO;
        }

        //MAP INPUT DTO TO MCQ DTO
        private MCQDTO MapInputDTOToMCQDTO(MCQInputDTO inputDTO)
        {
            MCQDTO mCQDTO= new MCQDTO();
            mCQDTO.Category = inputDTO.Category;
            mCQDTO.QuestionText = inputDTO.QuestionText;
            mCQDTO.DifficultyLevel = inputDTO.DifficultyLevel;
            mCQDTO.Points = inputDTO.Points;
            mCQDTO.Choice1 = inputDTO.Choice1;
            mCQDTO.Choice2 = inputDTO.Choice2;
            mCQDTO.Choice3 = inputDTO.Choice3;
            mCQDTO.Choice4 = inputDTO.Choice4;
            mCQDTO.CorrectAnswer = inputDTO.CorrectAnswer;
            return mCQDTO;

        }
    }
}
