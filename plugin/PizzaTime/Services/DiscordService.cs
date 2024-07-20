using Discord.Webhook;
using System.Threading.Tasks;

namespace PizzaTime.Services;

internal sealed class DiscordService(string discordWebhookUrl)
{
    private readonly DiscordWebhookClient webhookClient = new(discordWebhookUrl);

    internal async Task<ulong> SendWebhookMessage(string message)
    {
        return await webhookClient.SendMessageAsync(message);
    }
}
