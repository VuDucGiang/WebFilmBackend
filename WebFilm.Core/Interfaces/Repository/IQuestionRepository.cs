using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Question;


namespace WebFilm.Core.Interfaces.Repository
{
    public interface IQuestionRepository : IBaseRepository<int, Question>
    {
        object GetQuestionsAndAnswers(int FilmID);

    }
}