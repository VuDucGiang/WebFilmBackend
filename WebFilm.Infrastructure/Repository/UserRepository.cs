using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
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
                var sqlCommand = $@"INSERT INTO user (UserID, UserName, FullName, Password, Email, DateOfBirth, ModifiedBy, Status, RoleType, ModifiedDate)
                                              VALUES (UUID(), @v_UserName, @v_FullName, @v_Password, @v_Email, @v_DateOfBirth, '', @v_Status, @v_RoleType, NOW());";
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


        #endregion
    }
}
