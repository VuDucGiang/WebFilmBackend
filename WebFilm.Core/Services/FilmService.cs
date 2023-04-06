using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class FilmService : BaseService<int, Film>, IFilmService
    {
        IFilmRepository _filmRepository;
        private readonly IConfiguration _configuration;

        public FilmService(IFilmRepository filmRepository, IConfiguration configuration) : base(filmRepository)
        {
            _filmRepository = filmRepository;
            _configuration = configuration;
        }

        public async Task<object> GetPaging(PagingParameterFilm parameter)
        {
            return await _filmRepository.GetPaging(parameter);
        }
    }
}
