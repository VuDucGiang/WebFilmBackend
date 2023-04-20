using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Enitites.List
{
    public class ListPopularDTO
    {
        public int ListID { get; set; }
        public int Total { get; set; }
        public string? Description { get; set; }
        public string? ListName { get; set; }
        public int TotalComment { get; set; }
        public int TotalLike { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<BaseFilmDTO> List { get; set; }
        public UserReviewDTO User { get; set; }
    }
}
