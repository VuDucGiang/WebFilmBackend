using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Block
{
    public class Block : BaseEntity
    {
        [Key]
        public int BlockID { get; set; }
        public Guid UserID { get; set; }
        public Guid BlockedUserID { get; set; }
    }
}
