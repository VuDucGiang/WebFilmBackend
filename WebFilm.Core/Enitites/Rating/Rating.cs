using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Rating
{
    public class Rating : BaseEntity
    {
        [Key]
        public int RatingID { get; set; }

        public Guid UserID { get; set;}

        public float Score { get; set; }

        public int FilmID { get; set; }
    }
}
