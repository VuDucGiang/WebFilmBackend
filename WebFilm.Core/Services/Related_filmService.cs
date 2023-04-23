using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class Related_filmService : BaseService<int, Related_film>, IRelated_filmService
    {
        IRelated_filmRepository _related_filmRepository;
        private readonly IConfiguration _configuration;

        public Related_filmService(IRelated_filmRepository related_filmRepository, IConfiguration configuration) : base(related_filmRepository)
        {
            _related_filmRepository = related_filmRepository;
            _configuration = configuration;
        }

        
    }
}
