using Microsoft.AspNetCore.Mvc;
using WebFilm.Controllers;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<User>
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
        [HttpPost("login")]
        public IActionResult Login(string userName, string password)
        {
            try
            {
                var res = _userService.Login(userName, password);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>IActionResult</returns>
        /// Author: Vũ Đức Giang
        [HttpGet("{userID}")]
        public IActionResult GetUserByID(Guid userID)
        {
            try
            {
                var user = _userService.GetUserByID(userID);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion
    }
}
