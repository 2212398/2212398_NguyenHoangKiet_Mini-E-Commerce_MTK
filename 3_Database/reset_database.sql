-- ================================================
-- Reset Mini E-Commerce Database
-- This script will delete all data and recreate the database
-- Date: 2025-10-24
-- ================================================

-- Disable foreign keys temporarily
PRAGMA foreign_keys = OFF;

-- ================================================
-- Drop all tables
-- ================================================
DROP TABLE IF EXISTS "OrderItems";
DROP TABLE IF EXISTS "Orders";
DROP TABLE IF EXISTS "CartItems";
DROP TABLE IF EXISTS "ShippingMethods";
DROP TABLE IF EXISTS "Products";
DROP TABLE IF EXISTS "Users";

-- ================================================
-- Drop all indexes
-- ================================================
DROP INDEX IF EXISTS "IX_Users_Email";
DROP INDEX IF EXISTS "IX_ShippingMethods_Code";
DROP INDEX IF EXISTS "IX_CartItems_ProductId";
DROP INDEX IF EXISTS "IX_CartItems_UserId_ProductId";
DROP INDEX IF EXISTS "IX_Orders_UserId";
DROP INDEX IF EXISTS "IX_OrderItems_OrderId";
DROP INDEX IF EXISTS "IX_OrderItems_ProductId";

-- Enable foreign keys
PRAGMA foreign_keys = ON;

-- ================================================
-- Recreate schema from schema.sql
-- ================================================
.read schema.sql

-- ================================================
-- Load seed data from seed_data.sql
-- ================================================
.read seed_data.sql

-- ================================================
-- Verification
-- ================================================
SELECT '=== DATABASE RESET COMPLETE ===' as Message;
SELECT 'Total Users: ' || COUNT(*) as Info FROM Users;
SELECT 'Total Products: ' || COUNT(*) as Info FROM Products;
SELECT 'Total Shipping Methods: ' || COUNT(*) as Info FROM ShippingMethods;
