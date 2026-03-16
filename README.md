# Project Tracking Console Application

A functional .NET 10.0 console application for managing projects, tasks, and user workloads. The application connects to SQL Server via ADO.NET and provides an interactive menu interface for project and task management.

## Quick Start

### Prerequisites
- .NET 10.0 SDK
- SQL Server (SQLEXPRESS or higher)
- ProjectTrackingDemo database

### Build
```powershell
dotnet build
```

### Run
```powershell
dotnet run
```

## Features

### Menu Options
1. **View all projects** - Display all projects with status and dates
2. **View project details** - Show project with all associated tasks
3. **View tasks by user** - Filter tasks by assigned user
4. **Show project workload** - Task count summary per project (by status)
5. **Show user workload** - Task count summary per user (by status)
6. **Create a new task** - Interactive task creation form
7. **Reassign a task** - Move task to a different user
8. **Mark a task as completed** - Update task status to completed
9. **Exit** - Close the application

## Architecture

### Project Structure
```
ProjectTrackingApp/
├── Program.cs                  # Application entry point
├── ProjectTrackingApp.csproj   # Project configuration
│
├── Models/                     # Entity classes
│   ├── User.cs                # User and UserWorkload classes
│   ├── Project.cs             # Project, ProjectDetail, ProjectWorkloadSummary
│   └── Task.cs                # Task, TaskDetail, TaskSummary
│
├── Data/                       # Data Access Layer
│   └── DatabaseService.cs     # ADO.NET database operations
│
├── Services/                   # Business Logic Layer
│   ├── ProjectService.cs      # Project-related operations
│   ├── TaskService.cs         # Task-related operations
│   └── UserService.cs         # User-related operations
│
├── UI/                         # User Interface Layer
│   └── ConsoleMenu.cs         # Main console menu and interaction
│
└── Documentation/
    ├── PROJECT_CREATION_PROMPT.md   # Comprehensive creation guide
    └── README.md                    # This file
```

### Layers

**Data Access Layer (`Data/DatabaseService.cs`)**
- Handles all SQL Server operations using ADO.NET
- Methods organized by functional area (Users, Projects, Tasks)
- Proper connection lifecycle management with `using` statements
- Parameterized queries to prevent SQL injection

**Service Layer (`Services/*`)**
- ProjectService: Project display and workload summary
- TaskService: Task management and user interaction
- UserService: User workload and listing
- No direct database access; delegates to DatabaseService

**UI Layer (`UI/ConsoleMenu.cs`)**
- Interactive menu loop
- User input validation and routing
- Data display formatting with box-drawing characters
- Status feedback for operations

## Database Schema

Discovered via MCP tools. Actual database includes:

### Users Table
```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY,
    Name NVARCHAR(MAX),
    Email NVARCHAR(MAX),
    Role NVARCHAR(MAX)
)
```

### Projects Table
```sql
CREATE TABLE Projects (
    ProjectId INT PRIMARY KEY,
    ProjectName NVARCHAR(MAX),
    StartDate DATETIME2,
    EndDate DATETIME2,
    Status NVARCHAR(MAX)
)
```

### Tasks Table
```sql
CREATE TABLE Tasks (
    TaskId INT PRIMARY KEY,
    ProjectId INT FOREIGN KEY REFERENCES Projects,
    Title NVARCHAR(MAX),
    Description NVARCHAR(MAX),
    Status NVARCHAR(MAX),
    AssignedUserId INT FOREIGN KEY REFERENCES Users,
    CreatedDate DATETIME2
)
```

## Technologies

- **Language**: C# (.NET 10.0)
- **Database**: SQL Server (via Microsoft.Data.SqlClient 5.2.0)
- **Data Access**: ADO.NET
- **UI**: Console application with ANSI character formatting

## Configuration

### Connection String
Located in `Data/DatabaseService.cs`:
```csharp
private readonly string _connectionString = "Data Source=.\\SQLEXPRESS;Database=ProjectTrackingDemo;...";
```

Update the connection string to match your SQL Server instance and credentials.

## Key SQL Queries

### List All Projects
```sql
SELECT ProjectId, ProjectName, StartDate, EndDate, Status 
FROM Projects 
ORDER BY ProjectName
```

### Get Tasks by User
```sql
SELECT t.TaskId, t.ProjectId, p.ProjectName, t.Title, t.Description, t.Status, 
       t.AssignedUserId, u.Name AS AssignedUserName, t.CreatedDate
FROM Tasks t
JOIN Projects p ON t.ProjectId = p.ProjectId
JOIN Users u ON t.AssignedUserId = u.UserId
WHERE t.AssignedUserId = @UserId
ORDER BY t.CreatedDate DESC
```

