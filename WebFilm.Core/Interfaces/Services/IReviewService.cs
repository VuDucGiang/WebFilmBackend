using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IReviewService : IBaseService<int, Review>
    {
        Task<object> GetReviewOfUser(int pageSize, int pageIndex, string userName);

        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

        Task<object> GetPaging(PagingFilterParameter parameter);

        List<BaseReviewDTO> GetRecent();

        BaseReviewDTO GetDetail(int id, int limitUser);

        PagingCommentResult GetCommentReview(int ListID, PagingParameter parameter);

        List<BaseReviewDTO> getNewFromFriend();

        BaseFilmDTO filmReviewMonth();
    }
}
