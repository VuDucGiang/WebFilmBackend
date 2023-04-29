using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.Like;

namespace WebFilm.Core.Interfaces.Services
{
    public interface ILikeService : IBaseService<int, Like>
    {
        bool newLike(int id, string type);
    }
}
