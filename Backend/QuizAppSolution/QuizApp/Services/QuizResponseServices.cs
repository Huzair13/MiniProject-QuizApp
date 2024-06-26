using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.ResponseDTO;
using QuizApp.Models.DTOs.ResponseDTOs;

namespace QuizApp.Services
{
    public class QuizResponseServices : IQuizResponseServices
    {
        //REPOSITORIES
        private readonly IRepository<int, Quiz> _quizRepo;
        private readonly IRepository<int, Question> _questionRepository;
        private readonly IRepository<int, Response> _responseRepo;
        private readonly IRepository<int,Student> _studentRepo;
        private readonly IRepository<int, User> _userRepo;
        private readonly ILogger<QuizResponseServices> _logger;


        //INJECTING THE REPOSITORY
        public QuizResponseServices(
            IRepository<int, Quiz> quizRepo,IRepository<int, Question> questionRepository,
            IRepository<int, Response> responseRepo, IRepository<int, Student> studentRepo
            ,IRepository<int,User> userRepo, ILogger<QuizResponseServices> logger)
        {
            _quizRepo = quizRepo;
            _questionRepository = questionRepository;
            _responseRepo = responseRepo;
            _studentRepo = studentRepo;
            _userRepo = userRepo;
            _logger = logger;

        }

