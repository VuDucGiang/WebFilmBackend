using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseController<int, Comment>
    {
        #region Field
        ICommentService _commentService;
        #endregion

        public CommentsController(ICommentService commentService) : base(commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("{id}/lists")]
        public IActionResult GetRecent(int id, [FromBody] CommentCreateDTO dto)
        {
            try
            {
                var res = _commentService.CreateCommentInList(id, dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
