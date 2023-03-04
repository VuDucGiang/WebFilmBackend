using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<Guid, User>
    {
        /// <summary>
        /// Lấy thông tin user theo ID
        /// </summary>
        /// <returns>User</returns>
        /// Author: Vũ Đức Giang
        User GetUserByID(Guid userID);

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        int Signup(UserDto user);

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        UserDto Login(string userName);

        /// <summary>
        /// Kiểm tra có trùng tên đăng nhập không
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool CheckDuplicateUserName(string userName);

        bool ActiveUser(string userName);

    }
}
