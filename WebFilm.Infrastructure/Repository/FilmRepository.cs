﻿using Dapper;
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
using static Dapper.SqlMapper;

namespace WebFilm.Infrastructure.Repository
{
    public class FilmRepository : BaseRepository<int, Film>, IFilmRepository
    {
        IUserContext _userContext;

        public FilmRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        public async Task<FilmDto> GetDetailByID(int id)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = @$"SELECT f.*,
                                    JSON_ARRAYAGG(
                                        JSON_OBJECT(
                                          'type', c1.type,
                                          'known_for_department', c1.known_for_department,
                                          'name', c1.name,
                                          'personID', c1.personID,
                                          'original_name', c1.original_name,
                                          'character_', c1.character_,
                                          'job', c1.job,
                                          'credit_id', c1.credit_id,
                                          'poster_path', c1.poster_path
                                        )
                                      ) AS credits,
                                    COUNT(f1.ListID) AS Appears, 
                                    IF(l.LikeID IS NOT NULL, True, False) AS Liked 
                                    FROM film f
                                    LEFT JOIN `like` l ON f.FilmID = l.ParentID AND l.UserID = @userID AND l.Type = 'Film'
                                    LEFT JOIN filmlist f1 ON f.FilmID = f1.FilmID
                                    LEFT JOIN credit c1 ON f.FilmID = c1.FilmID
                                    WHERE f.FilmID = @id
                                    GROUP BY f.FilmID;
                                    ";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@userID", _userContext.UserId);
                //Trả dữ liệu về client
                var entities = await SqlConnection.QueryFirstOrDefaultAsync<FilmDto>(sqlCommand, parameters);
                SqlConnection.Close();
                return entities;
            }
        }

        public async Task<object> GetPaging(PagingParameterFilm parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT f.FilmID, f.poster_path, f.title, f.release_date, COUNT(f1.ListID) AS Appears, IF(l.LikeID IS NOT NULL, True, False) AS Liked FROM Film f LEFT JOIN `like` l ON f.FilmID = l.ParentID AND l.UserID = @userID AND l.Type = 'Film' LEFT JOIN filmlist f1 ON f.FilmID = f1.FilmID ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();

                if (parameter.year != null)
                {
                    parameters.Add("@fromYear", parameter.year);
                    parameters.Add("@toYear", parameter.year + 9);
                    where += @" AND YEAR(release_date) BETWEEN @fromYear AND @toYear";
                }

                if (parameter.vote_average != null)
                {
                    orderBy += @$"Order By vote_average {parameter.vote_average}";
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

                sql += where + @$" GROUP BY f.FilmID {orderBy} LIMIT @pageSize OFFSET @offset;
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

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;
                string where = "";
                switch (sort)
                {
                    case "All":
                        break;
                    case "Week":
                        where = "AND WEEK(l1.CreatedDate) = WEEK(CURDATE()) AND YEAR(l1.CreatedDate) = YEAR(CURDATE())";
                        break;
                    case "Month":
                        where = "AND MONTH(l1.CreatedDate) = MONTH(CURDATE()) AND YEAR(l1.CreatedDate) = YEAR(CURDATE())";
                        break;
                    case "Year":
                        where = "AND YEAR(l1.CreatedDate) = YEAR(CURDATE())";
                        break;
                    default:
                        break;
                }
                var sqlCommand = @$"SELECT f.*, COUNT(f1.ListID) AS Appears, IF(l.LikeID IS NOT NULL, True, False) AS Liked, COUNT(l1.LikeID) AS LikeInSort FROM film f
                                    LEFT JOIN `like` l ON f.FilmID = l.ParentID AND l.UserID = @userID AND l.Type = 'Film'
                                    LEFT JOIN filmlist f1 ON f.FilmID = f1.FilmID
                                    LEFT JOIN `like` l1 ON f.FilmID = l1.ParentID AND l1.Type = 'Film' {where}
                                    GROUP BY f.FilmID
                                    ORDER BY LikeInSort DESC
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(*) FROM film f;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filter", filter);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userID", _userContext.UserId);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                var total = result.Read<int>().Single();
                int totalPage = (int)Math.Ceiling((double)total / pageSize);
                return new
                {
                    Data = data,
                    Total = total,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalPage = totalPage
                };
            }
        }

        public async Task<object> JustReviewed()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = @$"SELECT r.ReviewID, r.FilmID, MAX(r.CreatedDate) AS LatestReviewDate, f.title, f.poster_path, f.release_date
                                    FROM review r
                                    JOIN film f ON r.FilmID = f.FilmID
                                    GROUP BY r.FilmID
                                    ORDER BY LatestReviewDate DESC
                                    LIMIT 12;

                                    SELECT COUNT(DISTINCT r.FilmID) FROM review r";

                var result = await SqlConnection.QueryMultipleAsync(sqlCommand);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                var total = result.Read<int>().Single();
                return new
                {
                    Data = data,
                    TotalReview = total,
                };
            }
        }

        public async Task<object> Related(int id, PagingParameter parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sqlCommand = @$"SELECT rf.* FROM related_film rf WHERE rf.DetailFilmID = @id LIMIT @pageSize OFFSET @offset;
                                    SELECT COUNT(rf.Related_filmID) FROM related_film rf WHERE rf.DetailFilmID = @id";

                //Trả dữ liệu về client
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@filter", parameter.filter);
                parameters.Add("@pageSize", parameter.pageSize);
                parameters.Add("@offset", offset);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
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

        public int UpdateLikeCount(int filmID, int likeCount)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "Update Film set LikesCount = @v_LikeCount where FilmID = @v_FilmID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_LikeCount", likeCount);
                parameters.Add("v_FilmID", filmID);
                var res = SqlConnection.Execute(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }
    }
}