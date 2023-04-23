namespace WebFilm.Core.Interfaces.Repository
{
    public interface IBaseRepository<TKey, TEntity>
    {
        /// <summary>
        /// Lấy danh sách của tất cả đối tượng TEntity 
        /// </summary>
        /// <returns>IEnumerable<Entity></returns>
        /// Author: Vũ Đức Giang
        IEnumerable<TEntity> GetAll();

        TEntity GetByID(TKey id);

        int Edit(TKey id, TEntity entity);

        int Add(TEntity entity);

        int Delete(TKey id);
    }
}
