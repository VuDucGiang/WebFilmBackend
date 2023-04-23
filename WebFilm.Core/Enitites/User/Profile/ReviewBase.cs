using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class ReviewBase
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string ReleaseYear { get; set; }

        public float Rating { get; set; }

        public string RatingCreatedAt { get; set; }
        
        public int LikeCount { get; set; }
    }
}
