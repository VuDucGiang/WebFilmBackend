using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.WatchList;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Infrastructure.Repository
{
    public class WatchListRepository : BaseRepository<int, WatchList>, IWatchListRepository
    {
        IUserContext _userContext;
        public WatchListRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }
        public async Task<bool> AddWatchList(int filmID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {

                var sqlCommand = @$"INSERT INTO watchlist (UserID, FilmID, CreatedDate, ModifiedDate)
                                    VALUES (@userID, @filmID, NOW(), NOW());";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("filmID", filmID);
                parameters.Add("userID", _userContext.UserId);
                await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                return true;
            }
        }

        public async Task<bool> DeleteWatchList(ParamDelete param)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var listfilmID = param.filmIDs.Split(',');
                foreach (var item in listfilmID)
                {
                    var sqlCommand = @$"DELETE FROM watchlist Where FilmID = @filmID AND UserID = @userID;";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("filmID", item);
                    parameters.Add("userID", _userContext.UserId);
                    await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                }

                //Trả dữ liệu về client
                return true;
            }
        }
    }
}
