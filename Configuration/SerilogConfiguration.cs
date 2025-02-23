using Serilog;

namespace serilog_payload_demo.Configuration;

public static class SerilogConfiguration
{
    public static ILoggingBuilder AddCustomSerilogConfiguration(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration);
        
        Log.Logger = loggerConfiguration.CreateLogger();
        
        return builder.AddSerilog(Log.Logger);
    }
}