        //GET QUIZ RESULT
        public async Task<IList<QuizResultDTO>> GetQuizResultAsync(int userId, int quizId)
        {
            try
            {
                List<QuizResultDTO> QuizResults = new List<QuizResultDTO>();

                Quiz quiz = await _quizRepo.Get(quizId);

                var AllResponses = await _responseRepo.Get();
                var responses = AllResponses
                            .Where(r => r.UserId == userId && r.QuizId == quizId)
                            .OrderByDescending(r => r.ScoredPoints).ThenBy(r=>r.TimeTaken);
                foreach(var response in responses)
                {
                    var answeredQuestions = response.ResponseAnswers.Select(ra => new AnsweredQuestionDTO
                    {
                        QuestionId = ra.QuestionId,
                        SubmittedAnswer = ra.SubmittedAnswer,
                        CorrectAnswer = (ra.Question is MultipleChoice mcq) ? mcq.CorrectChoice : (ra.Question is FillUps fillUps) ? fillUps.CorrectAnswer : null,
                        IsCorrect = ra.IsCorrect
                    }).ToList();

                    QuizResults.Add(new QuizResultDTO
                    {
                        ResponseId = response.Id,
                        UserId = userId,
                        QuizId = quizId,
                        Score = response.ScoredPoints,
                        TimeTaken = response.TimeTaken,
                        AnsweredQuestions = answeredQuestions
                    });
                }
                return QuizResults;
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found at GetQuizResult Service");
                throw new NoSuchQuizException(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured at GetQuizResult Service");
                throw new Exception(ex.Message);
            }

        }

        //START QUIZ
        public async Task<StartQuizResponseDTO> StartQuizAsync(int userId, int quizId)
        {
            try
            {
                var quiz = await _quizRepo.Get(quizId);
                bool isMultipleAttemptAllowed = quiz.IsMultipleAttemptAllowed;

                var responses = await _responseRepo.Get();
                var existingResponse = responses.FirstOrDefault(r => r.UserId == userId && r.QuizId == quizId);
                if (existingResponse!=null)
                {
                    if (!isMultipleAttemptAllowed)
                    {
                        throw new QuizAlreadyStartedException(quizId);
                    }
                    else
                    {
                        existingResponse.EndTime = DateTime.Now;
                        await _responseRepo.Update(existingResponse);
                        return await StartMultipleAttemptQuiz(userId, quizId, quiz);
                    }
                    
                }
                else
                {
                    return await StartSingleAttemptQuiz(userId, quizId, quiz);
                }

                
            }
            catch(QuizAlreadyStartedException ex)
            {
                _logger.LogError(ex, "Quiz already started error at StartQuiz service");
                throw new QuizAlreadyStartedException(ex.Message);
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found at StartQuiz service");
                throw new NoSuchQuizException(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred at StartQuiz service");
                throw new Exception(ex.Message);
            }
        }

        //START SINGLE ATTEMPT ALLOWED QUIZ
        private async Task<StartQuizResponseDTO> StartSingleAttemptQuiz(int userId, int quizId, Quiz quiz)
        {
            var response = new Response
            {
                UserId = userId,
                QuizId = quizId,
                ScoredPoints = 0,
                StartTime = DateTime.Now,
                EndTime = null
            };

            await _responseRepo.Add(response);

            var questions = quiz.QuizQuestions.Select(q => new QuizResponseQuestionDTO
            {
                QuestionId = q.QuestionId,
                QuestionText = q.Question.QuestionText,
                QuestionType = q.Question.QuestionType,
                Options = (q.Question is MultipleChoice mc)
                            ? new List<string> { mc.Choice1, mc.Choice2, mc.Choice3, mc.Choice4 }
                            : new List<string>()
            }).ToList();

            await UpdateNumsOfQuizAttended(userId);

            return new StartQuizResponseDTO
            {
                QuizId = quizId,
                QuizName = quiz.QuizName,
                Questions = questions
            };
        }

        //UPDATE NUMBER OF QUIZ ATTENDED
        private async Task UpdateNumsOfQuizAttended(int userId)
        {
            User user = await _userRepo.Get(userId);
            if (user is Student student)
            {
                student = await _studentRepo.Get(userId);
                int numOfQuizAttended = student.NumsOfQuizAttended ?? 0;
                if (numOfQuizAttended == 0)
                {
                    student.NumsOfQuizAttended = 1;
                }
                else
                {
                    student.NumsOfQuizAttended += 1;
                }
                await _studentRepo.Update(student);
            }
        }

        //START MULTIPLE ATTEMPT ALLOWED QUIZ
        private async Task<StartQuizResponseDTO> StartMultipleAttemptQuiz(int userId, int quizId, Quiz quiz)
        {
            var response = new Response
            {
                UserId = userId,
                QuizId = quizId,
                ScoredPoints = 0,
                StartTime = DateTime.Now,
                EndTime = null
            };

            await _responseRepo.Add(response);

            var questions = quiz.QuizQuestions.Select(q => new QuizResponseQuestionDTO
            {
                QuestionId = q.QuestionId,
                QuestionText = q.Question.QuestionText,
                QuestionType = q.Question.QuestionType,
                Options = (q.Question is MultipleChoice mc)
                            ? new List<string> { mc.Choice1, mc.Choice2, mc.Choice3, mc.Choice4 }
                            : new List<string>()
            }).ToList();


            return new StartQuizResponseDTO
            {
                QuizId = quizId,
                QuizName = quiz.QuizName,
                Questions = questions
            };
        }

        //SUBMIT ALL ANSWERS
        public async Task<string> SubmitAllAnswersAsync(SubmitAllAnswersDTO submitAllAnswersDTO)
        {
            try
            {
                
                var quiz = await _quizRepo.Get(submitAllAnswersDTO.QuizId);

                var responses = await _responseRepo.Get();
                var response = responses
                    .Where(r => r.UserId == submitAllAnswersDTO.UserId && r.QuizId == submitAllAnswersDTO.QuizId)
                    .OrderByDescending(r => r.StartTime)
                    .FirstOrDefault();

                if (response == null)
                {
                    throw new QuizNotStartedException(submitAllAnswersDTO.QuizId);
                }
                if (DateTime.Now > response.StartTime.Add(quiz.TimeLimit))
                {
                    response.EndTime = response.StartTime.Add(quiz.TimeLimit);
                    await _responseRepo.Update(response);
                    throw new QuizTimeLimitExceededException();
                }

                foreach (var questionId in submitAllAnswersDTO.QuestionAnswers.Keys)
                {
                    var answer = submitAllAnswersDTO.QuestionAnswers[questionId];

                    var quizQuestion = quiz.QuizQuestions.FirstOrDefault(q => q.QuestionId == questionId);
                    if (quizQuestion == null)
                    {
                        throw new NoSuchQuestionException(questionId);
                    }

                    var question = await _questionRepository.Get(questionId);

                    var existingAnswer = response.ResponseAnswers.FirstOrDefault(ra => ra.QuestionId == questionId);
                    if (existingAnswer != null)
                    {
                        throw new UserAlreadyAnsweredTheQuestionException($"User has already answered question with ID {questionId}.");
                    }

                    bool isCorrect = false;

                    if (question is MultipleChoice multipleChoiceQuestion)
                    {
                        //isCorrect = multipleChoiceQuestion.CorrectChoice == answer;
                        isCorrect = string.Equals(multipleChoiceQuestion.CorrectChoice, answer, StringComparison.OrdinalIgnoreCase);
                    }
                    else if (question is FillUps fillUpsQuestion)
                    {
                        //isCorrect = fillUpsQuestion.CorrectAnswer == answer;
                        isCorrect = string.Equals(fillUpsQuestion.CorrectAnswer, answer, StringComparison.OrdinalIgnoreCase);

                    }

                    response.ResponseAnswers.Add(new ResponseAnswer
                    {
                        QuestionId = questionId,
                        SubmittedAnswer = answer,
                        IsCorrect = isCorrect
                    });

                    if (isCorrect)
                    {
                        response.ScoredPoints += question.Points;
                    }
                }

                await UpdateStudentCoins(quiz, response, submitAllAnswersDTO.UserId ?? 0);
                
                var results=await _responseRepo.Update(response);
                return "Answers Submitted Successfully";
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found at SubmitAllAnswers service");
                throw ex;
            }
            catch(QuizNotStartedException ex)
            {
                _logger.LogError(ex, "Quiz not started error at SubmitAllAnswers service");
                throw new QuizNotStartedException(ex.Message);
            }
            catch (NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found at SubmitAllAnswers service");
                throw new NoSuchQuestionException(ex.Message);
            }
            catch (UserAlreadyAnsweredTheQuestionException ex)
            {
                _logger.LogError(ex, "Already Answered the question error at SubmitAllAnswers service");
                throw new UserAlreadyAnsweredTheQuestionException(ex.Message);
            }
            catch(NoSuchUserException ex)
            {
                _logger.LogError(ex, "User not found at SubmitAllAnswers service");
                throw new NoSuchUserException(ex.Message);
            }
            catch(QuizTimeLimitExceededException ex)
            {
                _logger.LogError(ex, "Quiz time limit exceeded error at SubmitAllAnswers service");
                throw ex;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred at SubmitAllAnswers service");
                throw new Exception(e.Message);
            }
        }

        //UPDATE STUDENT COINS
        private async Task UpdateStudentCoins(Quiz quiz, Response response, int userId)
        {
            var totalPoints = quiz.QuizQuestions.Sum(q => q.Question.Points);
            response.EndTime = DateTime.Now;
            if (totalPoints == response.ScoredPoints)
            {

                Student student = await _studentRepo.Get(userId);

                TimeSpan halfwayPoint = quiz.TimeLimit.Divide(2);
                TimeSpan elapsedTime = DateTime.Now - response.StartTime;

                int coinsEarned = student.CoinsEarned ?? 0;
                if (coinsEarned == 0)
                {
                    student.CoinsEarned = 10;
                }
                else
                {
                    student.CoinsEarned += 5;
                }
                if (elapsedTime < halfwayPoint)
                {
                    student.CoinsEarned += 3;
                }
                await _studentRepo.Update(student);
            }
        }

        //SUBMIT QUIZ ANSWER ONE BY ONE
        public async Task<string> SubmitAnswerAsync(SubmitAnswerDTO submitAnswerDTO)
        {
            try
            {
                
                var quiz = await _quizRepo.Get(submitAnswerDTO.QuizId);
                var quizQuestion = quiz.QuizQuestions.FirstOrDefault(q => q.QuestionId == submitAnswerDTO.QuestionId);

                if (quizQuestion == null)
                {
                    throw new NoSuchQuestionException($"Question with ID {submitAnswerDTO.QuestionId} is not part of the quiz with ID {submitAnswerDTO.QuizId}.");
                }

                var question = await _questionRepository.Get(submitAnswerDTO.QuestionId);

                var responses = await _responseRepo.Get();
                var response = responses
                    .Where(r => r.UserId == submitAnswerDTO.UserId && r.QuizId == submitAnswerDTO.QuizId)
                    .OrderByDescending(r => r.StartTime)
                    .FirstOrDefault();


                if (DateTime.Now > response.StartTime.Add(quiz.TimeLimit))
                {
                    response.EndTime = response.StartTime.Add(quiz.TimeLimit);
                    await _responseRepo.Update(response);
                    throw new QuizTimeLimitExceededException();
                }

                var existingAnswer = response.ResponseAnswers.FirstOrDefault(ra => ra.QuestionId == submitAnswerDTO.QuestionId);

                if (existingAnswer != null)
                {
                    throw new UserAlreadyAnsweredTheQuestionException();
                }

                bool isCorrect = false;

                if (question is MultipleChoice multipleChoiceQuestion)
                {
                    isCorrect = multipleChoiceQuestion.CorrectChoice == submitAnswerDTO.Answer;
                }
                else if (question is FillUps fillUpsQuestion)
                {
                    isCorrect = fillUpsQuestion.CorrectAnswer == submitAnswerDTO.Answer;
                }

                response.ResponseAnswers.Add(new ResponseAnswer
                {
                    QuestionId = submitAnswerDTO.QuestionId,
                    SubmittedAnswer = submitAnswerDTO.Answer,
                    IsCorrect = isCorrect
                });

                if (isCorrect)
                {
                    response.ScoredPoints += question.Points;
                }

                if (response.ResponseAnswers.Count == quiz.QuizQuestions.Count )
                {

                    await UpdateStudentCoins(quiz, response, submitAnswerDTO.UserId ?? 0);
                }

                var result = await _responseRepo.Update(response);
                if (result != null)
                {
                    return "Answer Submitted";
                }
                return string.Empty;
                
            }
            catch(NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found at SubmitAnswers service");
                throw new NoSuchQuizException(ex.Message);
            }
            catch(NoSuchQuestionException ex)
            {
                _logger.LogError(ex, "Question Not found at SubmitAnswers service");
                throw new NoSuchQuestionException(ex.Message);
            }
            catch(UserAlreadyAnsweredTheQuestionException ex)
            {
                _logger.LogError(ex, "Already Answered the question error at SubmitAnswers service");
                throw ex;
            }
            catch(QuizTimeLimitExceededException ex)
            {
                _logger.LogError(ex, "Quiz time limit exceeded error at SubmitAnswers service");
                throw ex;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "An error occurred at SubmitAnswers service");
                throw new Exception(e.Message);
            }
            
        }

        //GET QUIZ LEADERBOARD 
        public async Task<List<LeaderboardDTO>> GetQuizLeaderboardAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepo.Get(quizId);
                var responses = await _responseRepo.Get();
                var quizResponses = responses.Where(r => r.QuizId == quizId).ToList();

                if (quiz.IsMultipleAttemptAllowed)
                {
                    quizResponses = quizResponses.GroupBy(r => r.UserId)
                        .Select(g => g.OrderByDescending(r => r.ScoredPoints).First())
                        .ToList();
                }

                var sortedResponses = quizResponses.OrderByDescending(r => r.ScoredPoints).ThenBy(r=>r.TimeTaken).ToList();

                var leaderboard = new List<LeaderboardDTO>();

                var totalQuestions = quiz.NumOfQuestions;

                int rank = 1;
                foreach (var response in sortedResponses)
                {
                    if (response.EndTime != null)
                    {
                        var user = await _userRepo.Get(response.UserId);
                        var correctlyAsnweredQuestions = response.ResponseAnswers.Count(r => r.IsCorrect);

                        var percentage = ((decimal)correctlyAsnweredQuestions / totalQuestions) * 100;

                        leaderboard.Add(new LeaderboardDTO
                        {
                            Rank = rank++,
                            UserId = response.UserId,
                            UserName = user != null ? user.Name : "Unknown",
                            ScoredPoints = response.ScoredPoints,
                            StartTime = response.StartTime,
                            TimeTaken = response.TimeTaken,
                            correctPercentage = percentage,
                            EndTime = response.EndTime
                        });
                    }
                }

                return leaderboard;
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found at GetQuizLeaderboard Service");
                throw new NoSuchQuizException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred at GetQuizLeaderboard Service");
                throw new Exception(ex.Message);
            }
        }

