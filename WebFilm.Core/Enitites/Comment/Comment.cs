using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Comment
{
    public class Comment : BaseEntity
    {
        [Key]
        public int CommentID { get; set; }
        public Guid UserID { get; set; }
        public int ParentID { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
