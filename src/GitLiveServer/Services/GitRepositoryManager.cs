using GitLiveServer.Config;
using Microsoft.Extensions.Options;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace GitLiveServer.Services
{
    public class GitRepositoryManager
    {
        private readonly IHostingEnvironment env;
        private readonly List<GitRepositoryConfig> gitRepoConfigs;
        private List<GitRepository> repositories;

        public GitRepositoryManager(IHostingEnvironment env, IOptions<List<GitRepositoryConfig>> gitRepoConfigs)
        {
            this.gitRepoConfigs = gitRepoConfigs.Value;
            this.env = env;
        }

        public void Initialize()
        {
            if (repositories != null)
                return;
            var rootDir = Path.Combine(env.ContentRootPath, "Repositories");

            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);

            repositories = new List<GitRepository>();
            foreach (var config in gitRepoConfigs)
            {
                var localPath = Path.Combine(rootDir, config.Name);
                var repo = new GitRepository(config, localPath);
                repo.Initialize();
                repositories.Add(repo);
            }
        }

        public IEnumerable<GitRepository> GetRepositories() {

            return repositories;
        }

        public void FetchLatest()
        {
            foreach (var repo in repositories)
            {
                repo.Fetch();
                if(repo.HasChanges())
                {
                    repo.Reset();
                }
            }
        }
    }
}
