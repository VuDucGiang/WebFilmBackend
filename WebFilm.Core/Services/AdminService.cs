using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Answer;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Enitites.Similar_film;

using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.Credit;


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
        public User GetUserByID(Guid id)
        {
            return _adminRepository.GetUserByID(id);
        }

        public Film GetFilmByID(int id)
        {
            return _adminRepository.GetFilmByID(id);
        }

        public Journal GetJournalByID(int id)
        {
            return _adminRepository.GetJournalByID(id);
        }

        public async Task<object> GetPagingQuestion(PagingParameterQuestion_Admin parameter)
        {
            return await _adminRepository.GetPagingQuestion(parameter);
        }

        public int UpdateQuestion(int id, Question_Admin entity)
        {
            return _adminRepository.UpdateQuestion(id, entity);
        }
        public int AddQuestion(Question_Admin entity)
        {
            return _adminRepository.AddQuestion(entity);
        }
        public int DeleteQuestion(int id)
        {
            return _adminRepository.DeleteQuestion(id);
        }

        public Question GetQuestionByID(int id)
        {
            return _adminRepository.GetQuestionByID(id);
        }
        public async Task<object> GetPagingAnswer(PagingParameterAnswer_Admin parameter)
        {
            return await _adminRepository.GetPagingAnswer(parameter);
        }

        public int UpdateAnswer(int id, Answer_Admin entity)
        {
            return _adminRepository.UpdateAnswer(id, entity);
        }
        public int AddAnswer(Answer_Admin entity)
        {
            return _adminRepository.AddAnswer(entity);
        }
        public int DeleteAnswer(int id)
        {
            return _adminRepository.DeleteAnswer(id);
        }

        public Answer GetAnswerByID(int id)
        {
            return _adminRepository.GetAnswerByID(id);
        }

        public async Task<object> GetPagingRelated_film(PagingParameterRelated_film_Admin parameter)
        {
            return await _adminRepository.GetPagingRelated_film(parameter);
        }

        public int UpdateRelated_film(int id, Related_film_Admin entity)
        {
            return _adminRepository.UpdateRelated_film(id, entity);
        }
        public int AddRelated_film(Related_film_Admin entity)
        {
            return _adminRepository.AddRelated_film(entity);
        }
        public int DeleteRelated_film(int id)
        {
            return _adminRepository.DeleteRelated_film(id);
        }

        public Related_film GetRelated_filmByID(int id)
        {
            return _adminRepository.GetRelated_filmByID(id);
        }

        public async Task<object> GetPagingSimilar_film(PagingParameterSimilar_film_Admin parameter)
        {
            return await _adminRepository.GetPagingSimilar_film(parameter);
        }

        public int UpdateSimilar_film(int id, Similar_film_Admin entity)
        {
            return _adminRepository.UpdateSimilar_film(id, entity);
        }
        public int AddSimilar_film(Similar_film_Admin entity)
        {
            return _adminRepository.AddSimilar_film(entity);
        }
        public int DeleteSimilar_film(int id)
        {
            return _adminRepository.DeleteSimilar_film(id);
        }

        public Similar_film GetSimilar_filmByID(int id)
        {
            return _adminRepository.GetSimilar_filmByID(id);
        }
        public async Task<object> GetPagingCredit(PagingParameterCredit_Admin parameter)
        {
            return await _adminRepository.GetPagingCredit(parameter);
        }

        public int UpdateCredit(string id, Credit_Admin entity)
        {
            return _adminRepository.UpdateCredit(id, entity);
        }
        public int AddCredit(Credit_Admin entity)
        {
            return _adminRepository.AddCredit(entity);
        }
        public int DeleteCredit(string id)
        {
            return _adminRepository.DeleteCredit(id);
        }

        public Credit GetCreditByID(string id)
        {
            return _adminRepository.GetCreditByID(id);
        }
    }
}
