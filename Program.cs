using AuthenticationApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
IConfiguration configuration = builder.Configuration;   

//Add Authentication Sche

//Add Auth Service
builder.Services.AddTransient<IAuthService, AuthService>(provider => new AuthService(connectionString!, configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
