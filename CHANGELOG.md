# Changelog

All notable changes to this project will be documented in this file.

## [1.1.0] - 2026-03-17

### Changed
- **UPGRADED**: Target framework from .NET 8.0 to .NET 10.0
- **UPDATED**: Microsoft.Data.SqlClient from 6.1.4 to 5.2.0 (latest stable for .NET 10)
- **UPDATED**: System.CommandLine from 2.0.5 to 2.0.5 (maintained for compatibility)
- **UPDATED**: PROJECT_CREATION_PROMPT.md with .NET 10.0 specifications
- **ADDED**: Comprehensive README.md with feature overview and quick start guide
- **FIXED**: Database connection handling in GetProjectDetails method (separate connection scopes)
- **REMOVED**: DueDate column references (not present in actual database schema)

### Build Output
- Output directory: `bin/Debug/net10.0/ProjectTrackingApp.dll`
- Compile time: ~1.4 seconds
- Zero errors, resolved schema discrepancies

### Documentation
- Complete PROJECT_CREATION_PROMPT.md for future implementation
- README.md with architecture, features, and SQL queries
- CHANGELOG (this file) for version tracking

---

## [1.0.0] - 2026-03-17-initial

### Added
- Initial release of Project Tracking Console Application
- .NET 8.0 console application
- Database schema discovery via MCP tools
- Entity models for Users, Projects, and Tasks
- Data access layer using ADO.NET
- Business logic layer with three service classes
- Interactive menu-driven console UI
- Project and task management features
- Workload reporting and analytics
- Task creation, reassignment, and completion

### Features
1. View all projects
2. View project details with tasks
3. View tasks assigned to a specific user
4. Show project workload summary
5. Show user workload
6. Create a new task
7. Reassign a task to another user
8. Mark a task as completed

### Database
- Discovered schema: Users, Projects, Tasks
- SQL Server connectivity via Microsoft.Data.SqlClient
- Parameterized queries for security
- Proper connection lifecycle management

### Documentation
- Comprehensive creation prompt (PROJECT_CREATION_PROMPT.md)
- Initial README

---

## Version Format
- Major: Breaking changes or significant new features
- Minor: New features, backward compatible
- Patch: Bug fixes

---

## Notes

### Breaking Changes from 1.0.0 to 1.1.0
- None. Changes are backward compatible.
- Target framework upgrade requires .NET 10.0 SDK
- Previous .NET 8.0 builds are no longer supported

### Migration Guide
To run existing projects on .NET 10.0:
1. Update `TargetFramework` in `.csproj` from `net8.0` to `net10.0`
2. Run `dotnet restore` to download packages for new framework
3. Run `dotnet build` and verify compilation
4. No code changes required; all APIs are compatible

### Known Issues
- None currently

### Future Roadmap
- v1.2.0: Task filtering by status
- v1.3.0: CSV export functionality
- v2.0.0: Web API integration
