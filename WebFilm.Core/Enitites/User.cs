using WebFilm.Core.Enitites;

namespace WebFilm.Core.Enitites
{
    public class User : BaseEntity
    {
        #region Prop
        /// <summary>
        /// Id user
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Học và tên 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Có phải Admin không
        /// </summary>
        public bool IsAdmin { get; set; }


        #endregion
    }
}
