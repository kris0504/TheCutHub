# TheCutHub – ASP.NET Advanced (SoftUni, June 2025)

**TheCutHub** is a barber shop booking web app built as the final project for the **ASP.NET Advanced** course.  
Customers browse barbers and services, book appointments, and leave reviews. Administrators manage barbers, services, working hours, users, and appointments via a dedicated **Admin Area**.

---

## ✨ Features

- **Barber catalog & profiles** – photo, bio, portfolio/gallery, average rating, and reviews.
- **Service catalog** – name, description, price, and **duration (minutes)**.
- **Appointment scheduling** – choose date/time/service/barber; free time slots are calculated from working hours and current bookings (**AJAX** endpoint loads slots).
- **Working hours** – per-barber weekly schedule (start, end, interval, IsWorking). Default **09:00–18:00** seeded.
- **Reviews & ratings** – users can delete their own; admins can moderate.
- **Admin Area** – CRUD for services and barbers, appointment list (filter/sort/paginate), user management (assign/remove **Barber** role), working hours.
- **Auth & roles** – ASP.NET Identity (**Administrator**, **User**, **Barber**).
- **Validation & errors** – client/server validation; custom **404/500**.
- **Pagination & search** – server-side pagination with [X.PagedList](https://github.com/dncuug/X.PagedList).

---

## 🔧 Tech Stack

- **.NET 8**, ASP.NET Core MVC, Razor Views, Areas  
- **Entity Framework Core** + **SQL Server**  
- **Identity** (Users & Roles)  
- **Bootstrap**, Bootstrap Icons  
- **X.PagedList** (pagination)  
- **Tests**: xUnit, Moq

---

## 🚀 Quick Start (CLI)

### 1) Prerequisites
- **.NET SDK 8.0+**
- **SQL Server** 
- **Visual Studio 2022**=
### 2) Clone
```bash
git clone https://github.com/kris0504/TheCutHub.git
cd TheCutHub/TheCutHub
```

### 3) Configure database connection
Open `appsettings.json` and set `DefaultConnection`. Example for **LocalDB**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TheCutHub;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```
> If you use a different SQL Server, change `Server=...` and add credentials as needed.

### 4) Migrations & database
Install EF tools (if missing):
```bash
dotnet tool update --global dotnet-ef
```
Create/update the database:
```bash
dotnet ef database update --project TheCutHub/TheCutHub.csproj
```

### 5) Run
```bash
dotnet run --project TheCutHub/TheCutHub.csproj
```
Open the URL shown in the console.

### 6) Seeding (automatic on first run)
- Roles: **Administrator**, **User**, **Barber**
- **Admin account (Dev):** `admin@barberbook.com` / `Admin123!`  
> **Important:** change these for production or disable seeding.

---

## ▶️ Run with Visual Studio

1. Open the solution `TheCutHub.sln`.  
2. Set **TheCutHub** as the startup project.  
3. Verify `appsettings.json` → `DefaultConnection`.  
4. **Package Manager Console** (Default project: *TheCutHub*) → run:
   ```powershell
   Update-Database
   ```
5. Press **F5** (IIS Express / Kestrel).

---

## ✅ Tests
```bash
dotnet test
```
xUnit + Moq tests cover key business logic (slot generation, CRUD, admin services) and controller actions.

---

## 🗂️ Structure (short)

```
TheCutHub/
  Areas/
    Admin/        Controllers, Services, Views (admin panel)
    Barber/       (barber profile/area)
    Identity/     (Identity UI)
  Controllers/    Appointments, Barbers, Reviews, Services, Home, Error
  Data/           ApplicationDbContext, ApplicationDbInitializer (seed)
  Models/         Appointment, Barber, Review, Service, WorkingHour, WorkImage, ApplicationUser
  Services/       AppointmentService, BarberService, ReviewService, ServiceService
  Views/          Razor pages
```

---

## 🔐 Security
- ASP.NET Identity + Anti-forgery tokens  
- Server & client validation  
- EF Core avoids SQL Injection (no concatenated SQL)  
- Role-based authorization; protections against XSS/CSRF/parameter tampering

---


