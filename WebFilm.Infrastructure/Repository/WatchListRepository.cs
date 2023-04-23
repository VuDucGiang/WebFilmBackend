using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.WatchList;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class WatchListRepository : BaseRepository<int, WatchList>, IWatchListRepository
    {
        public WatchListRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
