using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using DBWorldCsvApi.Services;
using DBWorldCsvApi.Data;

namespace DBWorldCsvApi.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppDbContext _context;
        private readonly IHashingService _hashingService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            AppDbContext context,
            IHashingService hashingService)
            : base(options, logger, encoder, clock)
        {
            _context = context;
            _hashingService = hashingService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                var authHeaderValue = authHeader.Split(' ')[1];
                var bytes = Convert.FromBase64String(authHeaderValue);
                var credentials = Encoding.UTF8.GetString(bytes).Split(':');
                var email = credentials[0];
                var password = credentials[1];

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !_hashingService.VerifyHash(password, user.Password))
                {
                    return AuthenticateResult.Fail("Invalid Username or Password");
                }

                var claims = new[] {
                    new Claim(ClaimTypes.Name, email)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
