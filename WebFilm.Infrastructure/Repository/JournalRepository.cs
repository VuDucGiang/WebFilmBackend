using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using static Dapper.SqlMapper;
using Newtonsoft.Json;
using WebFilm.Core.Enitites.User.Search;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Credit;

namespace WebFilm.Infrastructure.Repository
{
    public class JournalRepository : BaseRepository<int, Journal>, IJournalRepository
    {
        
        public JournalRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<JournalLite> GetListNewJournal()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {

                var sqlCommand = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM Journal order by CreatedDate desc LIMIT 7";
                var journal = SqlConnection.Query<JournalLite>(sqlCommand);


                SqlConnection.Close();
                return journal.ToList();

            }
        }
        public List<MentionedInArticle> GetMentionedInArticle(int filmID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT j.JournalID, j.Title, j.Banner FROM Journal j WHERE j.MentionedFilm = @filmID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@filmID", filmID);
                var journal = SqlConnection.Query<MentionedInArticle>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return journal.ToList();
            }
        }
        public List<JournalLite> GetReviewJournalsList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {

                var sqlCommand = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM Journal WHERE Category = 'Review' order by CreatedDate desc LIMIT 3";
                var journal = SqlConnection.Query<JournalLite>(sqlCommand);

                
                SqlConnection.Close();
                return journal.ToList();
               
            }
        }

        public List<JournalLite> GetNewsJournalsList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {

                var sqlCommand = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM Journal WHERE Category = 'News' order by CreatedDate desc LIMIT 3";
                var journal = SqlConnection.Query<JournalLite>(sqlCommand);


                SqlConnection.Close();
                return journal.ToList();

            }
        }

        public object GetPaging(int pageSize, int pageIndex)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                int offset = (pageIndex - 1) * pageSize;
                var sql = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM Journal order by CreatedDate desc LIMIT @pageSize OFFSET @offset;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@offset", offset);
                var result = SqlConnection.Query(sql, parameters);
                var sql2 = "SELECT * FROM `Journal`;";
                var result2 = SqlConnection.Query(sql2);
                var total = result2.ToList().Count;
                int totalPage = (int)Math.Ceiling((double)total / pageSize);
                var a = new {Data = 5 };
                return new
                {
                    Data = result.ToList(),
                    Total = total,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalPage = totalPage,
                };
            }
        }
        public List<JournalLite> GetRelatedArticles(int JournalID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM Journal WHERE JournalID = @Jid;";
                
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Jid", JournalID);
                var journal = SqlConnection.QueryFirstOrDefault<Journal>(sqlCommand,parameters);
                
                var category = journal.Category;
                DynamicParameters parameters2 = new DynamicParameters();
                parameters2.Add("@category", category);
                parameters2.Add("@Jid", JournalID);
                var sqlCommand2 = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM Journal WHERE category = @category and JournalID != @Jid order by CreatedDate desc LIMIT 3;";
                var relatedArticles = SqlConnection.Query<JournalLite>(sqlCommand2, parameters2);
                SqlConnection.Close();
                return relatedArticles.ToList();
                
            }
        }
        public FilmSearchDTO GetMentionedFilm(int JournalID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                FilmSearchDTO filmDTO = new FilmSearchDTO();
                var sqlCommand = "SELECT * FROM Journal WHERE JournalID = @Jid;";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Jid", JournalID);
                var journal = SqlConnection.QueryFirstOrDefault<Journal>(sqlCommand, parameters);

                var FilmID = journal.MentionedFilm;
                DynamicParameters parameters2 = new DynamicParameters();
                parameters2.Add("@FilmID", FilmID);
                var sqlCommand2 = "SELECT * FROM Film WHERE FilmID = @FilmID;";
                var film = SqlConnection.QueryFirstOrDefault<Film>(sqlCommand2, parameters2);
                filmDTO.FilmID = film.FilmID;
                filmDTO.Title = film.Title;
                filmDTO.Poster_Path = film.Poster_path;
                filmDTO.ReleaseDate = film.Release_date;
                var sqlCommand3 = "SELECT * FROM Credit WHERE FilmID = @FilmID;";
                DynamicParameters parameters3 = new DynamicParameters();
                parameters3.Add("@FilmID", FilmID);
                var credit = SqlConnection.Query<Credit>(sqlCommand3, parameters3);
                List<Credit> credits = credit.ToList();
                filmDTO.Cast = string.Join(", ", credits.Where(c => "Acting".Equals(c.Known_for_department)).GroupBy(c => c.PersonID).Select(c => c.First().Name));
                filmDTO.Director = string.Join(", ", credits.Where(c => "Director".Equals(c.Job)).GroupBy(c => c.PersonID).Select(c => c.First().Name));
                SqlConnection.Close();
                return filmDTO;

            }
        }
    }
}
