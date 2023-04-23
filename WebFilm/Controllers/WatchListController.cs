using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.WatchList;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WatchListController : BaseController<int, WatchList>
    {
        #region Field
        IWatchListService _watchListService;
        #endregion

        #region Contructor
        public WatchListController(IWatchListService watchListService) : base(watchListService)
        {
            _watchListService = watchListService;
        }
        #endregion

       
    }
}
