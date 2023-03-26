using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.Follow
{
    public class Follow : BaseEntity
    {
        [Key]
        public int FollowID { get; set; }
        public Guid UserID { get; set; }
        public Guid FollowedUserID { get; set; }
    }
}
