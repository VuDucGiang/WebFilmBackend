using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        #endregion
    }
}
