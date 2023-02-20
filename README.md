# Git Live Server

A standalone dotnet core-based service that can serve static html files and contents in any Public/Private Git Repository. Functionally, It is very similar to GitHub Pages, but it can work as a stand alone service that can serve any git repo within a private network.

#### Demo

Check out the [Live Demo](https://git-live-server.azurewebsites.net/) 

The demo page is serving the static files from the **pages** branch in this repo. [pages branch](https://github.com/prakashrx/git-live-server/tree/pages)


##  Features
- Serve static files from a remote/local Git Repository
- Configure Multiple Git Repositories
- Pull the latest changes as and when it is available. 
- Easy deploy with docker container


## Configuration

Git repositories that needs to be served must be configured in `appsettings.json`.

A sample  config would look like this where **Repositories** is an array of git repo configurations.

```json
{
  "Repositories": [
    {
      "Name": "git-live-server-pages",
      "RepoUrl": "https://github.com/prakashrx/git-live-server.git",
      "BranchName": "pages",
      "WWWRoot":  "dist"
    }
  ],
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Here is a full set of configuration options for a git Repo

Field | Required | Default | Desciption
--- | --- |--- | ---
Name | `yes` | | The name of the Repo. This must be a unique value|
RepoUrl | `yes` | | The Git Repo url|
BranchName | `no` | `master`| The branch name (if different from *master*)|
WWWRoot | `no` | root directory | The relative path within the repo that needs to be served|
RequestPath| `no` | \ | The Url Path to serve the files. If there is already a repo served at root, this would default to `\{Name}`|
Username | `no` | | The username required to Authenticate (if required)|
Password | `no` | | The password required to Authenticate (if required)|


## Deploy

Deploy instantly Using *git-live-server* image from docker hub. The following commands create and mounts a empty volume to `/app/Repository` path and serves the page at [http://localhost](http://localhost)

```sh
docker run -v repositories:/app/Repositories -p 80:80 prakashrx/git-live-server:latest
```

Alternatively, you can create the image from the repo and run with docker-compose

```sh
git clone https://github.com/prakashrx/git-live-server.git
cd git-live-server
docker-compose up
```

## Develop

#### Requirements

- Visual Studio Code / Visual Studio 2022
- dotnet 6.0

#### Code
Checkout the source code
```sh
git clone https://github.com/prakashrx/git-live-server.git
cd git-live-server
```

The `/src` directory has the Project/Solution files

#### Build & Run


```
cd ./src/GitLiveServer
dotnet restore
dotnet build
dotnet run
```

#### Contributions

All Contributions are welcome. Please create an Issue of any bugs found.