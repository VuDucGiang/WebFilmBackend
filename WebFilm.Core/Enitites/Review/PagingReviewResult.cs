using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Enitites.Review
{
    public class PagingReviewResult
    {
        public List<BaseReviewDTO> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
    }
}
