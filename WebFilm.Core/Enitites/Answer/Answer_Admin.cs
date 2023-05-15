using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Answer
{
    public class Answer_Admin
    {
        [Key]
        public int? AnswerID { get; set; }
        public int? QuestionID { get; set; }
        public string? answer { get; set; }

        public string? Image { get; set; }

        public int? RightAnswer { get; set; }


    }
}
