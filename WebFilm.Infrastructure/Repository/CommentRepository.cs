using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class CommentRepository : BaseRepository<int, Comment>, ICommentRepository
    {
        public CommentRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
