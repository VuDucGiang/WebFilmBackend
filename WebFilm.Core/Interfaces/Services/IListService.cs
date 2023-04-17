using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.List;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IListService : IBaseService<int, List>
    {
        List<ListPopularDTO> GetListPopular();

        List<ListPopularDTO> GetListPopularWeek();

        List<ListPopularDTO> GetListRecentLikes();

        List<ListPopularDTO> GetListPopularMonth();

        List<ListPopularDTO> GetCrewList();

        List<ListPopularDTO> ListTop();
    }
}
