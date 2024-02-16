using Microsoft.OpenApi.Models;
using Reineur.API.Middlewares;
using Reineur.Share;
using Users.Infrastucture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
string SpecificOrigin = "AllowSpecificOrigin";
builder.Services.AddCors(opt =>
{

    opt.AddPolicy(SpecificOrigin,
        x =>
        {
            x.WithOrigins(EnvVariable.ORIGIN)
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
            .AllowCredentials();
        });
});
builder.Services.AddShareService();
builder.Services.UserServiceCollection();
builder.Services.AddTransient<ExceptionHandlerMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reineur API V1");
    if (!app.Environment.IsDevelopment())
        c.RoutePrefix = string.Empty;
});
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors(SpecificOrigin);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
