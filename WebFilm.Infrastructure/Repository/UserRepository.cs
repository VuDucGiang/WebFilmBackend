using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<Guid, User>, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #region Method
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
                var sqlCommand = $@"INSERT INTO user (UserID, UserName, FullName, Password, Email, DateOfBirth, Status, RoleType, ModifiedDate)
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
                var sqlCommand = $"SELECT * FROM user WHERE Email = @v_Email";
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
                var sqlCheck = "SELECT userName FROM User WHERE Email = @v_Email";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_Email", email);

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
                var sqlCheck = "UPDATE user SET Status = 2 WHERE Email = @v_Email";
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
                var sqlCheck = "UPDATE user u SET Password = @v_NewPass WHERE u.Email = @v_Email;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_Email", email);
                parameters.Add("@v_NewPass", newPass);

                var res = SqlConnection.Execute(sql: sqlCheck, param: parameters);
                if(res > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool AddTokenReset(UserDto user)
        {
           
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "UPDATE user u SET u.PasswordResetToken = @v_PasswordResetToken, u.ResetTokenExpires = @v_ResetTokenExpires WHERE u.UserID = @v_UserID;";
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
                switch (typeUser)
                {
                    case TypeUser.All:
                        break;
                    case TypeUser.Following:
                        tableJoin = "INNER JOIN follow f ON u.UserID = f.FollowedUserID";
                        where = "f.UserName = @userName";
                        break;
                    case TypeUser.Follower:
                        tableJoin = "INNER JOIN follow f ON u.UserID = f.UserID";
                        where = "f.FollowedUserName = @userName";
                        break;
                    case TypeUser.Blocked:
                        tableJoin = "INNER JOIN block b ON u.UserID = b.BlockedUserID";
                        where = "b.UserName = @userName";
                        break;
                    default:
                        break;
                }
                var sqlCommand = @$"SELECT DISTINCT u.*,
                                    COUNT(DISTINCT l.ListID) AS Lists,
                                    COUNT(DISTINCT l1.LikeID) AS Likes,
                                    COUNT(DISTINCT r.ReviewID) AS Reviews,
                                    IF(f1.FollowID IS NOT NULL, true, FALSE) AS Followed
                                    FROM user u 
                                    LEFT JOIN follow f1 ON u.UserID = f1.FollowedUserID
                                    LEFT JOIN list l ON u.UserID = l.UserID
                                    LEFT JOIN `like` l1 ON u.UserID = l1.UserID
                                    LEFT JOIN review r ON u.UserID = r.UserID
                                    {tableJoin}
                                    WHERE {where} AND (u.UserName LIKE CONCAT('%', @filter, '%') OR u.Email LIKE CONCAT('%', @filter, '%'))
                                    GROUP BY u.UserID
                                    ORDER BY {sort} LIMIT @pageSize OFFSET @offset;
                                    SELECT COUNT(*) FROM user u {tableJoin} WHERE {where};";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filter", filter);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userName", userName);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                var total = result.Read<int>().Single();
                SqlConnection.Close();
                return new PagingResult
                {
                    Data = data,
                    Total = total
                };
            }
        }

        public User getUserByUsername(string username)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM UserName WHERE  = @v_UserName";
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
