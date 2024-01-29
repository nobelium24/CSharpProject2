using ECommerceApp.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerceApp.Services;
using System.IdentityModel.Tokens.Jwt;
using ECommerceApp.Errors;
using ECommerceApp.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddHttpClient();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new IsNullException()))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context => {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Authentication failed");

            return Task.CompletedTask;
        }
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register the database context. The database context is registered with the AddDbContext method. The AddDbContext method takes a lambda expression that configures the database context. The lambda expression takes an options parameter that is used to configure the database context. The options parameter has a UseSqlServer method that takes a connection string as an argument. The connection string is retrieved from the appsettings.json file using the GetConnectionString method. The GetConnectionString method takes the name of the connection string as an argument. The name of the connection string is "DefaultConnection".
builder.Services.AddDbContext<ApplicationDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

// Register the TokenService class
builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<CloudinaryService>();

//Register the error handler middleware
builder.Services.AddScoped<ErrorHandlerMiddleware>();

//Register the mailer service 
builder.Services.AddScoped<SendMail>();

builder.Services.AddScoped<CodeGenerator>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();


IConfiguration configuration = app.Configuration;

IWebHostEnvironment environment = app.Environment;

app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<WebSocketMiddleware>();

app.Run();
