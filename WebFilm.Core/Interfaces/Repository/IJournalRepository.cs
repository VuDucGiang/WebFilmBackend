using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User.Search;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IJournalRepository : IBaseRepository<int, Journal>
    {
        List<JournalLite> GetListNewJournal();
        List<MentionedInArticle> GetMentionedInArticle(int filmID);
        List<JournalLite> GetReviewJournalsList();
        List<JournalLite> GetNewsJournalsList();

        object GetPaging(int pageSize, int pageIndex);
        List<JournalLite> GetRelatedArticles(int JournalID);
        FilmSearchDTO GetMentionedFilm(int JournalID);

    }
}
