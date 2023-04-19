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
    [AllowAnonymous]
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
        #endregion
    }
}
