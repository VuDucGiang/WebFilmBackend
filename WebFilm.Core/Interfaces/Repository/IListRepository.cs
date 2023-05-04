using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Follow;
using WebFilm.Core.Enitites.List;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IListRepository : IBaseRepository<int, List>
    {
        Task<object> GetListOfUser(int pageSize, int pageIndex, string userName);
        Task<object> AddListDetail(ListDTO list);
        Task<bool> EditListDetail(ListDTO list);
        Task<object> GetPaging(PagingFilterParameter parameter);
        List<ListPopularWeekDTO> PopularWeekList();
        List<ListRecentLikeDTO> RecentLikeList();
        List<ListPopularWeekDTO> PopularMonthList();
        List<ListPopularWeekDTO> ListCrew();
        List<ListPopularWeekDTO> ListTopLike();
        int UpdateCommentCount(int listID, int commentCount);

        int UpdateLikeCount(int ListID, int likeCount);

    }
}
