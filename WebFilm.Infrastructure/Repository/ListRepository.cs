using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class ListRepository : BaseRepository<int, List>, IListRepository
    {
        public ListRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
