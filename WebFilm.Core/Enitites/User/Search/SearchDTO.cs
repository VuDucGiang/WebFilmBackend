using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review.dto;

namespace WebFilm.Core.Enitites.User.Search
{
    public class SearchDTO
    {
        public List<FilmSearchDTO> films { get; set; }
        public List<BaseReviewDTO> reviews { get; set; }

        public List<ListPopularDTO> lists { get; set; }
        public List<UserSearchDTO> members { get; set; }
    }
}
