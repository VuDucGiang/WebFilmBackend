using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Core.Services
{
    public class BaseService<TEntity>
    {
        IBaseRepository<TEntity> _baseRepository;
        public BaseService(IBaseRepository<TEntity> baseRepository)
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

    }
}
