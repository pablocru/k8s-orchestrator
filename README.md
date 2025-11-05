# k8s-orchestrator

`k8s-orchestrator` is a service built with **.NET 8** that acts as a **Task
Orchestrator**. Its main purpose is to query a list of HTTP endpoints representing other
services (Workers) and collect their responses.

Currently, the project is designed to:

- Run an internal CronJob that orchestrates calls to the endpoints.
- Control the CronJob via API (start, stop, run once, status).
- Deploy to **Kubernetes** using dedicated manifests.

**Future roadmap**:

- Extend the Orchestrator to query **.NET Workers** that will perform specific tasks.
- Turn the project into a **.NET Monorepo** to share libraries between the Orchestrator
  and the Workers.
- Deploy the entire ecosystem together in Kubernetes.

## API Endpoints

The orchestrator exposes a REST API to control the cronjob and trigger orchestration tasks
manually.

1. GET `/api/cronjob/status`: Returns the current status of the cronjob.

    **Example response:**

    ```json
    { "status": "Running" }
    ```

    Possible values: Idle, Running, Stopped.

1. GET `/api/cronjob/start`: Starts the cronjob loop if it is not already running.

    **Example response:**

    ```json
    { "message": "CronJob started successfully" }
    ```

1. GET `/api/cronjob/stop`: Stops the cronjob loop and cancels any ongoing orchestration
   task.

    **Example response:**

    ```json
    { "message": "CronJob stopped successfully" }
    ```

1. GET `/api/cronjob/run-once`: Executes the orchestration task once without starting the
   cronjob loop. If the cronjob is already running, it returns an HTTP 409 Conflict.

    **Example response:**

    ```json
    { "message": "Job executed successfully" }
    ```

    **Example conflict response:**

    ```json
    { "message": "Job could not run because CronJob is already running" }
    ```

## Prerequisites

To work with this project, you need to install:

- [**.NET 8 SDK**](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Version is
  defined in `globals.json`).
- [**Docker**](https://www.docker.com/) To build and run the project image.
- [**Kubernetes CLI (kubectl)**](https://kubernetes.io/docs/tasks/tools/) To manage
  Kubernetes deployments.
- [**Minikube**](https://minikube.sigs.k8s.io/docs/start/) To run a local Kubernetes
  cluster.

## Running locally

### .NET direct mode

Run the project directly using the .NET SDK.

```bash
# Move to folder
cd src/orchestrator

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Watch mode
dotnet watch run
```

By default, the API will be available at <http://localhost:5096>

### Docker mode

Build and run the project using Docker.

```bash
# Move to folder
cd src

# Exec deployment script
sh deploy-orch.docker.sh
```

### Kubernetes with Minikube mode

Deploy the project to a local Kubernetes cluster using Minikube.

```bash
# Move to folder
cd infra

# Exec deployment script
sh deploy-project.k8s.sh
```

## Contribute

If you notice any mistakes or have suggestions, I’m all ears! I appreciate any feedback so
don't hesitate to [open an Issue on
GitHub](https://github.com/pablocru/k8s-orchestrator/issues) or submit a `Pull Request`.

### Contribution Guidelines

1. `Fork` the repository and make your changes in a `new branch`.
1. Document your additions.
1. Use [Conventional Commits](https://www.conventionalcommits.org).
1. Submit a `Pull Request` with a clear description of your changes.

Thank you for helping improve this project!
