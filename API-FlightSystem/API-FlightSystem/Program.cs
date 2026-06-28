using API_FlightBooking.Registers;
using Application;
using Infrastructure;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.RegisterLoggerServices(builder.Configuration);
builder.Services.RegisterGeneralServices(builder.Configuration);
builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.AddInfrastructureConfiguration(builder.Configuration);
builder.Services.RegisterSwaggerServices();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

app.RegisterGeneralApp(app.Environment);
app.RegisterSwaggerApp(builder);

app.Run();