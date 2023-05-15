using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Question
{
    public class Question_Admin 
    {
        [Key]
        
        public int? QuestionID { get; set; }
        public int? FilmID { get; set; }

        public string? question { get; set; }
    }
}
