using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class CommentService : BaseService<int, Comment>, ICommentService
    {
        ICommentRepository _commentRepository;
        IListRepository _listRepository;
        IUserRepository _userRepository;
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public CommentService(ICommentRepository commentRepository, IConfiguration configuration,
            IListRepository listRepository, IUserRepository userRepository, IUserContext userContext) : base(commentRepository)
        {
            _commentRepository = commentRepository;
            _configuration = configuration;
            _listRepository = listRepository;
            _userRepository = userRepository;
            _userContext = userContext;
        }

        public int CreateCommentInList(int ListID, CommentCreateDTO dto)
        {
            List list = _listRepository.GetByID(ListID);
            if (list == null)
            {
                throw new ServiceException("Không tìm thấy list phù hợp");
            }
            try
            {
                int commentCount = list.CommentsCount + 1;

                Comment newComment = new Comment();
                newComment.Content = dto.Content;
                newComment.ParentID = ListID;
                Guid? id = _userContext.UserId;
                newComment.UserID = (Guid)_userContext.UserId;
                newComment.CreatedDate = DateTime.Now;
                newComment.ModifiedDate = DateTime.Now;
                newComment.Type = "List";

                _listRepository.UpdateCommentCount(ListID, commentCount);
                return _commentRepository.Add(newComment);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message);
            }
            
        }
    }
}
