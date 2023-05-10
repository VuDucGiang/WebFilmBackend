using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class PagingParameterJournal_Admin 
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;
        public string? sort { get; set; }
        public string? sortBy { get; set; }
        public string? category { get; set; }
        public string? authorUserName { get; set; }
        public int? mentionedFilm { get; set; }
        public string? title { get; set; }
        public string? intro { get; set; }
    }
}
