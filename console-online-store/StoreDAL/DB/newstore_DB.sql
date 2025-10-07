-- Console Online Store Database Schema
-- Path: console-online-store/StoreDAL/DB/newstore_DB.sql
-- 
-- This schema is 100% compliant with the Technical Requirements diagram
-- with necessary extensions for inventory management (stock_quantity, reserved_quantity)
--
-- Changes from previous version:
--   - users.name → users.first_name (per TZ diagram)
--   - categories.name → categories.category_name (per TZ diagram)
--   - product_titles.title → product_titles.product_title (per TZ diagram)
--   - manufacturers.name → manufacturers.manufacturer_name (per TZ diagram)
--   - customer_order_details.order_id → customer_order_details.customer_order_id (per TZ diagram)
--   - Added products.stock_quantity and products.reserved_quantity (required for business logic)

CREATE TABLE user_roles
(
    id INTEGER PRIMARY KEY,
    user_role_name TEXT NOT NULL
);

CREATE TABLE users
(
    id INTEGER PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    login TEXT NOT NULL,
    password TEXT NOT NULL,
    user_role_id INTEGER NOT NULL,
    is_blocked INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (user_role_id) REFERENCES user_roles (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE categories
(
    id INTEGER PRIMARY KEY,
    category_name TEXT NOT NULL
);

CREATE TABLE product_titles 
(
    id INTEGER PRIMARY KEY,
    product_title TEXT NOT NULL,
    category_id INTEGER NOT NULL,
    FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE manufacturers
(
    id INTEGER PRIMARY KEY,
    manufacturer_name TEXT NOT NULL
);

CREATE TABLE products
(
    id INTEGER PRIMARY KEY,
    product_title_id INTEGER NOT NULL,
    manufacturer_id INTEGER NOT NULL,
    unit_price REAL NOT NULL,
    comment TEXT NOT NULL DEFAULT '',
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    reserved_quantity INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (product_title_id) REFERENCES product_titles (id) ON DELETE CASCADE ON UPDATE NO ACTION,
    FOREIGN KEY (manufacturer_id) REFERENCES manufacturers (id) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE order_states
(
    id INTEGER PRIMARY KEY,
    state_name TEXT NOT NULL
);

CREATE TABLE customer_orders 
(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    customer_id INTEGER NOT NULL,
    operation_time TEXT NOT NULL,
    order_state_id INTEGER NOT NULL, 
    FOREIGN KEY (customer_id) REFERENCES users (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (order_state_id) REFERENCES order_states (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE customer_order_details
(
    id INTEGER PRIMARY KEY,
    customer_order_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    price REAL NOT NULL,
    product_amount INTEGER NOT NULL,
    FOREIGN KEY (customer_order_id) REFERENCES customer_orders (id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Seed data for user roles (required reference data)
INSERT INTO user_roles (id, user_role_name) VALUES (1, 'Admin');
INSERT INTO user_roles (id, user_role_name) VALUES (2, 'Registered');
INSERT INTO user_roles (id, user_role_name) VALUES (3, 'Guest');

-- Seed data for order states (required reference data)
INSERT INTO order_states (id, state_name) VALUES (1, 'New Order');
INSERT INTO order_states (id, state_name) VALUES (2, 'Cancelled by user');
INSERT INTO order_states (id, state_name) VALUES (3, 'Cancelled by administrator');
INSERT INTO order_states (id, state_name) VALUES (4, 'Confirmed');
INSERT INTO order_states (id, state_name) VALUES (5, 'Moved to delivery company');
INSERT INTO order_states (id, state_name) VALUES (6, 'In delivery');
INSERT INTO order_states (id, state_name) VALUES (7, 'Delivered to client');
INSERT INTO order_states (id, state_name) VALUES (8, 'Delivery confirmed by client');