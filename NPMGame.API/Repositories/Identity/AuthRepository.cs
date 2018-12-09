using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Authentication.Cookies;
using NPMGame.API.Models.Requests;
using NPMGame.Core.Base;
using NPMGame.Core.Models.Identity;

namespace NPMGame.API.Repositories.Identity
{
    public class AuthRepository : BaseRepository
    {
        public AuthRepository(IDocumentStore documentStore) : base(documentStore)
        {
        }

        public async Task<ClaimsPrincipal> AuthenticateUser(LoginRequest loginRequest)
        {
            using (var session = _store.QuerySession())
            {
                var user = await session.Query<User>()
                    .Where(u => u.Email == loginRequest.Email || u.UserName == loginRequest.Email)
                    .FirstOrDefaultAsync();

                // User doesn't exist
                if (user == null)
                {
                    return null;
                }

                var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

                // Bad password
                if (!isPasswordValid)
                {
                    return null;
                }

                // User is legit, create a new claim identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var userRoleClaims = user.Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())).ToList();
                claims.AddRange(userRoleClaims);

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                return claimsPrincipal;
            }
        }

        public async Task<User> RegisterNewUser(RegisterRequest registerRequest)
        {
            using (var session = _store.QuerySession())
            {
                if (session.Query<User>().Any(u => u.Email == registerRequest.Email || u.UserName == registerRequest.UserName))
                {
                    return null;
                }

                using (var openSession = _store.OpenSession())
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                    var newUser = new User
                    {
                        Email = registerRequest.Email,
                        UserName = registerRequest.UserName,
                        Password = passwordHash,
                        Roles = new List<Role> { Role.Member }
                    };

                    openSession.Insert(newUser);
                    await openSession.SaveChangesAsync();

                    return newUser;
                }
            }
        }

        public async Task<User> GetUserByEmail(string emailAddress)
        {
            if (emailAddress == null)
            {
                return null;
            }

            using (var session = _store.QuerySession())
            {
                var user = await session.Query<User>()
                    .Where(u => u.Email == emailAddress)
                    .FirstOrDefaultAsync();

                return user;
            }
        }
    }
}