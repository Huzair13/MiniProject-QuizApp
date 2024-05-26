using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class FillUpsRepository : IRepository<int, FillUps>
    {
        private readonly QuizAppContext _context;
        public FillUpsRepository(QuizAppContext context)
        {
            _context = context;
        }

        public async Task<FillUps> Add(FillUps item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<FillUps> Delete(int QuestionId)
        {
            var question = await Get(QuestionId);
            if (question != null)
            {
                _context.Remove(question);
                _context.SaveChangesAsync(true);
                return question;
            }
            throw new NoSuchQuestionException(QuestionId);
        }

        public async Task<FillUps> Get(int QuestionId)
        {
            var question = await _context.FillUps.FirstOrDefaultAsync(e => e.Id == QuestionId);
            if (question != null)
            {
                return question;
            }

            throw new NoSuchQuestionException(QuestionId);
        }

        public async Task<IEnumerable<FillUps>> Get()
        {
            var questions = await _context.FillUps.ToListAsync();
            if (questions.Count != 0)
            {
                return questions;
            }
            throw new NoSuchQuestionException();
        }

        public async Task<FillUps> Update(FillUps item)
        {
            var question = await Get(item.Id);
            if (question != null)
            {
                _context.Update(item);
                _context.SaveChangesAsync(true);
                return question;
            }
            throw new NoSuchQuestionException(item.Id);
        }
    }
}
