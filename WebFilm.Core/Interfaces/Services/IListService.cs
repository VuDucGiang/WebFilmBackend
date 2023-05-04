using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.List;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IListService : IBaseService<int, List>
    {
        Task<object> GetListOfUser(int pageSize, int pageIndex, string userName);

        Task<object> AddListDetail(ListDTO list);
        Task<bool> EditListDetail(ListDTO list);
        Task<bool> DeleteListDetail(int listID);
        Task<object> GetPaging(PagingFilterParameter parameter);

        List<ListPopularDTO> GetListPopular();

        List<ListPopularDTO> GetListPopularWeek();

        List<ListPopularDTO> GetListRecentLikes();

        List<ListPopularDTO> GetListPopularMonth();

        List<ListPopularDTO> GetCrewList();

        List<ListPopularDTO> ListTop();

        PagingFilmResult DetailList(int id, PagingDetailList paging);

        ListPopularDTO GetListByID(int id);

        PagingCommentResult GetCommentList(int ListID, PagingParameter parameter);
    }
}
