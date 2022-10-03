using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Xunit;

namespace TodoApi.Tests;

public class TodoTests
{
    const string _path = "/todoitems";
    System.Net.Http.HttpClient? _client = null;

    public TodoTests()
    {
       var application = new TodoApplication();
      _client = application.CreateClient();   
    }

    [Fact]
    public async Task GetTodos_ShouldReturnEmpty()
    {      
        var todos = await _client?.GetFromJsonAsync<List<Todo>>(_path);

        Assert.Empty(todos);
    }

     [Fact]
    public async Task PostTodos_ShouldReturnOnlyOne()
    {
        // Act
        var response = await _client?.PostAsJsonAsync(_path, new TodoItemDTO { Name = "I want to do this thing tomorrow" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var todos = await _client.GetFromJsonAsync<List<TodoItemDTO>>(_path);

        var todo = Assert.Single(todos);
        Assert.Equal("I want to do this thing tomorrow", todo.Name);
        Assert.False(todo.IsComplete);
    }
    
    [Fact]
    public async Task DeleteTodos_WhenLastDeleted_ShouldReturnNotFound()
    {
        await using var application = new TodoApplication();

        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync(_path, new TodoItemDTO { Name = "I want to do this thing tomorrow" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var todos = await client.GetFromJsonAsync<List<Todo>>(_path);

        var todo = Assert.Single(todos);
        Assert.Equal("I want to do this thing tomorrow", todo.Name);
        Assert.False(todo.IsComplete);

        // Act
        response = await client.DeleteAsync($"{_path}/{todo.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await client.GetAsync($"{_path}/{todo.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

class TodoApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<TodoDbContext>));

            services.AddDbContext<TodoDbContext>(options =>
                options.UseInMemoryDatabase("Testing", root));
        });

        return base.CreateHost(builder);
    }
}
