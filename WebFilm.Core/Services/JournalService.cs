using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User.Search;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class JournalService : BaseService<int, Journal>, IJournalService
    {
        IJournalRepository _journalRepository;
        private readonly IConfiguration _configuration;

        public JournalService(IJournalRepository journalRepository, IConfiguration configuration) : base(journalRepository)
        {
            _journalRepository = journalRepository;
            _configuration = configuration;
        }

        

        public List<JournalLite> GetListNewJournal()
        {
            return _journalRepository.GetListNewJournal();
        }
        
        public List<JournalLite> GetReviewJournalsList()
        {
            return  _journalRepository.GetReviewJournalsList();
        }
        public List<JournalLite> GetNewsJournalsList()
        {
            return _journalRepository.GetNewsJournalsList();
        }
        public object GetPaging(int pageSize, int pageIndex)
        {
            return _journalRepository.GetPaging(pageSize,pageIndex);
        }
        public List<JournalLite> GetRelatedArticles(int JournalID)
        {
            return _journalRepository.GetRelatedArticles(JournalID);
        }
        public FilmSearchDTO GetMentionedFilm(int JournalID)
        {
            return _journalRepository.GetMentionedFilm(JournalID);
        }
    }
}
