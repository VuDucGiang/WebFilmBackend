using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace WebFilm.Core.Enitites.Journal
{
    public class Journal : BaseEntity
    {
        [Key]
        public int JournalID { get; set; }
        public string Author { get; set; }
        public string Banner { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Intro { get; set; }
        
        public int MentionedFilm { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; set; }
        

        
        
    }
}
