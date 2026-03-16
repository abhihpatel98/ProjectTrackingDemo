# Project Tracking Console Application - Complete Creation Prompt

## Overview

Build a functional .NET 8 C# console application for project and task management that connects to a SQL Server database via an MCP (Model Context Protocol) server. The application must dynamically discover the database schema and implement real-world business functionality rather than generic CRUD operations.

---

## Phase 1: Database Schema Discovery

### Requirements

1. **Use MCP tools to inspect the database schema**
   - Call `mcp_sql-mcp-serve_describe_entities` with `nameOnly=false` to get full metadata
   - Identify all tables, columns, data types, and relationships
   - Document primary keys and foreign key relationships
   - **DO NOT assume the schema beforehand** - always retrieve it via MCP

2. **Create a schema discovery document**
   - List all tables and their columns
   - Identify all relationships between tables
   - Note nullable vs non-nullable columns
   - Document actual column names as they exist in the database

3. **Verify schema accuracy**
   - Use `mcp_sql-mcp-serve_read_records` to test each entity
   - Confirm which columns actually exist vs. which are claimed in metadata
   - Handle discrepancies between schema metadata and actual data

---

## Phase 2: Entity Model Generation

### Requirements

Generate C# entity classes based on the actual database schema:

1. **Core Entity Classes** (one file per entity)
   - Properties must match actual database column names exactly
   - Use appropriate C# data types (int, string, DateTime, decimal, etc.)
   - Initialize string properties with `string.Empty`
   - Include XML documentation or clear purpose

2. **Extended/DTO Classes** for complex queries
   - Create `*Detail` classes for entities with joined data
   - Create `*Workload` or `*Summary` classes for aggregate data
   - Example: `ProjectDetail`, `ProjectWorkloadSummary`, `UserWorkload`

3. **Location**
   ```
   Models/
   ├── User.cs (User, UserWorkload)
   ├── Project.cs (Project, ProjectDetail, ProjectWorkloadSummary)
   └── Task.cs (Task, TaskDetail, TaskSummary)
   ```

### Key Constraint
- **Handle schema discrepancies gracefully**
  - If metadata claims columns that don't exist (e.g., DueDate), exclude them
  - Properties should only map to columns confirmed to exist via test queries
  - Initialize unused properties with default values

---

## Phase 3: Data Access Layer

### Implementation Strategy

1. **Technology Stack**
   - Use ADO.NET with `Microsoft.Data.SqlClient`
   - Do NOT use ORM frameworks initially
   - Handle raw SQL queries with parameter binding

2. **DatabaseService Class** 
   Location: `Data/DatabaseService.cs`

   Methods must be organized by functional area:

   **User Operations:**
   - `GetAllUsers()` → List<User>
   - Query: `SELECT UserId, Name, Email, Role FROM Users ORDER BY Name`

   **Project Operations:**
   - `GetAllProjects()` → List<Project>
   - `GetProjectDetails(projectId)` → ProjectDetail
   - `GetProjectWorkloadSummary()` → List<ProjectWorkloadSummary>
   
   **Task Operations:**
   - `GetTasksByUser(userId)` → List<TaskDetail>
   - `GetUserWorkload()` → List<UserWorkload>
   - `CreateNewTask(projectId, title, description, userId)` → bool
   - `ReassignTask(taskId, newUserId)` → bool
   - `MarkTaskAsCompleted(taskId)` → bool
   - `GetTaskById(taskId)` → Task?

3. **SQL Query Patterns**

   **Joins across related tables:**
   ```sql
   SELECT t.*, p.ProjectName, u.Name AS AssignedUserName
   FROM Tasks t
   JOIN Projects p ON t.ProjectId = p.ProjectId
   JOIN Users u ON t.AssignedUserId = u.UserId
   WHERE t.ProjectId = @ProjectId
   ```

   **Aggregation for workload summaries:**
   ```sql
   SELECT u.UserId, u.Name, u.Role,
          COUNT(CASE WHEN t.TaskId IS NOT NULL THEN 1 END) AS TotalTasks,
          COUNT(CASE WHEN t.Status = 'Completed' THEN 1 END) AS CompletedTasks
   FROM Users u
   LEFT JOIN Tasks t ON u.UserId = t.AssignedUserId
   GROUP BY u.UserId, u.Name, u.Role
   ```

   **Parameterized Inserts/Updates:**
   ```sql
   INSERT INTO Tasks (ProjectId, Title, Description, Status, AssignedUserId, CreatedDate)
   VALUES (@ProjectId, @Title, @Description, 'Pending', @AssignedUserId, @CreatedDate)
   ```

