using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IJournalService : IBaseService<int, Journal>
    {

        List<JournalLite> GetListNewJournal();
        List<JournalLite> GetReviewJournalsList();
        List<JournalLite> GetNewsJournalsList();
        object GetPaging(int pageSize, int pageIndex);
        List<JournalLite> GetRelatedArticles(int JournalID);

    }
}
