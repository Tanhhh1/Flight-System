using Serilog;

namespace API_FlightBooking.Registers
{
    public static class LoggingRegister
    {
        public static void RegisterLoggerServices(this ILoggingBuilder loggingBuilder, IConfiguration Configuration)
        {
            const string logFolderName = "Logs";
            if (!Directory.Exists(logFolderName))
            {
                Directory.CreateDirectory(logFolderName);
            }

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger);
        }
    }
}
