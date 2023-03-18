using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Controllers;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<Guid, User>
    {
        #region Field
        IUserService _userService;
        #endregion

        #region Contructor
        public UsersController(IUserService userService) : base(userService)
        {
            _userService = userService;
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
                var res = _userService.ResetPassword(token, pass, confirmPass);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

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
