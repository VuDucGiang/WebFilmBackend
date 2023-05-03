using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Infrastructure.Repository
{
    public class ReviewRepository : BaseRepository<int, Review>, IReviewRepository
    {
        IUserContext _userContext;
        public ReviewRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        public async Task<object> GetReviewOfUser(int pageSize, int pageIndex, string userName)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;

                var sqlCommand = @$"SELECT r.ReviewID, r.Content, r.LikesCount, r.HaveSpoiler, r.WatchedDate, r.Score, r.CommentsCount,
                                    JSON_OBJECT(
                                        'FilmID', f.FilmID,
                                        'Poster_path', f.poster_path,
                                        'Release_date', f.release_date,
                                        'Title', f.title
                                    ) AS Film,
                                    JSON_OBJECT(
                                        'UserID', u.UserID,
                                        'UserName', u.UserName,
                                        'Avatar', u.Avatar
                                    ) AS User
                                    FROM review r
                                    LEFT JOIN film f ON r.FilmID = f.FilmID
                                    LEFT JOIN user u ON r.UserID = u.UserID
                                    WHERE u.UserName = @userName
                                    ORDER BY r.WatchedDate DESC
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(DISTINCT r.ReviewID) 
                                    FROM review r
                                    LEFT JOIN film f ON r.FilmID = f.FilmID
                                    LEFT JOIN user u ON r.UserID = u.UserID
                                    WHERE u.UserName = @userName;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userName", userName);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                foreach (var item in data)
                {
                    var reviews = (IDictionary<string, object>)item;
                    var film = (string)reviews["Film"];
                    if (!string.IsNullOrEmpty(film))
                    {
                        reviews["Film"] = JsonConvert.DeserializeObject<BaseFilmDTO>(film);
                    }

                    var user = (string)reviews["User"];
                    if (!string.IsNullOrEmpty(user))
                    {
                        var userObject = JObject.Parse(user);
                        reviews["User"] = new
                        {
                            UserID = (string)userObject["UserID"],
                            UserName = (string)userObject["UserName"],
                            Avatar = (string?)userObject["Avatar"]
                        };

                    }
                }
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

        public List<Review> GetReviewByUserID(Guid userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM Review WHERE UserID = @v_UserID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var reviews = SqlConnection.Query<Review>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return reviews.ToList();
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
                var sqlCommand = @$"SELECT r.*, u.UserName, f.title AS Title, f.poster_path AS Poster_path, f.overview Overview, f.release_date Release_date, f.vote_average Vote_average, IF(l.LikeID IS NOT NULL, True, False) AS Liked, COUNT(l1.LikeID) AS LikeInSort FROM review r 
                                    INNER JOIN user u ON u.UserID = r.UserID
                                    INNER JOIN film f ON f.FilmID = r.FilmID
                                    LEFT JOIN `like` l ON r.ReviewID = l.ParentID AND l.UserID = @userID AND l.Type = 'Review'
                                    LEFT JOIN `like` l1 ON r.ReviewID = l1.ParentID AND l1.Type = 'Review' {where}
                                    GROUP BY r.ReviewID
                                    ORDER BY LikeInSort DESC
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(r.ReviewID) FROM review r;";
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

        public async Task<object> GetPaging(PagingFilterParameter parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (parameter.pageIndex - 1) * parameter.pageSize;
                string orderBy = "";
                switch (parameter.sort.ToLower())
                {
                    case "popularity":
                        orderBy = "ORDER BY r.LikesCount DESC";
                        break;
                    case "highest rating":
                        orderBy = "ORDER BY r.Score DESC";
                        break;
                    case "lowest rating":
                        orderBy = "ORDER BY r.Score ASC";
                        break;
                    case "most recent":
                        orderBy = "ORDER BY r.CreatedDate DESC";
                        break;
                    case "earliest":
                        orderBy = "ORDER BY r.CreatedDate ASC";
                        break;
                    default:
                        break;
                }

                string where = "";
                string join = "";
                if (_userContext.UserId != null)
                {
                    switch (parameter.from.ToLower())
                    {
                        case "everyone":
                            break;
                        case "friends":
                            join = "LEFT JOIN follow f2 ON f2.UserID = @userID ";
                            where = "AND r.UserID = f2.FollowedUserID";
                            break;
                        case "you":
                            where = "AND r.UserID = @userID";
                            break;
                        default:
                            break;
                    }
                }
                var sqlCommand = @$"SELECT r.*, u.UserName,
                                    IF(l.LikeID IS NOT NULL, True, False) AS Liked
                                    FROM review r 
                                    {join}
                                    INNER JOIN user u ON u.UserID = r.UserID {where}
                                    INNER JOIN film f ON f.FilmID = r.FilmID
                                    LEFT JOIN `like` l ON r.ReviewID = l.ParentID AND l.UserID = @userID AND l.Type = 'Review'
                                    WHERE r.FilmID = @filmID
                                    GROUP BY r.ReviewID
                                    {orderBy}
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(DISTINCT r.ReviewID) FROM review r
                                    {join}
                                    INNER JOIN user u ON u.UserID = r.UserID {where}
                                    INNER JOIN film f ON f.FilmID = r.FilmID
                                    WHERE r.FilmID = @filmID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filter", parameter.filter);
                parameters.Add("@pageSize", parameter.pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userID", _userContext.UserId);
                parameters.Add("@filmID", parameter.id);
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

        public List<ListPopularWeekDTO> GetRecentWeek()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT ParentID as ListID, COUNT(*) as LikeCounts FROM `like` " +
                    "WHERE date >= DATE_SUB(NOW(), INTERVAL 1 WEEK) and `type` = 'Review' " +
                    "GROUP BY ParentID ORDER BY LikeCounts DESC LIMIT 6;";
                DynamicParameters parameters = new DynamicParameters();
                var lists = SqlConnection.Query<ListPopularWeekDTO>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return lists.ToList();
            }
        }

        public List<RateStatDTO> GetRatesByUserID(Guid userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT r.Score as Value, count(r.Score) as Total," +
                    " ROUND((COUNT(r.Score) * 100 / (SELECT COUNT(r.Score) FROM Review r WHERE  r.UserID = @v_UserID)),2) AS Percent" +
                    " FROM Review r WHERE  r.UserID = @v_UserID group by r.Score";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var dto = SqlConnection.Query<RateStatDTO>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return dto.ToList();
            }
        }

        public int UpdateCommentCount(int reviewID, int commentCount)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "Update Review set CommentsCount = @v_CommentCount where ReviewID = @v_ReviewID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_CommentCount", commentCount);
                parameters.Add("v_ReviewID", reviewID);
                var res = SqlConnection.Execute(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateLikeCount(int reviewID, int likeCount)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "Update Review set LikesCount = @v_LikeCount where ReviewID = @v_ReviewID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_LikeCount", likeCount);
                parameters.Add("v_ReviewID", reviewID);
                var res = SqlConnection.Execute(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }
    } 
}
