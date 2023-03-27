using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Rating;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IRatingRepository : IBaseRepository<int, Rating>
    {
    }
}
