using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class AdminService : BaseService<int, Admin>, IAdminService
    {
        IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;

        public AdminService(IAdminRepository adminRepository, IConfiguration configuration) : base(adminRepository)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
        }
        public async Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter)
        {
            return await _adminRepository.GetPagingFilm(parameter);
        }

        public int UpdateFilm(int id, Film_Admin entity)
        {
            return  _adminRepository.UpdateFilm(id, entity);
        }
        public int AddFilm(Film_Admin entity)
        {
            return _adminRepository.AddFilm(entity);
        }
        public int DeleteFilm(int id)
        {
            return _adminRepository.DeleteFilm(id);
        }

    }
}
