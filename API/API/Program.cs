using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var _config = builder.Configuration;  // Initialize the configuration object
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppIdentityDbContext>(options => 
options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(c => {
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(_config); //Pass the configuration object to the extension method

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
        await StoreContextSeed.SeedAsync(context, loggerFactory);  // seeds initial data into the database.

        //Retrieves the UserManager<AppUser> service for managing users.
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        //handles identity-related database operations.
        var identityContext = services.GetRequiredService<AppIdentityDbContext>(); 
        //Applies any pending migrations to ensure the identity database schema is up-to-date.
        await identityContext.Database.MigrateAsync();
        // A static method that seeds initial identity-related data into the database.
        await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
