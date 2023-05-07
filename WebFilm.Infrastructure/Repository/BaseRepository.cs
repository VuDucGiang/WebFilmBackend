using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Org.BouncyCastle.Math.Field;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Infrastructure.Repository
{
    public abstract class BaseRepository<TKey, TEntity>
    {
        #region Field
        protected MySqlConnection SqlConnection;
        protected readonly IConfiguration _configuration;
        protected string _connectionString;
        protected string className = typeof(TEntity).Name;
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
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `{className}`";
                //Trả dữ liệu về client
                var entities = SqlConnection.Query<TEntity>(sqlCommand);
                SqlConnection.Close();
                return entities;
            }
        }

        public virtual TEntity GetByID(TKey id)
        {
            var keyName = GetKeyName<TEntity>();
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                //Thực thi lấy dữ liệu
                var sqlCommand = $"SELECT * FROM `{className}` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                //Trả dữ liệu về client
                var entities = SqlConnection.QueryFirstOrDefault<TEntity>(sqlCommand, parameters);
                SqlConnection.Close();
                return entities;
            }
        }

        public int Edit(TKey id, TEntity entity)
        {
            var keyName = GetKeyName<TEntity>();
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                StringBuilder sql = new StringBuilder($"UPDATE `{className}` SET ");

                PropertyInfo[] properties = typeof(TEntity).GetProperties();

                DynamicParameters parameters = new DynamicParameters();

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != keyName && property.Name != "CreatedDate")
                    {
                        if(property.Name == "ModifiedDate")
                        {
                            sql.Append($"{property.Name} = @{property.Name}, ");
                            parameters.Add(property.Name, DateTime.Now);

                        }
                        else
                        {
                            sql.Append($"{property.Name} = @{property.Name}, ");
                            parameters.Add(property.Name, property.GetValue(entity));

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

        public int Add(TEntity entity)
        {
            var keyName = GetKeyName<TEntity>();
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                var properties = typeof(TEntity).GetProperties();

                foreach (var property in properties)
                {
                    if (property.Name != keyName)
                    {
                        if (property.Name == "ModifiedDate" || property.Name == "CreatedDate") {
                            parameters.Add("@" + property.Name, DateTime.Now);
                        } else
                        {
                            parameters.Add("@" + property.Name, property.GetValue(entity));
                        }
                    }
                }

                var columns = string.Join(", ", properties.Where(p => p.Name != keyName).Select(p => p.Name));
                var values = string.Join(", ", properties.Where(p => p.Name != keyName).Select(p => "@" + p.Name));
                var query = $"INSERT INTO `{className}` ({columns}) VALUES ({values})";

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public int Delete(TKey id)
        {
            var keyName = GetKeyName<TEntity>();
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                string query = $"DELETE FROM `{className}` WHERE {keyName} = @id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);

                //Trả dữ liệu về client
                var res = SqlConnection.Execute(query, parameters);
                SqlConnection.Close();
                return res;
            }
        }

        public static string GetKeyName<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                KeyAttribute keyAttr = property.GetCustomAttribute<KeyAttribute>();
                if (keyAttr != null)
                {
                    return property.Name;
                }
            }
            return null;
        }
        #endregion
    }
}
