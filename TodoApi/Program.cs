using Microsoft.EntityFrameworkCore;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
});

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
}

app.UseHttpsRedirection();

AddRoutes(app);

void AddRoutes(WebApplication app)
{
    var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    app.MapGet("/", () => Results.Json(new Todo {
                      Name = "Walk dog", IsComplete = false }, options));
   AddTodoItemsRoute(app);
}

void AddTodoItemsRoute(WebApplication app){
    var path =  "/todoitems";    

    app.MapGet(path, async (TodoDbContext db) =>
        await db.Todos.ToListAsync()
    );

    app.MapGet($"{path}/complete", async (TodoDbContext db) =>
        await db.Todos.Where(to => to.IsComplete).ToListAsync()
    );

    app.MapGet($"{path}/{{id}}", async (int id, TodoDbContext db) =>  await db.Todos.FindAsync(id)
        is Todo todo
        ? Results.Ok(todo)
        : Results.NotFound());

    app.MapPost(path, async (TodoItemDTO inputTodo, TodoDbContext db) => {
        var todo = new Todo{
            Id = inputTodo.Id,
            IsComplete = inputTodo.IsComplete,
            Name = inputTodo.Name,
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return Results.Created($"{path}/{todo.Id}", todo);
    });

    app.MapPut($"{path}/{{id}}", async (int id, TodoItemDTO inputTodo, TodoDbContext db) => {
        var todo = await db.Todos.FindAsync(id);
        if(todo == null) return Results.NotFound();

        todo.Name = inputTodo.Name;
        todo.IsComplete = inputTodo.IsComplete;

        await db.SaveChangesAsync();

        return Results.NoContent();
    });

    app.MapDelete($"{path}/{{id}}", async (int id, TodoDbContext db) => {
        var todo = await db.Todos.FindAsync(id);
        if(todo == null) return Results.NotFound();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    });
}

app.Run();

public partial class Program { }