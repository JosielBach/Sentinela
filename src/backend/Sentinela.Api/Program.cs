using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Sentinela.Api.Converters;
using Sentinela.Api.Filters;
using Sentinela.Api.Token;
using Sentinela.Application;
using Sentinela.Communication.Responses;
using Sentinela.Domain.Repositories.User;
using Sentinela.Domain.Security.Tokens;
using Sentinela.Exception;
using Sentinela.Infrastructure;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(
    options => options.JsonSerializerOptions.Converters.Add(new StringConverter()));
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter only your access token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    options.AddSecurityRequirement(openApiDocment =>
    {
        return new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", openApiDocment), []
            }
        };
    });

});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAccessTokenProvider, HttpContextTokenProvider>();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo> { new("pt-BR"), new("en") };
    options.DefaultRequestCulture = new RequestCulture("en");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = [
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

builder.Services.AddMvc(options => options.Filters.Add<ExceptionFilter>());

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jtwoptions =>
{
    var signingKey = builder.Configuration.GetValue<string>("Jwt:SigningKey")!;
    jtwoptions.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ClockSkew = TimeSpan.Zero,
        ValidateAudience = false,
        ValidateIssuer = false
    };
    jtwoptions.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context => 
        {
            var subject = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(subject, out var userId) == false)
            {
                context.Fail("UserId is missing in the token.");
                return;
            }

            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserReadOnlyRepository>();

            var userExists = await userRepository.ExistActiveUserWithId(userId);
            if (userExists == false)
            {
                context.Fail("Invalid UserId.");
                return;
            }
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = context.AuthenticateFailure switch
            {
                null => new ResponseErrorJson(ResourceMessagesException.VALIDATION_ACCESS_TOKEN_REQUIRED),
                SecurityTokenArgumentException => new ResponseErrorJson("Token Expired", accessTokenExpired: true),
                _ => new ResponseErrorJson(ResourceMessagesException.VALIDATION_RESOURCE_ACCESS_DENIED)
            };

            await context.Response.WriteAsJsonAsync(response);
           
        }
    };
});

var app = builder.Build();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }