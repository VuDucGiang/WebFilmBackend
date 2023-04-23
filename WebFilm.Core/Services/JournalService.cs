using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.Journal;
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

        public Journal GetLastestJournal()
        {
            return _journalRepository.GetLastestJournal();
        }

        public List<Journal> GetListNewJournal()
        {
            return _journalRepository.GetAll().OrderByDescending(p => p.CreatedDate).Take(3).ToList();
        }
    }
}
