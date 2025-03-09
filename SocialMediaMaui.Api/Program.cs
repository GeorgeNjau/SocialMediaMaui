using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialMediaMaui.Api.Data;
using SocialMediaMaui.Api.Data.Entities;
using SocialMediaMaui.Api.EndPoints;
using SocialMediaMaui.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient<AuthService>()
                .AddTransient<PostService>()
                .AddTransient<IPasswordHasher<User>, PasswordHasher<User>>()
                .AddTransient<UserService>()
                .AddTransient<PhotoUploadService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
    {
        var issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
        var secretKey = builder.Configuration.GetValue<string>("Jwt:SecretKey");
        var securityKey = System.Text.Encoding.UTF8.GetBytes(secretKey);
        var symetricKey = new SymmetricSecurityKey(securityKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symetricKey
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

#if DEBUG
AutoMigrateDb(app.Services);
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication()
    .UseAuthorization();

app.MapAuthEndPoints()
    .MapUserEndPoints()
   .MapPostEndPoints();

app.Run();


static void AutoMigrateDb(IServiceProvider provider)
{
    var scope = provider.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    if(dataContext.Database.GetPendingMigrations().Any())
    {
        dataContext.Database.Migrate();
    }
}