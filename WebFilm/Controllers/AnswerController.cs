using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Answer;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : BaseController<int, Answer>
    {
        #region Field
        IAnswerService _answerService;
        #endregion

        #region Contructor
        public AnswerController(IAnswerService answerService) : base(answerService)
        {
            _answerService = answerService;
        }
        #endregion

       
    }
}
