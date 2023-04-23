using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User
{
    public class PagingParameterMember : PagingParameter
    {
        public TypeUser typeUser { get; set; } = TypeUser.All;
        public string userName { get; set; } = "";
    }
}
