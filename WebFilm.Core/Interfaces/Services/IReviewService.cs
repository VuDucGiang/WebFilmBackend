using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IReviewService : IBaseService<int, Review>
    {
        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

        List<BaseReviewDTO> GetRecent();

        BaseReviewDTO GetDetail(int id, int limitUser);

        PagingCommentResult GetCommentReview(int ListID, PagingParameter parameter);
    }
}
