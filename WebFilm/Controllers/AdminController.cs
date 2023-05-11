using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

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
    }
}
