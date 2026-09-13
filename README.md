# E-Commerce Backend API

A production-style E-Commerce Backend REST API built using ASP.NET Core.

## Features

- User Authentication & Authorization using JWT
- Role-Based Authorization (Admin/User)
- Product Management
- Category Management
- Shopping Cart
- Checkout System
- Order Management
- Coupon System
- Coupon Usage Tracking
- Payment Module
- Shipment Management
- Return & Refund Management
- Product Reviews & Ratings
- Wishlist
- Address Management
- Global Exception Handling
- Repository Pattern
- Service Layer Architecture
- DTO Pattern
- Entity Framework Core
- SQL Server Database

##  Technologies Used

- ASP.NET Core Web API
- .NET 10
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger / OpenAPI


##  Architecture

The project follows a clean layered architecture:

Controllers
↓
Services
↓
Repositories
↓
Database

DTOs are used to transfer data between the API and client.

##  Main Modules

###  Authentication
- User Registration
- Login
- JWT Token Authentication
- Forgot Password

### Products & Categories
- Product CRUD
- Category CRUD
- Search and Filtering
- Sorting
- Pagination

###  Cart & Checkout
- Add Products to Cart
- Update Cart Quantity
- Remove Cart Items
- Checkout
- Stock Validation

###  Orders
- Create Orders
- Order History
- Order Status Management

###  Coupons
- Create Coupons
- Apply Coupons
- Usage Tracking
- Active/Inactive Coupons

###  Payments
- Payment Processing
- Payment Status Management

###  Shipment
- Shipment Creation
- Shipment Tracking
- Shipment Status Management

### Return & Refund
- Return Requests
- Refund Processing
- Return Status Management

### ⭐ Reviews & Ratings
- Product Reviews
- Product Ratings

### ❤️ Wishlist
- Add Products to Wishlist
- Remove Products from Wishlist
- View Wishlist

## 🔐 Security

- JWT Authentication
- Role-Based Authorization
- Protected API Endpoints
- Global Exception Handling

## 🗄 Database

The project uses SQL Server with Entity Framework Core and Code-First Migrations.

## ▶️ How to Run

1. Clone the repository

```bash
git clone https://github.com/raza19ahmed-netizen/E-Commerce-Backend-API.git


-Open the project in Visual Studio.
-Update the database connection string in appsettings.json.
-Run database migrations.
-Start the application.
-Open Swagger to test the API.
👨‍💻 Developer
Ahmed Raza
ASP.NET Core / .NET Backend Developer
