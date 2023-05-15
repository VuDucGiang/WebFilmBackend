using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class PagingParameterAnswer_Admin 
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;
        public string? sort { get; set; }
        public string? sortBy { get; set; }
        public string? answer { get; set; }
        public int? QuestionID { get; set; }
        public int? RightAnswer { get; set; }

    }
}
