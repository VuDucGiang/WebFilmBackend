using WebFilm.Core.Enitites.Review;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IReviewService : IBaseService<int, Review>
    {
        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);
    }
}
