using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class ListRepository : BaseRepository<int, List>, IListRepository
    {
        public ListRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<ListPopularWeekDTO> PopularMonthList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT ParentID as ListID, COUNT(*) as LikeCounts FROM `like` " +
                    "WHERE date >= DATE_SUB(NOW(), INTERVAL 1 MONTH) and `type` = 'List' " +
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
                    "WHERE date >= DATE_SUB(NOW(), INTERVAL 1 WEEK) and `type` = 'List' " +
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
                var sqlCommand = "SELECT parentID as ListID, MAX(date) as Date FROM `like` " +
                    "where `type` = 'List' " +
                    "GROUP BY parentID ORDER BY Date DESC LIMIT 10;";
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
                var sqlCommand = "SELECT l.ListID as ListID, COUNT(lf.FilmID) AS LikeCounts " +
                    "FROM list l " +
                    "join filmlist lf on l.ListID  = lf.ListID " +
                    "GROUP BY lf.ListID " +
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
    }
}
