using Microsoft.Data.SqlClient;
using ProjectTrackingApp.Models;
using TaskModel = ProjectTrackingApp.Models.Task;

namespace ProjectTrackingApp.Data;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=.\\SQLEXPRESS;Database=ProjectTrackingDemo;Persist Security Info=True;User ID=sa;Password=triveni@123;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;";

    // ===== USER QUERIES =====
    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = "SELECT UserId, Name, Email, Role FROM Users ORDER BY Name";
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new User
            {
                UserId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Role = reader.GetString(3)
            });
        }
        return users;
    }

    // ===== PROJECT QUERIES =====
    public List<Project> GetAllProjects()
    {
        var projects = new List<Project>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = "SELECT ProjectId, ProjectName, StartDate, EndDate, Status FROM Projects ORDER BY ProjectName";
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            projects.Add(new Project
            {
                ProjectId = reader.GetInt32(0),
                ProjectName = reader.GetString(1),
                Description = string.Empty,
                OwnerId = 0,
                StartDate = reader.GetDateTime(2),
                EndDate = reader.GetDateTime(3),
                Status = reader.GetString(4),
                CreatedDate = DateTime.MinValue
            });
        }
        return projects;
    }

    public ProjectDetail GetProjectDetails(int projectId)
    {
        ProjectDetail? projectDetail = null;
        
        // Get project info
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = @"
                SELECT p.ProjectId, p.ProjectName, p.StartDate, p.EndDate, p.Status
                FROM Projects p
                WHERE p.ProjectId = @ProjectId";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProjectId", projectId);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                projectDetail = new ProjectDetail
                {
                    ProjectId = reader.GetInt32(0),
                    ProjectName = reader.GetString(1),
                    Description = string.Empty,
                    OwnerId = 0,
                    OwnerName = "Unknown",
                    StartDate = reader.GetDateTime(2),
                    EndDate = reader.GetDateTime(3),
                    Status = reader.GetString(4),
                    CreatedDate = DateTime.MinValue,
                    Tasks = new List<TaskSummary>()
                };
            }
        }

        if (projectDetail == null) return null!;

        // Get tasks for the project
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var taskQuery = @"
                SELECT t.TaskId, t.Title, t.Status, u.Name AS AssignedUserName
                FROM Tasks t
                LEFT JOIN Users u ON t.AssignedUserId = u.UserId
                WHERE t.ProjectId = @ProjectId
                ORDER BY t.CreatedDate DESC";
            
            using var taskCommand = new SqlCommand(taskQuery, connection);
            taskCommand.Parameters.AddWithValue("@ProjectId", projectId);
            using var taskReader = taskCommand.ExecuteReader();
            
            while (taskReader.Read())
            {
                projectDetail.Tasks.Add(new TaskSummary
                {
                    TaskId = taskReader.GetInt32(0),
                    Title = taskReader.GetString(1),
                    Status = taskReader.GetString(2),
                    AssignedUserName = taskReader.IsDBNull(3) ? "Unassigned" : taskReader.GetString(3)
                });
            }
        }

        return projectDetail;
    }

    public List<ProjectWorkloadSummary> GetProjectWorkloadSummary()
    {
        var summaries = new List<ProjectWorkloadSummary>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = @"
            SELECT p.ProjectId, p.ProjectName,
                   COUNT(CASE WHEN t.TaskId IS NOT NULL THEN 1 END) AS TotalTasks,
                   COUNT(CASE WHEN t.Status = 'Completed' THEN 1 END) AS CompletedTasks,
                   COUNT(CASE WHEN t.Status = 'Pending' THEN 1 END) AS PendingTasks,
                   COUNT(CASE WHEN t.Status = 'In Progress' THEN 1 END) AS InProgressTasks
            FROM Projects p
            LEFT JOIN Tasks t ON p.ProjectId = t.ProjectId
            GROUP BY p.ProjectId, p.ProjectName
            ORDER BY p.ProjectName";
        
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            summaries.Add(new ProjectWorkloadSummary
            {
                ProjectId = reader.GetInt32(0),
                ProjectName = reader.GetString(1),
                TotalTasks = reader.GetInt32(2),
                CompletedTasks = reader.GetInt32(3),
                PendingTasks = reader.GetInt32(4),
                InProgressTasks = reader.GetInt32(5)
            });
        }
        return summaries;
    }

    // ===== TASK QUERIES =====
    public List<TaskDetail> GetTasksByUser(int userId)
    {
        var tasks = new List<TaskDetail>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = @"
            SELECT t.TaskId, t.ProjectId, p.ProjectName, t.Title, t.Description, t.Status, 
                   t.AssignedUserId, u.Name AS AssignedUserName, t.CreatedDate
            FROM Tasks t
            JOIN Projects p ON t.ProjectId = p.ProjectId
            JOIN Users u ON t.AssignedUserId = u.UserId
            WHERE t.AssignedUserId = @UserId
            ORDER BY t.CreatedDate DESC";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@UserId", userId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tasks.Add(new TaskDetail
            {
                TaskId = reader.GetInt32(0),
                ProjectId = reader.GetInt32(1),
                ProjectName = reader.GetString(2),
                Title = reader.GetString(3),
                Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Status = reader.GetString(5),
                AssignedUserId = reader.GetInt32(6),
                AssignedUserName = reader.GetString(7),
                CreatedDate = reader.GetDateTime(8)
            });
        }
        return tasks;
    }

    public List<UserWorkload> GetUserWorkload()
    {
        var workloads = new List<UserWorkload>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = @"
            SELECT u.UserId, u.Name, u.Role,
                   COUNT(CASE WHEN t.TaskId IS NOT NULL THEN 1 END) AS TotalAssignedTasks,
                   COUNT(CASE WHEN t.Status = 'Completed' THEN 1 END) AS CompletedTasks,
                   COUNT(CASE WHEN t.Status = 'Pending' THEN 1 END) AS PendingTasks,
                   COUNT(CASE WHEN t.Status = 'In Progress' THEN 1 END) AS InProgressTasks
            FROM Users u
            LEFT JOIN Tasks t ON u.UserId = t.AssignedUserId
            GROUP BY u.UserId, u.Name, u.Role
            ORDER BY u.Name";
        
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            workloads.Add(new UserWorkload
            {
                UserId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Role = reader.GetString(2),
                TotalAssignedTasks = reader.GetInt32(3),
                CompletedTasks = reader.GetInt32(4),
                PendingTasks = reader.GetInt32(5),
                InProgressTasks = reader.GetInt32(6)
            });
        }
        return workloads;
    }

    // ===== CREATE TASK =====
    public bool CreateNewTask(int projectId, string title, string description, int assignedUserId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var query = @"
                INSERT INTO Tasks (ProjectId, Title, Description, Status, AssignedUserId, CreatedDate)
                VALUES (@ProjectId, @Title, @Description, 'Pending', @AssignedUserId, @CreatedDate)";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProjectId", projectId);
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@AssignedUserId", assignedUserId);
            command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
            
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating task: {ex.Message}");
            return false;
        }
    }

    // ===== REASSIGN TASK =====
    public bool ReassignTask(int taskId, int newAssignedUserId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var query = "UPDATE Tasks SET AssignedUserId = @AssignedUserId WHERE TaskId = @TaskId";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TaskId", taskId);
            command.Parameters.AddWithValue("@AssignedUserId", newAssignedUserId);
            
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reassigning task: {ex.Message}");
            return false;
        }
    }

    // ===== MARK TASK AS COMPLETED =====
    public bool MarkTaskAsCompleted(int taskId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var query = "UPDATE Tasks SET Status = 'Completed' WHERE TaskId = @TaskId";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TaskId", taskId);
            
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error marking task as completed: {ex.Message}");
            return false;
        }
    }

    public TaskModel? GetTaskById(int taskId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var query = "SELECT TaskId, ProjectId, Title, Description, Status, DueDate, AssignedUserId, CreatedDate FROM Tasks WHERE TaskId = @TaskId";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@TaskId", taskId);
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            return new TaskModel
            {
                TaskId = reader.GetInt32(0),
                ProjectId = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Status = reader.GetString(4),
                DueDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                AssignedUserId = reader.GetInt32(6),
                CreatedDate = reader.GetDateTime(7)
            };
        }
        return null;
    }
}