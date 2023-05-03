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

        [HttpGet("Users")]
        public async Task<IActionResult> GetListOfUser(int pageSize, int pageIndex, string userName)
        {
            try
            {
                var res = await _listService.GetListOfUser(pageSize, pageIndex, userName);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPost("AddList")]
        public async Task<IActionResult> AddListDetail([FromBody] ListDTO list)
        {
            try
            {
                var res = await _listService.AddListDetail(list);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

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

        [HttpGet("MostEngaged")]
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
