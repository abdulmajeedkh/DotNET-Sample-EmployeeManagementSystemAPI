EmployeeManagement System (Clean Architecture, ASP.NET Core 9.0 API)

A full-featured, multi-tenant Employee Management System built using **ASP.NET Core 9** with **Clean Architecture**, **Modular Design**, **JWT Authentication**, **Serilog Logging**, and **xUnit Testing**. This backend API covers essential HR functionalities like Employee, Department, Attendance, Leave, Payroll, Training, and Reporting with filters, sorting, pagination, and export (CSV/PDF).

📌 Features

✅ Clean Architecture with proper separation of concerns  
✅ Modular structure: Employee, Department, Leave, Attendance, Payroll, Training, Reports  
✅ JWT Authentication + Microsoft Identity  
✅ Role-Based Authorization (Admin, Manager, Staff)  
✅ Centralized `ResponseDto<T>` structure  
✅ AutoMapper integration  
✅ Serilog Logging with request tracking  
✅ Custom Exception Handling Middleware  
✅ Pagination, Filtering, Sorting in all modules  
✅ Export data to CSV/PDF  
✅ xUnit + Moq-based Unit Testing for all services  
✅ Multi-Tenant Support

 Tech Stack

| Layer            | Technology              |
|------------------|--------------------------|
| Backend          | ASP.NET Core 9.0 Web API |
| Database         | SQL Server + EF Core     |
| Authentication   | Microsoft Identity + JWT |
| Mapping          | AutoMapper               |
| Logging          | Serilog                  |
| Testing          | xUnit + Moq              |
| Docs             | Swagger                  |

 📂 Project Structure

```bash
EmployeeManagement/
│
├── EmployeeManagement.API             # Presentation Layer (Controllers, Middleware)
├── EmployeeManagement.Application     # Application Layer (DTOs, Interfaces, Services)
├── EmployeeManagement.Domain          # Domain Models & Enums
├── EmployeeManagement.Infrastructure  # DB Context, EF Migrations, Service Impl
├── EmployeeManagement.Tests           # Unit Tests (xUnit, Moq)
└── README.md


## Getting Started

1. Clone the repo
2. Configure `appsettings.json` (DB, JWT, admin user)
3. Run migrations & seed data (auto on startup)
4. Start API: `dotnet run --project EmployeeManagement.API`
5. Access Swagger at `https://localhost:443/swagger`

## API Endpoints

- `api/employees`
- `api/departments`
- `api/attendance`
- `api/leaves`
- `api/payroll`
- `api/training`
- `api/authentication`
- `api/reports`

## Testing

Run unit tests: