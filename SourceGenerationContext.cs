using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Session))]
[JsonSerializable(typeof(BotClient.LoginSendData))]
[JsonSerializable(typeof(BotClient.PostData))]
internal partial class SourceGenerationContext : JsonSerializerContext { }