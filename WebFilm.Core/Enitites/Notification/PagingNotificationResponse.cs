using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;

namespace WebFilm.Core.Enitites.Notification
{
    public class PagingNotificationResponse
    {
        public List<Notification> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
        public int TotalUnseen { get; set; }
    }
}
