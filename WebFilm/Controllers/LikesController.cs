using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LikesController : BaseController<int, Like>
    {
        #region Field
        ILikeService _likeService;
        #endregion

        public LikesController(ILikeService likeService) : base(likeService)
        {
            _likeService = likeService;
        }

        [HttpPost("{id}")]
        public IActionResult NewLike(int id, string type)
        {
            try
            {
                var res = _likeService.newLike(id, type);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}