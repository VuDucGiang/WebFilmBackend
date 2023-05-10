using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User;
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
        public async Task<object> GetPagingUser(PagingParameterUser_Admin parameter)
        {
            return await _adminRepository.GetPagingUser(parameter);
        }

        public int UpdateUser(Guid id, User_Admin entity)
        {
            return _adminRepository.UpdateUser(id, entity);
        }

        public int DeleteUser(Guid id)
        {
            return _adminRepository.DeleteUser(id);
        }

        public async Task<object> GetPagingJournal(PagingParameterJournal_Admin parameter)
        {
            return await _adminRepository.GetPagingJournal(parameter);
        }

        public int UpdateJournal(int id, Journal_Admin entity)
        {
            return _adminRepository.UpdateJournal(id, entity);
        }
        public int AddJournal(Journal_Admin entity)
        {
            return _adminRepository.AddJournal(entity);
        }
        public int DeleteJournal(int id)
        {
            return _adminRepository.DeleteJournal(id);
        }
    }
}
