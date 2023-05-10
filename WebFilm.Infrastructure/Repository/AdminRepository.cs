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
using WebFilm.Core.Enitites.Journal;
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

                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
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
                    listData = data,
                    total = total,
                    pageSize = parameter.pageSize,
                    pageIndex = parameter.pageIndex,
                    totalPage = totalPage
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
        public int AddFilm(Film_Admin entity)
        {
            var keyName = "FilmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Film_Admin).GetProperties();

                foreach (var property in properties)
                {
                    if (property.Name != keyName)
                    {
                        if (property.Name == "ModifiedDate" || property.Name == "CreatedDate")
                        {
                            //parameters.Add("@" + property.Name, DateTime.Now);
                        }
                        else
                        {
                            parameters.Add("@" + property.Name, property.GetValue(entity));
                        }
                    }
                }

                var columns = string.Join(", ", properties.Where(p => p.Name != keyName).Select(p => p.Name));
                var values = string.Join(", ", properties.Where(p => p.Name != keyName).Select(p => "@" + p.Name));
                var query = $"INSERT INTO `film` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }
        public int DeleteFilm(int id)
        {
            var keyName = "FilmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Film` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public async Task<object> GetPagingUser(PagingParameterUser_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM User u ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();

                if (parameter.roleType != null)
                {
                    parameters.Add("@roleType", parameter.roleType);         
                    where += @" AND RoleType = @roleType";
                }

                if (parameter.status != null)
                {
                    parameters.Add("@status", parameter.status);
                    where += @" AND Status = @status";
                }

                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

               

                if (!string.IsNullOrEmpty(parameter.fullName))
                {
                    parameters.Add("@fullName", parameter.fullName);
                    where += @" AND FullName LIKE CONCAT('%', @fullName, '%')";
                }

                if (!string.IsNullOrEmpty(parameter.userName))
                {
                    parameters.Add("@userName", parameter.userName);
                    where += @" AND UserName LIKE CONCAT('%', @userName, '%')";
                }

                sql += where + @$" GROUP BY u.UserID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(UserID) FROM User " + where;

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
                    listData = data,
                    total = total,
                    pageSize = parameter.pageSize,
                    pageIndex = parameter.pageIndex,
                    totalPage = totalPage
                };
            }
        }
        public int UpdateUser(Guid id, User_Admin entity)
        {
            var keyName = "UserID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `user` SET ");

                PropertyInfo[] properties = typeof(User_Admin).GetProperties();

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
        public int DeleteUser(Guid id)
        {
            var keyName = "UserID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `User` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public async Task<object> GetPagingJournal(PagingParameterJournal_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM journal j ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(parameter.category))
                {
                    parameters.Add("@category", parameter.category);
                    where += @" AND Category = @category";
                }

                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                if (parameter.mentionedFilm != null)
                {
                    parameters.Add("@mentionedFilm", parameter.mentionedFilm);
                    where += @" AND MentionedFilm = @mentionedFilm";
                }

                if (!string.IsNullOrEmpty(parameter.title))
                {
                    parameters.Add("@title", parameter.title);
                    where += @" AND Title LIKE CONCAT('%', @title, '%')";
                }

                if (!string.IsNullOrEmpty(parameter.intro))
                {
                    parameters.Add("@intro", parameter.intro);
                    where += @" AND Intro LIKE CONCAT('%', @intro, '%')";
                }

                if (!string.IsNullOrEmpty(parameter.authorUserName))
                {
                    parameters.Add("@authorUserName", parameter.authorUserName);
                    where += @" AND json_extract(Author, '$.name') LIKE CONCAT('%', @authorUserName, '%')";
                }

                sql += where + @$" GROUP BY j.JournalID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(JournalID) FROM Journal " + where;

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
                    listData = data,
                    total = total,
                    pageSize = parameter.pageSize,
                    pageIndex = parameter.pageIndex,
                    totalPage = totalPage
                };
            }
        }
        public int UpdateJournal(int id, Journal_Admin entity)
        {
            var keyName = "JournalID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `journal` SET ");

                PropertyInfo[] properties = typeof(Journal_Admin).GetProperties();

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
        public int AddJournal(Journal_Admin entity)
        {
            var keyName = "JournalID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Journal_Admin).GetProperties();

                foreach (var property in properties)
                {
                    if (property.Name != keyName)
                    {
                        if (property.Name == "ModifiedDate" || property.Name == "CreatedDate")
                        {
                            //parameters.Add("@" + property.Name, DateTime.Now);
                        }
                        else
                        {
                            parameters.Add("@" + property.Name, property.GetValue(entity));
                        }
                    }
                }

                var columns = string.Join(", ", properties.Where(p => p.Name != keyName).Select(p => p.Name));
                var values = string.Join(", ", properties.Where(p => p.Name != keyName).Select(p => "@" + p.Name));
                var query = $"INSERT INTO `journal` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }
        public int DeleteJournal(int id)
        {
            var keyName = "JournalID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Journal` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }


    }
}
