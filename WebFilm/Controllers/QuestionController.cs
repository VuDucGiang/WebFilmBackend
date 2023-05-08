using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : BaseController<int, Question>
    {
        #region Field
        IQuestionService _questionService;
        #endregion

        #region Contructor
        public QuestionController(IQuestionService questionService) : base(questionService)
        {
            _questionService = questionService;
        }
        [HttpGet("QuestionsAndAnswers")]
        public async Task<IActionResult> GetQuestionsAndAnswers(int FilmID)
        {
            try
            {
                var res = _questionService.GetQuestionsAndAnswers(FilmID);
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
