﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Enitites.Film
{
    public class FilmDto : Film
    {
        public string Credits { get; set; }
        public int Appears { get; set; }
        public RateStat RateStats { get; set; }
        public List<MentionedInArticle> MentionedInArticles { get; set; }
    }
}
