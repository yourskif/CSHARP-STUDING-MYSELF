CREATE TABLE categories
(
id INTEGER PRIMARY KEY,
category_name TEXT NOT NULL
);

CREATE TABLE manufacturers
(
id INTEGER PRIMARY KEY,
manufacturer_name TEXT NOT NULL
);

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
Password TEXT NOT NULL,
user_role_id INTEGER NOT NULL,
FOREIGN KEY (user_role_id) REFERENCES user_roles (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE product_titles 
(
id INTEGER PRIMARY KEY,
product_title TEXT NOT NULL,
category_id INTEGER NOT NULL,
FOREIGN KEY (product_category_id) REFERENCES product_categories (id) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE products
(
id INTEGER PRIMARY KEY,
product_title_id INTEGER NOT NULL,
manufacturer_id INTEGER NOT NULL,
unit_price REAL NOT NULL,
comment TEXT NOT NULL,
FOREIGN KEY (product_title_id) REFERENCES product_titles (id) ON DELETE CASCADE ON UPDATE NO ACTION,
FOREIGN KEY (product_manufacturer_id) REFERENCES product_manufacturers (id) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE order_states
(
id INTEGER PRIMARY KEY,
state_name TEXT NOT NULL
);

CREATE TABLE customer_orders 
(
id INTEGER PRIMARY KEY AUTOINCREMENT,
operation_time TEXT NOT NULL,
customer_id INTEGER NOT NULL,
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
FOREIGN KEY (product_id) REFERENCES shop_products (product_id) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (customer_order_id) REFERENCES customer_orders (customer_order_id) ON DELETE CASCADE ON UPDATE CASCADE
);

/*
INSERT INTO location_city VALUES (1, 'Kyiv', 'Ukraine');
INSERT INTO location_city VALUES (2, 'Vinnitsya', 'Ukraine');
INSERT INTO location_city VALUES (3, 'Kharkiv', 'Ukraine');
INSERT INTO location_city VALUES (4, 'London', 'United Kingdom');
INSERT INTO location_city VALUES (5, 'Lviv', 'Ukraine');
INSERT INTO locations VALUES (1, 'Arsenalnaya 122', 1);
INSERT INTO locations VALUES (2, 'Shevchenko 13B', 1);
INSERT INTO locations VALUES (3, 'Kyivskaya 177A', 2);
INSERT INTO locations VALUES (4, 'Magistrtskaya 8/A', 2);
INSERT INTO locations VALUES (5, 'Lesnaya 12', 3);
INSERT INTO locations VALUES (6, 'Zelenaya 5A', 3);
INSERT INTO locations VALUES (7, 'Krasnaya 10', 3);
INSERT INTO locations VALUES (8, 'Chornya 5A', 2);
INSERT INTO locations VALUES (9, 'Franka 89', 1);
INSERT INTO locations VALUES (10, 'Fantastichna 101', 1);
INSERT INTO locations VALUES (11, 'Fantastichna 10A', 1);
INSERT INTO locations VALUES (12, 'Shevchenko 12A', 1);
INSERT INTO locations VALUES (13, 'Kyivskaya 5A', 2);
INSERT INTO locations VALUES (14, 'Magistrtskaya 23/A', 2);
INSERT INTO locations VALUES (15, 'Lesnaya 23-A', 3);
INSERT INTO supermarkets VALUES (1, 'Best Choice');
INSERT INTO supermarkets VALUES (2, 'Smart Choice');
INSERT INTO supermarkets VALUES (3, 'Your Choice');
INSERT INTO supermarkets VALUES (4, 'Cheep Choice');
INSERT INTO supermarkets VALUES (5, 'Kramnichka');
INSERT INTO supermarkets VALUES (6, 'FastMart');
INSERT INTO supermarkets VALUES (7, 'Small market');
INSERT INTO supermarkets VALUES (8, 'Big mall');
INSERT INTO supermarkets VALUES (9, 'Nice mall');
INSERT INTO supermarkets VALUES (10, 'Metro');
INSERT INTO supermarkets VALUES (11, 'Swey mall');
INSERT INTO supermarkets VALUES (12, 'Boo mall');
INSERT INTO supermarket_locations VALUES (1, 1, 1);
INSERT INTO supermarket_locations VALUES (2, 1, 2);
INSERT INTO supermarket_locations VALUES (3, 2, 3);
INSERT INTO supermarket_locations VALUES (4, 2, 4);
INSERT INTO supermarket_locations VALUES (5, 3, 5);
INSERT INTO supermarket_locations VALUES (6, 4, 6);
INSERT INTO supermarket_locations VALUES (7, 5, 7);
INSERT INTO supermarket_locations VALUES (8, 6, 8);
INSERT INTO supermarket_locations VALUES (9, 7, 9);
INSERT INTO supermarket_locations VALUES (10, 8, 10);
INSERT INTO supermarket_locations VALUES (11, 9, 11);
INSERT INTO supermarket_locations VALUES (12, 10, 12);
INSERT INTO supermarket_locations VALUES (13, 11, 13);
INSERT INTO supermarket_locations VALUES (14, 12, 14);
INSERT INTO supermarket_locations VALUES (15, 12, 15);
INSERT INTO persons VALUES (1, 'Yevgen', 'Kojevskiy', '2000-01-01');
INSERT INTO persons VALUES (2, 'Yan', 'Petrenko', '2001-11-01');
INSERT INTO persons VALUES (3, 'Janna', 'Ivanova', '2002-12-12');
INSERT INTO persons VALUES (4, 'Denis', 'Galushko', '1995-10-01');
INSERT INTO persons VALUES (5, 'Filip', 'Kromvel', '1980-08-08');
INSERT INTO persons VALUES (6, 'Bogdan', 'Steciuk', '2002-07-01');
INSERT INTO persons VALUES (7, 'Yeva', 'Krasivaya', '1992-08-08');
INSERT INTO persons VALUES (8, 'Nadegda', 'Vesela', '1997-05-11');
INSERT INTO persons VALUES (9, 'Arsen', 'Govorliviy', '1990-11-21');
INSERT INTO persons VALUES (10, 'Mariya', 'Velikolepnaya', '1988-04-01');
INSERT INTO persons VALUES (11, 'Dasha', 'Ruda', '1987-10-01');
INSERT INTO persons VALUES (12, 'Bohdan', 'Chervoniy', '1950-12-01');
INSERT INTO persons VALUES (13, 'Ivan', 'Chorniy', '2000-13-01');
INSERT INTO persons VALUES (14, 'Vasyl', 'Anotin', '2001-12-12');
INSERT INTO persons VALUES (15, 'Leha', 'Katin', '1977-12-12');
INSERT INTO persons VALUES (16, 'Vadim', 'Patin', '1966-08-08');
INSERT INTO persons VALUES (17, 'Vova', 'Yellow', '1998-04-02');
INSERT INTO persons VALUES (18, 'Katya', 'Adushkina', '1999-03-04');
INSERT INTO persons VALUES (19, 'Roman', 'Valekiy', '2010-06-05');
INSERT INTO persons VALUES (20, 'Ivan', 'Paliy', '1994-08-07');
INSERT INTO persons VALUES (21, 'Dima', 'Visokiy', '1954-05-10');
INSERT INTO persons VALUES (22, 'Bohdan', 'Krasov', '1965-10-11');
INSERT INTO persons VALUES (23, 'Yarik', 'Shevchenko', '1996-12-04');
INSERT INTO persons VALUES (24, 'Stepan', 'Franko', '1999-10-02');
INSERT INTO persons VALUES (25, 'Sasha', 'Shevchuk', '1967-11-05');
INSERT INTO persons VALUES (26, 'Maksym', 'Ukranin', '1978-12-04');
INSERT INTO persons VALUES (27, 'Stepan', 'Opertan', '1998-04-05');
INSERT INTO persons VALUES (28, 'Anton', 'Antonov', '1998-04-01');
INSERT INTO persons VALUES (29, 'Artem', 'Artemov', '1999-06-02');
INSERT INTO persons VALUES (30, 'Vika', 'Bashynskaya', '2000-05-03');
INSERT INTO contact_types VALUES (1, 'phone');
INSERT INTO contact_types VALUES (2, 'email');
INSERT INTO contact_types VALUES (3, 'skype');
INSERT INTO contact_types VALUES (4, 'viber');
INSERT INTO person_contacts VALUES (1, 9, 1, '+38(077)123-45-67');
INSERT INTO person_contacts VALUES (2, 9, 2, 'user9@example.com');
INSERT INTO person_contacts VALUES (3, 10, 1, '+38(088)123-45-67');
INSERT INTO person_contacts VALUES (4, 10, 2, 'user10@example.com');
INSERT INTO person_contacts VALUES (5, 8, 1, '+38(099)123-45-67');
INSERT INTO person_contacts VALUES (6, 8, 2, 'user8@example.com');
INSERT INTO person_contacts VALUES (7, 7, 1, '+38(007)123-45-67');
INSERT INTO person_contacts VALUES (8, 7, 2, 'user7@example.com');
INSERT INTO person_contacts VALUES (9, 6, 1, '+38(066)123-45-67');
INSERT INTO person_contacts VALUES (10, 6, 2, 'user6@example.com');
INSERT INTO person_contacts VALUES (11, 5, 1, '+38(055)123-45-67');
INSERT INTO person_contacts VALUES (12, 5, 2, 'user5@example.com');
INSERT INTO person_contacts VALUES (13, 4, 1, '+38(044)123-45-67');
INSERT INTO person_contacts VALUES (14, 4, 2, 'user4@example.com');
INSERT INTO person_contacts VALUES (15, 3, 1, '+38(033)123-45-67');
INSERT INTO person_contacts VALUES (16, 3, 2, 'user3@example.com');
INSERT INTO person_contacts VALUES (17, 2, 1, '+38(022)123-45-67');
INSERT INTO person_contacts VALUES (18, 2, 2, 'user2@example.com');
INSERT INTO person_contacts VALUES (19, 1, 1, '+38(011)123-45-67');
INSERT INTO person_contacts VALUES (20, 1, 2, 'user1@example.com');
INSERT INTO person_contacts VALUES (21, 1, 3, 'skype1');
INSERT INTO person_contacts VALUES (22, 11, 1, 'user11@example.com');
INSERT INTO person_contacts VALUES (23, 11, 2, '+38(111)123-45-67');
INSERT INTO person_contacts VALUES (24, 12, 1, 'user12@example.com');
INSERT INTO person_contacts VALUES (25, 12, 2, '+38(112)123-45-67');
INSERT INTO person_contacts VALUES (26, 13, 1, 'user13@example.com');
INSERT INTO person_contacts VALUES (27, 13, 2, '+38(113)123-45-67');
INSERT INTO person_contacts VALUES (28, 14, 1, 'user14@example.com');
INSERT INTO person_contacts VALUES (29, 14, 2, '+38(114)123-45-67');
INSERT INTO person_contacts VALUES (30, 15, 1, 'user15@example.com');
INSERT INTO person_contacts VALUES (31, 15, 2, '+38(115)123-45-67');
INSERT INTO person_contacts VALUES (32, 16, 1, 'user16@example.com');
INSERT INTO person_contacts VALUES (33, 16, 2, '+38(116)123-45-67');
INSERT INTO person_contacts VALUES (34, 16, 4, 'viber16');
INSERT INTO person_contacts VALUES (35, 17, 1, 'user17@example.com');
INSERT INTO person_contacts VALUES (36, 17, 2, '+38(117)123-45-67');
INSERT INTO person_contacts VALUES (37, 17, 3, 'skype17');
INSERT INTO person_contacts VALUES (38, 18, 1, 'user18@example.com');
INSERT INTO person_contacts VALUES (39, 18, 2, '+38(118)123-45-67');
INSERT INTO person_contacts VALUES (40, 18, 3, 'skype18');
INSERT INTO person_contacts VALUES (41, 19, 1, 'user19@example.com');
INSERT INTO person_contacts VALUES (42, 19, 2, '+38(119)123-45-67');
INSERT INTO person_contacts VALUES (43, 19, 3, 'skype19');
INSERT INTO person_contacts VALUES (44, 20, 1, 'user20@example.com');
INSERT INTO person_contacts VALUES (45, 21, 1, 'user21@example.com');
INSERT INTO person_contacts VALUES (46, 22, 1, 'user22@example.com');
INSERT INTO person_contacts VALUES (47, 23, 1, 'user23@example.com');
INSERT INTO person_contacts VALUES (48, 24, 1, 'user24@example.com');
INSERT INTO person_contacts VALUES (49, 25, 1, 'user25@example.com');
INSERT INTO person_contacts VALUES (50, 26, 1, 'user26@example.com');
INSERT INTO person_contacts VALUES (51, 27, 1, 'user27@example.com');
INSERT INTO person_contacts VALUES (52, 28, 1, 'user28@example.com');
INSERT INTO person_contacts VALUES (53, 29, 1, 'user29@example.com');
INSERT INTO person_contacts VALUES (54, 30, 1, 'user30@example.com');

INSERT INTO customers VALUES (1, 921212121, 0.10);
INSERT INTO customers VALUES (2, 935456021, 0.02);
INSERT INTO customers VALUES (3, 909090283, 0.10);
INSERT INTO customers VALUES (4, 120985320, 0.02);
INSERT INTO customers VALUES (5, 437238943, 0.10);
INSERT INTO customers VALUES (6, 129034871, 0.02);
INSERT INTO customers VALUES (7, 438927489, 0.05);
INSERT INTO customers VALUES (8, 321794012, 0.05);
INSERT INTO customers VALUES (9, 218324131, 0);
INSERT INTO customers VALUES (10, 423443222, 0);
INSERT INTO customers VALUES (11, 4324236, 0);
INSERT INTO customers VALUES (12, 34912748, 0);
INSERT INTO customers VALUES (13, 231893122, 0.10);
INSERT INTO customers VALUES (14, 2130931293, 0.10);
INSERT INTO customers VALUES (15, 23418941894, 0.05);
INSERT INTO customers VALUES (16, 2341034128931, 0.05);
INSERT INTO customers VALUES (17, 231983189, 0.05);
INSERT INTO customers VALUES (18, 213839189321, 0.05);
INSERT INTO customers VALUES (19, 23108932189, 0.05);
INSERT INTO customers VALUES (20, 64893589, 0.05);

INSERT INTO product_categories VALUES (1, 'fruits');
INSERT INTO product_categories VALUES (2, 'drinks');
INSERT INTO product_categories VALUES (3, 'vegetables');
INSERT INTO product_categories VALUES (4, 'fish');
INSERT INTO product_categories VALUES (5, 'meet');
INSERT INTO product_categories VALUES (6, 'grocery');

INSERT INTO product_manufacturers VALUES (1, 'First manufacturer');
INSERT INTO product_manufacturers VALUES (2, 'Second manufacturer');
INSERT INTO product_manufacturers VALUES (3, 'Third manufacturer');
INSERT INTO product_manufacturers VALUES (4, 'Fourth manufacturer');
INSERT INTO product_manufacturers VALUES (5, 'Useless1 manufacturer');
INSERT INTO product_manufacturers VALUES (6, 'Useless2 manufacturer');

INSERT INTO product_suppliers VALUES (1, 'First supplier');

INSERT INTO product_titles VALUES (1, 'Red Apple', 1);
INSERT INTO product_titles VALUES (2, 'Banana', 1);
INSERT INTO product_titles VALUES (3, 'Orange', 1);
INSERT INTO product_titles VALUES (4, 'Water', 2);
INSERT INTO product_titles VALUES (5, 'Juice', 2);
INSERT INTO product_titles VALUES (6, 'Cola', 2);
INSERT INTO product_titles VALUES (7, 'Carrot', 3);
INSERT INTO product_titles VALUES (8, 'Potato', 3);
INSERT INTO product_titles VALUES (9, 'Cabbage', 3);
INSERT INTO product_titles VALUES (10, 'Tomato', 3);
INSERT INTO product_titles VALUES (11, 'Lemon', 1);
INSERT INTO product_titles VALUES (12, 'Beer', 2);
INSERT INTO product_titles VALUES (13, 'Cod', 4);
INSERT INTO product_titles VALUES (14, 'Pike', 4);
INSERT INTO product_titles VALUES (15, 'Carp', 4);
INSERT INTO product_titles VALUES (16, 'Beer', 2);
INSERT INTO product_titles VALUES (17, 'Strawberry0', 1);
INSERT INTO product_titles VALUES (18, 'Blueberry', 1);
INSERT INTO product_titles VALUES (19, 'Onion', 3);
INSERT INTO product_titles VALUES (20, 'Shark', 4);


INSERT INTO shop_products VALUES (1, 1, 1, 1, 10.11, 'Sweet apple');
INSERT INTO shop_products VALUES (2, 1, 2, 1, 5.05, 'Small apple');
INSERT INTO shop_products VALUES (3, 2, 3, 1, 15.0, 'African Banana');
INSERT INTO shop_products VALUES (4, 2, 4, 1, 20.0, 'Brazilian Banana');
INSERT INTO shop_products VALUES (5, 3, 1, 1, 16.2, 'Ukrainian Orange');
INSERT INTO shop_products VALUES (6, 3, 2, 1, 12.0, 'Italian Orange');
INSERT INTO shop_products VALUES (7, 4, 3, 1, 13.0, 'Clear water');
INSERT INTO shop_products VALUES (8, 4, 4, 1, 50.0, 'Sweet water');
INSERT INTO shop_products VALUES (9, 5, 1, 1, 100.0, 'Orange Juice');
INSERT INTO shop_products VALUES (10, 5, 2, 1, 120.0, 'Banana Juice');
INSERT INTO shop_products VALUES (11, 6, 3, 1, 60.0, 'Coca Cola');
INSERT INTO shop_products VALUES (12, 6, 4, 1, 5.5, 'Pepsi Cola');
INSERT INTO shop_products VALUES (13, 7, 1, 1, 12.23, 'Big carrot');
INSERT INTO shop_products VALUES (14, 7, 2, 1, 10.02, 'Small carrot');
INSERT INTO shop_products VALUES (15, 8, 3, 1, 89.0, 'big potato');
INSERT INTO shop_products VALUES (16, 8, 1, 1, 88.99, 'ukrainian potato');
INSERT INTO shop_products VALUES (17, 9, 4, 1, 32.0, 'Green Cabbage');
INSERT INTO shop_products VALUES (18, 9, 1, 1, 33.33, 'Typical cabbage');
INSERT INTO shop_products VALUES (19, 10, 2, 1, 44.44, 'Yellow tomato');
INSERT INTO shop_products VALUES (20, 10, 3, 1, 55.55, 'Pink tomato');
INSERT INTO shop_products VALUES (21, 15, 4, 1, 10.11, 'blue carp');
INSERT INTO shop_products VALUES (22, 15, 1, 1, 5.05, 'big carp');
INSERT INTO shop_products VALUES (23, 16, 2, 1, 15.0, 'ocean shark');
INSERT INTO shop_products VALUES (24, 16, 3, 1, 20.0, 'white shark');
INSERT INTO shop_products VALUES (25, 16, 4, 1, 16.2, 'orange shark');
INSERT INTO shop_products VALUES (26, 16, 1, 1, 12.0, 'red shark');
INSERT INTO shop_products VALUES (27, 15, 2, 1, 13.0, 'carpik');
INSERT INTO shop_products VALUES (28, 15, 3, 1, 50.0, 'small carp');
INSERT INTO shop_products VALUES (29, 5, 4, 1, 100.0, 'Orange Juice');
INSERT INTO shop_products VALUES (30, 5, 3, 1, 120.0, 'Banana Juice');
INSERT INTO shop_products VALUES (31, 6, 2, 1, 60.0, 'Coca Cola');
INSERT INTO shop_products VALUES (32, 6, 1, 1, 5.5, 'Pepsi Cola');
INSERT INTO shop_products VALUES (33, 7, 4, 1, 12.23, 'Big carrot');
INSERT INTO shop_products VALUES (34, 7, 3, 1, 10.02, 'Small carrot');
INSERT INTO shop_products VALUES (35, 8, 2, 1, 89.0, 'big potato');
INSERT INTO shop_products VALUES (36, 8, 4, 1, 88.99, 'ukrainian potato');
INSERT INTO shop_products VALUES (37, 9, 2, 1, 32.0, 'Green Cabbage');
INSERT INTO shop_products VALUES (38, 9, 3, 1, 33.33, 'Typical cabbage');
INSERT INTO shop_products VALUES (39, 10, 1, 1, 44.44, 'Yellow tomato');
INSERT INTO shop_products VALUES (40, 10, 4, 1, 55.55, 'Pink tomato');
INSERT INTO shop_products VALUES (41, 1, 4, 1, 5.5, 'Sweet apple');
INSERT INTO shop_products VALUES (42, 1, 3, 1, 12.23, 'Russian apple');
INSERT INTO shop_products VALUES (43, 2, 2, 1, 10.02, 'green banana but good');
INSERT INTO shop_products VALUES (44, 2, 1, 1, 89.0, 'african banana');
INSERT INTO shop_products VALUES (45, 3, 4, 1, 88.99, 'italian orange');
INSERT INTO shop_products VALUES (46, 3, 2, 1, 32.0, 'greece orange');
INSERT INTO shop_products VALUES (47, 4, 2, 1, 33.33, 'crystal water');
INSERT INTO shop_products VALUES (48, 4, 1, 1, 44.44, 'soda water');

INSERT INTO customer_orders VALUES (1, '2020-01-01', 1, 1);
INSERT INTO customer_orders VALUES (2, '2020-02-02', 2, 2);
INSERT INTO customer_orders VALUES (3, '2020-02-02', 3, 3);
INSERT INTO customer_orders VALUES (4, '2020-03-03', 4, 4);
INSERT INTO customer_orders VALUES (5, '2020-04-04', 5, 5);
INSERT INTO customer_orders VALUES (6, '2020-05-05', 6, 6);
INSERT INTO customer_orders VALUES (7, '2020-06-06', 7, 7);
INSERT INTO customer_orders VALUES (8, '2020-07-07', 1, 8);
INSERT INTO customer_orders VALUES (9, '2020-08-08', 2, 9);
INSERT INTO customer_orders VALUES (10, '2020-09-11', 3, 10);
INSERT INTO customer_orders VALUES (11, '2020-10-10', 4, 11);
INSERT INTO customer_orders VALUES (12, '2020-11-11', 5, 12);
INSERT INTO customer_orders VALUES (13, '2021-01-01', 6, 13);
INSERT INTO customer_orders VALUES (14, '2021-01-01', 7, 14);
INSERT INTO customer_orders VALUES (15, '2022-01-01', 8, 15);
INSERT INTO customer_orders VALUES (16, '2022-01-01', 9, 1);
INSERT INTO customer_orders VALUES (17, '2021-01-01', 11, 2);
INSERT INTO customer_orders VALUES (18, '2019-01-01', 12, 3);
INSERT INTO customer_orders VALUES (19, '2017-01-01', 13, 4);
INSERT INTO customer_orders VALUES (20, '2018-01-01', 14, 5);
INSERT INTO customer_orders VALUES (21, '2016-02-02', 1, 6);
INSERT INTO customer_orders VALUES (22, '2019-03-03', 2, 7);
INSERT INTO customer_orders VALUES (23, '2019-02-02', 3, 8);
INSERT INTO customer_orders VALUES (24, '2019-03-03', 4, 9);
INSERT INTO customer_orders VALUES (25, '2022-02-02', 5, 10);
INSERT INTO customer_orders VALUES (26, '2022-02-02', 6, 11);
INSERT INTO customer_orders VALUES (27, '2022-01-01', 7, 1);
INSERT INTO customer_orders VALUES (28, '2021-10-10', 11, 2);
INSERT INTO customer_orders VALUES (29, '2022-03-03', 12, 3);
INSERT INTO customer_orders VALUES (30, '2022-03-13', 13, 4);
INSERT INTO customer_orders VALUES (31, '2021-10-03', 4, 5);
INSERT INTO customer_orders VALUES (32, '2020-11-11', 5, 1);
INSERT INTO customer_orders VALUES (33, '2021-01-01', 6, 2);
INSERT INTO customer_orders VALUES (34, '2021-01-01', 7, 3);
INSERT INTO customer_orders VALUES (35, '2022-01-01', 11, 10);
INSERT INTO customer_orders VALUES (36, '2022-01-01', 12, 1);
INSERT INTO customer_orders VALUES (37, '2021-01-01', 2, 6);
INSERT INTO customer_orders VALUES (38, '2019-01-01', 13, 7);
INSERT INTO customer_orders VALUES (39, '2017-01-01', 14, 8);
INSERT INTO customer_orders VALUES (40, '2018-01-01', 15, 9);
INSERT INTO customer_orders VALUES (41, '2022-01-01', 2, 10);
INSERT INTO customer_orders VALUES (42, '2021-01-01', 2, 11);
INSERT INTO customer_orders VALUES (43, '2019-01-01', 3, 12);
INSERT INTO customer_orders VALUES (44, '2017-01-01', 4, 13);
INSERT INTO customer_orders VALUES (45, '2018-01-01', 5, 14);
INSERT INTO customer_orders VALUES (46, '2022-12-11', 2, NULL);
INSERT INTO customer_orders VALUES (47, '2021-12-11', 2, NULL);
INSERT INTO customer_orders VALUES (48, '2019-10-11', 3, NULL);
INSERT INTO customer_orders VALUES (49, '2017-09-10', 4, NULL);
INSERT INTO customer_orders VALUES (50, '2018-07-08', 5, NULL);

INSERT INTO customer_order_details VALUES (1, 1, 1, 20, 5, 4);
INSERT INTO customer_order_details VALUES (2, 2, 2, 80, 10, 5);
INSERT INTO customer_order_details VALUES (3, 3, 3, 90, 10, 6);
INSERT INTO customer_order_details VALUES (4, 4, 4, 30, 5, 2);
INSERT INTO customer_order_details VALUES (5, 5, 5, 40, 5, 1);
INSERT INTO customer_order_details VALUES (6, 6, 6, 50, 5, 3);
INSERT INTO customer_order_details VALUES (7, 7, 7, 60, 5, 11);
INSERT INTO customer_order_details VALUES (8, 8, 8, 70, 5, 13);
INSERT INTO customer_order_details VALUES (9, 9, 9, 80, 5, 14);
INSERT INTO customer_order_details VALUES (10, 10, 10, 90, 2, 2);
INSERT INTO customer_order_details VALUES (11, 11, 11, 120, 3, 1);
INSERT INTO customer_order_details VALUES (12, 12, 12, 130, 50, 5);
INSERT INTO customer_order_details VALUES (13, 13, 13, 40, 8, 6);
INSERT INTO customer_order_details VALUES (14, 14, 14, 50, 7, 7);
INSERT INTO customer_order_details VALUES (15, 15, 15, 33, 6, 8);
INSERT INTO customer_order_details VALUES (16, 16, 16, 44, 5, 10);
INSERT INTO customer_order_details VALUES (17, 16, 16, 55, 4, 4);
INSERT INTO customer_order_details VALUES (18, 17, 17, 66, 3, 5);
INSERT INTO customer_order_details VALUES (19, 17, 20, 77, 11, 3);
INSERT INTO customer_order_details VALUES (20, 18, 19, 88, 5, 4);
INSERT INTO customer_order_details VALUES (21, 18, 20, 99, 5, 4);
INSERT INTO customer_order_details VALUES (22, 19, 21, 80, 10, 5);
INSERT INTO customer_order_details VALUES (23, 19,22, 90, 10, 6);
INSERT INTO customer_order_details VALUES (24, 20, 23, 110, 5, 9);
INSERT INTO customer_order_details VALUES (25, 20, 24, 120, 5, 8);
INSERT INTO customer_order_details VALUES (26, 21, 25, 130, 5, 12);
INSERT INTO customer_order_details VALUES (27, 21, 26, 140, 5, 11);
INSERT INTO customer_order_details VALUES (28, 22, 27, 50, 5, 13);
INSERT INTO customer_order_details VALUES (29,22, 28, 60, 5, 3);
INSERT INTO customer_order_details VALUES (30, 23, 29, 300, 5, 4);
INSERT INTO customer_order_details VALUES (31, 23, 30, 40, 5, 5);
INSERT INTO customer_order_details VALUES (32, 24, 31, 144, 5, 6);
INSERT INTO customer_order_details VALUES (33, 24, 32, 120, 5, 7);
INSERT INTO customer_order_details VALUES (34, 25, 33, 122, 5, 8);
INSERT INTO customer_order_details VALUES (35, 25, 34, 134, 100, 9);
INSERT INTO customer_order_details VALUES (36, 26, 35, 153, 110, 1);
INSERT INTO customer_order_details VALUES (37, 26, 36, 50, 23, 1);
INSERT INTO customer_order_details VALUES (38, 27, 37, 32, 21, 1);
INSERT INTO customer_order_details VALUES (39, 27, 38, 30, 29, 3);
INSERT INTO customer_order_details VALUES (40, 28, 39, 40, 39, 4);
INSERT INTO customer_order_details VALUES (41, 28, 40, 50, 40, 5);
INSERT INTO customer_order_details VALUES (42, 28, 41, 80, 10, 6);
INSERT INTO customer_order_details VALUES (43, 29, 42, 90, 10, 7);
INSERT INTO customer_order_details VALUES (44, 29, 43, 200, 100, 10);
INSERT INTO customer_order_details VALUES (45, 29,44, 100, 50, 3);
INSERT INTO customer_order_details VALUES (46, 30, 45, 20, 5, 4);
INSERT INTO customer_order_details VALUES (47, 30, 45, 34, 30, 11);
INSERT INTO customer_order_details VALUES (48, 30, 8, 50, 40, 13);
INSERT INTO customer_order_details VALUES (49, 31, 9, 50, 50, 14);
INSERT INTO customer_order_details VALUES (50, 31, 10, 60, 50, 2);
INSERT INTO customer_order_details VALUES (51, 31, 11, 70, 50, 1);
INSERT INTO customer_order_details VALUES (52, 32, 12, 80, 50, 4);
INSERT INTO customer_order_details VALUES (53, 32, 13, 90, 50, 5);
INSERT INTO customer_order_details VALUES (54, 32, 14, 111, 50, 12);
INSERT INTO customer_order_details VALUES (55, 33, 15, 112, 100, 20);
INSERT INTO customer_order_details VALUES (56, 33, 16, 10, 5, 10);
INSERT INTO customer_order_details VALUES (57, 33, 17, 20, 10, 23);
INSERT INTO customer_order_details VALUES (58, 34, 18, 30, 20, 1);
INSERT INTO customer_order_details VALUES (59, 34, 19, 40, 30, 2);
INSERT INTO customer_order_details VALUES (60, 34, 20, 50, 40, 3);
INSERT INTO customer_order_details VALUES (61, 35, 21, 60, 50, 6);
INSERT INTO customer_order_details VALUES (62, 35, 22, 80, 30, 8);
INSERT INTO customer_order_details VALUES (63, 35, 23, 90, 40, 9);
INSERT INTO customer_order_details VALUES (64, 36, 24, 5, 2, 10);
INSERT INTO customer_order_details VALUES (65, 36, 25, 12, 10, 2);
INSERT INTO customer_order_details VALUES (66, 36, 26, 11, 10, 3);
INSERT INTO customer_order_details VALUES (67, 37, 27, 13, 5, 6);
INSERT INTO customer_order_details VALUES (68, 37, 28, 14, 5, 7);
INSERT INTO customer_order_details VALUES (69, 37, 29, 15, 5, 8);
INSERT INTO customer_order_details VALUES (70, 38, 30, 21, 20, 15);
INSERT INTO customer_order_details VALUES (71, 38, 31, 31, 20, 1);
INSERT INTO customer_order_details VALUES (72, 38, 32, 41, 20, 2);
INSERT INTO customer_order_details VALUES (73, 39, 33, 50, 30, 5);
INSERT INTO customer_order_details VALUES (74, 39, 34, 60, 50, 4);
INSERT INTO customer_order_details VALUES (75, 39, 35, 23, 20, 6);
INSERT INTO customer_order_details VALUES (76, 39, 36, 41, 30, 7);
INSERT INTO customer_order_details VALUES (77, 40, 37, 51, 30, 8);
INSERT INTO customer_order_details VALUES (78, 40, 38, 93, 80, 4);
INSERT INTO customer_order_details VALUES (79, 40, 39, 21, 15, 5);
INSERT INTO customer_order_details VALUES (80, 40, 40, 34, 20, 6);
INSERT INTO customer_order_details VALUES (81, 41, 41, 56, 50, 7);
INSERT INTO customer_order_details VALUES (82, 41, 42, 80, 50, 8);
INSERT INTO customer_order_details VALUES (83, 41, 43, 90, 70, 2);
INSERT INTO customer_order_details VALUES (84, 41, 44, 65, 50, 3);
INSERT INTO customer_order_details VALUES (85, 42, 45, 122, 100, 4);
INSERT INTO customer_order_details VALUES (86, 42, 6, 134, 100, 12);
INSERT INTO customer_order_details VALUES (87, 42, 7, 155, 100, 11);
INSERT INTO customer_order_details VALUES (88, 42, 8, 166, 100, 13);
INSERT INTO customer_order_details VALUES (89, 42, 9, 1, 0, 14);
INSERT INTO customer_order_details VALUES (90, 43, 10, 3, 0, 15);
INSERT INTO customer_order_details VALUES (91, 43, 11, 4, 0, 1);
INSERT INTO customer_order_details VALUES (92, 43, 12, 54, 0, 5);
INSERT INTO customer_order_details VALUES (93, 43, 13, 34, 0, 6);
INSERT INTO customer_order_details VALUES (94, 44, 14, 87, 0, 7);
INSERT INTO customer_order_details VALUES (95, 44, 15, 15, 0, 2);
INSERT INTO customer_order_details VALUES (96, 44, 16, 14, 0, 3);
INSERT INTO customer_order_details VALUES (97, 44, 17, 123, 0, 1);
INSERT INTO customer_order_details VALUES (98, 45, 18, 11, 0, 1);
INSERT INTO customer_order_details VALUES (99, 46, 14, 87, 0, 7);
INSERT INTO customer_order_details VALUES (100, 47, 15, 15, 0, 2);
INSERT INTO customer_order_details VALUES (101, 48, 16, 14, 0, 3);
INSERT INTO customer_order_details VALUES (102, 49, 17, 123, 0, 1);
INSERT INTO customer_order_details VALUES (103, 50, 18, 11, 0, 1);
*/