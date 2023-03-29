using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class RatingRepository : BaseRepository<int, Rating>, IRatingRepository
    {
        public RatingRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
