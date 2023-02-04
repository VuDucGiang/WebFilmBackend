namespace WebFilm.Core.Interfaces.Services
{
    public interface IBaseService<TEntity>
    {
        /// <summary>
        /// Kiểm tra trước khi lấy tất cả MISAEntity
        /// </summary>
        /// <returns></returns>
        /// Author: Vũ Đức Giang(6/9/2022)
        IEnumerable<TEntity> GetAll();
    }
}
