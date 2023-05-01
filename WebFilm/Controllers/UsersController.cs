using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Controllers;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<Guid, User>
    {
        #region Field
        IUserService _userService;
        public static IWebHostEnvironment _webHostEnvironment;
        IUserContext _userContext;

        #endregion

        #region Contructor
        public UsersController(IUserService userService, IWebHostEnvironment webHostEnvironment, IUserContext userContext) : base(userService)
        {
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _userContext = userContext;
        }
        #endregion

        #region Method

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("signup")]
        [AllowAnonymous]
        public IActionResult Signup(UserDto user)
        {
            try
            {
                var res = _userService.Signup(user);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(string email, string password)
        {
            try
            {
                var res = _userService.Login(email, password);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Active")]
        [AllowAnonymous]
        public IActionResult ActiveUser(string email)
        {
            try
            {
                var res = _userService.ActiveUser(email);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(string email, string oldPass, string newPass)
        {
            try
            {
                var res = _userService.ChangePassword(email, oldPass, newPass);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet("ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                var res = _userService.ForgotPassword(email);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string pass, string confirmPass)
        {
            try
            {
                var res = await _userService.ResetPassword(token, pass, confirmPass);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Lấy danh sách người dùng theo tìm kiếm
        /// </summary>
        /// <param name="pageSize">Số lượng bán ghi/1 trang</param>
        /// <param name="pageIndex">Trang thứ mấy</param>
        /// <param name="filter">tìm kiếm theo userName hoăc email</param>
        /// <returns></returns>
        [HttpPost("Paging")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaging([FromBody] PagingParameterMember parameter)
        {
            try
            {
                if(parameter.userName == "")
                {
                    parameter.userName = _userContext.UserName != null ? _userContext.UserName : parameter.userName;
                }
                var res = await _userService.GetPaging(parameter.pageSize, parameter.pageIndex, parameter.filter, parameter.sort, parameter.typeUser, parameter.userName);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách member popular this week
        /// </summary>
        /// <param name="pageSize">Số lượng bán ghi/1 trang</param>
        /// <param name="pageIndex">Trang thứ mấy</param>
        /// <param name="filter">tìm kiếm theo userName hoăc email</param>
        /// <returns></returns>
        [HttpPost("Popular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopular([FromBody] PagingParameter parameter)
        {
            try
            {
                var res = await _userService.GetPopular(parameter.pageSize, parameter.pageIndex, parameter.filter, parameter.sort);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{userName}/Profile")]
        [AllowAnonymous]
        public IActionResult getProfile(string userName)
        {
            try
            {
                return Ok(_userService.getProfile(userName)) ;
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }

        }

        [HttpGet("{userName}/Profile-info")]
        [AllowAnonymous]
        public IActionResult getProfileInfo(string userName)
        {
            try
            {
                return Ok(_userService.getInfoProfile(userName));
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }

        }

        [HttpGet("{id}/check")]
        public IActionResult checkLike(int id, string type)
        {
            try
            {
                return Ok(_userService.checkLikeUser(id, type));
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }

        }

        [HttpPost("{id}/userLike")]
        [AllowAnonymous]
        public IActionResult getUserLike([FromBody] PagingParameter parameter, string type, int id)
        {
            try
            {
                return Ok(_userService.getUserLiked(parameter, type, id));
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }

        }

        [HttpPost("{userName}/Profile/Watchlist")]
        [AllowAnonymous]
        public IActionResult getWatchListProfile([FromBody] PagingParameterFilm parameter, string userName)
        {
            try
            {
                return Ok(_userService.watchListProfile(parameter, userName));
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }

        }

        //[HttpPost("{userID}/Avatar")]
        //public IActionResult SaveAvatar(Guid userID, IFormFile avatar)
        //{
        //    try
        //    {
        //        string path = _webHostEnvironment.WebRootPath + "\\Avatars\\";
        //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        //        string fileName = "Avatar_" + userID + ".png";
        //        if (System.IO.File.Exists(path + fileName))
        //        {
        //            System.IO.File.Delete(path + fileName);
        //        }
        //        using (FileStream fileStream = System.IO.File.Create(path + fileName))
        //        {
        //            avatar.CopyTo(fileStream);
        //            fileStream.Flush();

        //        }
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}


        //[HttpGet("{userID}/Avatar")]
        //public IActionResult GetAvatar(Guid userID)
        //{
        //    try
        //    {
        //        string fileName = "Avatar_" + userID + ".png";
        //        var path = Path.Combine(_webHostEnvironment.WebRootPath, "Avatars", fileName);
        //        if (System.IO.File.Exists(path))
        //        {
        //            return PhysicalFile(path, "image/jpeg"); ;
        //        }
        //        return PhysicalFile(Path.Combine(_webHostEnvironment.WebRootPath, "Avatars", "Avatar_default.png"), "image/jpeg");
        //    }
        //    catch (Exception ex)
        //    {

        //        return HandleException(ex);
        //    }

        //}

        ///// <summary>
        ///// Lấy thông tin người dùng theo Id
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <returns>IActionResult</returns>
        ///// Author: Vũ Đức Giang
        //[HttpGet("{userID}")]
        //public IActionResult GetUserByID(Guid userID)
        //{
        //    try
        //    {
        //        var user = _userService.GetUserByID(userID);
        //        return Ok(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}
        #endregion
    }
}
