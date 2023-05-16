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
using static Dapper.SqlMapper;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Enitites.Similar_film;
using Newtonsoft.Json.Linq;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Credit;

namespace WebFilm.Infrastructure.Repository
{
    public class FilmRepository : BaseRepository<int, Film>, IFilmRepository
    {
        IUserContext _userContext;
        IReviewRepository _reviewRepository;
        IJournalRepository _journalRepository;

        public FilmRepository(IConfiguration configuration, IUserContext userContext, IReviewRepository reviewRepository, IJournalRepository journalRepository) : base(configuration)
        {
            _userContext = userContext;
            _reviewRepository = reviewRepository;
            _journalRepository = journalRepository;
        }

        public async Task<object> GetListUserLiked(int pageSize, int pageIndex, int filmID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;
                var sql = @$"SELECT u.UserID, u.UserName, u.FullName, u.Avatar,
                            COUNT(DISTINCT f1.FollowID) AS Follower,
                            COUNT(DISTINCT f2.FollowID) AS Following,
                            COUNT(DISTINCT r.ReviewID) AS Reviews
                            FROM `like` l
                            LEFT JOIN user u ON l.UserID = u.UserID
                            LEFT JOIN follow f1 ON u.UserID = f1.FollowedUserID 
                            LEFT JOIN follow f2 ON u.UserID = f2.UserID
                            LEFT JOIN review r ON u.UserID = r.UserID
                            WHERE l.Type = 'Film' AND l.ParentID = @filmID
                            Group by u.UserID
                            LIMIT @pageSize OFFSET @offset;
                            
                            SELECT COUNT(DISTINCT l.UserID)
                            FROM `like` l
                            WHERE l.Type = 'Film' AND l.ParentID = @filmID";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@filmID", filmID);

                var result = await SqlConnection.QueryMultipleAsync(sql, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                var total = result.Read<int>().Single();
                int totalPage = (int)Math.Ceiling((double)total / pageSize);
                sql = @$"SELECT f.FilmID, f.poster_path Poster_path, f.release_date Release_date, f.title Title FROM film f WHERE f.FilmID = @filmID";
                var film = await SqlConnection.QueryAsync(sql, parameters);
                return new
                {
                    Data = data,
                    Film = film.FirstOrDefault(),
                    Total = total,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalPage = totalPage
                };
            }
        }

