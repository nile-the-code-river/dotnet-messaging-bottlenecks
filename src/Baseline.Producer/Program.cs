using Baseline.Producer;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args); // Changed from WebApplication

//var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    //x.UsingInMemory((context, config) =>
    //{
    //    config.ConfigureEndpoints(context);
    //});

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

builder.Services.AddHostedService<OrderProducer>();

var app = builder.Build();
app.Run();