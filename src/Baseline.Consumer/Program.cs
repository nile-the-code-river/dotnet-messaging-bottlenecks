using Baseline.Consumer;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

// Configure MassTransit
builder.Services.AddMassTransit(x =>
{
    // Add your consumers
    x.AddConsumers(typeof(Program).Assembly);

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

var host = builder.Build();
host.Run();
