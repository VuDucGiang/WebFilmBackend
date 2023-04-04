using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Enitites
{
    public class PagingParameter
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;
        public string filter { get; set; } = "";
        public string sort { get; set; } = "";

    }
}
