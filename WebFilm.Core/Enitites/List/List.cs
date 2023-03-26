using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.List
{
    public class List : BaseEntity
    {
        [Key]
        public int ListID { get; set; }
        public Guid UserID { get; set; }
        public string ListName { get; set; }
        public string Description { get; set; }
        public int LikesCount { get; set; }
        public int Private { get; set; }

    }
}
