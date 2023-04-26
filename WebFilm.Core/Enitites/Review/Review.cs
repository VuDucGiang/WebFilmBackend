using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Review
{
    public class Review : BaseEntity
    {
        [Key]
        public int ReviewID { get; set; }
        public Guid UserID { get; set; }
        public int FilmID { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public bool HaveSpoiler { get; set; }
        public DateTime? WatchedDate { get; set; }
        public float Score { get; set; }
    }
}
