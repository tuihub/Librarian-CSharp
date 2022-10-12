using Librarian.Models;
using Librarian.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

// Get Configuration
GlobalContext.SystemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>();
GlobalContext.JwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

// Add Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
        options.TokenValidationParameters = new()
        {
            //ValidIssuer = GlobalContext.JwtConfig.Issuer,
            //ValidAudience = GlobalContext.JwtConfig.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RequireExpirationTime = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Librarian.Services.Sephirah.SephirahService>();

//app.MapGrpcService<GreeterService>();
//app.MapGrpcService<FileGrpcService>();
//app.MapGrpcService<UserService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Enable Auth
app.UseAuthentication();
app.UseAuthorization();

app.Run();
