using Core;
using Core.Configuration;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestApi.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // 1. Omit null properties from response JSON
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                    // 2. Convert Enums ("Admin" instead of 0) for controllers
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
//Convert Enums for minimal APIs
builder.Services.ConfigureHttpJsonOptions(opt => { opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddProblemDetails(op => op.CustomizeProblemDetails = context =>
{
    //endpoint that failed
    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    //requestId for tracing request in log file
    context.ProblemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier; 
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();
 
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Auth"));

builder.Services.AddCore();
builder.Services.AddInfrastructure();

builder.Services.AddScoped<JwtOptions>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var allowedOrigins =builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

JwtOptions jwtOptions = builder.Configuration.GetSection("Auth").Get<JwtOptions>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
    };

    // when Challenge, return ProblemDetails response
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Bypass default handling
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = context.ErrorDescription ?? "You must provide a valid JWT bearer token to access this resource.",
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();


app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
