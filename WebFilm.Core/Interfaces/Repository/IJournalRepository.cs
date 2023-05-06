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
        
        List<MentionedInArticle> GetMentionedInArticle(int filmID);
        List<Journal> GetReviewJournalsList();
        List<Journal> GetNewsJournalsList();

        object GetPaging(int pageSize, int pageIndex);

    }
}
