using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using System.Text.Json;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;
using System.Data;

namespace WebFilm.Infrastructure.Repository
{
    public class FilmRepository : BaseRepository<int, Film>, IFilmRepository
    {
        IUserContext _userContext;

        public FilmRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;

        }

        public async Task<object> GetPaging(PagingParameterFilm parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT f.*, IF(l.LikeID IS NOT NULL, True, False) AS Liked FROM Film f LEFT JOIN `like` l ON f.FilmID = l.ParentID AND l.UserID = @userID "; 
                var where = "WHERE 1=1";
                DynamicParameters parameters = new DynamicParameters();

                if (parameter.year != null)
                {
                    parameters.Add("@year", parameter.year);
                    where += @" AND YEAR(release_date) = @year";
                }

                if (parameter.vote_average != null)
                {
                    parameters.Add("@voteAverage", parameter.vote_average);
                    where += @" AND Vote_average = @voteAverage";
                }

                if (!string.IsNullOrEmpty(parameter.genre))
                {
                    parameters.Add("@genre", $"{{\"name\":\"{parameter.genre}\"}}", DbType.String);
                    where += " AND JSON_CONTAINS(genres, @genre, '$')";
                }

                if (!string.IsNullOrEmpty(parameter.title))
                {
                    parameters.Add("@title", parameter.title);
                    where += @" AND title LIKE CONCAT('%', @title, '%')";
                }

                sql += where + @$" LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(FilmID) FROM Film " + where;
               
                parameters.Add("@userID", _userContext.UserId);
                parameters.Add("@filter", parameter.filter);
                parameters.Add("@pageSize", parameter.pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@name", "name");

                var result = await SqlConnection.QueryMultipleAsync(sql, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                var total = result.Read<int>().Single();
                int totalPage = (int)Math.Ceiling((double)total / parameter.pageSize);
                return new
                {
                    Data = data,
                    Total = total,
                    PageSize = parameter.pageSize,
                    PageIndex = parameter.pageIndex,
                    TotalPage = totalPage
                };
            }
        }
    }
}
