using System.Text.Json.Serialization;

public record DidDoc(
    [property: JsonPropertyName("@context")] IReadOnlyList<string> Context,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("alsoKnownAs")] IReadOnlyList<string> AlsoKnownAs,
    [property: JsonPropertyName("verificationMethod")] IReadOnlyList<VerificationMethod> VerificationMethod,
    [property: JsonPropertyName("service")] IReadOnlyList<Service> Service
);
public record Session(
    [property: JsonPropertyName("did")] string Did,
    [property: JsonPropertyName("didDoc")] DidDoc DidDoc,
    [property: JsonPropertyName("handle")] string Handle,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("emailConfirmed")] bool EmailConfirmed,
    [property: JsonPropertyName("emailAuthFactor")] bool EmailAuthFactor,
    [property: JsonPropertyName("accessJwt")] string AccessJwt,
    [property: JsonPropertyName("refreshJwt")] string RefreshJwt,
    [property: JsonPropertyName("active")] bool Active
);
public record Service(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("serviceEndpoint")] string ServiceEndpoint
);
public record VerificationMethod(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("controller")] string Controller,
    [property: JsonPropertyName("publicKeyMultibase")] string PublicKeyMultibase
);
