using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using serverSide;

var builder = Host.CreateDefaultBuilder(args);

// Add services to the container.

builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
});

var app = builder.Build();

app.Run();
