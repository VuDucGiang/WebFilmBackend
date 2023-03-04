using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.User
{
    public class User : BaseEntity
    {
        #region Prop
        [Key]
        /// <summary>
        /// Id user
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Học và tên 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Có phải Admin không
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Trạng thái người dùng
        /// 1: Chờ xác nhận, 2: Đang hoạt động
        /// </summary>
        public int Status { get; set; } 


        #endregion
    }
}
