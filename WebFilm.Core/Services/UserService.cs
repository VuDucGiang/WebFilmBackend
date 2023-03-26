using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Mail;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class UserService : BaseService<Guid, User>, IUserService
    {
        IUserRepository _userRepository;
        private readonly IMailService _mail;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IMailService mail, IConfiguration configuration) : base(userRepository)
        {
            _userRepository = userRepository;
            _mail = mail;
            _configuration = configuration;
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

            //Email không được phép trùng
            var isDuplicateEmail = _userRepository.CheckDuplicateEmail(user.Email);
            if (isDuplicateEmail)
            {
                throw new ServiceException(Resources.Resource.Error_Duplicate_Email);
            }
            //Chờ xác nhận
            user.Status = 1;
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var res = _userRepository.Signup(user);

            // Gửi mail
            MailTemplate welcomeMail = new MailTemplate()
            {
                Email = user.Email,
                Name = user.UserName
            };
            MailData mailData = new MailData()
            {
                To = new List<string>() { user.Email },
                Subject = "Thank you for signing up",
                Body = _mail.GetEmailTemplate("welcome", welcomeMail)
            };
            _mail.SendAsync(mailData, new CancellationToken());

            return res;
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public Dictionary<string, object> Login(string email, string password)
        {
            var userDto = _userRepository.Login(email);
            if(userDto != null)
            {
                if(userDto.Status == 1)
                {
                    throw new ServiceException("Tài khoản chưa xác nhận email kích hoạt");
                }
                var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, userDto.Password);
                if(isPasswordCorrect)
                {
                    var token = GenarateToken(userDto);
                    User user = new User();
                    user.UserID = userDto.UserID;
                    user.UserName = userDto.UserName;
                    user.FullName = userDto.FullName;
                    user.Email = userDto.Email;
                    user.DateOfBirth = userDto.DateOfBirth;
                    user.Status = userDto.Status;
                    user.RoleType = userDto.RoleType;
                    user.FavouriteFilmList = userDto.FavouriteFilmList;
                    return new Dictionary<string, object>()
                    {
                        { "User", user },
                        { "Token", token }
                    };
                }
            }
            throw new ServiceException("Thông tin tài khoản hoặc mật khẩu không chính xác");
        }

        private string GenarateToken(UserDto user)
        {
            // Authenticate user credentials and get the user's claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Password)
                // Add any other user claims as needed
            };

                    // Generate a symmetric security key using your secret key
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

                    // Create a signing credentials object using the key
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    // Set token expiration time
                    var expires = DateTime.UtcNow.AddDays(30);

                    // Create a JWT token
                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: expires,
                        signingCredentials: credentials
                    );

            // Serialize the token to a string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return "Bearer " + tokenString;
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

        public bool ActiveUser(string email)
        {
            return _userRepository.ActiveUser(email);
        }

        public bool ChangePassword(string email, string oldPass, string newPass)
        {
            var user = _userRepository.Login(email);
            if (user != null)
            {
                if (user.Status == 1)
                {
                    throw new ServiceException("Tài khoản chưa xác nhận email kích hoạt");
                }
                var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(oldPass, user.Password);
                if (isPasswordCorrect)
                {
                    newPass = BCrypt.Net.BCrypt.HashPassword(newPass);
                    return _userRepository.ChangePassword(email, newPass);
                }
            }
            throw new ServiceException("Mật khẩu không chính xác");
        }

        public bool ForgotPassword(string email)
        {
            var userDto = _userRepository.Login(email);
            if (userDto == null)
            {
                throw new ServiceException("Email không tồn tại");
            }

            userDto.PasswordResetToken = CreateRandomToken();
            userDto.ResetTokenExpires = DateTime.Now.AddDays(1);

            var res = _userRepository.AddTokenReset(userDto);

            // Gửi mail
            if(res)
            {
                MailTemplate welcomeMail = new MailTemplate()
                {
                    Email = email,
                    Token = userDto.PasswordResetToken
                };
                MailData mailData = new MailData()
                {
                    To = new List<string>() { email },
                    Subject = "Reset your FilmLogger password",
                    Body = _mail.GetEmailTemplate("resetPassword", welcomeMail)
                };
                _mail.SendAsync(mailData, new CancellationToken());
                return true;
            }
            return false;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<bool> ResetPassword(string token, string pass, string confirmPass)
        {
            if(pass != confirmPass)
            {
                throw new ServiceException("The passwords you entered were not identical. Please try again.");
            }
            var user = await _userRepository.GetUserByTokenReset(token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                throw new ServiceException("Invalid Token");
            }
            pass = BCrypt.Net.BCrypt.HashPassword(pass);
            return _userRepository.ChangePassword(user.Email, pass);
        }

        public async Task<PagingResult> GetPaging(int? pageSize = 20, int? pageIndex = 1, string? filter = "", string? sort = "UserName", TypeUser? typeUser = TypeUser.All, Guid? userID = null)
        {
            return await _userRepository.GetPaging(pageSize, pageIndex, filter, sort, typeUser, userID);
        }

        public ProfileDTO getProfile(Guid userID)
        {
            User user = _userRepository.GetByID(userID);
            ProfileDTO profile= new ProfileDTO();
            List<FavouriteFilmDTO> dtos = new List<FavouriteFilmDTO>();
            if (user.FavouriteFilmList != null)
            {
                JArray favouriteFilms = JArray.Parse(user.FavouriteFilmList);
                foreach (JObject obj in favouriteFilms.ToArray())
                {
                    string id = (string)obj.GetValue("id");
                    string posterPath = (string)obj.GetValue("poster_path");
                    string title = (string)obj.GetValue("title");
                    FavouriteFilmDTO dto = new FavouriteFilmDTO();
                    dto.id = id;
                    dto.posterPath = posterPath;
                    dto.title = title;
                    dtos.Add(dto);
                }
            }

            profile.UserName = user.UserName;
            profile.FavouriteFilms = dtos;

            return profile;
        }

        #endregion
    }
}
