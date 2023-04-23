using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Block;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : BaseController<int, Block>
    {
        #region Field
        IBlockService _blockService;
        #endregion

        #region Contructor
        public BlockController(IBlockService blockService) : base(blockService)
        {
            _blockService = blockService;
        }
        #endregion

       
    }
}
