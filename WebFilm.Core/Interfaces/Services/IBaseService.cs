namespace WebFilm.Core.Interfaces.Services
{
    public interface IBaseService<TKey, TEntity>
    {
        /// <summary>
        /// Kiểm tra trước khi lấy tất cả Entity
        /// </summary>
        /// <returns></returns>
        /// Author: Vũ Đức Giang(6/9/2022)
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Lấy dưc liệu theo id
        /// </summary>
        /// <returns></returns>
        /// Author: Vũ Đức Giang(6/9/2022)
        TEntity GetByID(TKey id);

        int Edit(TKey id, TEntity entity);

        int Add(TEntity entity);

        int Delete(TKey id);
    }
}
