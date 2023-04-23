using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Credit;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class CreditService : BaseService<int, Credit>, ICreditService
    {
        ICreditRepository _creditRepository;
        private readonly IConfiguration _configuration;

        public CreditService(ICreditRepository creditRepository, IConfiguration configuration) : base(creditRepository)
        {
            _creditRepository = creditRepository;
            _configuration = configuration;
        }

        
    }
}
