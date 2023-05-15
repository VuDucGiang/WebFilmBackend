using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Answer;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.Similar_film;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;
using WebFilm.Core.Enitites.Credit;

namespace WebFilm.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseAdminController<int, Admin>
    {
        #region Field
        IAdminService _adminService;
        #endregion

        #region Contructor
        public AdminController(IAdminService adminService) : base(adminService)
        {
            _adminService = adminService;
        }
        #endregion

        [HttpPost("PagingFilm")]
        public async Task<IActionResult> GetPagingFilm([FromBody] PagingParameterFilm_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingFilm(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateFilm")]
        public async Task<IActionResult> UpdateFilm(int id, Film_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateFilm(id,entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddFilm")]
        public async Task<IActionResult> AddFilm(Film_Admin entity)
        {
            try
            {
                var res = _adminService.AddFilm(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteFilm")]
        public async Task<IActionResult> DeleteFilm(int id)
        {
            try
            {
                var res = _adminService.DeleteFilm(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost("PagingUser")]
        public async Task<IActionResult> GetPagingUser([FromBody] PagingParameterUser_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingUser(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(Guid id, User_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateUser(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var res = _adminService.DeleteUser(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("PagingJournal")]
        public async Task<IActionResult> GetPagingJournal([FromBody] PagingParameterJournal_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingJournal(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateJournal")]
        public async Task<IActionResult> UpdateJournal(int id, Journal_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateJournal(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddJournal")]
        public async Task<IActionResult> AddJournal(Journal_Admin entity)
        {
            try
            {
                var res = _adminService.AddJournal(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteJournal")]
        public async Task<IActionResult> DeleteJournal(int id)
        {
            try
            {
                var res = _adminService.DeleteJournal(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetUserByID")]
        public async Task<IActionResult> GetUserByID(Guid id)
        {
            try
            {
                var res = _adminService.GetUserByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetFilmByID")]
        public async Task<IActionResult> GetFilmByID(int id)
        {
            try
            {
                var res = _adminService.GetFilmByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetJournalByID")]
        public async Task<IActionResult> GetJournalByID(int id)
        {
            try
            {
                var res = _adminService.GetJournalByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("PagingQuestion")]
        public async Task<IActionResult> GetPagingQuestion([FromBody] PagingParameterQuestion_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingQuestion(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion(int id, Question_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateQuestion(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion(Question_Admin entity)
        {
            try
            {
                var res = _adminService.AddQuestion(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteQuestion")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                var res = _adminService.DeleteQuestion(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetQuestionByID")]
        public async Task<IActionResult> GetQuestionByID(int id)
        {
            try
            {
                var res = _adminService.GetQuestionByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("PagingAnswer")]
        public async Task<IActionResult> GetPagingAnswer([FromBody] PagingParameterAnswer_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingAnswer(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateAnswer")]
        public async Task<IActionResult> UpdateAnswer(int id, Answer_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateAnswer(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddAnswer")]
        public async Task<IActionResult> AddAnswer(Answer_Admin entity)
        {
            try
            {
                var res = _adminService.AddAnswer(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteAnswer")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            try
            {
                var res = _adminService.DeleteAnswer(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetAnswerByID")]
        public async Task<IActionResult> GetAnswerByID(int id)
        {
            try
            {
                var res = _adminService.GetAnswerByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost("PagingRelated_film")]
        public async Task<IActionResult> GetPagingRelated_film([FromBody] PagingParameterRelated_film_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingRelated_film(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateRelated_film")]
        public async Task<IActionResult> UpdateRelated_film(int id, Related_film_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateRelated_film(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddRelated_film")]
        public async Task<IActionResult> AddRelated_film(Related_film_Admin entity)
        {
            try
            {
                var res = _adminService.AddRelated_film(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteRelated_film")]
        public async Task<IActionResult> DeleteRelated_film(int id)
        {
            try
            {
                var res = _adminService.DeleteRelated_film(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetRelated_filmByID")]
        public async Task<IActionResult> GetRelated_filmByID(int id)
        {
            try
            {
                var res = _adminService.GetRelated_filmByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("PagingSimilar_film")]
        public async Task<IActionResult> GetPagingSimilar_film([FromBody] PagingParameterSimilar_film_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingSimilar_film(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateSimilar_film")]
        public async Task<IActionResult> UpdateSimilar_film(int id, Similar_film_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateSimilar_film(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddSimilar_film")]
        public async Task<IActionResult> AddSimilar_film(Similar_film_Admin entity)
        {
            try
            {
                var res = _adminService.AddSimilar_film(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteSimilar_film")]
        public async Task<IActionResult> DeleteSimilar_film(int id)
        {
            try
            {
                var res = _adminService.DeleteSimilar_film(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetSimilar_filmByID")]
        public async Task<IActionResult> GetSimilar_filmByID(int id)
        {
            try
            {
                var res = _adminService.GetSimilar_filmByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("PagingCredit")]
        public async Task<IActionResult> GetPagingCredit([FromBody] PagingParameterCredit_Admin parameter)
        {
            try
            {
                var res = await _adminService.GetPagingCredit(parameter);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("UpdateCredit")]
        public async Task<IActionResult> UpdateCredit(string id, Credit_Admin entity)
        {
            try
            {
                var res = _adminService.UpdateCredit(id, entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("AddCredit")]
        public async Task<IActionResult> AddCredit(Credit_Admin entity)
        {
            try
            {
                var res = _adminService.AddCredit(entity);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete("DeleteCredit")]
        public async Task<IActionResult> DeleteCredit(string id)
        {
            try
            {
                var res = _adminService.DeleteCredit(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("GetCreditByID")]
        public async Task<IActionResult> GetCreditByID(string id)
        {
            try
            {
                var res = _adminService.GetCreditByID(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
