using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.WatchList;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IWatchListRepository : IBaseRepository<int, WatchList>
    {
        Task<bool> AddWatchList(int filmID);
        Task<bool> DeleteWatchList(ParamDelete param);
    }
}