        public async Task<string> CheckDuplicateFilmInList(int filmID, string listIDs)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var listIDArr = listIDs.Split(',');
                var msg = "";
                foreach (var item in listIDArr)
                {
                    var sqlCommand = @$"SELECT l.ListName FROM filmlist f LEFT JOIN list l ON f.ListID = l.ListID WHERE f.FilmID = @filmID AND f.ListID = @listID";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("filmID", filmID);
                    parameters.Add("listID", item);
                    var result = await SqlConnection.QueryAsync<List>(sqlCommand, parameters);
                    if (result.ToList().Count > 0)
                    {
                        msg += result.ToList()[0].ListName + ", ";
                    }
                }
                //Trả dữ liệu về client
                return msg;
            }
        }

        public async Task<bool> CheckPermissionInList(string listIDs)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var listIDArr = listIDs.Split(',');
                foreach (var item in listIDArr)
                {
                    var sqlCommand = @$"SELECT l.UserID FROM list l WHERE l.ListID = @listID";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("listID", item);
                    var result = await SqlConnection.QueryAsync<Guid>(sqlCommand, parameters);
                    if (result.ToList().Count > 0)
                    {
                        if (result.ToList()[0] != _userContext.UserId)
                        {
                            return false;
                        }
                    }
                }
                //Trả dữ liệu về client
                return true;
            }
        }

        public async Task<bool> AddFilmToList(int filmID, string listIDs)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var listIDArr = listIDs.Split(',');
                foreach (var item in listIDArr)
                {
                    var sqlCommand = @$"INSERT INTO filmlist (FilmID, ListID, CreatedDate, ModifiedDate)
                                        VALUES (@filmID, @listID, NOW(), NOW());";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("filmID", filmID);
                    parameters.Add("listID", item);
                    await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                }
                //Trả dữ liệu về client
                return true;
            }
        }

        public async Task<FilmDto> GetDetailByID(int id)
        {
            RateStat rateStats = new RateStat();
            List<RateStatDTO> rateStatsPopular = _reviewRepository.GetRatesByFilmID(id);
            if (rateStatsPopular.Count > 0)
            {
                List<float> ratesValue = new List<float>();
                for (float i = 1; i <= 10; i++)
                {
                    ratesValue.Add(i / 2f);
                }
                foreach (RateStatDTO statDTO in rateStatsPopular)
                {
                    if (ratesValue.Contains(statDTO.Value))
                    {
                        ratesValue.Remove(statDTO.Value);
                    }
                }

                foreach (float rate in ratesValue)
                {
                    RateStatDTO newRate = new RateStatDTO();
                    newRate.Value = rate;
                    rateStatsPopular.Add(newRate);
                }


                rateStats.List = rateStatsPopular;
                rateStats.Total = rateStatsPopular.Select(p => p.Total).Sum();
                if (rateStats.Total > 0)
                {
                    rateStats.RateAverage = rateStatsPopular.Select(p => p.Value * p.Total).Sum() / rateStats.Total;
                }
            }

            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = @$"SELECT f.*,
                                    COUNT(DISTINCT f1.ListID) AS Appears
                                    FROM film f
                                    LEFT JOIN filmlist f1 ON f.FilmID = f1.FilmID
                                    WHERE f.FilmID = @id
                                    GROUP BY f.FilmID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@userID", _userContext.UserId);
                //Trả dữ liệu về client
                var entities = await SqlConnection.QueryFirstOrDefaultAsync<FilmDto>(sqlCommand, parameters);
                entities.RateStats = rateStats;
                entities.MentionedInArticles = _journalRepository.GetMentionedInArticle(id);
                sqlCommand = @$"SELECT JSON_ARRAYAGG(
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
                                      ) AS credits
                                    FROM credit c1 
                                    WHERE c1.FilmID = @id;";
                var credits = await SqlConnection.QueryAsync<string>(sqlCommand, parameters);
                if (credits.ToList().Count > 0)
                {
                  entities.Credits = credits.ToList()[0];
                }
                SqlConnection.Close();
                return entities;
            }
        }

        public async Task<object> GetInfoUser(int id)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = @$"SELECT 
                                    IF(r.ReviewID IS NOT NULL, True, False) AS Reviewed,
                                    IF(w.watchlistID IS NOT NULL, True, False) AS Watchlisted,
                                    IF(l.LikeID IS NOT NULL, True, False) AS Liked 
                                    FROM film f
                                    LEFT JOIN `like` l ON f.FilmID = l.ParentID AND l.UserID = @userID AND l.Type = 'Film'
                                    LEFT JOIN review r ON r.FilmID = f.FilmID AND r.UserID = @userID
                                    LEFT JOIN watchlist w ON  f.FilmID = w.FilmID AND w.UserID = @userID
                                    WHERE f.FilmID = @id
                                    GROUP BY f.FilmID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@userID", _userContext.UserId);
                //Trả dữ liệu về client
                var entities = await SqlConnection.QueryFirstOrDefaultAsync<object>(sqlCommand, parameters);
                SqlConnection.Close();
                return entities;
            }
        }

        public async Task<object> GetPaging(PagingParameterFilm parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sql = "SELECT f.FilmID, f.poster_path AS Poster_path, f.title AS Title, f.release_date AS Release_date, COUNT(f1.ListID) AS Appears, IF(l.LikeID IS NOT NULL, True, False) AS Liked FROM Film f LEFT JOIN `like` l ON f.FilmID = l.ParentID AND l.UserID = @userID AND l.Type = 'Film' LEFT JOIN filmlist f1 ON f.FilmID = f1.FilmID ";
                var where = "WHERE 1=1";
                var orderBy = "";
                DynamicParameters parameters = new DynamicParameters();

                if (parameter.year != null)
                {
                    parameters.Add("@fromYear", parameter.year);
                    parameters.Add("@toYear", parameter.year + 9);
                    where += @" AND YEAR(release_date) BETWEEN @fromYear AND @toYear";
                }

                if (parameter.rating != null)
                {
                    orderBy += @$"Order By vote_average {parameter.rating}";
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
                var sqlCommand = @$"SELECT f.FilmID, f.poster_path AS Poster_path, f.title AS Title, f.release_date AS Release_date, f.LikesCount, f.ReviewsCount, COUNT(f1.ListID) AS Appears, IF(l.LikeID IS NOT NULL, True, False) AS Liked, COUNT(l1.LikeID) AS LikeInSort FROM film f
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
                var sqlCommand = @$"SELECT r.ReviewID, r.FilmID, MAX(r.CreatedDate) AS LatestReviewDate, f.title AS Title, f.poster_path AS Poster_path, f.release_date AS Release_date
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
                var sqlCommand = @$"SELECT rf.FilmID, rf.poster_path Poster_path FROM related_film rf WHERE rf.DetailFilmID = @id LIMIT @pageSize OFFSET @offset;
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

        public async Task<object> Similar(int id, PagingParameter parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                var sqlCommand = @$"SELECT sf.FilmID, sf.poster_path Poster_path FROM similar_film sf WHERE sf.DetailFilmID = @id LIMIT @pageSize OFFSET @offset;
                                    SELECT COUNT(sf.Similar_filmID) FROM similar_film sf WHERE sf.DetailFilmID = @id";

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
