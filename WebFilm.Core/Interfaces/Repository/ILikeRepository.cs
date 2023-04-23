using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface ILikeRepository : IBaseRepository<int, Like>
    {
        Like getlikeByUserIDAndTypeAndParentID(Guid userID, string type, int parentID);
    }
}
