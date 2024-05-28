﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
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

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllQuestions")]
        [ProducesResponseType(typeof(IList<QuestionReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<Question>>> GetAllQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllMCQQuestions")]
        [ProducesResponseType(typeof(IList<QuestionReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<MultipleChoice>>> GetAllMCQQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllMCQQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllFillUpsQuestions")]
        [ProducesResponseType(typeof(IList<FillUpsReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<MultipleChoice>>> GetAllFillUpsQuestions()
        {
            try
            {
                var result = await _questionServices.GetAllFillUpsQuestionsAsync();
                return Ok(result);
            }
            catch (NoSuchQuestionException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("AddMCQQuestion")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> AddMCQQuestion([FromBody] MCQInputDTO inputDTO)
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
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("AddFillUpsQuestion")]
        [ProducesResponseType(typeof(FillUpsReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> AddFillUpsQuestion([FromBody] FillUpsInputDTO inputDTO)
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
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        //[Authorize(Roles = "Teacher")]
        [HttpPut("EditFillUps")]
        [ProducesResponseType(typeof(FillUpsReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> EditFillUps([FromBody] FillUpsUpdateDTO inputDTO)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _questionServices.EditFillUpsQuestionById(inputDTO,userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("EditMCQ")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> EditMCQ([FromBody] MCQUpdateDTO inputDTO)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _questionServices.EditMCQByQuestionID(inputDTO,userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("EditQuestionByID")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionReturnDTO>> EditQuestionByID([FromBody] UpdateQuestionDTO inputDTO)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _questionServices.EditQuestionByID(inputDTO, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }


        [Authorize(Roles = "Teacher")]
        [HttpDelete("DeleteQuestionByID")]
        [ProducesResponseType(typeof(QuestionReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<QuestionDTO>> DeleteQuestionById(int questionId)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var result = await _questionServices.DeleteQuestionByID(questionId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

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
