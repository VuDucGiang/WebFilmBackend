using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IUserService : IBaseService<Guid, User>
    {
        Task<bool> EditFollow(Guid userID, bool follow);

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

        Task<PagingResult> GetPaging(int pageSize, int pageIndex, string filter, string sort, TypeUser typeUser, string userName);

        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);

        ProfileDTO getProfile(string userName);

        ProfileInfo getInfoProfile(string userName);

        bool checkLikeUser(int id, string type);

        List<UserReviewDTO> getUserLiked(PagingParameter paging, string type, int id);

        PagingFilmResult watchListProfile(PagingParameterFilm parameters, string userName);

        PagingFilmResult filmLikeProfile(PagingParameter parameters, string userName);

        PagingReviewResult reviewLikeProfile(PagingParameter parameters, string userName);

        PagingListResult listLikeProfile(PagingParameter parameters, string userName);
    }
}
