using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace WebFilm.Infrastructure.Repository
{
    public class BaseRepository<TEntity>
    {
        #region Field
        protected MySqlConnection SqlConnection;
        protected readonly IConfiguration _configuration;
        protected string _connectionString;
        #endregion

        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MyConnectionString");
        }

        #region Method
        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <typeparam name="TEntity">type of obj</typeparam>
        /// <returns>Danh sách obj</returns>
        /// Author: Vũ Đức Giang
        public IEnumerable<TEntity> GetAll()
        {
            var className = typeof(TEntity).Name;
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM {className}";
                //Trả dữ liệu về client
                var entities = SqlConnection.Query<TEntity>(sqlCommand);
                SqlConnection.Close();
                return entities;
            }
        }
        #endregion
    }
}
