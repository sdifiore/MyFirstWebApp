var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(static async (HttpContext context) =>
{
    foreach (var key in context.Request.Query.Keys)
    {
        await context.Response.WriteAsync($"{key}: {context.Request.Query[key]}\n");
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