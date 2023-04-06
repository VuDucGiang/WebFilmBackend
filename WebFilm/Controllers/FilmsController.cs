using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Interfaces.Services;

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
        #endregion
    }
}
