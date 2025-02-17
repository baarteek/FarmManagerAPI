# FarmManagerAPI

## 📌 Project Description

FarmManager Backend is a robust and scalable **ASP.NET Core** application designed to manage farms, fields, and crops efficiently. It provides a REST API for handling various aspects of farm management, including field tracking, agrotechnical interventions, soil measurement, and crop lifecycle monitoring.

This backend system integrates with frontend applications and ensures high performance, data security, and scalability for modern farm management solutions.

---

## ⚡ Features

### 🏠 1. User Authentication & Authorization
- User registration and login.

### 🌾 2. Farm and Field Management
- CRUD operations for farms and fields.
- Management of reference parcels.
- GeoJSON-based field visualization.

### 🌱 3. Crop Lifecycle Tracking
- Adding, activating, and deactivating crops.
- Crop lifecycle history tracking.

### 🚜 4. Agrotechnical Operations
- Managing **fertilization, plant protection, and cultivation operations**.
- Tracking and recording agrotechnical interventions.

### 📂 5. Data Import and Export
- GML and CSV file support for **field and crop data import**.
- Exporting reports in **PDF, Excel, and HTML formats**.

### 📊 6. Report Generation
- Generating **agrotechnical activities reports**.
- Customizable reports for farm operations.

### 🛠️ 7. API Endpoints for Data Retrieval
- Fetching **farm, field, and crop details**.
- Retrieving map data with **GeoJSON format support**.
- Fetching **soil type and crop status information**.

### ✅ 8. Unit Testing
- Comprehensive unit tests for controllers and services.
- Ensuring system stability and performance.

---

## 🚀 Technologies Used
- **ASP.NET Core** – Backend framework.
- **Entity Framework Core** – ORM for database management.
- **SQL Server** – Primary database.
- **JWT Authentication** – Secure authentication.
- **xUnit** – Unit testing framework.

---

## 🛠️ Installation & Setup

### 📌 Prerequisites
- .NET SDK
- SQL Server or Docker for database

### 🔧 Steps to Run the Application

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/baarteek/FarmManagerAPI
   cd FarmManagerAPI
   ```

2. **Setup Database:**
   - Update connection string in `appsettings.json`.
   - Run database migrations:
     ```bash
     dotnet ef database update
     ```

3. **Run the Application:**
   ```bash
   dotnet run
   ```





