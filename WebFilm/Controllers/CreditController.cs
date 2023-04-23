using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Credit;
using WebFilm.Core.Interfaces.Services;

using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CreditController : BaseController<int, Credit>
    {
        #region Field
        ICreditService _creditService;
        #endregion

        #region Contructor
        public CreditController(ICreditService creditService) : base(creditService)
        {
            _creditService = creditService;
        }
        #endregion

       
    }
}
