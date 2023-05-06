using WebFilm.Core.Enitites.WatchList;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IWatchListService : IBaseService<int, WatchList>
    {
        Task<bool> AddWatchList(int filmID);
        Task<bool> DeleteWatchList(ParamDelete param);
    }
}
