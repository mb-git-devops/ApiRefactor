using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ApiRefactor.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ApiRefactor.Authentication;

public sealed class DummyBearerAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptionsMonitor<DummyAuthOptions> dummyAuth)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IOptionsMonitor<DummyAuthOptions> _dummyAuth = dummyAuth;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header."));
        }

        if (!AuthenticationHeaderValue.TryParse(authorization.ToString(), out var headerValue)
            || headerValue is null
            || !string.Equals(headerValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrEmpty(headerValue.Parameter))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header."));
        }

        var expected = _dummyAuth.CurrentValue.BearerToken;
        if (string.IsNullOrEmpty(expected))
        {
            return Task.FromResult(AuthenticateResult.Fail("Dummy auth is not configured."));
        }

        if (!string.Equals(headerValue.Parameter, expected, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid bearer token."));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "dummy-user") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
