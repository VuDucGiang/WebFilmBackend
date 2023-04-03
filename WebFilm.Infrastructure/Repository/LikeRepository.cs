using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class LikeRepository : BaseRepository<int, Like>, ILikeRepository
    {
        public LikeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Like getlikeByUserIDAndTypeAndParentID(Guid userID, string type, int parentID)
        {
                using (SqlConnection = new MySqlConnection(_connectionString))
                {
                    var sqlCommand = "SELECT * FROM `Like` WHERE  UserID = @v_UserID And Type = @v_type And ParentID = @v_parentID";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("v_UserID", userID);
                    parameters.Add("v_type", type);
                    parameters.Add("v_parentID", parentID);
                var like = SqlConnection.QueryFirstOrDefault<Like>(sqlCommand, parameters);

                    //Trả dữ liệu về client
                    SqlConnection.Close();
                    return like;
                }
        }
    }
}
