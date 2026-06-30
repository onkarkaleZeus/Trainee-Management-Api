# Trainee Management system

## Technology Used
C#, .NET, Redis, RabbitMQ, MySQL

## Project structure

```text
TraineeManagement.Api
│
├── Controllers
├── Data
├── DTOs
├── Exceptions
├── Interfaces
├── logs
├── Middleware
├── Migrations
├── Models
├── Services
├── Settings
├── Uploads
├── Dockerfile
└── .env

SubmissionProcessor.Worker
│
├── Data
├── DTOs
├── Interfaces
├── logs
├── Models
├── Services
├── Settings
├── Workers
├── Dockerfile
└── .env
 
TrainingDirectory.Api
│
├── Controllers
├── DTOs
└── Dockerfile
```

## Backend setup steps
- Download and extract the project 
- ensure dotnet, dotnet.sdk is installed
- run command "dotnet restore" to install dependencies
- run the command "dotnet run"
- naviagte to "/swagger" endpoint to test the created Apis.

## MySQL setup steps
- Install MySQL server
- Login to mysql using command : "mysql -u root -p"
- Create Database of name "trainee_management_db"
- ensure "Pomelo.EntityFrameworkCore.MySql" package is installed in dependencies

## Configuration
- appsettings.json
- appsettings.Development.json
- .env files
- Docker Compose environment variables
 
## Docker Compose
 
```bash
docker compose up --build -d
```
 
The following services will be started automatically:
 
- MySQL
- Redis
- RabbitMQ
- TraineeManagement.Api
- SubmissionProcessor.Worker
- TrainingDirectory.Api

## EF Core migration commands
```bash
dotnet ef migrations add <migration_name>
dotnet ef database update
```

## Login Credentials for testing
```json
{
  "username": "Admin",
  "password": "Admin@123"
}
```

## Jwt usage instructions 
- navigate to /swagger
- Login using /api/Auth/login using the login credentials
- click authorize button in the top right corner 
- Enter "Bearer <token>" in the input
- Click authorize

## API Endpoints

### Health api
- GET /api/health/live : Gets the Server status at that particular timestamp
- GET /api/health/ready : Gets the service status ( reachable/ unreachable ) for RabbitMQ, Redis and MySQL status.

### User apis
- POST /api/auth/login: Returns a JWT token along with ExpiryMinutes for correct login credentials ( Username and Password )

### Trainee apis
- GET /api/health: Returns server status at current timestamp
- GET /api/Trainees: Returns all the Trainees records in batches from mysql database
- GET /api/Trainees/{id}: Returns the details of the a particular trainee by id
- GET /api/trainees?pageNumber=1&pageSize=10&search=amit&status=Active : Returns the Valid Trainee records in batches of pageSize which contains the query parameter in either FirstName, LastName, Email or TechStack, else returns 404 Not Found status code.
- POST /api/Trainees: Adds a new Trainee record to the database
- PUT /api/Trainees/{id}: updates the Trainee details of a Trainee matched on the id provided as parameter
- DELETE /api/Trainees/{id}: Deletes the Trainee with the matching id provided

### Mentor apis
- GET /api/mentors : get all the Mentor records in batches from mysql database. Uses search and status parameter to filter the response.
- GET /api/mentors/{id} : Returns the details of the a particular mentor by id
- POST /api/mentors : Adds a new Mentor record to the database
- PUT /api/mentors/{id} : updates the Mentor details of a Mentor matched on the id provided as parameter
- DELETE /api/mentors/{id} : Deletes the Mentor with the matching id provided

### Learning task apis
- GET /api/learning-tasks : get all the Learning Task records in batches from mysql database. Uses search and status parameter to filter the response.
- GET /api/learning-tasks/{id} : Returns the details of the a particular Task by id
- POST /api/learning-tasks : Adds a new Learning task record to the database
- PUT /api/learning-tasks/{id} : updates the task details of a Learning task matched on the id provided as parameter
- DELETE /api/learning-tasks/{id} : Deletes the learning task with the matching id provided

