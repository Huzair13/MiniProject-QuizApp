using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class QuestionRepository : IRepository<int, Question>
    {
        private readonly QuizAppContext _context;
        public QuestionRepository(QuizAppContext context)
        {
            _context = context;
        }

        public async Task<Question> Add(Question item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Question> Delete(int QuestionId)
        {
            var question = await Get(QuestionId);
            _context.Remove(question);
            await _context.SaveChangesAsync(true);
            return question;
        }

        public async Task<Question> Get(int QuestionId)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(e => e.Id == QuestionId);
            if (question != null)
            {
                return question;
            }

            throw new NoSuchQuestionException(QuestionId);              
        }

        public async Task<IEnumerable<Question>> Get()
        {
            var questions = await _context.Questions.ToListAsync();
            return questions;
        }

        public async Task<Question> Update(Question item)
        {
            var question = await Get(item.Id);
            _context.Update(item);
            await _context.SaveChangesAsync(true);
            return question;
        }
    }
}
