# 🗓️ Smart Leave Approval System (SLAS)

A production-ready **.NET Core** backend system for automated employee leave management with business rules engine, audit logging, and JWT-based role authentication.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [API Endpoints](#api-endpoints)
- [Business Rules](#business-rules)
- [Authentication](#authentication)
- [Design Patterns Used](#design-patterns-used)
- [Audit Logging](#audit-logging)

---

## Overview

SLAS is a backend REST API that allows employees to apply for leaves, automatically evaluates them using a configurable business rules engine, and enables admins to override decisions. Every status change is tracked in an immutable audit log.

**Key Capabilities:**
- Employee leave application with overlap validation
- Auto-approval / auto-rejection via `LeaveEvaluator` service
- Admin override with comment support
- Full audit trail per leave request
- JWT authentication with `Employee` and `Admin` roles

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET Core 8 |
| Database | SQL Server |
| ORM / Data Access | Dapper + SQL Stored Procedures |
| Validation | FluentValidation |
| Logging | Serilog (Console + File sink) |
| Auth | JWT Bearer Tokens |
| DI Container | Built-in `IServiceCollection` |
| Architecture | Clean Architecture (API → Service → Repository) |

---

## Architecture

```
┌─────────────────────────────────────────────┐
│                  API Layer                  │
│         Controllers + Middleware            │
└────────────────────┬────────────────────────┘
                     │
┌────────────────────▼────────────────────────┐
│               Service Layer                 │
│   LeaveService  │  LeaveEvaluator           │
│   AdminService  │  (Strategy/Spec Pattern)  │
└────────────────────┬────────────────────────┘
                     │
┌────────────────────▼────────────────────────┐
│             Repository Layer                │
│    ILeaveRepository  │  IAuditRepository    │
│       (Dapper + SQL Stored Procedures)      │
└────────────────────┬────────────────────────┘
                     │
┌────────────────────▼────────────────────────┐
│              SQL Server Database            │
│   LeaveRequests  │  LeaveAuditLog           │
└─────────────────────────────────────────────┘
```

---

## Project Structure

```
SLAS/
├── SLAS.API/                          # Entry point - Controllers, Middleware, Program.cs
│   ├── Controllers/
│   │   ├── LeaveController.cs         # Employee leave endpoints
│   │   └── AdminController.cs         # Admin override endpoints
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   └── Program.cs
│
├── SLAS.Application/                  # Business Logic Layer
│   ├── Services/
│   │   ├── LeaveService.cs
│   │   └── AdminService.cs
│   ├── Evaluators/
│   │   ├── ILeaveEvaluator.cs
│   │   ├── LeaveEvaluator.cs          # Strategy Pattern for rules engine
│   │   └── Rules/
│   │       ├── ShortDurationRule.cs   # Auto-approve if <= 2 days
│   │       └── PendingLimitRule.cs    # Auto-reject if > 3 pending
│   ├── DTOs/
│   │   ├── LeaveRequestDto.cs
│   │   └── AdminOverrideDto.cs
│   └── Validators/
│       ├── LeaveRequestValidator.cs
│       └── AdminOverrideValidator.cs
│
├── SLAS.Domain/                       # Domain Models + Enums
│   ├── Entities/
│   │   ├── LeaveRequest.cs
│   │   └── LeaveAuditLog.cs
│   └── Enums/
│       ├── LeaveStatus.cs             # Pending, Approved, Rejected
│       └── LeaveType.cs               # Sick, Casual, Annual, etc.
│
├── SLAS.Infrastructure/               # Data Access Layer
│   ├── Repositories/
│   │   ├── ILeaveRepository.cs
│   │   ├── LeaveRepository.cs         # Dapper + Stored Procedures
│   │   ├── IAuditRepository.cs
│   │   └── AuditRepository.cs
│   └── DependencyInjection.cs
│
├── SLAS.Database/                     # SQL Scripts
│   ├── Tables/
│   │   ├── CreateLeaveRequests.sql
│   │   └── CreateLeaveAuditLog.sql
│   └── StoredProcedures/
│       ├── usp_ApplyLeave.sql
│       ├── usp_GetLeavesByEmployee.sql
│       ├── usp_UpdateLeaveStatus.sql
│       └── usp_InsertAuditLog.sql
│
└── SLAS.Tests/                        # Unit Tests
    ├── Services/
    └── Evaluators/
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/smart-leave-approval-system.git
cd smart-leave-approval-system
```

### 2. Configure Connection String

Update `SLAS.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SLASDB;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_HERE_MIN_32_CHARS",
    "Issuer": "SLAS",
    "Audience": "SLASUsers",
    "ExpiryMinutes": 60
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/slas-.txt", "rollingInterval": "Day" } }
    ]
  }
}
```

### 3. Setup Database

Run the SQL scripts in order (see [Database Setup](#database-setup) below).

### 4. Run the Application

```bash
cd SLAS.API
dotnet restore
dotnet run
```

API will be available at: `https://localhost:5001`  
Swagger UI: `https://localhost:5001/swagger`

---

## Database Setup

Run the following SQL scripts in order:

```sql
-- 1. Create Database
CREATE DATABASE SLASDB;
USE SLASDB;

-- 2. Run table creation scripts
-- /SLAS.Database/Tables/CreateLeaveRequests.sql
-- /SLAS.Database/Tables/CreateLeaveAuditLog.sql

-- 3. Run stored procedures
-- /SLAS.Database/StoredProcedures/usp_ApplyLeave.sql
-- /SLAS.Database/StoredProcedures/usp_GetLeavesByEmployee.sql
-- /SLAS.Database/StoredProcedures/usp_UpdateLeaveStatus.sql
-- /SLAS.Database/StoredProcedures/usp_InsertAuditLog.sql
```

See the `/SLAS.Database/` folder for all scripts.

---

## API Endpoints

### 🔐 Auth

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| `POST` | `/api/auth/login` | Get JWT token | Public |

### 📝 Leave (Employee)

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| `POST` | `/api/leave/apply` | Apply for leave | Employee |
| `GET` | `/api/leave/my-leaves` | Get own leave requests + audit trail | Employee |
| `GET` | `/api/leave/my-leaves?status=Pending&from=2024-01-01` | Filtered with pagination | Employee |

### 🛡️ Admin

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| `PUT` | `/api/admin/leave/{id}/override` | Approve or reject with comment | Admin |
| `GET` | `/api/admin/leaves` | Get all leave requests | Admin |

### Sample Request — Apply for Leave

```json
POST /api/leave/apply
Authorization: Bearer <token>

{
  "employeeId": "EMP001",
  "startDate": "2024-02-10",
  "endDate": "2024-02-11",
  "leaveType": "Casual",
  "reason": "Personal work"
}
```

### Sample Request — Admin Override

```json
PUT /api/admin/leave/3/override
Authorization: Bearer <admin_token>

{
  "status": "Approved",
  "comment": "Approved by manager on priority basis"
}
```

---

## Business Rules

The `LeaveEvaluator` service applies the following rules automatically on every new leave request:

| Rule | Condition | Outcome |
|------|-----------|---------|
| Short Duration | Leave duration **≤ 2 days** | ✅ Auto-Approved |
| Pending Overload | Employee has **> 3 pending** requests | ❌ Auto-Rejected |
| Default | Neither rule matched | ⏳ Stays Pending |

Rules are implemented using the **Strategy Pattern** — each rule is an independent `ILeaveRule` implementation, making it easy to add/remove rules without touching core logic.

---

## Authentication

JWT-based authentication with two roles:

| Role | Permissions |
|------|-------------|
| `Employee` | Apply for leave, view own leaves |
| `Admin` | Override any leave, view all leaves |

Include token in every request:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Design Patterns Used

| Pattern | Where Used | Purpose |
|---------|-----------|---------|
| **Strategy Pattern** | `LeaveEvaluator` + `ILeaveRule` | Pluggable, extensible business rules |
| **Repository Pattern** | `ILeaveRepository`, `IAuditRepository` | Decouple data access from business logic |
| **Dependency Injection** | Throughout | Loose coupling, testability |

---

## Audit Logging

Every status change (auto or manual) is recorded in the `LeaveAuditLog` table:

```json
{
  "leaveRequestId": 5,
  "changedBy": "System / Admin",
  "oldStatus": "Pending",
  "newStatus": "Approved",
  "comment": "Auto-approved: duration <= 2 days",
  "changedAt": "2024-02-10T09:30:00Z"
}
```

This ensures full traceability of every decision made on a leave request.

---

## 📄 License

This project was developed as part of a .NET Developer Practical Assessment.
