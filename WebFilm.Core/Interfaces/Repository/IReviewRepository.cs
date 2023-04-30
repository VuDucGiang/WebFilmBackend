using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface  IReviewRepository : IBaseRepository<int, Review>
    {
        List<Review> GetReviewByUserID(Guid userID);

        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

        List<ListPopularWeekDTO> GetRecentWeek();

        List<RateStatDTO> GetRatesByUserID(Guid userID);

        int UpdateCommentCount(int reviewID, int commentCount);

        int UpdateLikeCount(int reviewID, int likeCount);
    }
}
