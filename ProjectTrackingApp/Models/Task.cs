namespace ProjectTrackingApp.Models;

public class Task
{
    public int TaskId { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int AssignedUserId { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class TaskDetail
{
    public int TaskId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AssignedUserId { get; set; }
    public string AssignedUserName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class TaskSummary
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AssignedUserName { get; set; } = string.Empty;
}