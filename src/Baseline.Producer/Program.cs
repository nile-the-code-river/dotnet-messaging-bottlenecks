using Baseline.Producer;
using MassTransit;

//var builder = WebApplication.CreateBuilder(args);
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // Configure the bus (example with RabbitMQ)
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

//builder.Services.AddHostedService<DummyProducer>();
builder.Services.AddHostedService<SampleProducer>();

var app = builder.Build();
app.Run();