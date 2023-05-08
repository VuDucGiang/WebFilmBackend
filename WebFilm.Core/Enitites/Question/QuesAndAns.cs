using System.ComponentModel.DataAnnotations;
using WebFilm.Core.Enitites.Answer;

namespace WebFilm.Core.Enitites.Question
{
    public class QuesAndAns 
    {

        public int QuestionID { get; set; }
        public string question { get; set; }
        public List<AnswerLite> answers {get; set;}
    }
}
