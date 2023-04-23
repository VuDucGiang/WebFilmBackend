using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Block;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Infrastructure.Repository
{
    public class BlockRepository : BaseRepository<int, Block>, IBlockRepository
    {
        IUserContext _userContext;
        public BlockRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        

        
    }
}
