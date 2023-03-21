using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class FollowRepository : BaseRepository<int, Follow>, IFollowRepository
    {
        public FollowRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
