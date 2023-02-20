using GitLiveServer.Config;
using GitLiveServer.Services;
using LibGit2Sharp;
using Microsoft.Extensions.Options;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

public class RepositoryUpdateService : BackgroundService
{
    private readonly int _pullIntervalSeconds = 10;
    private readonly GitRepositoryManager repositoryManager;

    public RepositoryUpdateService(GitRepositoryManager repositoryManager)
    {
        this.repositoryManager = repositoryManager;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                this.repositoryManager.FetchLatest();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Wait for the specified interval before pulling again
            await Task.Delay(TimeSpan.FromSeconds(_pullIntervalSeconds), stoppingToken);
        }
    }
}