using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Enitites.Review.dto
{
    public class BaseReviewDTO
    {
        public int ReviewID { get; set; }

        public string Content { get; set; }

        public string RatingCreatedAt { get; set; }

        public float Rate { get; set; }

        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }
        public bool HaveSpoiler { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? WatchedDate { get; set; }

        public BaseFilmDTO Film { get; set; }

        public UserReviewDTO User { get; set; }
        public List<UserReviewDTO> ReviewsLikedByUser { get; set; }
    }
}
