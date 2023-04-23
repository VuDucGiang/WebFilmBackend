using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Related_filmController : BaseController<int, Related_film>
    {
        #region Field
        IRelated_filmService _related_filmService;
        #endregion

        #region Contructor
        public Related_filmController(IRelated_filmService related_filmService) : base(related_filmService)
        {
            _related_filmService = related_filmService;
        }
        #endregion

       
    }
}
