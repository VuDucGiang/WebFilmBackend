using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using WebFilm.Core.Enitites;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
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
                var sqlCommand = "Proc_GetUserByID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("m_UserID", userID);
                var user = SqlConnection.Query<User>(sqlCommand, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();

                //Trả dữ liệu về client
                SqlConnection.Close();
                return user[0];
            }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Signup(User user)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "Proc_InserUser";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserName", user.UserName);
                parameters.Add("v_Password", user.Password);
                parameters.Add("v_Email", user.Email);
                var res = SqlConnection.Execute(sqlCommand, parameters, commandType: System.Data.CommandType.StoredProcedure);

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
        public User Login(string userName)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT * FROM user WHERE UserName = @v_UserName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_UserName", userName);
                var res = SqlConnection.QueryFirstOrDefault<User>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }

        /// <summary>
        /// Check trùng username
        /// </summary>
        /// Author: Vũ Đức Giang
        public bool CheckDuplicateUserName(string userName)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "SELECT userName FROM User WHERE UserName = @v_UserName";
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

        #endregion
    }
}
