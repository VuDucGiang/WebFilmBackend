namespace WebFilm.Core.Interfaces.Repository
{
    public interface IBaseRepository<TEntity>
    {
        /// <summary>
        /// Lấy danh sách của tất cả đối tượng TEntity 
        /// </summary>
        /// <returns>IEnumerable<MISAEntity></returns>
        /// Author: Vũ Đức Giang
        IEnumerable<TEntity> GetAll();
    }
}
