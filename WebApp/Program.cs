var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (HttpContext context) =>
{
    if (context.Request.Method == "GET")
    {
        if (context.Request.Path.StartsWithSegments("/"))
        {
            await context.Response.WriteAsync($"The method is: {context.Request.Method}\r\n");
            await context.Response.WriteAsync($"The Url is: {context.Request.Path}\r\n");
            await context.Response.WriteAsync($"\r\n");

            foreach (var key in context.Request.Headers.Keys)
            {
                string? Key = null;
                await context.Response.WriteAsync($"{Key}: {context.Request.Headers[key]}\r\n");
            }
        }
        else if (context.Request.Path.StartsWithSegments("/employees"))
        {
            var employees = EmployeeRepository.GetEmployees();

            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"Id: {employee.Id}, Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}\r\n");
            }
        }
    }
});

app.Run();

internal static class EmployeeRepository
{
    private static readonly List<Employee> _employees = new()
    {
        new Employee(1, "John Doe", "Software Engineer", 60000),
        new Employee(2, "Jane Smith", "Project Manager", 75000),
        new Employee(3, "Sam Brown", "UX Designer", 50000)
    };

    public static IEnumerable<Employee> GetEmployees() => _employees;

    public static Employee? GetById(int id) => _employees.FirstOrDefault(e => e.Id == id);
}

public class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Position { get; set; }
    public double Salary { get; set; }

    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}