using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTrackingApp.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSingleton<DatabaseService>()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly() // Scan for mcp tools
    .WithResourcesFromAssembly() // Scan for mcp resources
    .WithPromptsFromAssembly(); // Scan for mcp prompts
  
await builder.Build().RunAsync();