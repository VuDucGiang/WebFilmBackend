using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IJournalService : IBaseService<int, Journal>
    {
        Journal GetLastestJournal();

        List<Journal> GetListNewJournal();
    }
}
