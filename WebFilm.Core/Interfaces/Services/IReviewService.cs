using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IReviewService : IBaseService<int, Review>
    {
        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

        List<BaseReviewDTO> GetRecent();
    }
}
