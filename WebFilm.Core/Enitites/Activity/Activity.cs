using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Activity
{
    public class Activity : BaseEntity
    {
        public int ActivityID { get; set; }
        public Guid UserID { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }
}
