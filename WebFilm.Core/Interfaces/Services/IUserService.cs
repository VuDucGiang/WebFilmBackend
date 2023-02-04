using WebFilm.Core.Enitites;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IUserService : IBaseService<User>
    {
        /// <summary>
        /// Kiểm tra trước khi lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// Author: Vũ Đức Giang(1/9/2022)
        User GetUserByID(Guid userID);

        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        int Signup(User user);

        User Login(string userName, string password);

    }
}
