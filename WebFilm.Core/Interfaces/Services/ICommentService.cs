using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Film;

namespace WebFilm.Core.Interfaces.Services
{
    public interface ICommentService : IBaseService<int, Comment>
    {
        int CreateCommentInList(int ListID, CommentCreateDTO dto);

        int CreateCommentInReview(int ReviewID, CommentCreateDTO dto);
    }
}
