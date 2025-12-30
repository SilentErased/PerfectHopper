using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class DiscordLogger
{
    static HttpClient Client = new HttpClient();

    public static async Task SendEmbedAsync(string webhookUrl, string title, string description, int color)
    {
        try
        {
            var payload = new
            {
                embeds = new[]
                {
                    new {
                        title = title,
                        description = description,
                        color = color
                    }
                }
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await Client.PostAsync(webhookUrl, content);
        }
        catch
        {
        }
    }

    public static async Task SendEmbedAsync(string webhookUrl, string title, string description, string hexColor, string thumbnailUrl = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hexColor))
            {
                await SendEmbedAsync(webhookUrl, title, description, 0);
                return;
            }

            string cleaned = hexColor.Trim().TrimStart('#');
            if (cleaned.Length == 3)
            {
                cleaned = $"{cleaned[0]}{cleaned[0]}{cleaned[1]}{cleaned[1]}{cleaned[2]}{cleaned[2]}";
            }
            if (cleaned.Length != 6)
            {
                await SendEmbedAsync(webhookUrl, title, description, 0);
                return;
            }

            int color = int.Parse(cleaned, NumberStyles.HexNumber);
            object embed;
            if (string.IsNullOrWhiteSpace(thumbnailUrl))
            {
                embed = new
                {
                    embeds = new[]
                    {
                        new {
                            title = title,
                            description = description,
                            color = color
                        }
                    }
                };
            }
            else
            {
                embed = new
                {
                    embeds = new[]
                    {
                        new {
                            title = title,
                            description = description,
                            color = color,
                            thumbnail = new { url = thumbnailUrl }
                        }
                    }
                };
            }

            var content = new StringContent(JsonSerializer.Serialize(embed), Encoding.UTF8, "application/json");
            await Client.PostAsync(webhookUrl, content);
        }
        catch
        {
        }
    }
}
