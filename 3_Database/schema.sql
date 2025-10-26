-- ================================================
-- Mini E-Commerce Database Schema
-- SQLite Database
-- Date: 2025-10-24
-- ================================================

-- Enable foreign keys
PRAGMA foreign_keys = ON;

-- Set WAL mode for better performance
PRAGMA journal_mode = WAL;

-- ================================================
-- Table: Users
-- ================================================
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Email" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");

-- ================================================
-- Table: Products
-- ================================================
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "Weight" REAL NOT NULL,
    "Stock" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Description" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

-- ================================================
-- Table: ShippingMethods
-- ================================================
CREATE TABLE IF NOT EXISTS "ShippingMethods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ShippingMethods" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "DisplayName" TEXT NOT NULL,
    "ParamsJSON" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_ShippingMethods_Code" ON "ShippingMethods" ("Code");

-- ================================================
-- Table: CartItems
-- ================================================
CREATE TABLE IF NOT EXISTS "CartItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CartItems" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CartItems_Products_ProductId" 
        FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_CartItems_ProductId" ON "CartItems" ("ProductId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_CartItems_UserId_ProductId" ON "CartItems" ("UserId", "ProductId");

-- ================================================
-- Table: Orders
-- ================================================
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "Subtotal" DECIMAL(18,2) NOT NULL,
    "Discount" DECIMAL(18,2) NOT NULL,
    "Tax" DECIMAL(18,2) NOT NULL,
    "ShippingFee" DECIMAL(18,2) NOT NULL,
    "GrandTotal" DECIMAL(18,2) NOT NULL,
    "MethodCode" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "TotalWeight" REAL NOT NULL,
    "Distance" REAL NOT NULL,
    "Region" TEXT NOT NULL,
    "ShippingCalculationDetails" TEXT NULL,
    CONSTRAINT "FK_Orders_Users_UserId" 
        FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_Orders_UserId" ON "Orders" ("UserId");

-- ================================================
-- Table: OrderItems
-- ================================================
CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderItems" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" DECIMAL(18,2) NOT NULL,
    "LineTotal" DECIMAL(18,2) NOT NULL,
    CONSTRAINT "FK_OrderItems_Orders_OrderId" 
        FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Products_ProductId" 
        FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");
CREATE INDEX IF NOT EXISTS "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");
