using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : BaseController<int, Review>
    {
        #region Field
        IReviewService _reviewService;
        #endregion

        #region Contructor
        public ReviewsController(IReviewService reviewService) : base(reviewService)
        {
            _reviewService = reviewService;
        }
        #endregion

        [AllowAnonymous]
        [HttpPost("Popular")]
        public async Task<IActionResult> GetPopular([FromBody] PagingParameter parameter)
        {
            try
            {
                var res = await _reviewService.GetPopular(parameter.pageSize, parameter.pageIndex, parameter.filter, parameter.sort);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("Recent")]
        public IActionResult GetRecent()
        {
            try
            {
                var res = _reviewService.GetRecent();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
