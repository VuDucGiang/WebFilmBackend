using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IUserService : IBaseService<Guid, User>
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
        int Signup(UserDto user);

        Dictionary<string, object> Login(string email, string password);

        bool ActiveUser(string email);

        bool ChangePassword(string email, string oldPass, string newPass);

        bool ForgotPassword(string email);

        Task<bool> ResetPassword(string token, string pass, string confirmPass);

        Task<PagingResult> GetPaging(int? pageSize = 20, int? pageIndex = 1, string? filter = "", string? sort = "UserName", TypeUser? typeUser = TypeUser.All, Guid? userID = null);

        ProfileDTO getProfile(Guid userID);

    }
}
