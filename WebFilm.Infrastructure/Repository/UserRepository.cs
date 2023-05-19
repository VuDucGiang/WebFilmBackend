using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<Guid, User>, IUserRepository
    {
        IUserContext _userContext;
        public UserRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }
        #region Method

        public async Task<bool> EditFollow(Guid userID, bool follow)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @$"DELETE FROM Follow WHERE FollowedUserID = @followedUserID AND UserID = @userID;";
                if (follow)
                {
                    sqlCommand = @$"INSERT INTO Follow (UserID, UserName, FollowedUserID, FollowedUserName, CreatedDate, ModifiedDate)
                                        SELECT @userID, @userName, u.UserID, u.UserName, NOW(), NOW() FROM User u WHERE u.UserID = @followedUserID;";
                }
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("userID", _userContext.UserId);
                parameters.Add("userName", _userContext.UserName);
                parameters.Add("followedUserID", userID);
                var res = await SqlConnection.ExecuteAsync(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return true;
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// Author: Vũ Đức Giang(1/9/2022)
        public User GetUserByID(Guid userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM User WHERE UserID = @v_UserID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var user = SqlConnection.QueryFirstOrDefault<User>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return user;
            }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Signup(UserDto user)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"INSERT INTO User (UserID, UserName, FullName, Password, Email, DateOfBirth, Status, RoleType, ModifiedDate)
                                              VALUES (UUID(), @v_UserName, @v_FullName, @v_Password, @v_Email, @v_DateOfBirth, @v_Status, @v_RoleType, NOW());";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserName", user.UserName);
                parameters.Add("v_FullName", user.FullName);
                parameters.Add("v_Password", user.Password);
                parameters.Add("v_Email", user.Email);
                parameters.Add("v_DateOfBirth", user.DateOfBirth);
                parameters.Add("v_Status", user.Status);
                parameters.Add("v_RoleType", user.RoleType);
                var res = SqlConnection.Execute(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserDto Login(string email)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT * FROM User WHERE Email = @v_Email";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_Email", email);
                var res = SqlConnection.QueryFirstOrDefault<UserDto>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }

        /// <summary>
        /// Check trùng username
        /// </summary>
        /// Author: Vũ Đức Giang
        public bool CheckDuplicateEmail(string email)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "SELECT @v_Email IN (SELECT email from User);";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_Email", email);

                var res = SqlConnection.QueryFirstOrDefault<int>(sql: sqlCheck, param: parameters);
                if (res == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CheckDuplicateUserName(string userName)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "SELECT UserName FROM User WHERE UserName = @v_UserName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_UserName", userName);

                var res = SqlConnection.QueryFirstOrDefault<string>(sql: sqlCheck, param: parameters);
                if (res != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool ActiveUser(string email)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "UPDATE User SET Status = 2 WHERE Email = @v_Email";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_Email", email);

                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                
                return true;
            }
        }

        public bool ChangePassword(string email, string newPass)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "UPDATE User u SET Password = @v_NewPass WHERE u.Email = @v_Email;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_Email", email);
                parameters.Add("@v_NewPass", newPass);

                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool ChangeInfo(ChangeInfoParam user)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = @$"UPDATE User u 
                                SET UserName = @UserName,
                                    FullName = @FullName,
                                    Bio = @Bio,
                                    FavouriteFilmList = @FavouriteFilmList,
                                    ModifiedDate = NOW()
                                WHERE UserID = @UserID;
                                
                                UPDATE Follow f 
                                LEFT JOIN User u1 ON f.FollowedUserID = u1.UserID
                                SET f.FollowedUserName = u1.UserName
                                WHERE f.FollowedUserID = @UserID;

                                UPDATE Follow f 
                                LEFT JOIN User u ON f.UserID = u.UserID
                                SET f.UserName = u.UserName
                                WHERE f.UserID = @UserID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserID", _userContext.UserId);
                parameters.Add("@UserName", user.UserName);
                parameters.Add("@FullName", user.FullName);
                parameters.Add("@Bio", user.Bio);
                parameters.Add("@FavouriteFilmList", user.FavouriteFilmList);

                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                return true;
            }
        }

        public bool ChangeAvatar(string url)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = @$"UPDATE User u 
                                SET Avatar = @url,
                                    ModifiedDate = NOW()
                                WHERE UserID = @UserID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserID", _userContext.UserId);
                parameters.Add("@url", url);
                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                return true;
            }
        }

        public bool ChangeBanner(string url)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = @$"UPDATE User u 
                                SET Banner = @url,
                                    ModifiedDate = NOW()
                                WHERE UserID = @UserID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserID", _userContext.UserId);
                parameters.Add("@url", url);
                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                return true;
            }
        }

        public bool AddTokenReset(UserDto user)
        {
           
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "UPDATE User u SET u.PasswordResetToken = @v_PasswordResetToken, u.ResetTokenExpires = @v_ResetTokenExpires WHERE u.UserID = @v_UserID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_UserID", user.UserID);
                parameters.Add("@v_PasswordResetToken", user.PasswordResetToken);
                parameters.Add("@v_ResetTokenExpires", user.ResetTokenExpires);

                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<UserDto> GetUserByTokenReset(string token)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM User WHERE PasswordResetToken = @v_PasswordResetToken";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_PasswordResetToken", token);
                var user = await SqlConnection.QueryFirstOrDefaultAsync<UserDto>(sqlCommand, parameters);
                //Trả dữ liệu về client
                SqlConnection.Close();
                return user;
            }
        }

        public async Task<PagingResult> GetPaging(int pageSize, int pageIndex, string filter, string sort, TypeUser typeUser, string userName)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;
                var tableJoin = "";
                var where = "1 = 1";
                var orderBy = "";
                if(!String.IsNullOrEmpty(sort)) {
                    orderBy += @$"ORDER BY {sort}";
                }
                switch (typeUser)
                {
                    case TypeUser.All:
                        break;
                    case TypeUser.Following:
                        tableJoin = "INNER JOIN Follow f ON u.UserID = f.FollowedUserID";
                        where = "f.UserName = @userName";
                        break;
                    case TypeUser.Follower:
                        tableJoin = "INNER JOIN Follow f ON u.UserID = f.UserID";
                        where = "f.FollowedUserName = @userName";
                        break;
                    case TypeUser.Blocked:
                        tableJoin = "INNER JOIN Block b ON u.UserID = b.BlockedUserID";
                        where = "b.UserName = @userName";
                        break;
                    default:
                        break;
                }
                var sqlCommand = @$"SET GLOBAL sql_mode=(SELECT REPLACE(@@sql_mode,'ONLY_FULL_GROUP_BY',''));SELECT DISTINCT u.UserID, u.UserName, u.FullName, u.Avatar,
                                    COUNT(DISTINCT l.ListID) AS Lists,
                                    COUNT(DISTINCT l1.LikeID) AS Likes,
                                    COUNT(DISTINCT r.ReviewID) AS Reviews,
                                    IF(f1.FollowID IS NOT NULL, True, False) AS Followed,
                                    COUNT(DISTINCT f3.FollowID) AS Follower,
                                    COUNT(DISTINCT f4.FollowID) AS Following
                                    FROM User u 
                                    LEFT JOIN Follow f1 ON u.UserID = f1.FollowedUserID AND @userName = f1.UserName
                                    LEFT JOIN Follow f3 ON u.UserID = f3.FollowedUserID 
                                    LEFT JOIN Follow f4 ON u.UserID = f4.UserID
                                    LEFT JOIN List l ON u.UserID = l.UserID
                                    LEFT JOIN `Like` l1 ON u.UserID = l1.UserID
                                    LEFT JOIN Review r ON u.UserID = r.UserID
                                    {tableJoin}
                                    WHERE {where} AND (u.UserName LIKE CONCAT('%', @filter, '%') OR u.Email LIKE CONCAT('%', @filter, '%'))
                                    GROUP BY u.UserID
                                    {orderBy} LIMIT @pageSize OFFSET @offset;
                                    SELECT COUNT(*) FROM User u {tableJoin} WHERE {where};";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filter", filter);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userName", userName);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                var total = result.Read<int>().Single();
                int totalPage = (int)Math.Ceiling((double)total / pageSize);
                SqlConnection.Close();
                return new PagingResult
                {
                    Data = data,
                    Total = total,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalPage = totalPage,
                };
            }
        }

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;
                string where = "";
                switch (sort)
                {
                    case "All":
                        break;
                    case "Week":
                        where = "AND WEEK(f2.CreatedDate) = WEEK(CURDATE()) AND YEAR(f2.CreatedDate) = YEAR(CURDATE())";
                        break;
                    case "Month":
                        where = "AND MONTH(f2.CreatedDate) = MONTH(CURDATE()) AND YEAR(f2.CreatedDate) = YEAR(CURDATE())";
                        break;
                    case "Year":
                        where = "AND YEAR(f2.CreatedDate) = YEAR(CURDATE())";
                        break;
                    default:
                        break;
                }
                var sqlCommand = @$"SET GLOBAL sql_mode=(SELECT REPLACE(@@sql_mode,'ONLY_FULL_GROUP_BY',''));SELECT u.UserID, u.UserName, u.FullName, u.FavouriteFilmList, u.Avatar,
                                    IF(f1.FollowID IS NOT NULL, true, FALSE) AS Followed,
                                    r.LikesCount,
                                    COUNT(DISTINCT r.ReviewID) AS Reviews,
                                    COUNT(DISTINCT f2.FollowID) AS FollowInSort,
                                    COUNT(DISTINCT f3.FollowID) AS Follower,
                                    COUNT(DISTINCT f4.FollowID) AS Following 
                                    FROM User u
                                    LEFT JOIN Follow f1 ON f1.UserID = @userId AND u.UserID = f1.FollowedUserID
                                    LEFT JOIN Follow f2 ON u.UserID = f2.FollowedUserID {where}
                                    LEFT JOIN Follow f3 ON u.UserID = f3.FollowedUserID 
                                    LEFT JOIN Follow f4 ON u.UserID = f4.UserID
                                    LEFT JOIN Review r ON u.UserID = r.UserID 
                                    WHERE (u.UserName LIKE CONCAT('%', @filter, '%') OR u.Email LIKE CONCAT('%', @filter, '%'))
                                    GROUP BY u.UserID
                                    ORDER BY FollowInSort DESC 
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(*) FROM User u WHERE (u.UserName LIKE CONCAT('%', @filter, '%') OR u.Email LIKE CONCAT('%', @filter, '%'));";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filter", filter);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userId", _userContext.UserId);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                foreach (var item in data)
                {
                    var user = (IDictionary<string, object>)item;
                    var favouriteFilmList = (string)user["FavouriteFilmList"];
                    if (!string.IsNullOrEmpty(favouriteFilmList))
                    {
                        var films = JsonConvert.DeserializeObject<List<BaseFilmDTO>>(favouriteFilmList);
                        user["FavouriteFilmList"] = films;
                    }
                }
                var total = result.Read<int>().Single();
                int totalPage = (int)Math.Ceiling((double)total / pageSize);
                SqlConnection.Close();
                return new
                {
                    Data = data,
                    Total = total,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalPage = totalPage
                };
            }
        }

        public User getUserByUsername(string username)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM User WHERE  UserName = @v_UserName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserName", username);
                var user = SqlConnection.QueryFirstOrDefault<User>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return user;
            }
        }


        #endregion
    }
}
