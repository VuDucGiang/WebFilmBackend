using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class RatingRepository : BaseRepository<int, Rating>, IRatingRepository
    {
        public RatingRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<RateStatDTO> GetRatesByUserID(Guid userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT r.Score as Value, count(r.Score) as Total," +
                    " (COUNT(r.Score) * 100 / (SELECT COUNT(r.Score) FROM Rating r WHERE  r.UserID = @v_UserID)) AS Percent" +
                    " FROM Rating r WHERE  r.UserID = @v_UserID group by r.Score";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var dto = SqlConnection.Query<RateStatDTO>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return dto.ToList();
            }
        }
    }
}
