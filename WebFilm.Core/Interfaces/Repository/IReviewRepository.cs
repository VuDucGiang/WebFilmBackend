﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface  IReviewRepository : IBaseRepository<int, Review>
    {
        Task<bool> AddReview(ReviewDTO review);
        Task<bool> EditReview(ReviewDTO review);
        Task<bool> DeleteReview(int reviewID);
        Task<object> GetReviewOfUser(int pageSize, int pageIndex, string userName);
        List<Review> GetReviewByUserID(Guid userID);

        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

        Task<object> GetPaging(PagingFilterParameter parameter);

        List<ListPopularWeekDTO> GetRecentWeek();

        List<RateStatDTO> GetRatesByUserID(Guid userID);
        List<RateStatDTO> GetRatesByFilmID(int filmID);

        int UpdateCommentCount(int reviewID, int commentCount);

        int UpdateLikeCount(int reviewID, int likeCount);

        List<ListPopularWeekDTO> TopReviewMonth();
    }
}
