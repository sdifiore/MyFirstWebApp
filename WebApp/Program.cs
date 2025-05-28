using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(static async (HttpContext context) =>
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
    else if (context.Request.Method == "POST")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            EmployeeRepository.AddEmployee(employee!);
        }
    }
    else if (context.Request.Method == "PUT")
    {
        // Fix for CS0815: Cannot assign void to an implicitly-typed variable
        // The issue is that `EmployeeRepository.AddEmployee` returns void, but the code attempts to assign its result to a variable.
        // To fix this, remove the assignment and directly call the method.

        if (context.Request.Method == "PUT")
        {
            if (context.Request.Path.StartsWithSegments("/employees"))
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var employee = JsonSerializer.Deserialize<Employee>(body);

                var result = EmployeeRepository.UpdateEmployee(employee!); // Correct method call for updating an employee
                if (result)
                {
                    context.Response.StatusCode = 200; // OK
                    await context.Response.WriteAsync("Employee updated successfully.");
                }
                else
                {
                    context.Response.StatusCode = 404; // Not Found
                    await context.Response.WriteAsync("Employee not found.");
                }
            }
        }
        else if (context.Request.Method == "DELETE")
        {
            if (context.Request.Path.StartsWithSegments("/employees"))
            {
                if (context.Request.Query.ContainsKey("id"))
                {
                    var id = context.Request.Query["id"].ToString();
                    if (int.TryParse(id, out int employeeId))
                    {
                        if (context.Request.Headers["Authorization"] == "Sergio")
                        {
                            var search = EmployeeRepository.DeleteEmployee(employeeId);

                            if (search)
                            {
                                await context.Response.WriteAsync("Employee deleted successfully.");
                            }
                            else
                            {
                                await context.Response.WriteAsync("Employee not found.");
                            }
                        }
                        else
                        {
                            await context.Response.WriteAsync("You are not authorized.");
                        }

                        var result = EmployeeRepository.DeleteEmployee(employeeId);

                        if (result)
                        {
                            await context.Response.WriteAsync("Employee deleted successfully.");
                        }
                        else
                        {
                            await context.Response.WriteAsync("Employee not found.");
                        }
                    }
                }
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

    public static void AddEmployee(Employee? employee)
    {
        if (employee != null)
        {
            _employees.Add(employee);
        }
    }

    public static bool UpdateEmployee(Employee? employee)
    {
        if (employee != null)
        {
            var empl = _employees.FirstOrDefault(e => e.Id == employee.Id);

            if (empl != null)
            {
                empl.Name = employee.Name;
                empl.Position = employee.Position;
                empl.Salary = employee.Salary;

                return true;
            }

            return false;
        }

        return false; // Ensure all code paths return a value
    }

    public static bool DeleteEmployee(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);

        if (employee != null)
        {
            _employees.Remove(employee);
            return true;
        }
        return false;
    }
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