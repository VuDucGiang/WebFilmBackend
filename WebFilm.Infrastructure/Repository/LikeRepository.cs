using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class LikeRepository : BaseRepository<int, Like>, ILikeRepository
    {
        public LikeRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
