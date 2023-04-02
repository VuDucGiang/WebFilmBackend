using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IUserContext
    {
        Guid UserId { get; }
        string UserName { get; }
        string Email { get; }
        int RoleType { get; }
    }
}
