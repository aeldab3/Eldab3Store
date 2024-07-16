using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(c => {
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddApplicationServices();

var app = builder.Build();


// Apply migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = services.GetRequiredService<StoreContext>();  //We create a scope to resolve StoreContext and ILoggerFactory.
        await context.Database.MigrateAsync(); // We attempt to migrate the database and catch any exceptions to log them.
        await StoreContextSeed.SeedAsync(context, loggerFactory);  // Seed the database
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred during migration or seeding");
    }
}


    app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
