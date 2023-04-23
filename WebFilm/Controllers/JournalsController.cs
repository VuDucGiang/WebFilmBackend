using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class JournalsController : BaseController<int, Journal>
    {
        #region Field
        IJournalService _journalService;
        #endregion

        #region Contructor
        public JournalsController(IJournalService journalService) : base(journalService)
        {
            _journalService = journalService;
        }

        [AllowAnonymous]
        [HttpGet("Lastest")]
        public IActionResult GetLastest()
        {
            try
            {
                var res = _journalService.GetLastestJournal();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet("New")]
        public IActionResult GetNewJournal()
        {
            try
            {
                var res = _journalService.GetListNewJournal();
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