### Project Workload Summary
```sql
SELECT p.ProjectId, p.ProjectName,
       COUNT(CASE WHEN t.TaskId IS NOT NULL THEN 1 END) AS TotalTasks,
       COUNT(CASE WHEN t.Status = 'Completed' THEN 1 END) AS CompletedTasks,
       COUNT(CASE WHEN t.Status = 'Pending' THEN 1 END) AS PendingTasks,
       COUNT(CASE WHEN t.Status = 'In Progress' THEN 1 END) AS InProgressTasks
FROM Projects p
LEFT JOIN Tasks t ON p.ProjectId = t.ProjectId
GROUP BY p.ProjectId, p.ProjectName
ORDER BY p.ProjectName
```

### User Workload Summary
```sql
SELECT u.UserId, u.Name, u.Role,
       COUNT(CASE WHEN t.TaskId IS NOT NULL THEN 1 END) AS TotalAssignedTasks,
       COUNT(CASE WHEN t.Status = 'Completed' THEN 1 END) AS CompletedTasks,
       COUNT(CASE WHEN t.Status = 'Pending' THEN 1 END) AS PendingTasks,
       COUNT(CASE WHEN t.Status = 'In Progress' THEN 1 END) AS InProgressTasks
FROM Users u
LEFT JOIN Tasks t ON u.UserId = t.AssignedUserId
GROUP BY u.UserId, u.Name, u.Role
ORDER BY u.Name
```

## Sample Console Output

### Main Menu
```
╔════════════════════════════════════════════════════════════════╗
║        PROJECT TRACKING CONSOLE APPLICATION                    ║
╚════════════════════════════════════════════════════════════════╝

1. View all projects
2. View project details (including all tasks)
3. View tasks assigned to a specific user
4. Show project workload summary
5. Show user workload
6. Create a new task for a project
7. Reassign a task to another user
8. Mark a task as completed
9. Exit

Choose an option:
```

### Project Details View
```
╔════════════════════════════════════════════════════════════════╗
║  AI MCP Demo Application
╚════════════════════════════════════════════════════════════════╝

Status: Active
Period: 2024-01-10 to 2024-06-30

Tasks (3):
───────────────────────────────────────────────────
  [1] Design Database Schema
       Status: Completed | Assigned to: Alice Johnson
  [2] Implement MCP Tools
       Status: In Progress | Assigned to: Bob Smith
  [3] Write Unit Tests
       Status: Pending | Assigned to: Diana Prince
```

### Workload Summary
```
╔════════════════════════════════════════════════════════════════╗
║                    USER WORKLOAD SUMMARY                       ║
╚════════════════════════════════════════════════════════════════╝

User Name              Role            Total  Done  In Prog  Pending
───────────────────────────────────────────────────────────────────
Alice Johnson          Manager         5      2     1        2
Bob Smith              Developer       3      1     2        0
Charlie Brown          Developer       2      1     1        0
Diana Prince           Tester          2      1     0        1
```

## Error Handling

- Database connection errors are caught and logged to console
- Invalid user input is validated and re-prompted
- Failed operations show error messages without crashing
- Proper resource cleanup with `using` statements

## Build Output

- Target: `bin/Debug/net10.0/ProjectTrackingApp.dll`
- Executable: `bin/Debug/net10.0/ProjectTrackingApp.exe`

## Dependencies

- **Microsoft.Data.SqlClient** (5.2.0) - SQL Server connectivity
- **System.CommandLine** (2.0.5) - Optional command-line parsing (included in csproj but not used in current version)

## Future Enhancements

- Filter tasks by status
- Export data to CSV/Excel
- Task priority levels
- Project completion percentage tracking
- Web API wrapper
- Unit tests
- Dapper ORM integration
- Configuration file for connection string
- Logging framework

## Notes

- Connection string contains sensitive credentials - should be moved to configuration/secrets in production
- Application loads all data into memory on startup
- Console works best with 120+ character width for table display

## Version History

- **v1.0.0** - Initial release
  - .NET 10.0
  - ADO.NET data access
  - 9-option menu system
  - Project and task management
  - Workload reporting

## Documentation

See [PROJECT_CREATION_PROMPT.md](PROJECT_CREATION_PROMPT.md) for detailed creation guide and implementation specifications.
