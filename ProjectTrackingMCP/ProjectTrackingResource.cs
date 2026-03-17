
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;
using ProjectTrackingApp.Data;

[McpServerResourceType]
public class ProjectTrackingResource
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly DatabaseService _dbService;

    public ProjectTrackingResource(DatabaseService dbService)
    {
        _dbService = dbService;
    }
    
    [McpServerResource(Name = "getAllProjects", MimeType = "application/json")]
    [Description("Get all projects")]
    public string GetAllProjects()
    {
        try
        {
            var projects = _dbService.GetAllProjects();

            var result = new GetProjectsResponse
            {
                Success = true,
                Message = $"Retrieved {projects.Count} project(s)",
                Result = projects
            };

            return JsonSerializer.Serialize(result, _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            var result = new GetProjectsResponse
            {
                Success = false,
                Message = $"Error retrieving projects: {ex.Message}",
                Result = new()
            };

            return JsonSerializer.Serialize(result, _jsonSerializerOptions);
        }
    }
}

public class GetProjectsResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("projects")]
    public object Result { get; set; } = new();
}