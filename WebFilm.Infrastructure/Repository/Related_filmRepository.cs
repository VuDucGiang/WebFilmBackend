using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Infrastructure.Repository
{
    public class Related_filmRepository : BaseRepository<int, Related_film>, IRelated_filmRepository
    {
        IUserContext _userContext;
        public Related_filmRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        

        
    }
}
