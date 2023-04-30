using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class ProfileDTO
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Bio { get; set; }

        public string Avatar { get; set; }

        public int TotalReview { get; set; }

        public int TotalLists { get; set; }

        public Following Followers { get; set; }

        public Following Following { get; set; }

        public FavouriteFilmDTO FavouriteFilms { get; set; }

        public WatchListDTO WatchList { get; set; }

        public List<RecentListDTO> ListRecentList { get; set; }

        public List<RecentLikeDTO> RecentLikes { get; set; }

        public List<BaseReviewDTO> ListRecentReview { get; set; }

        public List<BaseReviewDTO> ListPopularReview { get; set; }

        public List<BaseReviewDTO> RecentLikeReview { get; set; }

        public RateStat RateStats { get; set; }
    }
}
