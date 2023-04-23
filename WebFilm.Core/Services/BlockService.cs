using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Block;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class BlockService : BaseService<int, Block>, IBlockService
    {
        IBlockRepository _blockRepository;
        private readonly IConfiguration _configuration;

        public BlockService(IBlockRepository blockRepository, IConfiguration configuration) : base(blockRepository)
        {
            _blockRepository = blockRepository;
            _configuration = configuration;
        }

        
    }
}
