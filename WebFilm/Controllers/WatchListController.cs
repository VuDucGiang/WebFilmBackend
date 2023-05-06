using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.WatchList;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
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

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> AddWatchList(int filmID)
        {
            try
            {
                var res = await _watchListService.AddWatchList(filmID);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteWatchList([FromBody] ParamDelete param)
        {
            try
            {
                var res = await _watchListService.DeleteWatchList(param);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