        //GET STUDENT LEADERBOARD
        public async Task<List<LeaderboardDTO>> GetStudentQuizLeaderboardAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepo.Get(quizId);
                var responses = await _responseRepo.Get();
                var quizResponses = responses.Where(r => r.QuizId == quizId).ToList();

                var totalQuestions = quiz.NumOfQuestions;

                if (quiz.IsMultipleAttemptAllowed)
                {
                    quizResponses = quizResponses.GroupBy(r => r.UserId)
                        .Select(g => g.OrderByDescending(r => r.ScoredPoints).First())
                        .ToList();
                }

                var sortedResponses = quizResponses.OrderByDescending(r => r.ScoredPoints).ThenBy(r=>r.TimeTaken).ToList();

                var leaderboard = new List<LeaderboardDTO>();

                int rank = 1;
                foreach (var response in sortedResponses)
                {
                    var user = await _userRepo.Get(response.UserId);
                    if(user is Student student)
                    {
                        var correctlyAsnweredQuestions = response.ResponseAnswers.Count(r => r.IsCorrect);

                        var percentage = ((decimal)correctlyAsnweredQuestions / totalQuestions) * 100;

                        leaderboard.Add(new LeaderboardDTO
                        {
                            Rank = rank++,
                            UserId = response.UserId,
                            UserName = student != null ? student.Name : "Unknown",
                            ScoredPoints = response.ScoredPoints,
                            correctPercentage = percentage,
                            TimeTaken = response.TimeTaken,
                            StartTime = response.StartTime,
                            EndTime = response.EndTime
                        });
                    }
                }

