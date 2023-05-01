using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
using WebFilm.Core.Enitites.Review.dto;
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
        IUserContext _userContext;
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
            IRatingRepository ratingRepository,
            IUserContext userContext) : base(userRepository)
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
            _userContext = userContext;
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

            //UserName không được phép trùng
            var isDuplicateUsername = _userRepository.CheckDuplicateUserName(user.UserName);
            if (isDuplicateUsername)
            {
                throw new ServiceException(Resources.Resource.Error_Duplicate_UserName);
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
                    user.Avatar = userDto.Avatar;
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

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            return await _userRepository.GetPopular(pageSize, pageIndex, filter, sort);
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
                    int id = (int)obj.GetValue("FilmID");
                    string posterPath = (string)obj.GetValue("poster_path");
                    string title = (string)obj.GetValue("title");
                    BaseFilmDTO dto = new BaseFilmDTO();
                    dto.FilmID = id;
                    dto.Poster_path = posterPath;
                    dto.Title = title;
                    dtos.Add(dto);
                }
            }
            filmFavourite.Films = dtos;

            List<Review> reviews = _reviewRepository.GetReviewByUserID(userID);
            List<List> lists = _listRepository.GetAll().Where(p => p.UserID == userID).ToList();
            //followers
            List<Follow> followers = _followRepository.getFollowByUserID(userID);
            Following follower = new Following();
            List<Guid> userIdsFollower = followers.Select(p => p.FollowedUserID).ToList();
            List<User> usersFollower = _userRepository.GetAll().Where(p => userIdsFollower.Contains(p.UserID)).Take(20).ToList();
            List<FollowingDTO> userDtosFollower = new List<FollowingDTO>();
            foreach (User u in usersFollower)
            {
                FollowingDTO dto = new FollowingDTO();
                dto.UserID = u.UserID;
                dto.Avatar = u.Avatar;
                dto.UserName = u.UserName;
                userDtosFollower.Add(dto);
            }
            follower.Total = followers.Count;
            follower.List = userDtosFollower;

            //following
            List<Follow> followings = _followRepository.getFollowingByUserID(userID);
            Following following = new Following();
            List<Guid> userIds = followings.Select(p => p.UserID).ToList();
            List<User> users = _userRepository.GetAll().Where(p => userIds.Contains(p.UserID)).Take(20).ToList();
            List<FollowingDTO> userDtos = new List<FollowingDTO>();
            foreach (User u in users)
            {
                FollowingDTO dto = new FollowingDTO();
                dto.UserID = u.UserID;
                dto.Avatar = u.Avatar;
                dto.UserName = u.UserName;
                userDtos.Add(dto);
            }
            following.Total = followings.Count;
            following.List = userDtos;

            //handle watchlist
            WatchListDTO watchListDTO = new WatchListDTO();
            List<WatchList> watchList = _watchListRepository.GetAll().Where(p => p.UserID.Equals(userID)).Take(5).ToList();
            List<int> ids = watchList.Select(p => p.FilmID).ToList();
            List<Film> films = _filmRepository.GetAll().Where(p => ids.Contains(p.FilmID)).ToList();
            List<BaseFilmDTO> watchListBase = new List<BaseFilmDTO>();
            foreach (Film film in films)
            {
                BaseFilmDTO dto = new BaseFilmDTO();
                dto.FilmID = film.FilmID;
                dto.Title = film.Title;
                dto.Poster_path = film.Poster_path;
                watchListBase.Add(dto);
            }
            watchListDTO.List = watchListBase;
            watchListDTO.Total = watchList.Count;

            //recent list
            List<RecentListDTO> recentListDTO = new List<RecentListDTO>();
            List<List> listRecents = _listRepository.GetAll().OrderByDescending(p => p.ModifiedDate).Take(3).ToList();
            foreach (List list in listRecents)
            {
                RecentListDTO dto = new RecentListDTO();
                List<FilmList> filmLists = _filmListRepository.GetAll().Where(p => p.ListID == list.ListID).Take(5).ToList();
                List<int> filmListIDS = filmLists.Select(p => p.FilmID).ToList();
                List<Film> filmRecent = _filmRepository.GetAll().Where(p => filmListIDS.Contains(p.FilmID)).ToList();
                List<BaseFilmDTO> filmRecentBase = new List<BaseFilmDTO>();
                foreach (Film film in filmRecent)
                {
                    BaseFilmDTO dto2 = new BaseFilmDTO();
                    dto2.FilmID = film.FilmID;
                    dto2.Title = film.Title;
                    dto2.Poster_path = film.Poster_path;
                    filmRecentBase.Add(dto2);
                }
                dto.Total = filmRecent.Count;
                dto.List = filmRecentBase;
                dto.Description = list.Description;
                dto.ListID = list.ListID;
                dto.Title = list.ListName;
                recentListDTO.Add(dto);
            }
           

            //recent like
            List<RecentLikeDTO> recentLikeDTOs = new List<RecentLikeDTO>();
            List<Like> likes = _likeRepository.GetAll()
                .Where(p => (p.UserID == userID && p.Type.Equals(TypeLike.Film.ToString())))
                .OrderByDescending(p => p.Date).Take(4).ToList();
            List<int> recentLikeIds = likes.Select(p => p.ParentID).ToList();
            List<Film> filmRecentLikes = _filmRepository.GetAll().Where(p => recentLikeIds.Contains(p.FilmID)).ToList();
            foreach (Film film in filmRecentLikes)
            {
                RecentLikeDTO dto = new RecentLikeDTO();
                //Rating rating = _ratingRepository.GetAll().Where(p => (p.FilmID == film.FilmID && p.UserID.Equals(userID))).First();
                dto.FilmID = film.FilmID;
                dto.Title = film.Title;
                dto.PosterPath = film.Poster_path;
                recentLikeDTOs.Add(dto);
            }

            //recent review
            List<BaseReviewDTO> baseRecentReviews = new List<BaseReviewDTO>();
            List<Review> recentReviews = _reviewRepository.GetReviewByUserID(userID).OrderByDescending(p => p.CreatedDate)
                .Take(2).ToList();
            baseRecentReviews = this.enRichReviews(baseRecentReviews, recentReviews, userID);

            //popular review
            List<BaseReviewDTO> popularReviews = new List<BaseReviewDTO>();
            List<Review> recentReviewsPopular = _reviewRepository.GetReviewByUserID(userID).OrderByDescending(p => p.LikesCount)
                .Take(2).ToList();
            popularReviews = this.enRichReviews(popularReviews, recentReviewsPopular, userID);

            profile.UserName = user.UserName;
            if (user.FullName!= null)
            {
                profile.FullName = user.FullName;
            }
            if (user.Bio != null)
            {
                profile.Bio = user.Bio;
            }
            if (user.Avatar!= null)
            {
                profile.Avatar = user.Avatar;
            }

            //rate stat
            RateStat rateStats= new RateStat();
            List<RateStatDTO> rateStatsPopular = _reviewRepository.GetRatesByUserID(userID).ToList();
            if (rateStatsPopular.Count > 0)
            {
                List<float> ratesValue = new List<float>();
                for (float i = 1; i <= 10; i++)
                {
                    ratesValue.Add(i / 2f);
                }
                foreach (RateStatDTO statDTO in rateStatsPopular)
                {
                    if (ratesValue.Contains(statDTO.Value))
                    {
                        ratesValue.Remove(statDTO.Value);
                    }
                }

                foreach (float rate in ratesValue)
                {
                    RateStatDTO newRate = new RateStatDTO();
                    newRate.Value = rate;
                    rateStatsPopular.Add(newRate);
                }


                rateStats.List = rateStatsPopular;
                rateStats.Total = rateStatsPopular.Select(p => p.Total).Sum();
                rateStats.RateAverage = rateStatsPopular.Select(p => p.Value * p.Total).Sum() / rateStats.Total;
            }
            

            // recent like review
            List<BaseReviewDTO> recentLikesReview = new List<BaseReviewDTO>();
            List<Like> likeRecentss = _likeRepository.GetAll().Where(p => p.UserID == user.UserID && "Review".Equals(p.Type))
                .OrderByDescending(p => p.Date).Take(4).ToList();
            List<int> recentIDS = likeRecentss.Select(p => p.ParentID).ToList();
            List<Review> reviewRecentss = _reviewRepository.GetAll().Where(p => recentIDS.Contains(p.ReviewID)).ToList();
            recentLikesReview = enRichReviews(recentLikesReview, reviewRecentss, userID);
            foreach (BaseReviewDTO review in recentLikesReview)
            {
                UserReviewDTO userDto = new UserReviewDTO();
                Review reviewDto = _reviewRepository.GetByID(review.ReviewID);
                User user1 = _userRepository.GetByID(reviewDto.UserID);
                if (user1 != null) {
                    userDto.Avatar = user1.Avatar;
                    userDto.UserID = user1.UserID;
                    userDto.FullName = user1.FullName;
                    userDto.UserName = user1.UserName;
                    review.User = userDto;
                }
            }

            profile.FavouriteFilms = filmFavourite;
            profile.TotalReview = reviews.Count;
            profile.TotalLists = lists.Count;
            profile.Followers = follower;
            profile.Following = following;
            profile.WatchList = watchListDTO;
            profile.ListRecentList = recentListDTO;
            profile.RecentLikes = recentLikeDTOs;
            profile.ListRecentReview = baseRecentReviews;
            profile.ListPopularReview = popularReviews;
            profile.RateStats = rateStats;
            profile.RecentLikeReview = recentLikesReview;

            return profile;
        }

        private List<BaseReviewDTO> enRichReviews(List<BaseReviewDTO> reviewsRecent, List<Review> reviews, Guid userID)
        {
            User user = _userRepository.GetByID(userID);
            foreach (Review review in reviews)
            {
                BaseReviewDTO dto = new BaseReviewDTO();
                Film film = _filmRepository.GetByID(review.FilmID);
                FilmReviewDTO filmReview = new FilmReviewDTO();
                List<Review> reviewss = _reviewRepository.GetAll()
                    .Where(p => (p.FilmID == film.FilmID && p.UserID.Equals(userID))).ToList();
                if (reviewss.Count > 0)
                {
                    Review rate = reviewss[0];
                    dto.Rate = rate.Score;
                    //dto.RatingCreatedAt = rate.CreatedDate.ToString().Substring(0, 10);
                }
                if (film != null)
                {
                    filmReview.FilmID = film.FilmID;
                    filmReview.Poster_path = film.Poster_path;
                    filmReview.Title = film.Title;
                    filmReview.Release_date = film.Release_date;
                }

                dto.ReviewID = review.ReviewID;
                dto.Film = filmReview;
                dto.TotalLike = review.LikesCount;
                dto.Content = review.Content;
                dto.CreatedDate = review.CreatedDate;
                dto.HaveSpoiler = review.HaveSpoiler;
                dto.TotalComment = review.CommentsCount;
                dto.WatchedDate = review.WatchedDate;

                reviewsRecent.Add(dto);
            }
            return reviewsRecent;
        }

        public ProfileInfo getInfoProfile(string userName)
        {
            User user = _userRepository.getUserByUsername(userName);
            if (user == null)
            {
                return null;
            }
            ProfileInfo res = new ProfileInfo();

            res.UserName = userName;
            if (user.FullName != null)
            {
                res.FullName = user.FullName;
            }
            if (user.Bio != null)
            {
                res.Bio = user.Bio;
            }
            if (user.Avatar != null)
            {
                res.Avatar = user.Avatar;
            }

            //followers
            List<Follow> followers = _followRepository.getFollowByUserID(user.UserID);
            Following follower = new Following();
            List<Guid> userIdsFollower = followers.Select(p => p.FollowedUserID).ToList();
            List<User> usersFollower = _userRepository.GetAll().Where(p => userIdsFollower.Contains(p.UserID)).Take(20).ToList();
            List<FollowingDTO> userDtosFollower = new List<FollowingDTO>();
            foreach (User u in usersFollower)
            {
                FollowingDTO dto = new FollowingDTO();
                dto.UserID = u.UserID;
                dto.Avatar = u.Avatar;
                dto.UserName = u.UserName;
                dto.Fullname = u.FullName;
                userDtosFollower.Add(dto);
            }
            follower.Total = followers.Count;
            follower.List = userDtosFollower;

            //following
            List<Follow> followings = _followRepository.getFollowingByUserID(user.UserID);
            Following following = new Following();
            List<Guid> userIds = followings.Select(p => p.UserID).ToList();
            List<User> users = _userRepository.GetAll().Where(p => userIds.Contains(p.UserID)).Take(20).ToList();
            List<FollowingDTO> userDtos = new List<FollowingDTO>();
            foreach (User u in users)
            {
                FollowingDTO dto = new FollowingDTO();
                dto.UserID = u.UserID;
                dto.Avatar = u.Avatar;
                dto.UserName = u.UserName;
                userDtos.Add(dto);
            }
            following.Total = followings.Count;
            following.List = userDtos;

            List<Review> reviews = _reviewRepository.GetReviewByUserID(user.UserID);
            List<List> lists = _listRepository.GetAll().Where(p => p.UserID == user.UserID).ToList();

            res.Banner = user.Banner;
            res.TotalReview = reviews.Count;
            res.TotalLists = lists.Count;
            res.Followers = follower;
            res.Following = following;

            return res;
        }

        public bool checkLikeUser(int id, string type)
        {
            Guid userID = (Guid)_userContext.UserId;
            if (userID == Guid.Empty)
            {
                throw new ServiceException("Hành động không khả thi");
            }

            if ("film".Equals(type))
            {
                List<Like> likeFilm = _likeRepository.GetAll().Where(p => p.UserID == userID && p.ParentID == id && "Film".Equals(p.Type)).ToList();
                if (likeFilm.Count > 0)
                {
                    return true;
                }
            }
            if ("review".Equals(type))
            {
                List<Like> reviews = _likeRepository.GetAll().Where(p => p.UserID == userID && p.ParentID == id && "Review".Equals(p.Type)).ToList();
                if (reviews.Count > 0)
                {
                    return true;
                }
            }
            if ("list".Equals(type))
            {
                List<Like> lists = _likeRepository.GetAll().Where(p => p.UserID == userID && p.ParentID == id && "List".Equals(p.Type)).ToList();
                if (lists.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public List<UserReviewDTO> getUserLiked(PagingParameter paging, string type, int id)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ServiceException("Type không được null hoặc khoảng trắng");
            }

            var likes = _likeRepository.GetAll();
            List<UserReviewDTO> res = new List<UserReviewDTO>();

            if ("review".Equals(type))
            {
                Review review = _reviewRepository.GetByID(id);
                if (review == null)
                {
                    throw new ServiceException("Không thấy review phù hợp");
                }
                likes = likes.Where(p => "Review".Equals(p.Type) && p.ParentID == id);
            }

            if ("film".Equals(type))
            {
                Film film = _filmRepository.GetByID(id);
                if (film == null)
                {
                    throw new ServiceException("Không thấy film phù hợp");
                }
                likes = likes.Where(p => "Film".Equals(p.Type) && p.ParentID == id);
            }

            if ("list".Equals(type))
            {
                List list = _listRepository.GetByID(id);
                if (list == null)
                {
                    throw new ServiceException("Không thấy list phù hợp");
                }
                likes = likes.Where(p => "List".Equals(p.Type) && p.ParentID == id);
            }

            int totalCount = likes.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / paging.pageSize);
            likes = likes.OrderByDescending(p => p.CreatedDate).Skip((paging.pageIndex - 1) * paging.pageSize).Take(paging.pageSize);
            likes = likes.ToList();

            foreach (var item in likes)
            {
                UserReviewDTO dto = new UserReviewDTO();
                User user = _userRepository.GetByID(item.UserID);
                if (user != null)
                {
                    dto.FullName = user.FullName;
                    dto.Avatar = user.Avatar;
                    dto.UserName = user.UserName;
                    dto.UserID = user.UserID;
                    res.Add(dto);
                }
            }
            return res;
        }

        #endregion
    }
}
