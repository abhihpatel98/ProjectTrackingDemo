using ProjectTrackingApp.Data;
using ProjectTrackingApp.Services;
using ProjectTrackingApp.UI;

var dbService = new DatabaseService();
var projectService = new ProjectService(dbService);
var taskService = new TaskService(dbService);
var userService = new UserService(dbService);

var menu = new ConsoleMenu(dbService, projectService, taskService, userService);
menu.Run();