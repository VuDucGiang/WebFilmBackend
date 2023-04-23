using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.WatchList;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class WatchListService : BaseService<int, WatchList>, IWatchListService
    {
        IWatchListRepository _watchListRepository;
        private readonly IConfiguration _configuration;

        public WatchListService(IWatchListRepository watchListRepository, IConfiguration configuration) : base(watchListRepository)
        {
            _watchListRepository = watchListRepository;
            _configuration = configuration;
        }

        
    }
}