### Task assignment apis
- GET /api/task-assignments : Gets all the Task assignments from the database
- GET /api/task-assignments/{id} : Gets a particular Task assignment with the id parameter
- POST /api/task-assignments : Adds a new Task assignment to the database
- PUT /api/task-assignments/{id}/status : Changes status of a particular Task assignment using id parameter

### Submission apis
- POST /api/submissions : Adds a new Submission to the database
- GET /api/submissions : Gets all the Submissions from the database
- GET /api/submissions/{id} : Gets a particular Submission with the id parameter
- POST /api/submissions/{submissionId}/files : Upload a submission file to the local storage

### Review apis
- POST /api/reviews : Adds a new Review to the database
- GET /api/reviews : Gets all the Reviews from the database
- GET /api/reviews/{id} : Gets a particular Review with the id parameter

### Submission file apis
- GET /api/submission-files/{id}/download : Download the submission file
- DELETE /api/submission-files/{id} : Delete a submission file from local storage along with metadata from database.

### Processing job api
- GET /api/processing-jobs/{id} : gets the processing jobs status along with number of attempts and encountered error summary.

## Features Completed
- Created Web API project
- Created Health Check API
- Created Trainee model
- Create temporary in memory List 
- Created GET and POST APIS, which fetches details of all trainees, fetches details of trainees using id parameter, and also  allows creation of a trainee.
- Created and used 3 DTOs which were used for creating trainee, updating trainee and for returning trainee details ( response ).
- Implemented Service Layer which is implemented and registered using dependency injection
- PUT and DELETE APIs have also been made along with the status codes for success, validation and not found scenarios
- Integrated MySQL in the project by migrating from inMemory Database.
- Created Users, Mentors, LearningTasks Entity along with their respective Dtos, services, and Controllers.
- Implemented Login POST api for the Users entity.
- Implemented JWT token creation
- Implemented password hashing for saving password hash instead of plain text. 
- Configured SwaggerUI to pass JWT access token in request headers
- Added pagination to get all the Trainee records in batches instead of passing all the records at the same time
- Added CORS configuration for react Integration
- Added Logging in the application to log important Information.
- Added File storage and metadata tracking in mySQL.
- Added redis Caching for faster retrieval of data.
- Handled fallback to database along with cache logs in case of redis failures.

## Design Decisions
### Redis
- Cache-aside pattern
- Absolute TTL: 30 Minutes
- Sliding Expiration: 10 minutes
- Automatic database fallback on failure
 
### RabbitMQ
- Durable Queue
- Persistent Messages
- Manual Acknowledgements
- Prefetch Count = 1
 
### Retry Strategy
- HTTP Client Resilience
- Timeout Handling
- Retry for Transient Failures
 
### Idempotency
- MessageId
- CorrelationId
These identifiers help prevent duplicate processing.
 
### Failure Behaviour
 
MySQL Unavailable
 
- Readiness check reports Unhealthy.
- API returns database-related errors.
 
Redis Unavailable
 
- Application falls back to MySQL.
- Cache failures are logged.
 
RabbitMQ Unavailable
 
- Publish operation logs failure.
- Worker remains unavailable until RabbitMQ recovers.
 
Worker Failure
 
- Message is negatively acknowledged (NACK).
- Processing is retried according to RabbitMQ configuration.

## Sample Request JSON
```json
{
  "firstName": "onkar",
  "lastName": "kale",
  "email": "onkar.kale@gmail.com",
  "techStack": "ReactJs",
  "status": 0
}
```

## Sample Response JSON
```json
{
  "id": 1,
  "firstName": "onkar",
  "lastName": "kale",
  "email": "onkar.kale@gmail.com",
  "techStack": "ReactJs",
  "status": 0,
  "createdDate": "2026-06-08T13:08:34.0151329Z",
  "updatedDate": "2026-06-08T13:08:34.0151461Z"
}
```

## Known Limitations
- No Email verification
- No password reset

## Challenges Faced
- dotnet installation  

## Next Phase Scope
- Integration with frontend