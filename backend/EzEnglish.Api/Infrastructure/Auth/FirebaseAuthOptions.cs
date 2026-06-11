using Microsoft.AspNetCore.Authentication;

namespace EzEnglish.Api.Infrastructure.Auth;

/// <summary>
/// Options for the Firebase ID-token authentication scheme.
/// </summary>
public sealed class FirebaseAuthOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "Firebase";

    /// <summary>
    /// Firebase project ID. Used as a sanity check against the verified token's
    /// audience. Populated from <c>Firebase:ProjectId</c> in appsettings.
    /// </summary>
    public string ProjectId { get; set; } = "";
}
