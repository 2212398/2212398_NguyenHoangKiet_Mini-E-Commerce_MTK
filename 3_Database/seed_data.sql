-- ================================================
-- Mini E-Commerce Seed Data
-- SQLite Database
-- Date: 2025-10-24
-- ================================================

-- ================================================
-- Seed Users
-- ================================================
-- Password: admin123 (hashed with BCrypt)
INSERT INTO "Users" ("Email", "PasswordHash", "Role", "CreatedAt", "IsActive")
VALUES ('admin@ecommerce.com', '$2a$11$XxWZq9v8SQWvNL3CZvD8JeqGdNhV4Zh5xK8gJYzQ7VL6Zq0r.1kNm', 'Admin', datetime('now'), 1);

-- Password: user123 (hashed with BCrypt)
INSERT INTO "Users" ("Email", "PasswordHash", "Role", "CreatedAt", "IsActive")
VALUES ('user@ecommerce.com', '$2a$11$YyXXz0w9TRXwOM4DAAwE9KfrHeOiW5Ai6ylZ9mHYzWM7Ar1s.2lOq', 'User', datetime('now'), 1);

-- ================================================
-- Seed Products
-- ================================================
INSERT INTO "Products" ("Name", "Category", "Price", "Weight", "Stock", "IsActive", "Description", "CreatedAt", "UpdatedAt")
VALUES 
('Laptop', 'Electronics', 999.99, 2.5, 50, 1, 'High-performance laptop', datetime('now'), datetime('now')),
('Smartphone', 'Electronics', 699.99, 0.3, 100, 1, 'Latest smartphone', datetime('now'), datetime('now')),
('Wireless Headphones', 'Electronics', 149.99, 0.2, 75, 1, 'Headphones', datetime('now'), datetime('now')),
('Mechanical Keyboard', 'Computer', 79.99, 0.8, 40, 1, 'Keyboard', datetime('now'), datetime('now')),
('Gaming Mouse', 'Computer', 49.99, 0.15, 60, 1, 'High-precision mouse', datetime('now'), datetime('now')),
('4K Monitor', 'Tech', 399.99, 5.0, 30, 1, 'Ultra HD monitor', datetime('now'), datetime('now')),
('HD Webcam', 'Electronics', 89.99, 0.25, 45, 1, '1080p webcam', datetime('now'), datetime('now')),
('Desk Lamp', 'Furniture', 29.99, 0.6, 80, 1, 'LED desk lamp', datetime('now'), datetime('now'));

-- ================================================
-- Seed Shipping Methods (Strategy Pattern)
-- ================================================

-- STANDARD Strategy
INSERT INTO "ShippingMethods" ("Code", "DisplayName", "ParamsJSON", "IsActive", "CreatedAt", "UpdatedAt")
VALUES ('STANDARD', 'Standard Shipping (3-5 days)', 
'{\"BaseRate\":5.00,\"WeightRate\":2.00,\"WeightThreshold\":5.0,\"DistanceRate\":0.50}', 
1, datetime('now'), datetime('now'));

-- EXPRESS Strategy
INSERT INTO "ShippingMethods" ("Code", "DisplayName", "ParamsJSON", "IsActive", "CreatedAt", "UpdatedAt")
VALUES ('EXPRESS', 'Express Delivery (1-2 days)', 
'{\"BaseRate\":15.00,\"WeightRate\":3.00,\"DistanceMultiplier\":1.5}', 
1, datetime('now'), datetime('now'));

-- SAME_DAY Strategy
INSERT INTO "ShippingMethods" ("Code", "DisplayName", "ParamsJSON", "IsActive", "CreatedAt", "UpdatedAt")
VALUES ('SAME_DAY', 'Same Day Delivery', 
'{\"BaseRate\":25.00,\"CutoffTime\":14}', 
1, datetime('now'), datetime('now'));

-- ECO Strategy
INSERT INTO "ShippingMethods" ("Code", "DisplayName", "ParamsJSON", "IsActive", "CreatedAt", "UpdatedAt")
VALUES ('ECO', 'Eco-Friendly Shipping', 
'{\"BaseRate\":3.00,\"WeightRate\":1.50,\"BulkDiscount\":0.10,\"BulkThreshold\":10.0}', 
1, datetime('now'), datetime('now'));

-- ================================================
-- Verification Queries
-- ================================================

-- Check users
SELECT 'Users:' as Info, COUNT(*) as Count FROM Users;

-- Check products
SELECT 'Products:' as Info, COUNT(*) as Count FROM Products;

-- Check shipping methods
SELECT 'Shipping Methods:' as Info, COUNT(*) as Count FROM ShippingMethods;

-- Display all data
SELECT '=== USERS ===' as Section;
SELECT Email, Role, IsActive FROM Users;

SELECT '=== PRODUCTS ===' as Section;
SELECT Name, Category, Price, Weight, Stock FROM Products;

SELECT '=== SHIPPING METHODS ===' as Section;
SELECT Code, DisplayName, IsActive FROM ShippingMethods;
