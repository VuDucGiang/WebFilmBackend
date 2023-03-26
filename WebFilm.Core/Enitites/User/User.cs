using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.User
{
    public class User
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
        /// Học và tên 
        /// </summary>
        public string? FullName { get; set; }


        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Học và tên 
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Trạng thái người dùng
        /// 1: Chờ xác nhận, 2: Đang hoạt động
        /// </summary>
        public int Status { get; set; } 

        /// <summary>
        /// Vai trò của người dùng
        /// </summary>
        public int RoleType { get; set; }

        /// <summary>
        /// Danh sách film yêu thích người dùng
        /// </summary>
        public string? FavouriteFilmList { get; set; }

        public string? Avatar { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        #endregion
    }
}
