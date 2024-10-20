using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class BotClient
{
    public string PdsHost { get; set; }
    public string ID { get; set; }
    public string Password { get; set; }
    public Session? CurrentSession { get; set; }
    private static HttpClient _httpClient;

    public record LoginSendData
    (
        [property: JsonPropertyName("identifier")] string Identifier,
        [property: JsonPropertyName("password")] string Password
    );

    public record PostData
    (
        [property: JsonPropertyName("repo")] string Repo,
        [property: JsonPropertyName("collection")] string collection,
        [property: JsonPropertyName("record")] RecordData Record
    );

    public record RecordData
    (
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("createdAt")] string CreatedAt
    );

    public BotClient(string pdsHost, string id, string password)
    {
        this.PdsHost = pdsHost;
        this.ID = id;
        this.Password = password;
        _httpClient = new HttpClient();
    }

    public async Task CreateNewSessionAsync()
    {
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new LoginSendData(this.ID, this.Password),
            SourceGenerationContext.Default.LoginSendData),
            Encoding.UTF8,
            "application/json");
        try
        {
            using HttpResponseMessage response = await _httpClient.PostAsync(
            PdsHost + "/xrpc/com.atproto.server.createSession",
            jsonContent);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Create Session Response returned {response.StatusCode}.\n{await response.Content.ReadAsStringAsync()}");

            var respStream = await response.Content.ReadAsStreamAsync();
            this.CurrentSession = await JsonSerializer.DeserializeAsync<Session>(respStream, SourceGenerationContext.Default.Session);
        }
        catch (Exception) { throw; }
    }

    public async Task RefreshSessionAsync()
    {
        var refreshJwt = CurrentSession?.RefreshJwt;
        var request = new HttpRequestMessage(HttpMethod.Post, PdsHost + "/xrpc/com.atproto.server.refreshSession");
        request.Headers.Add("Authorization", $"Bearer {refreshJwt}");

        try
        {
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Refresh Session Response returned {response.StatusCode}.\n{await response.Content.ReadAsStringAsync()}");

            var respStrem = await response.Content.ReadAsStreamAsync();
            this.CurrentSession = await JsonSerializer.DeserializeAsync<Session>(respStrem, SourceGenerationContext.Default.Session);
        }
        catch (Exception) { throw; }
    }

    public async Task PostAsync(string text)
    {
        var dateTime = DateTime.Now;
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new PostData(this.ID, "app.bsky.feed.post", new RecordData(text, dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.ffffffzzz"))),
            SourceGenerationContext.Default.PostData),
            Encoding.UTF8,
            "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, PdsHost + "/xrpc/com.atproto.repo.createRecord");
        request.Headers.Add("Authorization", $"Bearer {CurrentSession?.AccessJwt}");
        request.Content = jsonContent;

        try
        {
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Create Session Response returned {response.StatusCode}.\n{await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception) { throw; }
    }
}