using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Forwarded Headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add HttpClient and TMDB Service
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITMDBService, TMDBService>();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Use Forwarded Headers (đặt trước các middleware khác)
app.UseForwardedHeaders();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

// Add health check endpoint
app.MapGet("/", () => "API is running!");

app.MapControllers();

app.Run();