using System.ComponentModel.DataAnnotations;

namespace WebFilm.Core.Enitites.User
{
    public class User_Admin
    {
        [Key]
        public Guid? UserID { get; set; }
        
        public string? UserName { get; set; }

        
        public string? FullName { get; set; }


       
        public string? Email { get; set; }

        
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Trạng thái người dùng
        /// 1: Chờ xác nhận, 2: Đang hoạt động
        /// </summary>
        public int? Status { get; set; } 

        /// <summary>
        /// Vai trò của người dùng
        /// </summary>
        public int? RoleType { get; set; }

        /// <summary>
        /// Danh sách film yêu thích người dùng
        /// </summary>
        public string? FavouriteFilmList { get; set; }

        public string? Avatar { get; set; }

        public string? Bio { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        //public DateTime? ModifiedDate { get; set; }
        public string? Banner { get; set; }

        
    }
}
