var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (HttpContext context) =>
{
    await context.Response.WriteAsync($"The method is: {context.Request.Method}\r\n");
    await context.Response.WriteAsync($"The Url is: {context.Request.Path}\r\n");
    await context.Response.WriteAsync($"\r\n");

    foreach (var key in context.Request.Headers.Keys)
    {
        string? Key = null;
        await context.Response.WriteAsync($"{Key}: {context.Request.Headers[key]}\r\n");
    }
});

app.Run();