using WebFilm.Core.Enitites.Question;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IQuestionService : IBaseService<int, Question>
    {
        object GetQuestionsAndAnswers(int FilmID);
    }
}
