using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
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

        [HttpGet("Users")]
        public async Task<IActionResult> GetReviewOfUser(int pageSize, int pageIndex, string userName)
        {
            try
            {
                var res = await _reviewService.GetReviewOfUser(pageSize, pageIndex, userName);
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
                var res = await _reviewService.GetPopular(parameter.pageSize, parameter.pageIndex, parameter.filter, parameter.sort);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("Paging")]
        public async Task<IActionResult> GetPaging([FromBody] PagingFilterParameter parameter)
        {
            try
            {
                var res = await _reviewService.GetPaging(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("Popular/Week")]
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

        [HttpGet("{id}/detail")]
        public IActionResult GetDetail(int id, int limitUser)
        {
            try
            {
                var res = _reviewService.GetDetail(id, limitUser);
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
                var res = _reviewService.GetCommentReview(id, parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpGet("NewFromFriend")]
        public IActionResult getNewFromFriend()
        {
            try
            {
                var res = _reviewService.getNewFromFriend();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("HomeBanner")]
        public IActionResult getBannerFilmTopReview()
        {
            try
            {
                var res = _reviewService.filmReviewMonth();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
