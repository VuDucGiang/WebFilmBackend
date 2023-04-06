using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IFilmService : IBaseService<int, Film>
    {
        public Task<object> GetPaging(PagingParameterFilm parameter);
    }
}
