using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Enitites.Comment
{
    public class BaseCommentDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Avatar { get; set; }
        public int CommentID { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
