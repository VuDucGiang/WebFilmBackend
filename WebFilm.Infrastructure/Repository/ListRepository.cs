using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Collections;
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
    public class ListRepository : BaseRepository<int, List>, IListRepository
    {
        IUserContext _userContext;
        public ListRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        public async Task<object> GetListOfUser(int pageSize, int pageIndex, string userName)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;

                var sqlCommand = @$"SELECT l.ListID, l.ListName, l.Description, l.LikesCount, l.CommentsCount,
                                    COUNT(f.FilmID) FilmsCount,
                                    JSON_ARRAYAGG(
                                        JSON_OBJECT(
                                            'FilmID', f1.FilmID,
                                            'Poster_path', f1.poster_path,
                                            'Release_date', f1.release_date,
                                            'Title', f1.title
                                        )
                                    ) AS List,
                                    JSON_OBJECT(
                                        'UserID', u.UserID,
                                        'UserName', u.UserName,
                                        'Avatar', u.Avatar
                                    ) AS User
                                    FROM list l
                                    LEFT JOIN filmlist f ON l.ListID = f.ListID
                                    INNER JOIN film f1 ON f.FilmID = f1.FilmID
                                    LEFT JOIN user u ON u.UserID = l.UserID
                                    WHERE u.UserName = @userName AND 
                                    ((l.Private = 1 AND l.UserID = @userID) OR l.Private = 0) 
                                    GROUP BY l.ListID
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(DISTINCT f.ListID) FROM list l
                                    LEFT JOIN filmlist f ON l.ListID = f.ListID
                                    INNER JOIN film f1 ON f.FilmID = f1.FilmID
                                    LEFT JOIN user u ON u.UserID = l.UserID
                                    WHERE u.UserName = @userName AND 
                                    ((l.Private = 1 AND l.UserID = @userID) OR l.Private = 0);";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userName", userName);
                parameters.Add("@userID", _userContext.UserId);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                foreach (var item in data)
                {
                    var lists = (IDictionary<string, object>)item;
                    var list = (string)lists["List"];
                    if (!string.IsNullOrEmpty(list))
                    {
                        var films = JsonConvert.DeserializeObject<List<BaseFilmDTO>>(list);
                        if (films != null)
                        {
                            lists["List"] = films.Take(5);
                        }
                        else
                        {
                            lists["List"] = new List<BaseFilmDTO>();
                        }
                    }

                    var user = (string)lists["User"];
                    if (!string.IsNullOrEmpty(user))
                    {
                        var userObject = JObject.Parse(user);
                        lists["User"] = new
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

        public async Task<bool> AddListDetail(ListDTO list)
        {
            list.ListID = await this.GetNewListID();
            await this.AddListMaster(list);
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var filmIDs = list.FilmIDs.Split(',');
                foreach (var item in filmIDs)
                {
                    var sqlCommand = @$"INSERT INTO filmlist (FilmID, ListID, CreatedDate, ModifiedDate)
                                        VALUES (@filmID, @listID, NOW(), NOW());";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("filmID", item);
                    parameters.Add("listID", list.ListID);
                    var result = await SqlConnection.ExecuteAsync(sqlCommand, parameters);

                }
                return true;
            }
        }

        public async Task<bool> EditListDetail(ListDTO list)
        {
            await this.EditListMaster(list);
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @$"DELETE FROM filmlist WHERE ListID = @listID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("listID", list.ListID);
                await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                var filmIDs = list.FilmIDs.Split(',');
                foreach (var item in filmIDs)
                {
                    sqlCommand = @$"INSERT INTO filmlist (FilmID, ListID, CreatedDate, ModifiedDate)
                                        VALUES (@filmID, @listID, NOW(), NOW());";

                    parameters.Add("filmID", item);
                    await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                }
                return true;
            }
        }

        public async Task<int> AddListMaster(ListDTO list)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @$"INSERT INTO list(ListID, UserID, ListName, Description, CreatedDate, ModifiedDate)
                                    VALUES(@listID, @userID, @listName, @description, NOW(), NOW());";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("userID", _userContext.UserId);
                parameters.Add("listName", list.ListName);
                parameters.Add("description", list.Description);
                parameters.Add("listID", list.ListID);
                var result = await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                return result;
            }
        }

        public async Task<int> EditListMaster(ListDTO list)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @$"UPDATE list l 
                                    SET ListName = @listName,
                                        Description = @description,
                                        ModifiedDate = NOW()
                                    WHERE ListID = @listID;";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("listName", list.ListName);
                parameters.Add("description", list.Description);
                parameters.Add("listID", list.ListID);
                var result = await SqlConnection.ExecuteAsync(sqlCommand, parameters);
                return result;
            }
        }

        public async Task<int> GetNewListID()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @$"SELECT COALESCE(MAX(ListID), 0) + 1 FROM list;";

                var result = await SqlConnection.QueryAsync<int>(sqlCommand);
                return result.FirstOrDefault();
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
                        orderBy = "ORDER BY l.LikesCount DESC";
                        break;
                    case "most recent":
                        orderBy = "ORDER BY l.CreatedDate DESC";
                        break;
                    case "earliest":
                        orderBy = "ORDER BY l.CreatedDate ASC";
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
                            where = "AND l.UserID = f2.FollowedUserID";
                            break;
                        case "you":
                            where = "AND l.UserID = @userID";
                            break;
                        default:
                            break;
                    }
                }
                var sqlCommand = @$"SELECT l.*, u.UserName,
                                    IF(l1.LikeID IS NOT NULL, True, False) AS Liked,
                                    COUNT(f.FilmID) FilmsCount,
                                    JSON_ARRAYAGG(
                                        JSON_OBJECT(
                                            'FilmID', f1.FilmID,
                                            'Poster_path', f1.poster_path,
                                            'Release_date', f1.release_date,
                                            'Title', f1.title
                                        )
                                    ) AS Film
                                    FROM list l
                                    {join}
                                    LEFT JOIN filmlist f ON l.ListID = f.ListID
                                    INNER JOIN film f1 ON f.FilmID = f1.FilmID
                                    LEFT JOIN user u ON u.UserID = l.UserID {where}
                                    LEFT JOIN `like` l1 ON l.ListID = l1.ParentID AND l1.UserID = @userID AND l1.Type = 'List'
                                    WHERE f.FilmID = @filmID
                                    GROUP BY l.ListID
                                    {orderBy}
                                    LIMIT @pageSize OFFSET @offset;

                                    SELECT COUNT(DISTINCT l.ListID) FROM list l
                                    {join}
                                    LEFT JOIN filmlist f ON l.ListID = f.ListID
                                    INNER JOIN film f1 ON f.FilmID = f1.FilmID
                                    LEFT JOIN user u ON u.UserID = l.UserID {where}
                                    LEFT JOIN `like` l1 ON l.ListID = l1.ParentID AND l1.UserID = @userID AND l1.Type = 'List'
                                    WHERE f.FilmID = @filmID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filter", parameter.filter);
                parameters.Add("@pageSize", parameter.pageSize);
                parameters.Add("@offset", offset);
                parameters.Add("@userID", _userContext.UserId);
                parameters.Add("@filmID", parameter.id);
                var result = await SqlConnection.QueryMultipleAsync(sqlCommand, parameters);
                //Trả dữ liệu về client
                var data = result.Read<object>().ToList();
                foreach (var item in data)
                {
                    var lists = (IDictionary<string, object>)item;
                    var list = (string)lists["Film"];
                    if (!string.IsNullOrEmpty(list))
                    {
                        var films = JsonConvert.DeserializeObject<List<BaseFilmDTO>>(list);
                        if (films != null)
                        {
                            lists["Film"] = films.Take(5);
                        }
                        else
                        {
                            lists["Film"] = new List<BaseFilmDTO>();
                        }
                    }
                }
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

        public List<ListPopularWeekDTO> PopularMonthList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT ParentID as ListID, COUNT(*) as LikeCounts FROM `like` " +
                    "WHERE createdDate >= DATE_SUB(NOW(), INTERVAL 1 MONTH) and `type` = 'List' " +
                    "GROUP BY ParentID ORDER BY LikeCounts DESC LIMIT 5;";
                DynamicParameters parameters = new DynamicParameters();
                var lists = SqlConnection.Query<ListPopularWeekDTO>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return lists.ToList();
            }
        }

        public List<ListPopularWeekDTO> PopularWeekList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT ParentID as ListID, COUNT(*) as LikeCounts FROM `like` " +
                    "WHERE createdDate >= DATE_SUB(NOW(), INTERVAL 1 WEEK) and `type` = 'List' " +
                    "GROUP BY ParentID ORDER BY LikeCounts DESC LIMIT 3;";
                DynamicParameters parameters = new DynamicParameters();
                var lists = SqlConnection.Query<ListPopularWeekDTO>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return lists.ToList();
            }
        }

        public List<ListRecentLikeDTO> RecentLikeList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT parentID as ListID, MAX(createdDate) as Date FROM `like` " +
                    "where `type` = 'List' " +
                    "GROUP BY parentID ORDER BY Date DESC LIMIT 6;";
                DynamicParameters parameters = new DynamicParameters();
                var lists = SqlConnection.Query<ListRecentLikeDTO>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return lists.ToList();
            }
        }

        public List<ListPopularWeekDTO> ListCrew()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT l.ListID as ListID, l.CommentsCount AS LikeCounts " +
                    "FROM list l " +
                    "ORDER BY LikeCounts DESC LIMIT 3";
                DynamicParameters parameters = new DynamicParameters();
                var lists = SqlConnection.Query<ListPopularWeekDTO>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return lists.ToList();
            }
        }

        public List<ListPopularWeekDTO> ListTopLike()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT ParentID as ListID, COUNT(*) as LikeCounts FROM `like` " +
                    "WHERE `type` = 'List' " +
                    "GROUP BY ParentID ORDER BY LikeCounts DESC LIMIT 2;";
                DynamicParameters parameters = new DynamicParameters();
                var lists = SqlConnection.Query<ListPopularWeekDTO>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return lists.ToList();
            }
        }

        public int UpdateCommentCount(int listID, int commentCount)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "Update List set CommentsCount = @v_CommentCount where ListID = @v_ListID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_CommentCount", commentCount);
                parameters.Add("v_ListID", listID);
                var res = SqlConnection.Execute(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }

        public int UpdateLikeCount(int listID, int likeCount)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "Update List set LikesCount = @v_LikeCount where ListID = @v_ListID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_LikeCount", likeCount);
                parameters.Add("v_ListID", listID);
                var res = SqlConnection.Execute(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }
    }
}
