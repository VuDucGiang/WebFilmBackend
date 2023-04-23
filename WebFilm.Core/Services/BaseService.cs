using System.Security.Principal;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Core.Services
{
    public class BaseService<TKey, TEntity>
    {
        IBaseRepository<TKey, TEntity> _baseRepository;
        public BaseService(IBaseRepository<TKey, TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Kiểm tra trước khi lấy tất cả TEntity
        /// </summary>
        /// <returns></returns>
        /// Author: Vũ Đức Giang
        public IEnumerable<TEntity> GetAll()
        {
            var entity = _baseRepository.GetAll();
            return entity;
        }

        /// <summary>
        /// Lấy dưc liệu TEntity theo id
        /// </summary>
        /// <returns></returns>
        /// Author: Vũ Đức Giang
        public TEntity GetByID(TKey id)
        {
            var entity = _baseRepository.GetByID(id);
            return entity;
        }

        public int Edit(TKey id, TEntity entity)
        {
            return _baseRepository.Edit(id, entity);
        }

        public int Add(TEntity entity)
        {
            return _baseRepository.Add(entity);
        }

        public int Delete(TKey id)
        {
            return _baseRepository.Delete(id);
        }

    }
}
