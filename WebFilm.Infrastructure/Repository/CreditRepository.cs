using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Credit;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Infrastructure.Repository
{
    public class CreditRepository : BaseRepository<int, Credit>, ICreditRepository
    {
        IUserContext _userContext;
        public CreditRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        

        
    }
}
