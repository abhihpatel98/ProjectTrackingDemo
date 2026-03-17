using ProjectTrackingApp.Data;
using ProjectTrackingApp.Models;

namespace ProjectTrackingApp.Services;

public class ProjectService
{
    private readonly DatabaseService _dbService;

    public ProjectService(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void DisplayAllProjects()
    {
        var projects = _dbService.GetAllProjects();
        if (projects.Count == 0)
        {
            Console.WriteLine("No projects found.");
            return;
        }

        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                        ALL PROJECTS                              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        foreach (var project in projects)
        {
            Console.WriteLine($"[{project.ProjectId}] {project.ProjectName}");
            Console.WriteLine($"    Status: {project.Status} | Start: {project.StartDate:yyyy-MM-dd} | End: {project.EndDate:yyyy-MM-dd}");
            if (!string.IsNullOrEmpty(project.Description))
            {
                Console.WriteLine($"    Description: {project.Description}");
            }
            Console.WriteLine();
        }
    }

    public void DisplayProjectDetails(int projectId)
    {
        var project = _dbService.GetProjectDetails(projectId);
        if (project == null)
        {
            Console.WriteLine("Project not found.");
            return;
        }

        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  {project.ProjectName}");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"Status: {project.Status}");
        Console.WriteLine($"Period: {project.StartDate:yyyy-MM-dd} to {project.EndDate:yyyy-MM-dd}");
        Console.WriteLine();
        
        if (project.Tasks.Count == 0)
        {
            Console.WriteLine("No tasks in this project.");
        }
        else
        {
            Console.WriteLine($"Tasks ({project.Tasks.Count}):");
            Console.WriteLine("─────────────────────────────────────────────────────");
            foreach (var task in project.Tasks)
            {
                Console.WriteLine($"  [{task.TaskId}] {task.Title}");
                Console.WriteLine($"       Status: {task.Status} | Assigned to: {task.AssignedUserName}");
            }
        }
    }

    public void DisplayProjectWorkloadSummary()
    {
        var summaries = _dbService.GetProjectWorkloadSummary();
        if (summaries.Count == 0)
        {
            Console.WriteLine("No projects found.");
            return;
        }

        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              PROJECT WORKLOAD SUMMARY                           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"{"Project Name",-30} {"Total",-7} {"Done",-7} {"In Prog",-7} {"Pending",-7}");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"{summary.ProjectName,-30} {summary.TotalTasks,-7} {summary.CompletedTasks,-7} {summary.InProgressTasks,-7} {summary.PendingTasks,-7}");
        }
    }

    public void CreateNewProject(List<User> users)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    CREATE NEW PROJECT                           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("Enter project name: ");
        string projectName = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(projectName))
        {
            Console.WriteLine("Project name cannot be empty.");
            return;
        }

        Console.Write("Enter project description (optional): ");
        string description = Console.ReadLine() ?? "";

        Console.WriteLine("\nAvailable Users (for project owner):");
        foreach (var user in users)
        {
            Console.WriteLine($"[{user.UserId}] {user.Name} ({user.Role})");
        }

        Console.Write("\nEnter owner user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int ownerId) || !users.Any(u => u.UserId == ownerId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        Console.Write("Enter start date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("Invalid start date.");
            return;
        }

        Console.Write("Enter end date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
        {
            Console.WriteLine("Invalid end date.");
            return;
        }

        if (endDate < startDate)
        {
            Console.WriteLine("End date cannot be before start date.");
            return;
        }

        bool success = _dbService.CreateNewProject(projectName, startDate, endDate);

        if (success)
        {
            Console.WriteLine("\n✓ Project created successfully!");
        }
        else
        {
            Console.WriteLine("\n✗ Failed to create project.");
        }
    }
}
