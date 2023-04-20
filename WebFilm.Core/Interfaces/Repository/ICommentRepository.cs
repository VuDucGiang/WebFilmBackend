using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.FilmList;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface ICommentRepository : IBaseRepository<int, Comment>
    {
    }
}
