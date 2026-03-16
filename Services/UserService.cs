using ProjectTrackingApp.Data;
using ProjectTrackingApp.Models;

namespace ProjectTrackingApp.Services;

public class UserService
{
    private readonly DatabaseService _dbService;

    public UserService(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void DisplayUserWorkload()
    {
        var workloads = _dbService.GetUserWorkload();
        if (workloads.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    USER WORKLOAD SUMMARY                        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"{"User Name",-25} {"Role",-12} {"Total",-7} {"Done",-7} {"In Prog",-7} {"Pending",-7}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");

        foreach (var workload in workloads)
        {
            Console.WriteLine($"{workload.Name,-25} {workload.Role,-12} {workload.TotalAssignedTasks,-7} {workload.CompletedTasks,-7} {workload.InProgressTasks,-7} {workload.PendingTasks,-7}");
        }
    }

    public List<User> GetAllUsers()
    {
        return _dbService.GetAllUsers();
    }
}
