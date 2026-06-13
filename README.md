#  Task Management API

A scalable Task Management REST API built with **ASP.NET Core 8**, following clean architecture principles with authentication, caching, background processing, and full containerization.

> Built by Nadeen

---

## tech Stack

| Layer | Technology |
|---|---|
| API Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Caching | Redis |
| Auth | JWT Bearer Tokens |
| Testing | xUnit + Moq |
| Containerization | Docker & Docker Compose |
| Background Jobs | Channel-based processing |

---

## Architecture

Clean layered architecture:

```
API/                  → Controllers, Middleware
Application/          → Business Logic, Services, DTOs
Domain/               → Entities, Enums
Infrastructure/       → Database, Redis, Auth, Background Jobs
```

---

## Authentication & Authorization

- JWT-based authentication
- Role-based access control (`Admin` / `User`)
- Secure password hashing with BCrypt
- Protected endpoints using `[Authorize]`

---

## API Endpoints

### Auth
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### Users
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/users/me` | Get current user profile |
| GET | `/api/users` | Get all users (Admin only) |
| POST | `/api/users` | Create user (Admin only) |
| DELETE | `/api/users/{id}` | Soft delete user (Admin only) |

### Tasks
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/tasks` | Create a new task |
| GET | `/api/tasks` | Get all tasks (sorted by priority) |
| GET | `/api/tasks/{id}` | Get task by ID |
| PUT | `/api/tasks/{id}/status` | Update task status |

---

##  Redis Caching

- Cached endpoint: `GET /api/tasks/{id}`
- Cache key: `task:{id}`
- TTL: 5 minutes
- Cache is invalidated automatically on status update

---

## Background Processing

- In-memory queue using `Channel<T>`
- Worker reads queued tasks and processes them asynchronously
- Updates task status to `InProgress` and logs processing events

---

##  Business Rules

- Duplicate tasks on the same day are not allowed
- Tasks are ordered by priority (High → Low), then by creation date
- Task ownership is enforced — users can only access their own tasks

---

## Testing

Unit tests written with **xUnit** and **Moq**, covering:

- Authentication logic
- Task creation rules (duplicate prevention)

---

##  Running with Docker

All services are fully containerized. No local setup needed.

```bash
git clone <[repo-url](https://github.com/Nadeen-Badr/TaskManagementApi-.netcore8.git)>
cd TaskManagementApi

docker compose up --build
```

Then open Swagger at:

```
http://localhost:5000/swagger
```

### Services
- `api` — ASP.NET Core 8 API
- `sqlserver` — SQL Server 2022
- `redis` — Redis

---

##  Default Admin Credentials

```
Email:    admin@example.com
Password: Admin@123
```

---

##  What This Project Demonstrates

- Real-world clean architecture in .NET
- JWT authentication with role-based authorization
- Redis caching with cache invalidation strategy
- Asynchronous background processing
- Unit testing with mocking
- Production-ready Docker setup
