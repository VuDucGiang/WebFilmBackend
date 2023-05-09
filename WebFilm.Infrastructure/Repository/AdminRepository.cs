using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;
using static Dapper.SqlMapper;

namespace WebFilm.Infrastructure.Repository
{
    public class AdminRepository : BaseRepository<int, Admin>, IAdminRepository
    {
        IUserContext _userContext;
        public AdminRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        public async Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM Film f ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();

                if (parameter.year != null)
                {
                    parameters.Add("@fromYear", parameter.year);
                    parameters.Add("@toYear", parameter.year + 9);
                    where += @" AND YEAR(release_date) BETWEEN @fromYear AND @toYear";
                }

                if (parameter.sort != null && parameter.sortBy != null)
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                if (!string.IsNullOrEmpty(parameter.genre))
                {
                    parameters.Add("@genre", $"{{\"name\":\"{parameter.genre.ToLower()}\"}}", DbType.String);
                    where += " AND JSON_CONTAINS(LOWER(genres), @genre, '$')";
                }

                if (!string.IsNullOrEmpty(parameter.filmName))
                {
                    parameters.Add("@title", parameter.filmName);
                    where += @" AND title LIKE CONCAT('%', @title, '%')";
                }

                sql += where + @$" GROUP BY f.FilmID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(FilmID) FROM Film " + where;

                //parameters.Add("@userID", _userContext.UserId);
                //parameters.Add("@filter", parameter.filter);
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
        public int UpdateFilm(int id, Film_Admin entity)
        {
            var keyName = "FilmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `film` SET ");

                PropertyInfo[] properties = typeof(Film_Admin).GetProperties();

                DynamicParameters parameters = new DynamicParameters();

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != keyName && property.Name != "CreatedDate")
                    {
                        if (property.Name == "ModifiedDate")
                        {
                           // sql.Append($"{property.Name} = @{property.Name}, ");
                            //parameters.Add(property.Name, DateTime.Now);

                        }
                        else
                        {
                            if (property.GetValue(entity) != null)
                            {
                                sql.Append($"{property.Name} = @{property.Name}, ");
                                parameters.Add(property.Name, property.GetValue(entity));
                            }

                        }
                    }
                }

                sql.Remove(sql.Length - 2, 2); // remove the last comma and space

                sql.Append($" WHERE {keyName} = @{keyName}");

                parameters.Add(keyName, id);
                //Trả dữ liệu về client
                var res = SqlConnection.Execute(sql.ToString(), parameters);
                SqlConnection.Close();
                return res;
            }
        }


    }
}
