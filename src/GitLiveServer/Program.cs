using GitLiveServer.Config;
using GitLiveServer.Services;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<GitRepositoryManager>();
builder.Services.AddHostedService<RepositoryUpdateService>();
builder.Services.Configure<List<GitRepositoryConfig>>(builder.Configuration.GetSection("Repositories"));
var app = builder.Build();


var repositoryManager = app.Services.GetRequiredService<GitRepositoryManager>();
repositoryManager.Initialize();

// Configure the HTTP request pipeline.
//app.UseSwagger();
//app.UseSwaggerUI();

bool isRootPathTaken = false;
foreach (var repo in repositoryManager.GetRepositories())
{
    var requestPath = repo.Config.RequestPath;
    if (string.IsNullOrEmpty(requestPath) || requestPath == "/" || requestPath == "\\")
    {
        requestPath = null;
        if (isRootPathTaken)
            requestPath = $"/{repo.Config.Name}";
    }
    else if (!requestPath.StartsWith("/"))
    {
        requestPath = "/" + requestPath;
    }

    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(repo.LocalPath, repo.Config.wwwRoot ??  "" )),
        RequestPath= requestPath,
        EnableDefaultFiles = true
    });

    if (string.IsNullOrEmpty(requestPath))
        isRootPathTaken = true;
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
