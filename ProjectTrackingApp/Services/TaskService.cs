using ProjectTrackingApp.Data;
using ProjectTrackingApp.Models;

namespace ProjectTrackingApp.Services;

public class TaskService
{
    private readonly DatabaseService _dbService;

    public TaskService(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public void DisplayTasksByUser(int userId)
    {
        var tasks = _dbService.GetTasksByUser(userId);
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks assigned to this user.");
            return;
        }

        var user = tasks.FirstOrDefault();
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  Tasks Assigned to {user?.AssignedUserName}");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        foreach (var task in tasks)
        {
            Console.WriteLine($"[{task.TaskId}] {task.Title}");
            Console.WriteLine($"    Project: {task.ProjectName}");
            Console.WriteLine($"    Status: {task.Status} | Created: {task.CreatedDate:yyyy-MM-dd}");
            if (!string.IsNullOrEmpty(task.Description))
            {
                Console.WriteLine($"    Description: {task.Description}");
            }
            Console.WriteLine();
        }
    }

    public void CreateNewTask(List<Project> projects, List<User> users)
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    CREATE NEW TASK                              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Select project
        Console.WriteLine("Available Projects:");
        foreach (var proj in projects)
        {
            Console.WriteLine($"  [{proj.ProjectId}] {proj.ProjectName}");
        }
        Console.Write("Enter Project ID: ");
        if (!int.TryParse(Console.ReadLine(), out int projectId) || !projects.Any(p => p.ProjectId == projectId))
        {
            Console.WriteLine("Invalid project ID.");
            return;
        }

        // Enter task details
        Console.Write("Enter Task Title: ");
        string title = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Enter Task Description: ");
        string description = Console.ReadLine() ?? string.Empty;

        // Select assigned user
        Console.WriteLine("\nAvailable Users:");
        foreach (var user in users)
        {
            Console.WriteLine($"  [{user.UserId}] {user.Name} ({user.Role})");
        }
        Console.Write("Enter User ID to Assign: ");
        if (!int.TryParse(Console.ReadLine(), out int userId) || !users.Any(u => u.UserId == userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        // Create task
        if (_dbService.CreateNewTask(projectId, title, description, userId))
        {
            Console.WriteLine("\n✓ Task created successfully!");
        }
        else
        {
            Console.WriteLine("\n✗ Error creating task.");
        }
    }

    public void ReassignTask(List<User> users)
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    REASSIGN TASK                               ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("Enter Task ID: ");
        if (!int.TryParse(Console.ReadLine(), out int taskId))
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }

        var task = _dbService.GetTaskById(taskId);
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        Console.WriteLine($"\nTask: {task.Title}");
        Console.WriteLine($"Current Assignment: User ID {task.AssignedUserId}");
        Console.WriteLine();

        Console.WriteLine("Available Users:");
        foreach (var user in users)
        {
            Console.WriteLine($"  [{user.UserId}] {user.Name} ({user.Role})");
        }
        
        Console.Write("Enter New User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int newUserId) || !users.Any(u => u.UserId == newUserId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        if (_dbService.ReassignTask(taskId, newUserId))
        {
            Console.WriteLine("\n✓ Task reassigned successfully!");
        }
        else
        {
            Console.WriteLine("\n✗ Error reassigning task.");
        }
    }

    public void MarkTaskAsCompleted()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                MARK TASK AS COMPLETED                          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("Enter Task ID: ");
        if (!int.TryParse(Console.ReadLine(), out int taskId))
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }

        var task = _dbService.GetTaskById(taskId);
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        Console.WriteLine($"\nTask: {task.Title}");
        Console.WriteLine($"Current Status: {task.Status}");
        Console.Write("Mark as completed? (y/n): ");

        if (Console.ReadLine()?.ToLower() == "y")
        {
            if (_dbService.MarkTaskAsCompleted(taskId))
            {
                Console.WriteLine("\n✓ Task marked as completed!");
            }
            else
            {
                Console.WriteLine("\n✗ Error updating task.");
            }
        }
    }
}
