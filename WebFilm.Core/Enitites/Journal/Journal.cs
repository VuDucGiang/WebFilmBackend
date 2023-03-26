using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Journal
{
    public class Journal : BaseEntity
    {
        [Key]
        public int JournalID { get; set; }
        public Guid UserID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public string Related { get; set; }

    }
}
