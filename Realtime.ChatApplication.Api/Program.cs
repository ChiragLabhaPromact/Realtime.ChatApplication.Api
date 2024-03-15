using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Realtime.ChatApplication.Api.Hubs;
using Realtime.ChatApplication.DomianModels.Context;
using Realtime.ChatApplication.Repository.Contracts.Messages;
using Realtime.ChatApplication.Repository.Implementations.Messages;
using Realtime.ChatApplication.Service.Contracts.Messages;
using Realtime.ChatApplication.Service.Contracts.SignalR;
using Realtime.ChatApplication.Service.Contracts.Users;
using Realtime.ChatApplication.Service.Implementations.Auth;
using Realtime.ChatApplication.Service.Implementations.Messages;
using Realtime.ChatApplication.Service.Implementations.SignalR;
using Realtime.ChatApplication.Service.Implementations.Users;
using Realtime.ChatApplication.Service.OptionConfigurationModels;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "MyApp.ChatHub";
    });

builder.Services.AddSingleton(sp =>
{
    var redisConnection = ConnectionMultiplexer.Connect("localhost:6379");
    return redisConnection;
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();
builder.Services.AddScoped<JwtToken>();


builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

builder.Services.Configure<JwtConfigurationOptions>(builder.Configuration.GetSection(JwtConfigurationOptions.JwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(
    options =>
    {
        var jwtSettings = builder.Configuration.GetSection(JwtConfigurationOptions.JwtKey).Get<JwtConfigurationOptions>();
        if (jwtSettings == null)
        {
            throw new Exception("JWT settings are missing");
        }
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            Array.Empty<string>()
        }
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
builder =>
{
    builder.AllowAnyMethod().AllowAnyHeader()
    .WithOrigins("http://localhost:4200")
    .AllowCredentials();
}));

builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();
