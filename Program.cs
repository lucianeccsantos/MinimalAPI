using System.Reflection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<GreetingsService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

var action = () => new Todo[]{new(1, "Test"), new (2, "other")};


app.MapGet("/todos/action", action );
app.MapGet("/todos/{*name}", (string name) => new {Greetings = $"Hello, {name}!"} );
app.MapGet("/todos/{project}", ([FromRoute]string project, [FromQuery]int page) => 
    new {Greetings = $"Show todos from project: {project}, page {page}"} 
);
app.MapGet("/GreetingsService",(string name, GreetingsService greetingsService) => new {Greetings = greetingsService.Greet(name)});
app.MapGet("/OlaRequest", async(HttpRequest request, HttpResponse response) => 
{
    var name = request.Query["name"];
    await response.WriteAsync($"Hello, {name}!");
    }
);

app.MapPost("/test", (Todo2 todo)=> todo);
app.MapGet("/testService", ([AsParameters] GreetingAPIInput greetingAPIInput)=> greetingAPIInput.service.Greet(greetingAPIInput.name));
app.MapGet("/testServicePagination", ([AsParameters] PaginationDto paginationDto)=> paginationDto);

app.Run();

record Todo(int Id, string Title);
record  Todo2(int Id, string Title){
   /* public static bool TryParse(string todoEncoded, out Todo2? todo)
    {
        try{
            var parts = todoEncoded.Split(",");
            todo = new Todo2(int.Parse(parts[0]), parts[1]);
            return true;
        } catch(Exception ex){
            todo = null;
            return false;
        }
    }*/

   /* public static async ValueTask<Todo2?> BindAsync(HttpContext ctx, ParameterInfo parameterInfo)
    {
        try{
            var content = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
            var parts = content.Split(",");
            return new Todo2(int.Parse(parts[0]), parts[1]);
        } catch (Exception ex){
            return null;
        }
    }*/
}

record GreetingAPIInput(string name, GreetingsService service);

class GreetingsService{
    public string Greet(string name) => $"Hello {name}!";
}

record PaginationDto(int CurrentPage, int PerPage);