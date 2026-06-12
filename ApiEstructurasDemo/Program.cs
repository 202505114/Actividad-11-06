using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar servicios y dependencias

// Habilitar la exploración de endpoints y Swagger/OpenAPI para la documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Habilitar CORS permitiendo peticiones de cualquier origen, método y encabezado (ideal para Postman/Frontend local)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 2. Configurar el pipeline HTTP (middlewares)

// Usar Swagger y su interfaz gráfica en cualquier entorno
app.UseSwagger();
app.UseSwaggerUI();

// Aplicar la política de CORS definida arriba
app.UseCors("PermitirTodo");

// 3. Estructura de Datos en Memoria

// Lista estática inicializada con los dos nodos solicitados
var nodos = new List<NodoElemento>
{
    new NodoElemento { Id = 10, Valor = "Raíz Inicial (ABB)" },
    new NodoElemento { Id = 5, Valor = "Hijo Izquierdo" }
};

// 4. Endpoints (Minimal APIs)

// GET /api/nodos -> Retorna todos los nodos
app.MapGet("/api/nodos", () =>
{
    // Devuelve la lista en formato JSON con status 200 OK por defecto
    return Results.Ok(nodos);
})
.WithName("ObtenerNodos");

// POST /api/nodos -> Recibe y agrega un nuevo nodo
app.MapPost("/api/nodos", ([FromBody] NodoElemento nuevoNodo) =>
{
    // a) Validar que Id sea mayor a 0 y Valor no sea nulo ni esté vacío
    if (nuevoNodo.Id <= 0 || string.IsNullOrWhiteSpace(nuevoNodo.Valor))
    {
        return Results.BadRequest(new { error = "El Id debe ser mayor a 0 y el Valor no puede estar vacío." });
    }

    // b) Validar si ya existe un nodo con ese Id
    if (nodos.Any(n => n.Id == nuevoNodo.Id))
    {
        return Results.Conflict(new { error = $"Ya existe un nodo con el Id {nuevoNodo.Id}." });
    }

    // Si todo es válido, se agrega a la lista
    nodos.Add(nuevoNodo);

    // Retorna 201 Created, mandando la URI del "recurso creado" en el Header Location y el nodo agregado en el body
    return Results.Created($"/api/nodos/{nuevoNodo.Id}", nuevoNodo);
})
.WithName("AgregarNodo");

// Redirigir la ruta raíz al Swagger para que la página principal funcione de inmediato
app.MapGet("/", () => Results.Redirect("/swagger"));

// Iniciar la aplicación web
app.Run();

// 5. Modelo

// Definición de la clase NodoElemento
public class NodoElemento
{
    public int Id { get; set; }
    public string Valor { get; set; } = string.Empty;
}
