using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IFilmService : IBaseService<int, Film>
    {
        Task<object> GetListUserLiked(int pageSize, int pageIndex, int filmID);
        Task<FilmDto> GetDetailByID(int id);
        public Task<object> GetPaging(PagingParameterFilm parameter);
        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);
        Task<object> JustReviewed();
        Task<object> Related(int id, PagingParameter parameter);
        Task<object> Similar(int id, PagingParameter parameter);
    }
}
