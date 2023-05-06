using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IJournalService : IBaseService<int, Journal>
    {

        List<Journal> GetListNewJournal();
        List<Journal> GetReviewJournalsList();
        List<Journal> GetNewsJournalsList();
        object GetPaging(int pageSize, int pageIndex);

    }
}
