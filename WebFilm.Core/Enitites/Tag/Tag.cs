using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Tag
{
    public class Tag : BaseEntity
    {
        [Key]
        public int TagID { get; set; }
        public Guid UserID { get; set; }
        public int ParentID { get; set; }
        public string Type { get; set; }
        public string TagText{ get; set; }
    }
}
