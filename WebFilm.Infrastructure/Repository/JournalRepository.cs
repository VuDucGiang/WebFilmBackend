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

                var sqlCommand = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM journal order by CreatedDate desc LIMIT 7";
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

                var sqlCommand = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM journal WHERE Category = 'Review' order by CreatedDate desc LIMIT 3";
                var journal = SqlConnection.Query<JournalLite>(sqlCommand);

                
                SqlConnection.Close();
                return journal.ToList();
               
            }
        }

        public List<JournalLite> GetNewsJournalsList()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {

                var sqlCommand = "SELECT Author,Banner,Category,CreatedDate,Intro,JournalID,MentionedFilm,ModifiedDate,Title FROM journal WHERE Category = 'News' order by CreatedDate desc LIMIT 3";
                var journal = SqlConnection.Query<JournalLite>(sqlCommand);


                SqlConnection.Close();
                return journal.ToList();

            }
        }

        public object GetPaging(PagingJournal parameter)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                return new
                {
                    Data = 0,
                };
            }
        }
    }
}
