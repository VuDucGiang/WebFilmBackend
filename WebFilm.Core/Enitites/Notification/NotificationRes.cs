using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Enitites.Notification
{
    public class NotificationRes
    {
        public int NotificationID { get; set; }
        public Guid ReceiverUserId { get; set; }
        public string Link { get; set; }
        public bool Seen { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public UserReviewDTO Sender { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get;set; }
    }
}
