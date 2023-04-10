using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : BaseController<int, List>
    {
        #region Field
        IListService _listService;
        #endregion

        #region Contructor
        public ListsController(IListService listService) : base(listService)
        {
            _listService = listService;
        }

        [AllowAnonymous]
        [HttpGet("Popular")]
        public IActionResult GetPopular()
        {
            try
            {
                var res = _listService.GetListPopular();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion
    }
}
