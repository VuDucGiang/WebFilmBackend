using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
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

       
    }
}
