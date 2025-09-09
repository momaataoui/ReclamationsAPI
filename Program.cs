using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // <-- AJOUTER CE USING
using ReclamationsAPI;
using ReclamationsAPI.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuration des services ---

builder.Services.AddEndpointsApiExplorer();

// --- MODIFICATION ICI ---
// On remplace le simple AddSwaggerGen() par cette configuration complète
builder.Services.AddSwaggerGen(options =>
{
    // Titre et version de l'API pour Swagger UI
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Reclamations API", Version = "v1" });

    // Définition du schéma de sécurité "Bearer" (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Veuillez entrer un token JWT valide",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // On exige que le token soit fourni pour les endpoints qui le nécessitent
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// --- FIN DE LA MODIFICATION ---


// Définition de la politique CORS
string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Ajout des contrôleurs
builder.Services.AddControllers();

// Configuration de la base de données SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration de l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Ajout du service d'autorisation
builder.Services.AddAuthorization();

// --- 2. Construction de l'application ---
var app = builder.Build();

// --- 3. Initialisation des données (Seeding) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        SeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// --- 4. Configuration du pipeline de requêtes HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Votre configuration SwaggerUI est parfaite, pas besoin de la changer
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reclamations API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Redirection HTTPS
app.UseHttpsRedirection();

// Application de la politique CORS
app.UseCors(myAllowSpecificOrigins);

// Activation de l'authentification et de l'autorisation
app.UseAuthentication();
app.UseAuthorization();

// Routage des requêtes vers les contrôleurs
app.MapControllers();

// --- 5. Démarrage de l'application ---
app.Run();