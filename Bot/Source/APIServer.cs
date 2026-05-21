using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Aeoquotes;

public class APIServer
{
    private record Command(string action, string value);
    public APIServer(string[] consoleArgs, string dashboardToken)
    {
        var app = WebApplication.CreateBuilder(consoleArgs).Build();

        app.Urls.Add("http://localhost:6767");

        // gets
        app.MapGet("/api/getquotes", HandleQuotesRequest);

        // endpoint for commanding the bot from the dashboard
        // the dashboard should be the ONLY process communicating with this endpoint, 
        app.MapPost("/api/command", (Command req) =>
        {
            Console.WriteLine($"[API] Request: {req.action} recieved");

            switch (req.action)
            {
                case "quotes_import":

                    break;
                case "set_feature_enable":
                    break;
                case "set_feature_disable":
                    break;
                default:
                    break;
            }
            return Results.Ok("Authorized");
        }).AddEndpointFilter(async (context, next) =>
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Dashboard-Token", out var token) || token != dashboardToken)
            {
                // test failed, execute
                return Results.Forbid();
            }
            return await next(context);
        });

    }

    private string HandleQuotesRequest()
    {
        return JsonSerializer.Serialize(Program.Database?.Quotes);
    }
}