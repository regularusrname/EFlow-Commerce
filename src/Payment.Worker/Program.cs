using Payment.Worker;
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
