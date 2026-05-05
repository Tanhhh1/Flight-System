using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureConfiguration(builder.Configuration);

var app = builder.Build();

app.Run();
