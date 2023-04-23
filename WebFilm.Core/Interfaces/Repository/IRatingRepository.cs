using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IRatingRepository : IBaseRepository<int, Rating>
    {
        List<RateStatDTO> GetRatesByUserID(Guid userID);
    }
}
