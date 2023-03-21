using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class FollowService : BaseService<int, Follow>, IFollowService
    {
        IFollowRepository _followRepository;
        private readonly IConfiguration _configuration;

        public FollowService(IFollowRepository followRepository, IConfiguration configuration) : base(followRepository)
        {
            _followRepository = followRepository;
            _configuration = configuration;
        }
    }
}
