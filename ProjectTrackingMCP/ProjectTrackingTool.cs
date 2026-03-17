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
}

// ===== DTO CLASSES =====

public class ProjectActionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}