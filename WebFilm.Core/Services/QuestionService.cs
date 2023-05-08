using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class QuestionService : BaseService<int, Question>, IQuestionService
    {
        IQuestionRepository _questionRepository;
        private readonly IConfiguration _configuration;

        public QuestionService(IQuestionRepository questionRepository, IConfiguration configuration) : base(questionRepository)
        {
            _questionRepository = questionRepository;
            _configuration = configuration;
        }

        public object GetQuestionsAndAnswers(int FilmID)
        {
            return _questionRepository.GetQuestionsAndAnswers(FilmID);
        }
    }
}
