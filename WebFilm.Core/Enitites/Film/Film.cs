using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class Film
    {
        [Key]
        public int FilmID { get; set; }
        public bool Adult { get; set; }
        public string? Backdrop_path { get; set; }
        public string? Trailer { get; set; }
        public int Belongs_to_collection { get; set;}
        public string? Budget { get; set; }
        public string? Genres { get; set; }
        public string? Homepage { get; set;}
        public string? Imdb_id { get; set; }
        public string? Original_language { get; set; }
        public string? Original_title { get; set; }
        public string? Overview { get; set; }
        public float Popularity { get; set; }
        public string? Poster_path { get; set; }
        public string? Production_companies { get; set; }
        public string? Production_countries { get; set; }
        public DateTime Release_date { get; set; }
        public string? Revenue { get; set; }
        public int Runtime { get; set; }
        public string? Spoken_languages { get; set; }
        public string? Status { get; set; }
        public string? Tagline { get; set; }
        public string? Title { get; set; }
        public float Vote_average { get; set; }
        public int Vote_count { get; set; }
        public int LikesCount { get; set; }
        public int ReviewsCount { get; set; }
    }
}
