using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Review;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface  IReviewRepository : IBaseRepository<int, Review>
    {
        List<Review> GetReviewByUserID(Guid userID);

        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

    }
}
