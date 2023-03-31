using WebFilm.Core.Enitites;
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
        UserDto Login(string email);

        /// <summary>
        /// Kiểm tra có trùng tên đăng nhập không
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool CheckDuplicateEmail(string email);

        bool ActiveUser(string email);

        bool ChangePassword(string email, string newPass);

        bool AddTokenReset(UserDto user);

        Task<UserDto> GetUserByTokenReset(string token);

        Task<PagingResult> GetPaging(int? pageSize = 20, int? pageIndex = 1, string? filter = "", string? sort = "UserName", TypeUser? typeUser = TypeUser.All, Guid? userID = null);

        User getUserByUsername(string username);

    }
}
