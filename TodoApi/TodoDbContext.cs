using Microsoft.EntityFrameworkCore;
public class TodoDbContext: DbContext {
     public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {
     }
    public DbSet<Todo> Todos => Set<Todo>();
}

public record Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public string? Secret { get; set; }
}
