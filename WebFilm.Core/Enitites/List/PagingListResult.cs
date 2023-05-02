using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.List
{
    public class PagingListResult
    {
        public List<ListPopularDTO> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
    }
}
