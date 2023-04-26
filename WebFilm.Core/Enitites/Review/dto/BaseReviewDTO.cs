using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Review.dto
{
    public class BaseReviewDTO
    {
        public int ReviewID { get; set; }

        public string Content { get; set; }

        public string RatingCreatedAt { get; set; }

        public float Rate { get; set; }

        public int TotalComment { get; set; }

        public int TotalLike { get; set; }

        public DateTime? ReviewDate { get; set; }

        public FilmReviewDTO Film { get; set; }

        public UserReviewDTO User { get; set; }
    }
}
