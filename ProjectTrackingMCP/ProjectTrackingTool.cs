using System.ComponentModel;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;
using ProjectTrackingApp.Data;

[McpServerToolType]
public class ProjectTrackingTool
{
    private readonly DatabaseService _dbService;

    public ProjectTrackingTool(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    // ===== PROJECT OPERATIONS =====

    [McpServerTool(Name = "create_project")]
    [Description("Create a new project")]
    public ProjectActionResponse CreateProject(
        [Description("Project name")] string projectName,
        [Description("Project start date (yyyy-MM-dd format)")] string startDate,
        [Description("Project end date (yyyy-MM-dd format)")] string endDate,
        [Description("Project status (e.g., Active, On Hold, Completed). Default is Active")] string status = "Active")
    {
        try
        {
            if (!DateTime.TryParse(startDate, out var parsedStartDate))
            {
                return new ProjectActionResponse
                {
                    Success = false,
                    Message = "Invalid start date format. Please use yyyy-MM-dd"
                };
            }

            if (!DateTime.TryParse(endDate, out var parsedEndDate))
            {
                return new ProjectActionResponse
                {
                    Success = false,
                    Message = "Invalid end date format. Please use yyyy-MM-dd"
                };
            }

            if (parsedStartDate >= parsedEndDate)
            {
                return new ProjectActionResponse
                {
                    Success = false,
                    Message = "Start date must be before end date"
                };
            }

            var success = _dbService.CreateNewProject(projectName, parsedStartDate, parsedEndDate, status);

            return new ProjectActionResponse
            {
                Success = success,
                Message = success
                    ? $"Project '{projectName}' created successfully"
                    : "Failed to create project"
            };
        }
        catch (Exception ex)
        {
            return new ProjectActionResponse
            {
                Success = false,
                Message = $"Error creating project: {ex.Message}"
            };
        }
    }

    // ===== TASK OPERATIONS =====

    [McpServerTool(Name = "create_task")]
    [Description("Create a new task in a project")]
    public ProjectActionResponse CreateTask(
        [Description("Project ID")] int projectId,
        [Description("Task title")] string title,
        [Description("Task description")] string description,
        [Description("User ID to assign the task to")] int assignedUserId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return new ProjectActionResponse
                {
                    Success = false,
                    Message = "Task title cannot be empty"
                };
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return new ProjectActionResponse
                {
                    Success = false,
                    Message = "Task description cannot be empty"
                };
            }

            var success = _dbService.CreateNewTask(projectId, title, description, assignedUserId);

            return new ProjectActionResponse
            {
                Success = success,
                Message = success
                    ? $"Task '{title}' created successfully in project {projectId}"
                    : "Failed to create task"
            };
        }
        catch (Exception ex)
        {
            return new ProjectActionResponse
            {
                Success = false,
                Message = $"Error creating task: {ex.Message}"
            };
        }
    }
}

// ===== DTO CLASSES =====

public class ProjectActionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}