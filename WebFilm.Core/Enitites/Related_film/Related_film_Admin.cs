using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Related_film
{
    public class Related_film_Admin 
    {
        [Key]
        public int? Related_filmID { get; set; }
        public int? DetailFilmID { get; set; }
        public int? FilmID { get; set; }
        public string? Original_title { get; set; }
        public string? Overview { get; set; }
        public string? Poster_path { get; set; }
        public string? Title { get; set; }


    }
}
