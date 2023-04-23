using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : BaseController<int, Rating>
    {
        #region Field
        IRatingService _ratingService;
        #endregion

        #region Contructor
        public RatingController(IRatingService ratingService) : base(ratingService)
        {
            _ratingService = ratingService;
        }
        #endregion

       
    }
}
