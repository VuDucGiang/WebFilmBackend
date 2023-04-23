using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Answer
{
    public class Answer : BaseEntity
    {
        [Key]
        public int QuestionID { get; set; }
        public string answer { get; set; }
public int AnswerID { get; set; }
public DateTime CreatedDate { get; set; }
public string Image { get; set; }
public DateTime ModifiedDate { get; set; }

public int RightAnswer { get; set; }


    }
}
