using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : BaseController<int, Film>
    {
        #region Field
        IFilmService _filmService;
        #endregion

        #region Contructor
        public FilmsController(IFilmService filmService) : base(filmService)
        {
            _filmService = filmService;
        }

        [HttpGet("ListUserLiked")]
        public async Task<IActionResult> GetListUserLiked(int pageSize, int pageIndex, int filmID)
        {
            try
            {
                var res = await _filmService.GetListUserLiked(pageSize, pageIndex, filmID);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPost("AddFilmToList")]
        public async Task<IActionResult> AddFilmToList([FromBody] AddFilmToListParam param)
        {
            try
            {
                var res = await _filmService.AddFilmToList(param.filmID, param.listIDs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/Detail")]
        public async Task<IActionResult> GetDetailByID(int id)
        {
            try
            {
                var entity = await _filmService.GetDetailByID(id);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/GetInfoUser")]
        public async Task<IActionResult> GetInfoUser(int id)
        {
            try
            {
                var entity = await _filmService.GetInfoUser(id);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("Paging")]
        public async Task<IActionResult> GetPaging([FromBody] PagingParameterFilm parameter)
        {
            try
            {
                var res = await _filmService.GetPaging(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("Popular")]
        public async Task<IActionResult> GetPopular([FromBody] PagingParameter parameter)
        {
            try
            {
                var res = await _filmService.GetPopular(parameter.pageSize, parameter.pageIndex, parameter.filter, parameter.sort);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("JustReviewed")]
        public async Task<IActionResult> JustReviewed()
        {
            try
            {
                var res = await _filmService.JustReviewed();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/Related")]
        public async Task<IActionResult> Related(int id, [FromBody] PagingParameter parameter)
        {
            try
            {
                var res = await _filmService.Related(id, parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/Similar")]
        public async Task<IActionResult> Similar(int id, [FromBody] PagingParameter parameter)
        {
            try
            {
                var res = await _filmService.Similar(id, parameter);
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
