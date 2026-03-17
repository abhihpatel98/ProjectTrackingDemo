
using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerPromptType]
public class ProjectTrackingPrompt
{
    [McpServerPrompt(Name = "get_active_projects_from_database")]
    [Description("Get all the active projects from the database and display them as a table in the chat.")]
    public string GetActiveProjectsFromDatabase() => 
        "Get all the active project from database and show them as a table in the chat.";

    [McpServerPrompt(Name = "get_project_details_from_database")]
    [Description("Get the details of a specific project and its associated tasks from the database and display them as a table in the chat.")]
    public string GetProjectDetailsFromDatabase(string projectName) => 
        $"Get the details of the project {projectName} and associated tasks from database and show them as a table in the chat.";
}