using MassTransit;
using Payment.Worker;
using Payment.Worker.Consumers;
using Payment.Worker.Services;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start Payment.Worker");

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog(
        (services, loggerConf) =>
        {
            loggerConf.ReadFrom.Configuration(builder.Configuration).ReadFrom.Services(services);
        }
    );

    builder.Services.AddScoped<IPaymentProcessor, FakePaymentProcessor>();

    builder.Services.AddMassTransit(busRegConfigurator =>
    {
        busRegConfigurator.AddConsumer<OrderCreatedConsumer>();

        busRegConfigurator.UsingRabbitMq(
            (context, configurator) =>
            {
                configurator.Host(
                    new Uri(
                        builder.Configuration.GetValue<string>(
                            "ExternalServices:MessageBroker:Host"
                        )!
                    ),
                    h =>
                    {
                        h.Username(
                            builder.Configuration.GetValue<string>(
                                "ExternalServices:MessageBroker:User"
                            )!
                        );
                        h.Password(
                            builder.Configuration.GetValue<string>(
                                "ExternalServices:MessageBroker:Password"
                            )!
                        );
                    }
                );

                configurator.ConfigureEndpoints(context);
            }
        );
    });

    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex.Message);
}
finally
{
    await Log.CloseAndFlushAsync();
}
