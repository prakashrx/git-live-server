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

foreach (var repo in repositoryManager.GetRepositories())
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(repo.LocalPath, repo.Config.WWWRoot ??  "" )),
        RequestPath= repo.Config.RequestPath,
        EnableDefaultFiles = true
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