4. **Connection Management**
   - Use `using` statements for all SqlConnection, SqlCommand, SqlDataReader
   - Create new connection for each query operation
   - Handle NULL values with `IsDBNull()` checks
   - Never reuse connections across multiple sequential operations

5. **Error Handling**
   - Wrap database operations in try-catch blocks
   - Log exceptions to console with meaningful messages
   - Return default values or null on failure (don't throw to UI)

---

## Phase 4: Service Layer

### Architecture

Organize business logic into service classes by domain:

1. **ProjectService** (`Services/ProjectService.cs`)
   - `DisplayAllProjects()` - Format and display project list
   - `DisplayProjectDetails(projectId)` - Show project with tasks
   - `DisplayProjectWorkloadSummary()` - Task count summary table

2. **TaskService** (`Services/TaskService.cs`)
   - `DisplayTasksByUser(userId)` - List user's assigned tasks
   - `CreateNewTask(projects, users)` - Interactive task creation dialog
   - `ReassignTask(users)` - Interactive task reassignment
   - `MarkTaskAsCompleted()` - Mark task as done

3. **UserService** (`Services/UserService.cs`)
   - `DisplayUserWorkload()` - Workload summary table
   - `GetAllUsers()` - Fetch user list

### Requirements

- Services format data for display using console formatting
- Services handle user input validation
- Services do NOT handle raw database access (delegate to DatabaseService)
- Use box drawing characters for visual hierarchy:
  ```
  ╔════════╗
  ║ TITLE  ║
  ╚════════╝
  
  Table headers with:
  ─────────────
  ```
- Display data in organized tables with aligned columns

---

## Phase 5: Console UI Layer

### Main Menu Structure

Location: `UI/ConsoleMenu.cs` (class: `ConsoleMenu`)

**Menu Options** (derive functionality from discovered schema relationships):

```
1. View all projects
2. View project details (including all tasks)
3. View tasks assigned to a specific user
4. Show project workload summary
5. Show user workload
6. Create a new task for a project
7. Reassign a task to another user
8. Mark a task as completed
9. Exit
```

### Implementation
- Load all projects and users once on startup
- Display interactive menu in loop
- Capture user input and route to appropriate service
- Handle invalid input gracefully
- Show status messages for CRUD operations (✓ success, ✗ failure)
- Refresh data after modifications
- Press-any-key-to-continue pattern between operations

---

## Phase 6: Application Entry Point

### Program.cs

```csharp
using ProjectTrackingApp.Data;
using ProjectTrackingApp.Services;
using ProjectTrackingApp.UI;

var dbService = new DatabaseService();
var projectService = new ProjectService(dbService);
var taskService = new TaskService(dbService);
var userService = new UserService(dbService);

var menu = new ConsoleMenu(dbService, projectService, taskService, userService);
menu.Run();
```

### Requirements
- Instantiate all services with dependency injection pattern
- Pass DatabaseService to all services in constructor
- Start menu loop immediately
- Handle application-level exceptions gracefully

---

## Phase 7: Project Structure

```
ProjectTrackingApp/
├── Program.cs
├── ProjectTrackingApp.csproj
│   └── Packages: Microsoft.Data.SqlClient, System.CommandLine
├── Models/
│   ├── User.cs
│   ├── Project.cs
│   └── Task.cs
├── Data/
│   └── DatabaseService.cs
├── Services/
│   ├── ProjectService.cs
│   ├── TaskService.cs
│   └── UserService.cs
└── UI/
    └── ConsoleMenu.cs
```

---

## Phase 8: Build & Deployment

### Requirements

1. **Target Framework**
   - .NET 10.0
   - Enable implicit using statements
   - Enable nullable reference types

2. **ProjectTrackingApp.csproj**
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFramework>net10.0</TargetFramework>
       <ImplicitUsings>enable</ImplicitUsings>
       <Nullable>enable</Nullable>
     </PropertyGroup>
     <ItemGroup>
       <PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.0" />
       <PackageReference Include="System.CommandLine" Version="2.0.5" />
     </ItemGroup>
   </Project>
   ```

3. **Build Command**
   ```powershell
   dotnet build
   ```

4. **Run Command**
   ```powershell
   dotnet run
   ```

5. **Connection String**
   - Store in DatabaseService as private field
   - Format: SQL Server with encryption and certificate trust for local development
   - Example: `Data Source=.\\SQLEXPRESS;Database=ProjectTrackingDemo;...`

---

## Phase 9: Testing & Verification

### Test Each Feature

1. **Database Connection**
   - Verify MCP tools return schema
   - Test actual data retrieval from each table
   - Confirm no column mismatches

2. **Menu Option 1: View All Projects**
   - Should display formatted list of all projects
   - Show status and date range for each

3. **Menu Option 2: View Project Details**
   - Ask for project ID
   - Display project info
   - List all tasks in that project with status and assignment

4. **Menu Option 3: Tasks by User**
   - List all users
   - Ask for user ID
   - Display tasks assigned to that user with project context

5. **Menu Option 4: Project Workload**
   - Show table with task counts by status per project
   - Verify aggregation logic (total, completed, pending, in-progress)

6. **Menu Option 5: User Workload**
   - Show table with task counts by status per user
   - Include user role

7. **Menu Option 6: Create Task**
   - Interactive form for project, title, description, assignee
   - Validate inputs
   - Insert into database
   - Refresh data

8. **Menu Option 7: Reassign Task**
   - Ask for task ID
   - Show current assignment
   - Allow selection of new assignee
   - Update database

9. **Menu Option 8: Mark Complete**
   - Ask for task ID
   - Show current status
   - Update Status to 'Completed'
   - Verify update

10. **Compilation**
    - `dotnet build` must succeed with 0 errors
    - No compiler warnings for naming or patterns
    - All namespaces properly organized

---

## Critical Implementation Notes

### Schema Discovery Best Practices
- **Never hardcode column names** - always discover via MCP first
- Test discovered columns with sample queries before building queries
- Handle `NULL` database values gracefully in models
- If metadata columns don't exist in actual data, exclude them from models

### Database Connection
- Use separate `SqlConnection` for each logical operation
- Always wrap connections in `using` for proper disposal
- Check `IsDBNull()` before reading nullable columns
- Use parameterized queries exclusively (@paramName)

### Error Handling
- Catch `SqlException` specifically for database errors
- Log errors to console with context
- Allow operations to fail gracefully without crashing app
- Provide user-friendly error messages

### Code Organization
- Group related methods with comments: `// ===== SECTION NAME =====`
- Keep DatabaseService methods small and single-purpose
- Services format output, don't query database directly
- UI layer doesn't call database - only calls services

### Console Formatting
- Use UTF-8 box-drawing characters for visual separation
- Align table columns consistently
- Use empty lines to separate sections
- Show success (✓) and failure (✗) indicators

---

## Expected Output Examples

### Project List
```
╔════════════════════════════════════════════════════════════════╗
║                        ALL PROJECTS                            ║
╚════════════════════════════════════════════════════════════════╝

[1] AI MCP Demo Application
    Status: Active | Start: 2024-01-10 | End: 2024-06-30
```

### User Workload Table
```
╔════════════════════════════════════════════════════════════════╗
║                    USER WORKLOAD SUMMARY                       ║
╚════════════════════════════════════════════════════════════════╝

User Name              Role            Total  Done  In Prog  Pending
───────────────────────────────────────────────────────────────────
Alice Johnson          Manager         5      2     1        2
Bob Smith              Developer       3      1     2        0
```

---

## Success Criteria

- [x] Application compiles without errors (`dotnet build` succeeds)
- [x] Application starts without exceptions (`dotnet run` works)
- [x] All 9 menu options function without errors
- [x] Database queries match actual schema (no invalid column errors)
- [x] User can view projects, tasks, and workload
- [x] User can create, reassign, and complete tasks
- [x] All data displays in formatted, readable tables
- [x] No hardcoded assumptions about schema
- [x] Proper connection and resource management
- [x] Clear separation of concerns (Data/Services/UI)

---

## Future Enhancements (Not Required)

- Export data to CSV
- Filter tasks by status
- Advanced search/filtering
- Task priority levels
- Project completion percentage
- Dapper ORM integration
- Configuration file for connection string
- Unit tests
- Logging framework
- Web API wrapper

---

## References

- MCP SQL Tools: Use `describe_entities`, `read_records`, `create_record`, `update_record`, `delete_record`
- ADO.NET: `SqlConnection`, `SqlCommand`, `SqlDataReader`
- .NET 10: C# nullable reference types, implicit using statements, latest language features
- Best Practices: Dependency injection, separation of concerns, parameterized queries
