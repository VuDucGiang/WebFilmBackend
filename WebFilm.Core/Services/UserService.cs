using WebFilm.Core.Enitites.User;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class UserService : BaseService<Guid, User>, IUserService
    {
        IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        #region Method
        /// <summary>
        /// Kiểm tra trước khi lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// Author: Vũ Đức Giang
        public User GetUserByID(Guid userID)
        {
            var user = _userRepository.GetUserByID(userID);
            return user;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Signup(UserDto user)
        {
            //Email phải đúng định dạng
            if (user.Email != null && user.Email.Length > 0)
            {
                if (!IsValidEmail(email: user.Email))
                {
                    throw new ServiceException(Resources.Resource.Error_EmailFormat);
                }
            }

            //UserName không được phép trùng
            var isDuplicateUserName = _userRepository.CheckDuplicateUserName(user.UserName);
            if (isDuplicateUserName)
            {
                throw new ServiceException(Resources.Resource.Error_Duplicate_UserName);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var res = _userRepository.Signup(user);
            return res;
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public User Login(string userName, string password)
        {
            var user = _userRepository.Login(userName);
            if(user != null)
            {
                var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if(isPasswordCorrect)
                {
                    return user;
                }
            }
            throw new ServiceException("Thông tin tài khoản hoặc mật khẩu không chính xác");
        }

        /// <summary>
        /// Kiểm tra email có đúng định dạng hay không
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>
        /// true: Đúng
        /// false: Sai
        /// </returns>
        /// Author: Vũ Đức Giang
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; 
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
