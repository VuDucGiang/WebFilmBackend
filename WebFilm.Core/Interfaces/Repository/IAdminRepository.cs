using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Enitites.Answer;
using WebFilm.Core.Enitites.Related_film;
using WebFilm.Core.Enitites.Similar_film;
using WebFilm.Core.Enitites.Credit;


namespace WebFilm.Core.Interfaces.Repository
{
    public interface IAdminRepository : IBaseRepository<int, Admin>
    {
        Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter);
        int UpdateFilm(int id, Film_Admin entity);
        int AddFilm(Film_Admin entity);
        int DeleteFilm(int id);

        Task<object> GetPagingUser(PagingParameterUser_Admin parameter);
        int UpdateUser(Guid id, User_Admin entity);
        int DeleteUser(Guid id);
        Task<object> GetPagingJournal(PagingParameterJournal_Admin parameter);
        int UpdateJournal(int id, Journal_Admin entity);
        int AddJournal(Journal_Admin entity);
        int DeleteJournal(int id);
        User GetUserByID(Guid id);
        Film GetFilmByID(int id);
        Journal GetJournalByID(int id);

        Question GetQuestionByID(int id);
        Task<object> GetPagingQuestion(PagingParameterQuestion_Admin parameter);
        int UpdateQuestion(int id, Question_Admin entity);
        int AddQuestion(Question_Admin entity);
        int DeleteQuestion(int id);
        Answer GetAnswerByID(int id);
        Task<object> GetPagingAnswer(PagingParameterAnswer_Admin parameter);
        int UpdateAnswer(int id, Answer_Admin entity);
        int AddAnswer(Answer_Admin entity);
        int DeleteAnswer(int id);
        Related_film GetRelated_filmByID(int id);
        Task<object> GetPagingRelated_film(PagingParameterRelated_film_Admin parameter);
        int UpdateRelated_film(int id, Related_film_Admin entity);
        int AddRelated_film(Related_film_Admin entity);
        int DeleteRelated_film(int id);

        Similar_film GetSimilar_filmByID(int id);
        Task<object> GetPagingSimilar_film(PagingParameterSimilar_film_Admin parameter);
        int UpdateSimilar_film(int id, Similar_film_Admin entity);
        int AddSimilar_film(Similar_film_Admin entity);
        int DeleteSimilar_film(int id);
        Credit GetCreditByID(string id);
        Task<object> GetPagingCredit(PagingParameterCredit_Admin parameter);
        int UpdateCredit(string id, Credit_Admin entity);
        int AddCredit(Credit_Admin entity);
        int DeleteCredit(string id);
    }
}