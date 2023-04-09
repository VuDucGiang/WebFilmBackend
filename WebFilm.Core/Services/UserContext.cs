using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid? UserId
        {
            get
            {
                if (GetClaimValue("ID") != null)
                {
                    return Guid.Parse(GetClaimValue("ID"));
                }
                return null;
            }
        }

        public string? UserName
        {
            get
            {
                if(GetClaimValue("Name") != null)
                {
                    return GetClaimValue("Username");
                }
                return null;
                
            }
        }

        public string Email
        {
            get
            {
                return GetClaimValue("Email");
            }
        }

        public int RoleType
        {
            get
            {
                return int.Parse(GetClaimValue("Role"));
            }
        }

        private string GetClaimValue(string claimType)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
