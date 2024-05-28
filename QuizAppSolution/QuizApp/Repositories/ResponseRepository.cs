using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class ResponseRepository : IRepository<int, Response>
    {
        private readonly QuizAppContext _context;

        public ResponseRepository(QuizAppContext context)
        {
            _context = context;
        }

        public async Task<Response> Add(Response item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Response> Delete(int responseId)
        {
            var response = await Get(responseId);
            if (response != null)
            {
                _context.Remove(response);
                _context.SaveChangesAsync(true);
                return response;
            }
            throw new NoSuchResponseException(responseId);
        }

        public async Task<Response> Get(int responseId)
        {
            var response = await _context.Responses.FindAsync(responseId);
            if (response != null)
            {
                return response;
            }
            throw new NoSuchResponseException(responseId);
        }

        public async Task<IEnumerable<Response>> Get()
        {
            var responses = await _context.Responses.Include(ra=>ra.ResponseAnswers).ThenInclude(ra => ra.Question).ToListAsync();
            return responses;
        }

        public async Task<Response> Update(Response response)
        {
            var existingResponse = await Get(response.Id);

            if (existingResponse != null)
            {
                _context.Update(response);
                await _context.SaveChangesAsync();
                return existingResponse;
            }
            throw new NoSuchResponseException(response.Id);
        }
    }
}
