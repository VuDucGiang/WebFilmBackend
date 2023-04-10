using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class JournalRepository : BaseRepository<int, Journal>, IJournalRepository
    {
        public JournalRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Journal GetLastestJournal()
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM Journal order by CreatedDate desc LIMIT 1";
                var journal = SqlConnection.QueryFirstOrDefault<Journal>(sqlCommand);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return journal;
            }
        }
    }
}