                return leaderboard;
            }
            catch (NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not found at GetStudentQuizLeaderboard Service");
                throw new NoSuchQuizException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred at GetStudentQuizLeaderboard Service");
                throw new Exception(ex.Message);
            }
        }

        //GET STUDENT POSITION IN THE LEADERBOARD
        public async Task<int> GetStudentPositionInLeaderboardAsync(int userId, int quizId)
        {
            try
            {
                var leaderboard = await GetQuizLeaderboardAsync(quizId);

                var studentPosition = leaderboard.FindIndex(l => l.UserId == userId);

                if (studentPosition == -1)
                {
                    return -1;
                }
                return studentPosition + 1;
            }
            catch(NoSuchQuizException ex)
            {
                _logger.LogError(ex, "Quiz Not Found at GetStudentPositionInLeaderboard service");
                throw new NoSuchQuizException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred at GetStudentPositionInLeaderboard service");
                throw new Exception(ex.Message);
            }
        }
        

        //GET ALL RESPONSE BY THE STUDENT
        public async Task<List<QuizResponseDTO>> GetAllUseresponsesAsync(int userId)
        {
            try
            {
                var allResponses = await _responseRepo.Get();
                var userResponses = allResponses
                                        .Where(r => r.UserId == userId)
                                        .OrderByDescending(r => r.ScoredPoints)
                                        .ToList();

                var result = new List<QuizResponseDTO>();

                foreach (var response in userResponses)   
                {
                    if (response.EndTime != null)
                    {
                        var quiz = await _quizRepo.Get(response.QuizId);

                        var totalQuestions = quiz.NumOfQuestions;
                        var correctAnsweredQuestion = response.ResponseAnswers.Count(r => r.IsCorrect);

                        decimal percentage = ((decimal)correctAnsweredQuestion / totalQuestions) * 100;


                        var answeredQuestions = response.ResponseAnswers.Select(ra => new AnsweredQuestionDTO
                        {
                            QuestionId = ra.QuestionId,
                            SubmittedAnswer = ra.SubmittedAnswer,
                            CorrectAnswer = (ra.Question is MultipleChoice mcq) ? mcq.CorrectChoice : (ra.Question is FillUps fillUps) ? fillUps.CorrectAnswer : null,
                            IsCorrect = ra.IsCorrect
                        }).ToList();

                        result.Add(new QuizResponseDTO
                        {
                            ResponseId = response.Id,
                            UserId = response.UserId,
                            QuizId = response.QuizId,
                            QuizName = quiz.QuizName,
                            totalPoints = quiz.TotalPoints,
                            CorrectPercentage = percentage,
                            Score = response.ScoredPoints,
                            AnsweredQuestions = answeredQuestions,
                            StartTime = response.StartTime,
                            EndTime = response.EndTime
                        });
                    }
                }
                if(result.Count == 0)
                {
                    throw new NoSuchResponseException();
                }
                return result;
            }
            catch(NoSuchResponseException ex)
            {
                _logger.LogError(ex, "No Response Found error at GetAllUseresponses service");
                throw new NoSuchResponseException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred at GetAllUseresponses service");
                throw new Exception(ex.Message);
            }
        }

    }
}
