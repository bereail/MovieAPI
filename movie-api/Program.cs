using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using movie_api.Data;
using movie_api.Services.Implementations;
using movie_api.Services.Interfaces;
using MOVIE_API.Services.Implementations;
using MOVIE_API.Services.Interfaces;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("MovieBearer", new OpenApiSecurityScheme  //Esto va a permitir usar swagger con el token.
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Admin: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzIiwiZ2l2ZW5fbmFtZSI6ImFkbWluIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODk5NDk1NTYsImV4cCI6MTY4OTk1MzE1NiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE2OSIsImF1ZCI6ImNvbnN1bHRhYWx1bW5vc2FwaSJ9.0X3JKM20FCImAFkR3IYNmiFLLr_wLyS4ROGmxwuZWok" +
        "    " +
        " Client: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZ2l2ZW5fbmFtZSI6IkNsYXJhUmF6emV0dG8iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJDbGllbnQiLCJuYmYiOjE2ODk5NjU2NjEsImV4cCI6MTY4OTk2OTI2MSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE2OSIsImF1ZCI6ImNvbnN1bHRhYWx1bW5vc2FwaSJ9.9C8D-3bxA2bRef0QU3pVA_l81JLXZxJxj-Cdi2mhKIM"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "MovieBearer"  //Tiene que coincidir con el id seteado arriba en la definición
                }
            },
             new List<string>()
        }
    });
}); ;




//Agrego la inyección al context para que este disponibles en toda la app
//Paso al constructor la conexion a la base de datos
builder.Services.AddDbContext<movieDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//Auth
builder.Services.AddAuthentication("Bearer")  //"Bearer" es el tipo de auntenticación que tenemos que elegir después en PostMan para pasarle el token
    .AddJwtBearer(options =>  //Acá definimos la configuración de la autenticación. le decimos qué cosas queremos comprobar. La fecha de expiración se valida por defecto.
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
        };
    }
);


// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<movie_api.Services.Interfaces.IAuthenticationService, AuthenticateService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NuevaPolitica");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
