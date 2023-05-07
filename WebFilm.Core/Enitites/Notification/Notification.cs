using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Notification
{
    public class Notification : BaseEntity
    {
        [Key]
        public int NotificationID { get; set; }
        public Guid ReceiverUserId { get; set; }
        public Guid SenderUserID { get; set; }
        public string Link { get; set; }
        public bool Seen { get; set; }
        public string Content { get; set; }
        public DateTime?  Date { get; set; }
    }
}
