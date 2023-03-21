using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites
{
    public class PagingResult
    {
        public List<object> Data { get; set; }
        public int Total { get; set; }
    }
}
