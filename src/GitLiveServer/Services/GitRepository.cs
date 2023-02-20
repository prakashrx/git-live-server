using GitLiveServer.Config;
using LibGit2Sharp;

namespace GitLiveServer.Services
{
    public class GitRepository
    {
        private readonly GitRepositoryConfig config;
        private readonly string localPath;
        private readonly Credentials credentials;
        private Repository repo;
        public string LocalPath => localPath;
        public GitRepositoryConfig Config => config;
        public GitRepository(GitRepositoryConfig config, string localPath = null) {

            this.config = config;
            this.localPath = localPath ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
            {
                credentials = new UsernamePasswordCredentials
                {
                    Username = config.Username,
                    Password = config.Password
                };
            }
        }

        public void Initialize()
        {
            if (!Repository.IsValid(localPath))
            {
                Clone();
            }
            else
            {
                Fetch();
                Reset();
            }
        }

        public void Clone()
        {
            var cloneOptions = new CloneOptions
            {
                BranchName = config.BranchName ?? "master",
                CredentialsProvider = (_url, _usernameFromUrl, _types) => credentials
            };

            Repository.Clone(config.RepoUrl, localPath, cloneOptions);
        }

        public void Fetch()
        {
            using (var repo = new Repository(localPath))
            {
                var remote = repo.Network.Remotes["origin"];
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                var fetchOptions = new FetchOptions
                {
                    CredentialsProvider = (_url, _usernameFromUrl, _types) => credentials
                };

                // Fetch the latest changes
                Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, null);
            }
        }

        public void Reset()
        {
            using (var repo = new Repository(localPath))
            {
                var trackedBranch = repo.Head.TrackedBranch;
                repo.Reset(ResetMode.Hard, trackedBranch.Tip);
            }
        }

        public bool HasChanges()
        {
            using (var repo = new Repository(localPath))
            {
                var localCommit = repo.Head.Tip;
                var remoteCommit = repo.Lookup<Commit>("origin/" + repo.Head.FriendlyName);

                return localCommit.Sha != remoteCommit.Sha;
            }
        }
    }
}
