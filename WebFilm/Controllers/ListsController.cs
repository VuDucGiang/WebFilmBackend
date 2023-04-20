using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
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

        [AllowAnonymous]
        [HttpGet("Popular/Week")]
        public IActionResult GetPopularInWeek()
        {
            try
            {
                var res = _listService.GetListPopularWeek();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("RecentLike")]
        public IActionResult GetRecentLikeList()
        {
            try
            {
                var res = _listService.GetListRecentLikes();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("Popular/Month")]
        public IActionResult GetPopularInMonth()
        {
            try
            {
                var res = _listService.GetListPopularMonth();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("CrewList")]
        public IActionResult GetCrewList()
        {
            try
            {
                var res = _listService.GetCrewList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("TopLike")]
        public IActionResult GetListTop()
        {
            try
            {
                var res = _listService.ListTop();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("{id}/Films")]
        public IActionResult GetFilmInList(int id, [FromBody] PagingDetailList parameter)
        {
            try
            {
                var res = _listService.DetailList(id, parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/Detail")]
        public IActionResult DetailList(int id)
        {
            try
            {
                var res = _listService.GetListByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("{id}/Comments")]
        public IActionResult GetCommentInList(int id, [FromBody] PagingParameter parameter)
        {
            try
            {
                var res = _listService.GetCommentList(id, parameter);
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
