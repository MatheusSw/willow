using System.Security.Claims;
using System.Text.Encodings.Web;

using admin_application.Handlers.Interfaces.ApiKeys;
using admin_application.Queries;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using Serilog;

namespace admin_api.Security;

public sealed class ApiKeyAuthenticationHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder,
	IValidateApiKeyQueryHandler validateHandler) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	private readonly IValidateApiKeyQueryHandler _validateHandler = validateHandler;

	public const string SchemeName = "ApiKey";
	public const string HeaderName = "x-api-key";

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Request.Headers.TryGetValue(HeaderName, out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
		{
			Log.ForContext<ApiKeyAuthenticationHandler>().Warning("Missing API key header");

			return AuthenticateResult.Fail("Missing API key header");
		}

		var valid = await _validateHandler.HandleAsync(new ValidateApiKeyQuery { ApiKey = apiKey! }, CancellationToken.None);
		if (!valid)
		{
			Log.ForContext<ApiKeyAuthenticationHandler>().Warning("API key invalid");

			return AuthenticateResult.Fail("Invalid API key");
		}

		var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "api-key"), new Claim("apikey", "true") };
		var identity = new ClaimsIdentity(claims, Scheme.Name);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		return AuthenticateResult.Success(ticket);
	}
}