using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Question
{
    public class Question : BaseEntity
    {
        [Key]
        
public int QuestionID { get; set; }
        public DateTime CreatedDate { get; set; }
public int FilmID { get; set; }
public DateTime ModifiedDate { get; set; }
public string question { get; set; }
    }
}
