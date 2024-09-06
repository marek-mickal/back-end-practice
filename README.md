# ProductCatalogApi

## Part 1
**NOTICE: This part of readme does not have to be exact after commint "last commit part 1". After that commit some things may not be valid (ie. ProductsService interacts with DB)**
### Project Setup
You can clone the repo, but bellow its describe how the project was originally set up.

**Create the project, use the following command:**
```bash
dotnet new webapi -n ProductCatalogApi
```

**Add .gitignore:**
```bash
dotnet gitignore
```
**Add Entity Framework Core Packages:**
```shell
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design 
```
**Setup Connection String:**

In the appsettings.json file, configure the connection string in example: (Note: Setting TrustServerCertificate=True is only for development environments.)

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Project Structure
- **Product**: The model representing a product in the database, including properties like `Name`, `Price`, and `Description`.
- **ProductDtoPlain**: Data Transfer Object (DTO) used for transferring product data without including the auto-generated `Id` field.
- **ProductsController**: Manages HTTP requests related to products, such as creating, reading, updating, and deleting product records.
- **ProductsService**: Contains business logic related to products, such as adding, updating, and deleting products in the database.
- **AppDbContext**: The Entity Framework Core context class that handles database interactions and migrations.
- **Program.cs**: Configures services and middleware for the application, including setting up the `DbContext` and other dependencies.

**Note:**
In this initial implementation, the service layer directly interacts with the database context. In part 2, this will be refactored to use a repository pattern to improve separation of concerns and code organization.

### Prepare DB
After creating the Models you can **set up Db migration by:**
```shell
dotnet ef migrations add InitialCreate
```

**And apply the migration to the Db by:**
```shell
dotnet ef database update
```

### API
Now you can **run the app by:**
```shell
dotnet run
```

**The API provides the following endpoints for managing the product catalog:**

- GET /productCatalogApi/Products - Retrieve a list of all products.
- GET /productCatalogApi/Products/{id} - Retrieve a product by its ID.
- POST /productCatalogApi/Products - Create a new product.
- PUT /productCatalogApi/Products/{id} - Update an existing product by its ID.
- DELETE /productCatalogApi/Products/{id} - Delete a product by its ID.

## Part 2
### Overview
In this task, I refactored the project to implement the repository pattern and dependency injection, separating data access logic from the business logic and API controllers. Because I've decided to use service to separate the logic from the http management directed by controller I injected the repository into the service. This does not followe the assigment exactly, but I beleive this is an enhancment rather than complication. Having the concerns separate is good practice this way repository only accesses data and does not check for any unexpected result or any other logic, this is handled by the service. This allows the controller focus only on handling the HTTP requests.

### Changes Made
1. **Introduced Repository Interface and Implementation**
   - Added an interface `IProductRepository` to define the contract for data operations related to `Product`.
   - Created a concrete implementation `ProductRepository` that uses Entity Framework Core to interact with the database.

2. **Updated Dependency Injection Setup**
   - Registered the `ProductRepository` in the dependency injection container in the `Program.cs` file.

3. **Injected Repository into Service**
   - Updated `ProductsService` to use `IProductRepository` instead of directly interacting with `DbContext`.

4. **Refactored API Controller**
   - Modified `ProductsController` to use `ProductsService` which now depends on `IProductRepository`.

4. **Added API for discnounts**
   - To further show the importance of having the additional service layer I've created this ednpoint which serves to update the products price based on discnount.
   - POST /productCatalogApi/Products/discount/{id}/{percentage}