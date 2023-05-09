using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Admin
{
    public class Admin : BaseEntity
    {
        [Key]
        public int AdminID { get; set; }
        public Guid UserID { get; set; }
        public int ParentID { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
