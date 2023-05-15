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
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;
using static Dapper.SqlMapper;
using WebFilm.Core.Enitites.Answer;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Enitites.Similar_film;
using WebFilm.Core.Enitites.Credit;

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

                            var test = "dsfdsf".Replace('"', '\"');
                            if(test is String) { };
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

        public User GetUserByID(Guid id)
        {
            var keyName = "UserID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `user` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var user = SqlConnection.QueryFirstOrDefault<User>(sqlCommand, parameters);
                SqlConnection.Close();
                return user;
            }
        }

        public Film GetFilmByID(int id)
        {
            var keyName = "FilmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `film` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var film = SqlConnection.QueryFirstOrDefault<Film>(sqlCommand, parameters);
                SqlConnection.Close();
                return film;
            }
        }

        public Journal GetJournalByID(int id)
        {
            var keyName = "JournalID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `journal` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var journal = SqlConnection.QueryFirstOrDefault<Journal>(sqlCommand, parameters);
                SqlConnection.Close();
                return journal;
            }
        }

        public Question GetQuestionByID(int id)
        {
            var keyName = "QuestionID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `question` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var question = SqlConnection.QueryFirstOrDefault<Question>(sqlCommand, parameters);
                SqlConnection.Close();
                return question;
            }
        }

        public int DeleteQuestion(int id)
        {
            var keyName = "QuestionID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Question` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int AddQuestion(Question_Admin entity)
        {
            var keyName = "QuestionID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Question_Admin).GetProperties();

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
                var query = $"INSERT INTO `question` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateQuestion(int id, Question_Admin entity)
        {
            var keyName = "QuestionID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `question` SET ");

                PropertyInfo[] properties = typeof(Question_Admin).GetProperties();

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

                            var test = "dsfdsf".Replace('"', '\"');
                            if (test is String) { };
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

        public async Task<object> GetPagingQuestion(PagingParameterQuestion_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM question q ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();

                

                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                

                if (!string.IsNullOrEmpty(parameter.question))
                {
                    parameters.Add("@question", parameter.question);
                    where += @" AND Question LIKE CONCAT('%', @question, '%')";
                }
             
            

                sql += where + @$" GROUP BY q.QuestionID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(QuestionID) FROM Question " + where;

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

        public Answer GetAnswerByID(int id)
        {
            var keyName = "AnswerID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `answer` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var answer = SqlConnection.QueryFirstOrDefault<Answer>(sqlCommand, parameters);
                SqlConnection.Close();
                return answer;
            }
        }

        public int DeleteAnswer(int id)
        {
            var keyName = "AnswerID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Answer` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int AddAnswer(Answer_Admin entity)
        {
            var keyName = "AnswerID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Answer_Admin).GetProperties();

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
                var query = $"INSERT INTO `answer` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateAnswer(int id, Answer_Admin entity)
        {
            var keyName = "AnswerID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `answer` SET ");

                PropertyInfo[] properties = typeof(Answer_Admin).GetProperties();

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

                            var test = "dsfdsf".Replace('"', '\"');
                            if (test is String) { };
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

        public async Task<object> GetPagingAnswer(PagingParameterAnswer_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM answer a ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();



                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                if (parameter.RightAnswer != null)
                {
                    parameters.Add("@rightAnswer", parameter.RightAnswer);
                    where += @" AND RightAnswer = @rightAnswer";
                }

                if (parameter.QuestionID != null)
                {
                    parameters.Add("@questionID", parameter.QuestionID);
                    where += @" AND QuestionID = @questionID";
                }

                if (!string.IsNullOrEmpty(parameter.answer))
                {
                    parameters.Add("@answer", parameter.answer);
                    where += @" AND Answer LIKE CONCAT('%', @answer, '%')";
                }



                sql += where + @$" GROUP BY a.AnswerID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(AnswerID) FROM Answer " + where;

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

        public Related_film GetRelated_filmByID(int id)
        {
            var keyName = "Related_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `related_film` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var related_film = SqlConnection.QueryFirstOrDefault<Related_film>(sqlCommand, parameters);
                SqlConnection.Close();
                return related_film;
            }
        }

        public int DeleteRelated_film(int id)
        {
            var keyName = "Related_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Related_film` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int AddRelated_film(Related_film_Admin entity)
        {
            var keyName = "Related_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Related_film_Admin).GetProperties();

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
                var query = $"INSERT INTO `related_film` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateRelated_film(int id, Related_film_Admin entity)
        {
            var keyName = "Related_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `related_film` SET ");

                PropertyInfo[] properties = typeof(Related_film_Admin).GetProperties();

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

                            var test = "dsfdsf".Replace('"', '\"');
                            if (test is String) { };
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

        public async Task<object> GetPagingRelated_film(PagingParameterRelated_film_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM related_film r ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();



                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                if (parameter.DetailFilmID != null)
                {
                    parameters.Add("@detailFilmID", parameter.DetailFilmID);
                    where += @" AND DetailFilmID = @detailFilmID";
                }

                

                if (!string.IsNullOrEmpty(parameter.Title))
                {
                    parameters.Add("@title", parameter.Title);
                    where += @" AND title LIKE CONCAT('%', @title, '%')";
                }



                sql += where + @$" GROUP BY r.Related_filmID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(Related_filmID) FROM Related_film " + where;

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

        public Similar_film GetSimilar_filmByID(int id)
        {
            var keyName = "Similar_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `similar_film` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var similar_film = SqlConnection.QueryFirstOrDefault<Similar_film>(sqlCommand, parameters);
                SqlConnection.Close();
                return similar_film;
            }
        }

        public int DeleteSimilar_film(int id)
        {
            var keyName = "Similar_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Similar_film` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int AddSimilar_film(Similar_film_Admin entity)
        {
            var keyName = "Similar_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Similar_film_Admin).GetProperties();

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
                var query = $"INSERT INTO `similar_film` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateSimilar_film(int id, Similar_film_Admin entity)
        {
            var keyName = "Similar_filmID";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `similar_film` SET ");

                PropertyInfo[] properties = typeof(Similar_film_Admin).GetProperties();

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

                            var test = "dsfdsf".Replace('"', '\"');
                            if (test is String) { };
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

        public async Task<object> GetPagingSimilar_film(PagingParameterSimilar_film_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM similar_film s ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();



                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                if (parameter.DetailFilmID != null)
                {
                    parameters.Add("@detailFilmID", parameter.DetailFilmID);
                    where += @" AND DetailFilmID = @detailFilmID";
                }



                if (!string.IsNullOrEmpty(parameter.Title))
                {
                    parameters.Add("@title", parameter.Title);
                    where += @" AND title LIKE CONCAT('%', @title, '%')";
                }



                sql += where + @$" GROUP BY s.Similar_filmID {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(Similar_filmID) FROM Similar_film " + where;

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

        public Credit GetCreditByID(string id)
        {
            var keyName = "credit_id";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `credit` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var credit = SqlConnection.QueryFirstOrDefault<Credit>(sqlCommand, parameters);
                SqlConnection.Close();
                return credit;
            }
        }

        public int DeleteCredit(string id)
        {
            var keyName = "credit_id";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `Credit` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int AddCredit(Credit_Admin entity)
        {
            var keyName = "credit_id";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(Credit_Admin).GetProperties();

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
                var query = $"INSERT INTO `credit` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateCredit(string id, Credit_Admin entity)
        {
            var keyName = "credit_id";
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `credit` SET ");

                PropertyInfo[] properties = typeof(Credit_Admin).GetProperties();

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

                            var test = "dsfdsf".Replace('"', '\"');
                            if (test is String) { };
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

        public async Task<object> GetPagingCredit(PagingParameterCredit_Admin parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT * FROM credit c ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();



                if (!string.IsNullOrEmpty(parameter.sort) && !string.IsNullOrEmpty(parameter.sortBy))
                {
                    orderBy += @$"Order By {parameter.sortBy} {parameter.sort}";
                }

                if (!string.IsNullOrEmpty(parameter.Type))
                {
                    parameters.Add("@type", parameter.Type);
                    where += @" AND Type = @type";
                }



                if (!string.IsNullOrEmpty(parameter.Known_for_department))
                {
                    parameters.Add("@Known_for_department", parameter.Known_for_department);
                    where += @" AND Known_for_department LIKE CONCAT('%', @Known_for_department, '%')";
                }

                if (!string.IsNullOrEmpty(parameter.Name))
                {
                    parameters.Add("@Name1", parameter.Name);
                    where += @" AND name LIKE CONCAT('%', @Name1, '%')";
                }

                if (parameter.FilmID != null)
                {
                    parameters.Add("@FilmID", parameter.FilmID);
                    where += @" AND FilmID = @FilmID";
                }

                if (!string.IsNullOrEmpty(parameter.Original_name))
                {
                    parameters.Add("@Original_name", parameter.Original_name);
                    where += @" AND Original_name LIKE CONCAT('%', @Original_name, '%')";
                }

                if (!string.IsNullOrEmpty(parameter.Character_))
                {
                    parameters.Add("@Character_", parameter.Character_);
                    where += @" AND Character_ LIKE CONCAT('%', @Character_, '%')";
                }

                

                sql += where + @$" GROUP BY c.credit_id {orderBy} LIMIT @pageSize OFFSET @offset;
                                SELECT COUNT(credit_id) FROM Credit " + where;

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

    }
}
