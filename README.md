# 🍽️ El Criollo Backend API

**ASP.NET Core Web API for El Criollo Restaurant Management System**

## 🚀 Project Status: ✅ WORKING

- ✅ Database connection established
- ✅ Entity Framework configured  
- ✅ Basic CRUD operations
- ✅ Swagger documentation
- 🔄 Authentication (in progress)
- 🔄 Email service (planned)

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 9.0
- **Database:** SQL Server Express
- **ORM:** Entity Framework Core
- **Documentation:** Swagger/OpenAPI
- **Authentication:** JWT (planned)

## 📊 Database Schema

### Tables:
1. **Usuarios** - Employee management and authentication
2. **MESAS** - Restaurant table management (10 tables)
3. **PRODUCTOS** - Dominican menu items (22+ products)
4. **Pedidos** - Order management
5. **DETALLE_PEDIDOS** - Order line items
6. **FACTURAS** - Billing and payments
7. **RESERVAS** - Reservation system with email automation

## 🔗 API Endpoints

### Currently Available:
```
GET    /api/Usuarios              - List all users
GET    /api/Usuarios/{id}         - Get specific user
GET    /api/Usuarios/test-connection - Test database connection
```

### Planned:
```
POST   /api/Usuarios              - Create new user
PUT    /api/Usuarios/{id}         - Update user
DELETE /api/Usuarios/{id}         - Delete user
GET    /api/Mesas                 - Table management
GET    /api/Productos             - Menu management
POST   /api/Pedidos               - Order processing
```

## 🚀 Quick Start

### Prerequisites:
- .NET 9.0 SDK
- SQL Server Express
- Visual Studio 2022

### Setup:
1. Clone the repository
2. Update `appsettings.json` with your SQL Server connection
3. Run the application: `dotnet run`
4. Access Swagger UI: `https://localhost:7122/swagger`

### Connection String:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR-SERVER\\SQLEXPRESS;Database=RestaurantelCriollo;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

## 📈 Development Progress

- [x] **Week 1:** Database design and Entity Framework setup
- [x] **Week 1:** Basic API structure and Swagger
- [x] **Week 1:** Usuario CRUD operations
- [ ] **Week 2:** Authentication and authorization
- [ ] **Week 2:** Complete CRUD for all entities
- [ ] **Week 3:** Email service integration
- [ ] **Week 3:** Frontend integration

## 👨‍💻 Developer

**Josue Bernabe**
- 📧 Contact: josuebernabe929@gmail.com
-  🔗 Database Repo: [el-criollo-database](https://github.com/JosueBernabe17/el-criollo-database)

## 📄 License

This project is part of a learning portfolio for restaurant management systems.

---

**El Criollo - Sabor Dominicano Auténtico** 🇩🇴
