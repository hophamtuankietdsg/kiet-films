using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://kiet-films.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Add Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITMDBService, TMDBService>();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Rating API V1");
    c.RoutePrefix = "swagger";
});
app.UseCors();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();