# PowerFuel Backend API

.NET 10.0 Web API backend for the **POWERFUEL** fitness platform (supplements, equipment, coaches, bookings, cart, and orders) using Entity Framework Core and SQL Server.

## Solution structure

- **PowerFuel.API** – ASP.NET Core Web API (controllers, JWT auth, Swagger)
- **PowerFuel.Application** – DTOs, service interfaces, shared contracts
- **PowerFuel.Domain** – Domain entities
- **PowerFuel.Infrastructure** – EF Core `DbContext`, entity configurations, migrations, service implementations (auth, products, equipment, coaches, cart, orders, reviews, categories)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or .NET 9; change `TargetFramework` to `net9.0` in all `.csproj` files if needed)
- SQL Server (full instance or LocalDB)

---

## Run and set up the database on SQL Server

### 1. Create the database (optional)

You can either let the app create it, or create an empty database yourself:

- **Option A – Let the app create it**  
  Use a connection string where the database **does not exist** yet. The app will create `PowerFuelDb` when it runs migrations (see step 3).

- **Option B – Create it yourself**  
  In SQL Server Management Studio (SSMS) or Azure Data Studio:
  - Connect to your SQL Server instance.
  - Run: `CREATE DATABASE PowerFuelDb;`  
  Then use the connection string in step 2 pointing to `PowerFuelDb`.

### 2. Set the connection string

Edit **`src/PowerFuel.API/appsettings.json`** (or `appsettings.Development.json`) and set `ConnectionStrings:DefaultConnection` for **your** SQL Server.

**Windows Authentication (recommended on your machine):**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=PowerFuelDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
}
```

- Use `Server=.;` for the default instance on your PC.
- Use `Server=localhost;` or `Server=(local);` if you prefer.
- Use `Server=YourServerName\\InstanceName;` for a named instance (e.g. `SQLEXPRESS`).

**SQL Server authentication (username/password):**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=PowerFuelDb;User Id=sa;Password=YourPassword;MultipleActiveResultSets=true;TrustServerCertificate=true"
}
```

- Replace `User Id` and `Password` with your SQL login.
- `TrustServerCertificate=true` is often needed for local/dev; remove in production if you use a proper certificate.

### 3. Run the API (creates/updates the database)

From the project folder:

```powershell
cd "c:\Users\yasse\Desktop\New folder (2)\src\PowerFuel.API"
dotnet run
```

Or from the solution root:

```powershell
cd "c:\Users\yasse\Desktop\New folder (2)"
dotnet run --project src\PowerFuel.API
```

On **first run**, the app:

1. Applies all Entity Framework migrations (creates tables in `PowerFuelDb`).
2. Runs seed data (categories, sample coach, products, equipment).

You do **not** need to run `dotnet ef database update` manually unless you want to update the database without starting the API.

### 4. Confirm it’s working

- In the console you’ll see the listening URL (e.g. `https://localhost:7xxx` or `http://localhost:5xxx`).
- Open **Swagger**: `https://localhost:7xxx/swagger` (use your port).
- In SSMS/Azure Data Studio, refresh **Databases** and you should see **PowerFuelDb** with tables such as `Users`, `Products`, `Categories`, `Coaches`, `Orders`, etc.

### Troubleshooting

| Problem | What to do |
|--------|------------|
| **Cannot open database** | Ensure SQL Server is running. For default instance: `Server=.;` or `Server=localhost;`. For SQL Express: `Server=.\\SQLEXPRESS;`. |
| **Login failed for user** | Check `User Id` and `Password` in the connection string; ensure the login has rights to create databases if the DB doesn’t exist yet. |
| **Migrations error** | Ensure the connection string is correct and the server is reachable. You can run migrations manually: `dotnet ef database update --project src\PowerFuel.Infrastructure --startup-project src\PowerFuel.API`. |

---

## Configuration (other)

1. **Connection string**  
   As above: edit `DefaultConnection` in `src/PowerFuel.API/appsettings.json` (or `appsettings.Development.json`).

2. **JWT**  
   Optional overrides in `appsettings.json`:

   ```json
   "Jwt": {
     "Secret": "Your-Secret-Key-At-Least-32-Characters-Long!",
     "Issuer": "PowerFuelApi",
     "Audience": "PowerFuelClient",
     "ExpirationHours": 24
   }
   ```

## Run the API

From the solution root:

```bash
cd src/PowerFuel.API
dotnet run
```

Or from the repo root:

```bash
dotnet run --project src/PowerFuel.API
```

- API: **https://localhost:7xxx** or **http://localhost:5xxx** (see console for URLs)
- Swagger UI: **https://localhost:7xxx/swagger** (or the same host + `/swagger`)

On first run, migrations are applied and seed data (categories, sample coach, products, equipment) is inserted.

## Database migrations

- Add a new migration (from repo root, using API as startup):

  ```bash
  dotnet ef migrations add YourMigrationName --project src/PowerFuel.Infrastructure --startup-project src/PowerFuel.API --context ApplicationDbContext
  ```

- Update the database:

  ```bash
  dotnet ef database update --project src/PowerFuel.Infrastructure --startup-project src/PowerFuel.API --context ApplicationDbContext
  ```

Migrations are also applied automatically on app startup in the default setup.

## Main API endpoints

| Area | Method | Endpoint | Auth |
|------|--------|----------|------|
| Auth | POST | `/api/auth/register` | No |
| Auth | POST | `/api/auth/login` | No |
| Products | GET | `/api/products` | No |
| Products | GET | `/api/products/best-sellers` | No |
| Products | GET | `/api/products/{id}` | No |
| Equipments | GET | `/api/equipments` | No |
| Equipments | GET | `/api/equipments/{id}` | No |
| Coaches | GET | `/api/coaches` | No |
| Coaches | GET | `/api/coaches/{id}` | No |
| Coaches | GET | `/api/coaches/{id}/availability` | No |
| Coaches | POST | `/api/coaches/{id}/bookings` | Bearer |
| Cart | GET | `/api/cart` | Bearer |
| Cart | POST | `/api/cart/items` | Bearer |
| Cart | PUT | `/api/cart/items/{id}?quantity=` | Bearer |
| Cart | DELETE | `/api/cart/items/{id}` | Bearer |
| Orders | POST | `/api/orders` | Bearer |
| Orders | GET | `/api/orders` | Bearer |
| Orders | GET | `/api/orders/{id}` | Bearer |
| Reviews | GET | `/api/reviews/products/{id}` | No |
| Reviews | GET | `/api/reviews/equipment/{id}` | No |
| Reviews | POST | `/api/reviews/products/{id}` | Bearer |
| Reviews | POST | `/api/reviews/equipment/{id}` | Bearer |
| Categories | GET | `/api/categories/products` | No |
| Categories | GET | `/api/categories/equipment` | No |

## Authentication

- **Register**: `POST /api/auth/register` with body `{ "userName", "email", "password", "firstName?", "lastName?" }`.
- **Login**: `POST /api/auth/login` with body `{ "email", "password" }`.  
  Response includes a JWT in the `token` field.
- Send the token in the `Authorization` header: `Bearer <token>` for protected endpoints (cart, orders, coach bookings, reviews).

## Business rules (aligned with frontend)

- **Free shipping** for order subtotal ≥ $50.
- **Return policy**: 30 days (stored as constant; can be extended later with return requests).
- **Coach bookings**: Only within configured availability; no double-booking for the same coach/date/time.

## License

Private / internal use.
