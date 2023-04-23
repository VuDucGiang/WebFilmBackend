using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Notification
{
    public class Notification : BaseEntity
    {
        [Key]
        public int NotificationID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
public DateTime Date { get; set; }
public string Link { get; set; }
public DateTime ModifiedDate { get; set; }

public char OtherUserID { get; set; }
public int Seen { get; set; }
public Guid UserID { get; set; }



    }
}
