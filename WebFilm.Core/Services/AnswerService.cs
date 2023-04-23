using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Answer;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class AnswerService : BaseService<int, Answer>, IAnswerService
    {
        IAnswerRepository _answerRepository;
        private readonly IConfiguration _configuration;

        public AnswerService(IAnswerRepository answerRepository, IConfiguration configuration) : base(answerRepository)
        {
            _answerRepository = answerRepository;
            _configuration = configuration;
        }

        
    }
}
