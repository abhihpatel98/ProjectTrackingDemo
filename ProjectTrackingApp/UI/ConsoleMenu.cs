using ProjectTrackingApp.Data;
using ProjectTrackingApp.Models;
using ProjectTrackingApp.Services;

namespace ProjectTrackingApp.UI;

public class ConsoleMenu
{
    private readonly DatabaseService _dbService;
    private readonly ProjectService _projectService;
    private readonly TaskService _taskService;
    private readonly UserService _userService;
    private List<Project> _projects = new();
    private List<User> _users = new();

    public ConsoleMenu(DatabaseService dbService, ProjectService projectService, TaskService taskService, UserService userService)
    {
        _dbService = dbService;
        _projectService = projectService;
        _taskService = taskService;
        _userService = userService;
    }

    public void Run()
    {
        LoadData();
        
        while (true)
        {
            DisplayMainMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllProjects();
                    break;
                case "2":
                    ViewProjectDetails();
                    break;
                case "3":
                    ViewTasksByUser();
                    break;
                case "4":
                    ShowProjectWorkload();
                    break;
                case "5":
                    ShowUserWorkload();
                    break;
                case "6":
                    CreateNewTask();
                    break;
                case "7":
                    ReassignTask();
                    break;
                case "8":
                    MarkTaskCompleted();
                    break;
                case "9":
                    CreateNewProject();
                    break;
                case "10":
                    Console.WriteLine("\nGoodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private void LoadData()
    {
        Console.WriteLine("Loading data from database...");
        _projects = _dbService.GetAllProjects();
        _users = _userService.GetAllUsers();
        Console.WriteLine($"Loaded {_projects.Count} projects and {_users.Count} users.");
        Thread.Sleep(1000);
    }

    private void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        PROJECT TRACKING CONSOLE APPLICATION                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("1. View all projects");
        Console.WriteLine("2. View project details (including all tasks)");
        Console.WriteLine("3. View tasks assigned to a specific user");
        Console.WriteLine("4. Show project workload summary");
        Console.WriteLine("5. Show user workload");
        Console.WriteLine("6. Create a new task for a project");
        Console.WriteLine("7. Reassign a task to another user");
        Console.WriteLine("8. Mark a task as completed");
        Console.WriteLine("9. Create a new project");
        Console.WriteLine("10. Exit");
        Console.WriteLine();
        Console.Write("Choose an option: ");
    }

    private void ViewAllProjects()
    {
        _projectService.DisplayAllProjects();
    }

    private void ViewProjectDetails()
    {
        if (_projects.Count == 0)
        {
            Console.WriteLine("No projects available.");
            return;
        }

        Console.Clear();
        Console.WriteLine("Available Projects:");
        foreach (var project in _projects)
        {
            Console.WriteLine($"[{project.ProjectId}] {project.ProjectName}");
        }

        Console.Write("\nEnter Project ID: ");
        if (int.TryParse(Console.ReadLine(), out int projectId))
        {
            _projectService.DisplayProjectDetails(projectId);
        }
        else
        {
            Console.WriteLine("Invalid project ID.");
        }
    }

    private void ViewTasksByUser()
    {
        if (_users.Count == 0)
        {
            Console.WriteLine("No users available.");
            return;
        }

        Console.Clear();
        Console.WriteLine("Available Users:");
        foreach (var user in _users)
        {
            Console.WriteLine($"[{user.UserId}] {user.Name} ({user.Role})");
        }

        Console.Write("\nEnter User ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            _taskService.DisplayTasksByUser(userId);
        }
        else
        {
            Console.WriteLine("Invalid user ID.");
        }
    }

    private void ShowProjectWorkload()
    {
        _projectService.DisplayProjectWorkloadSummary();
    }

    private void ShowUserWorkload()
    {
        _userService.DisplayUserWorkload();
    }

    private void CreateNewTask()
    {
        if (_projects.Count == 0 || _users.Count == 0)
        {
            Console.WriteLine("Projects and users are required to create a task.");
            return;
        }

        _taskService.CreateNewTask(_projects, _users);
        LoadData(); // Refresh data
    }

    private void ReassignTask()
    {
        if (_users.Count == 0)
        {
            Console.WriteLine("No users available.");
            return;
        }

        _taskService.ReassignTask(_users);
    }

    private void MarkTaskCompleted()
    {
        _taskService.MarkTaskAsCompleted();
    }

    private void CreateNewProject()
    {
        if (_users.Count == 0)
        {
            Console.WriteLine("At least one user is required to create a project.");
            return;
        }

        _projectService.CreateNewProject(_users);
        LoadData(); // Refresh data
    }
}
