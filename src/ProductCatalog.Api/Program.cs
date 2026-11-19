
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la connexion SQL Server
var connectionString =
    builder.Configuration.GetConnectionString("CatalogDbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Activation de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger pour tester l'API
// Utilisation du format Swagger 2.0
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI();

// Création automatique de la base de données
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();
}

// Endpoint Healthcheck
app.MapGet("/health", () =>
    Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow
    }))
    .WithName("HealthCheck");

// GET : Liste des produits
app.MapGet("/api/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync())
    .WithName("GetAllProducts");

// POST : Ajouter un produit
app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/products/{product.Id}",
        product
    );
})
.WithName("CreateProduct");

app.Run();
