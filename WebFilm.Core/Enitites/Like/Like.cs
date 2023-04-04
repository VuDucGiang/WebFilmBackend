using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Like
{
    public class Like : BaseEntity
    {
        [Key]
        public int LikeID { get; set; }

        public Guid UserID { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public int ParentID { get; set; }
    }
}
