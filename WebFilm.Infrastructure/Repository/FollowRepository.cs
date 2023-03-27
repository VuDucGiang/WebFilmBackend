using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class FollowRepository : BaseRepository<int, Follow>, IFollowRepository
    {
        public FollowRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Follow> getFollowByUserID(Guid userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM Follow WHERE UserID = @v_UserID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var followers = SqlConnection.Query<Follow>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return followers.ToList();
            }
        }

        public List<Follow> getFollowingByUserID(Guid userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM Follow WHERE FollowedUserID = @v_UserID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var following = SqlConnection.Query<Follow>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return following.ToList();
            }
        }
    }
}
