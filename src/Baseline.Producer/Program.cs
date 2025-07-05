using Baseline.Producer;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, config) =>
    {
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddHostedService<PingPublisher>();

var app = builder.Build();
app.Run();