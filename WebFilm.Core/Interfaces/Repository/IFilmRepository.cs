using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IFilmRepository : IBaseRepository<int, Film>
    {
        Task<object> GetListUserLiked(int pageSize, int pageIndex, int filmID);
        Task<bool> AddFilmToList(int filmID, string listIDs);
        Task<string> CheckDuplicateFilmInList(int filmID, string listIDs);
        Task<bool> CheckPermissionInList(string listIDs);
        Task<FilmDto> GetDetailByID(int id);
        Task<object> GetInfoUser(int id);
        public Task<object> GetPaging(PagingParameterFilm parameter);
        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);
        Task<object> JustReviewed();
        Task<object> Related(int id, PagingParameter parameter);
        Task<object> Similar(int id, PagingParameter parameter);

        int UpdateLikeCount(int filmID, int likeCount);
    }
}
