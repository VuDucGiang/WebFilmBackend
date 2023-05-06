using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Exceptions;
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

        public async Task<object> GetListUserLiked(int pageSize, int pageIndex, int filmID)
        {
            return await _filmRepository.GetListUserLiked(pageSize, pageIndex, filmID);
        }
        public async Task<bool> AddFilmToList(int filmID, string listIDs)
        {
            if (string.IsNullOrEmpty(listIDs))
            {
                throw new ServiceException("ListIDs is required");
            }
            var check = await _filmRepository.CheckPermissionInList(listIDs);
            if (!check)
            {
                throw new ServiceException("No permission");
            }
            var msg = await _filmRepository.CheckDuplicateFilmInList(filmID, listIDs);
            if (!string.IsNullOrEmpty(msg))
            {
                throw new ServiceException("Lists " + msg.TrimEnd(',', ' ') + " already have this film.");
            }
            return await _filmRepository.AddFilmToList(filmID, listIDs);
        }
        public async Task<object> GetPaging(PagingParameterFilm parameter)
        {
            return await _filmRepository.GetPaging(parameter);
        }

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            return await _filmRepository.GetPopular(pageSize, pageIndex, filter, sort);
        }

        public async Task<FilmDto> GetDetailByID(int id)
        {
            return await _filmRepository.GetDetailByID(id);
        }

        public async Task<object> GetInfoUser(int id)
        {
            return await _filmRepository.GetInfoUser(id);
        }

        public async Task<object> JustReviewed()
        {
            return await _filmRepository.JustReviewed();
        }

        public async Task<object> Related(int id, PagingParameter parameter)
        {
            return await _filmRepository.Related(id, parameter);
        }
        public async Task<object> Similar(int id, PagingParameter parameter)
        {
            return await _filmRepository.Similar(id, parameter);
        }
    }
}
