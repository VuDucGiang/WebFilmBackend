using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Enitites
{
    public class PagingJournal
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;

    }
}
