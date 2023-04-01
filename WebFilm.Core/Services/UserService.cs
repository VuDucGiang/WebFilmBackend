using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Mail;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Enitites.WatchList;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class UserService : BaseService<Guid, User>, IUserService
    {
        IUserRepository _userRepository;
        IReviewRepository _reviewRepository;
        IListRepository _listRepository;
        IFollowRepository _followRepository;
        IWatchListRepository _watchListRepository;
        IFilmRepository _filmRepository;
        IFilmListRepository _filmListRepository;
        ILikeRepository _likeRepository;
        IRatingRepository _ratingRepository;
        private readonly IMailService _mail;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository,
            IMailService mail,
            IConfiguration configuration,
            IReviewRepository reviewRepository,
            IListRepository listRepository,
            IFollowRepository followRepository,
            IWatchListRepository watchListRepository,
            IFilmRepository filmRepository,
            IFilmListRepository filmListRepository,
            ILikeRepository likeRepository,
            IRatingRepository ratingRepository) : base(userRepository)
        {
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _listRepository = listRepository;
            _mail = mail;
            _configuration = configuration;
            _followRepository = followRepository;
            _watchListRepository = watchListRepository;
            _filmRepository = filmRepository;
            _filmListRepository = filmListRepository;
            _likeRepository = likeRepository;
            _ratingRepository = ratingRepository;
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
            if (userDto != null)
            {
                if (userDto.Status == 1)
                {
                    throw new ServiceException("Tài khoản chưa xác nhận email kích hoạt");
                }
                var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, userDto.Password);
                if (isPasswordCorrect)
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
                new Claim("ID", user.UserID.ToString()),
                new Claim("Username", user.UserName),
                new Claim("Email", user.Email),
                new Claim("Role", user.RoleType.ToString()),
                new Claim("Fullname", user.FullName ?? ""),
                new Claim("Avatar", user.Avatar ?? "")

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
            if (res)
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
            if (pass != confirmPass)
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

        public async Task<PagingResult> GetPaging(int pageSize, int pageIndex, string filter, string sort, TypeUser typeUser, string userName)
        {
            return await _userRepository.GetPaging(pageSize, pageIndex, filter, sort, typeUser, userName);
        }

        public ProfileDTO getProfile(string userName)
        {
            User user = _userRepository.getUserByUsername(userName);
            if (user == null)
            {
                return null;
            }
            Guid userID = user.UserID;
            ProfileDTO profile = new ProfileDTO();
            FavouriteFilmDTO filmFavourite = new FavouriteFilmDTO();
            List<BaseFilmDTO> dtos = new List<BaseFilmDTO>();
            if (user.FavouriteFilmList != null)
            {
                JArray favouriteFilms = JArray.Parse(user.FavouriteFilmList);
                foreach (JObject obj in favouriteFilms.ToArray())
                {
                    int id = (int)obj.GetValue("id");
                    string posterPath = (string)obj.GetValue("poster_path");
                    string title = (string)obj.GetValue("title");
                    BaseFilmDTO dto = new BaseFilmDTO();
                    dto.ID = id;
                    dto.PosterPath = posterPath;
                    dto.Title = title;
                    dtos.Add(dto);
                }
            }
            filmFavourite.Films = dtos;

            List<Review> reviews = _reviewRepository.GetReviewByUserID(userID);
            List<List> lists = _listRepository.GetAll().ToList();
            List<Follow> followers = _followRepository.getFollowByUserID(userID);

            //following
            List<Follow> followings = _followRepository.getFollowingByUserID(userID);
            Following following = new Following();
            List<Guid> userIds = followings.Select(p => p.UserID).ToList();
            List<User> users = _userRepository.GetAll().Where(p => userIds.Contains(p.UserID)).ToList();
            List<FollowingDTO> userDtos = new List<FollowingDTO>();
            foreach (User u in users)
            {
                FollowingDTO dto = new FollowingDTO();
                dto.UserID = u.UserID;
                dto.Avatar = u.Avatar;
                userDtos.Add(dto);
            }
            following.FollowingCount = followings.Count;
            following.Followings = userDtos;

            //handle watchlist
            WatchListDTO watchListDTO = new WatchListDTO();
            List<WatchList> watchList = _watchListRepository.GetAll().Where(p => p.UserID.Equals(userID)).Take(5).ToList();
            List<int> ids = watchList.Select(p => p.FilmID).ToList();
            List<Film> films = _filmRepository.GetAll().Where(p => ids.Contains(p.FilmID)).ToList();
            List<BaseFilmDTO> watchListBase = new List<BaseFilmDTO>();
            foreach (Film film in films)
            {
                BaseFilmDTO dto = new BaseFilmDTO();
                dto.ID = film.FilmID;
                dto.Title = film.Title;
                dto.PosterPath = film.Poster_path;
                watchListBase.Add(dto);
            }
            watchListDTO.Films = watchListBase;
            watchListDTO.FilmsCount = watchList.Count;

            //recent list
            RecentListDTO recentListDTO = new RecentListDTO();
            List list = _listRepository.GetAll().OrderByDescending(p => p.ModifiedDate).First();
            List<FilmList> filmLists = _filmListRepository.GetAll().Where(p => p.ListID == list.ListID).Take(5).ToList();
            List<int> filmListIDS = filmLists.Select(p => p.FilmID).ToList();
            List<Film> filmRecent = _filmRepository.GetAll().Where(p => filmListIDS.Contains(p.FilmID)).ToList();
            List<BaseFilmDTO> filmRecentBase = new List<BaseFilmDTO>();
            foreach (Film film in filmRecent)
            {
                BaseFilmDTO dto = new BaseFilmDTO();
                dto.ID = film.FilmID;
                dto.Title = film.Title;
                dto.PosterPath = film.Poster_path;
                filmRecentBase.Add(dto);
            }
            recentListDTO.ListCount = filmRecent.Count;
            recentListDTO.Films = filmRecentBase;
            recentListDTO.Description = list.Description;

            //recent like
            List<RecentLikeDTO> recentLikeDTOs = new List<RecentLikeDTO>();
            List<Like> likes = _likeRepository.GetAll()
                .Where(p => (p.UserID == userID && p.Type.Equals(TypeLike.Film)))
                .OrderByDescending(p => p.Date).Take(4).ToList();
            List<int> recentLikeIds = likes.Select(p => p.ParentID).ToList();
            List<Film> filmRecentLikes = _filmRepository.GetAll().Where(p => recentLikeIds.Contains(p.FilmID)).ToList();
            foreach (Film film in filmRecentLikes)
            {
                RecentLikeDTO dto = new RecentLikeDTO();
                Rating rating = _ratingRepository.GetAll().Where(p => (p.FilmID == film.FilmID && p.UserID.Equals(userID))).First();
                dto.ID = film.FilmID;
                dto.Title = film.Title;
                dto.PosterPath = film.Poster_path;
                if (rating != null)
                {
                    dto.CountRate = rating.Score;

                }
                recentLikeDTOs.Add(dto);
            }

            //recent review
            List<ReviewBase> baseRecentReviews = new List<ReviewBase>();
            List<Review> recentReviews = _reviewRepository.GetReviewByUserID(userID).OrderByDescending(p => p.CreatedDate)
                .Take(2).ToList();
            baseRecentReviews = this.enRichReviews(baseRecentReviews, recentReviews, userID);

            //popular review
            List<ReviewBase> popularReviews = new List<ReviewBase>();
            List<Review> recentReviewsPopular = _reviewRepository.GetReviewByUserID(userID).OrderByDescending(p => p.LikesCount)
                .Take(2).ToList();
            popularReviews = this.enRichReviews(popularReviews, recentReviewsPopular, userID);

            profile.UserName = user.UserName;
            profile.FavouriteFilms = filmFavourite;
            profile.ReviewCount = reviews.Count;
            profile.ListCount = lists.Count;
            profile.Followers = followers.Count;
            profile.Following = following;
            profile.WatchList = watchListDTO;
            profile.RecentList = recentListDTO;
            profile.RecentLikes = recentLikeDTOs;
            profile.RecentReview = baseRecentReviews;
            profile.PopularReview = popularReviews;

            return profile;
        }

        private List<ReviewBase> enRichReviews(List<ReviewBase> reviewsRecent, List<Review> reviews, Guid userID)
        {
            foreach (Review review in reviews)
            {
                ReviewBase dto = new ReviewBase();
                Film film = _filmRepository.GetByID(review.FilmID);
                List<Rating> rates = _ratingRepository.GetAll()
                    .Where(p => (p.FilmID == film.FilmID && p.UserID.Equals(userID))).ToList();
                if (rates.Count > 0)
                {
                    Rating rate = rates[0];
                    dto.Rating = rate.Score;
                    dto.RatingCreatedAt = rate.CreatedDate.ToString().Substring(0, 10);
                }
                if (film != null)
                {
                    dto.Title = film.Title;
                    dto.ReleaseYear = film.Release_date.Substring(0, 4);
                }
                dto.LikeCount = review.LikesCount;
                dto.Content = review.Content;

                reviewsRecent.Add(dto);
            }
            return reviewsRecent;
        }

        #endregion
    }
}
