using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace BskyZundamonBot;

internal class Program
{
    private static string[] _characters = { "ず", "ん", "だ", "も", "ん" };
    private static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();
        var pdsHost = DotNetEnv.Env.GetString("PDSHOST");
        var id = DotNetEnv.Env.GetString("ID");
        var password = DotNetEnv.Env.GetString("PASSWORD");

        var botClient = new BotClient(pdsHost, id, password);
        try
        {
            await botClient.CreateNewSessionAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Create Session Failed.\n{ex}");
            Environment.Exit(255);
        }

        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger("Program");
        logger.LogInformation($"Login succeeded as {id}");

        DateTime currentTime = DateTime.Now;
        DateTime previousTime = currentTime;
        TimeSpan timeSpan;
        await Task.Run(async () =>
        {
            while (true)
            {
                currentTime = DateTime.Now;
                if (currentTime.Minute == 0 && currentTime.Second == 0)
                {
                    try
                    {
                        var result = await GetCharas();
                        await botClient.PostAsync(result);
                        logger.LogInformation($"Posted {result} at {DateTime.Now.ToString("HH:mm")}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Post failed. {ex}");
                    }
                }

                timeSpan = currentTime - previousTime;
                if (timeSpan.Hours == 1)
                {
                    try
                    {
                        await botClient.RefreshSessionAsync();
                        logger.LogInformation($"Refreshed token at {DateTime.Now.ToString("HH:mm")}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Session refresh failed. {ex}");
                    }
                    previousTime = currentTime;
                }

                await Task.Delay(200);
            }
        });
    }

    private static async Task<string> GetCharas()
    {
        var charaIndexes = new List<int>() { 0, 1, 2, 3, 4 };
        var newIndexes = new List<int>();
        var result = "";
        var random = new Random();

        await Task.Run(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                var index = random.Next(0, charaIndexes.Count);
                newIndexes.Add(charaIndexes[index]);
                charaIndexes.Remove(charaIndexes[index]);
                result += _characters[newIndexes[i]];
                Thread.Sleep(2000);
            }
        });

        return result;
    }
}