using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Journal;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IJournalRepository : IBaseRepository<int, Journal>
    {
        List<JournalLite> GetListNewJournal();
        List<MentionedInArticle> GetMentionedInArticle(int filmID);
        List<JournalLite> GetReviewJournalsList();
        List<JournalLite> GetNewsJournalsList();

        object GetPaging(int pageSize, int pageIndex);

    }
}
