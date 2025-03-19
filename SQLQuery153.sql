-- Sử dụng database WebAppDB
USE WebAppDB;
GO

-- 1. Tạo bảng Users
--Tạo bảng Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
	Email NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User'
);
GO
DROP TABLE Users;
-- 2. Thêm dữ liệu cho Users
INSERT INTO Users (Username, Email, PasswordHash, Role)
VALUES 
('admin', 'thuylien2k4@gmail.com','1', 'Admin'),
('user1', 'lien@gmail.com','1', 'User');
GO

-- 3. Tạo bảng SanPham
CREATE TABLE SanPham (
    SanPhamID INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(100) NOT NULL,
    Gia DECIMAL(18,2) NOT NULL,
    GiaGiam DECIMAL(18,2) NULL,
    MoTa NVARCHAR(MAX) NULL,
    HinhAnh NVARCHAR(255) NULL,
    Loai NVARCHAR(50) NULL,
    GiamGia INT NULL
);
GO

-- 4. Thêm dữ liệu mẫu vào bảng SanPham
INSERT INTO SanPham (TenSanPham, Gia, GiaGiam, MoTa, HinhAnh, Loai, GiamGia)
VALUES
(N'Sweatshirt 2018 Off Shoulder', 110.00, 130.00, N'Sản phẩm áo sơ mi thời trang', 'images/product1.png', N'Áo', 15),
(N'Simple product', 50.00, NULL, N'Sản phẩm camera đơn giản', 'images/product2.png', N'Camera', NULL),
(N'Super stereo earbuds', 110.00, 130.00, N'Tai nghe stereo cao cấp', 'images/product3.png', N'Tai nghe', 15),
(N'Headset stereo headphones', 39.00, NULL, N'Tai nghe chụp tai stereo', 'images/product4.png', N'Tai nghe', NULL),
(N'New badger product', 80.00, NULL, N'Mũ lưỡi trai thời trang', 'images/product5.png', N'Mũ', NULL),
(N'Affiliate Product', 29.00, NULL, N'Áo khoác thể thao', 'images/product6.png', N'Áo khoác', NULL),
(N'Engagement rings for women', 110.00, 130.00, N'Nhẫn đính hôn cho nữ', 'images/product7.png', N'Trang sức', 15),
(N'Man concise classical', 80.00, NULL, N'Dây chuyền nam phong cách', 'images/product8.png', N'Trang sức', NULL);
GO



-- 7. Kiểm tra dữ liệu
SELECT * FROM Users;
SELECT * FROM SanPham;
GO


-- Table: TableFood
CREATE TABLE TableFood(
    TableId INT IDENTITY PRIMARY KEY,
    TableName NVARCHAR(100),
    TrangThai NVARCHAR(100) -- Trống, có khách, đã được đặt
);
GO

-- Table: AccRole
CREATE TABLE AccRole(
    RoleId INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL
);
GO


-- Table: Account
CREATE TABLE Account (
    AccountId INT IDENTITY PRIMARY KEY,
    DisplayName NVARCHAR(100) NOT NULL,
    UserName NVARCHAR(50) NOT NULL,
    PassWord NVARCHAR(50) NOT NULL,
    RoleName NVARCHAR(50) not null
);
GO

-- Table: Category
CREATE TABLE Category (
    CategoryId INT PRIMARY KEY IDENTITY,
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- Table: Ingredient
CREATE TABLE Ingredient (
    IngredientId INT PRIMARY KEY IDENTITY,
    IngredientName NVARCHAR(100) NOT NULL,
    SoLuong INT NOT NULL DEFAULT 0,
	PhanLoai Nvarchar(50) not null,
	ImageURL VARCHAR(255) NOT NULL,
    LastUpdated DATE NOT NULL DEFAULT GETDATE()
);
GO

-- Table: Food
CREATE TABLE Food (
    FoodId INT PRIMARY KEY IDENTITY,
    FoodName NVARCHAR(100) NOT NULL,
    CategoryId INT,
    IngredientId INT,
    Price DECIMAL(18, 3) NOT NULL DEFAULT 0,
	ImageURL VARCHAR(255) NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    FOREIGN KEY (IngredientId) REFERENCES Ingredient(IngredientId)
);
GO
SELECT *FROM FOOD
-- Table: Invoice
CREATE TABLE Invoice (
    InvoiceId INT PRIMARY KEY IDENTITY,
    TableId INT,
    DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
    DateCheckOut DATE,
    TrangThai INT, -- 1 là thanh toán, 0 là chưa thanh toán

    FOREIGN KEY (TableId) REFERENCES TableFood(TableId)
);
GO



-- Table: InvoiceDetail
CREATE TABLE InvoiceDetail(
    InvoiceDetailId INT PRIMARY KEY IDENTITY,
    InvoiceId INT,
    FoodId INT,
    SoLuong INT NOT NULL DEFAULT 0,
    Price DECIMAL(18, 3) NOT NULL,
    FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId),
    FOREIGN KEY (FoodId) REFERENCES Food(FoodId)
);
GO

-- Table: Staff
CREATE TABLE Staff (
    StaffId INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    DateOfBirth DATE NULL,
    Email NVARCHAR(50) NULL,
    Gender NVARCHAR(50) null,
    AccountId INT,
    RoleId INT,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId),
    FOREIGN KEY (RoleId) REFERENCES AccRole(RoleId)
);
GO
UPDATE Staff
SET Gender = CASE 
    WHEN Gender = 'True' THEN 'Nam'
    WHEN Gender = 'False' THEN 'Nữ'
END






-- Table: Warehouse
CREATE TABLE Warehouse (
    WarehouseId INT PRIMARY KEY IDENTITY,
    IngredientId INT,
    SoLuong INT NOT NULL DEFAULT 0,
    DateUpdate DATE NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (IngredientId) REFERENCES Ingredient(IngredientId)
);
GO



-- nhieu nguyen lieu
CREATE TABLE FoodIngredient (
    FoodIngredientId INT PRIMARY KEY IDENTITY,
    FoodId INT,
    IngredientId INT,
    Quantity INT NOT NULL, -- Số lượng nguyên liệu cho món ăn này
    FOREIGN KEY (FoodId) REFERENCES Food(FoodId),
    FOREIGN KEY (IngredientId) REFERENCES Ingredient(IngredientId)
);
GO

INSERT INTO TableFood (TableName, TrangThai)
VALUES
('Table 1', N'Đã có khách'),
('Table 2', N'Đã có khách'),
('Table 3', N'Bàn Trống'),
('Table 4', N'Bàn Trống')
,('Table 5', N'Đã có khách'),
('Table 6', N'Bàn Trống'),
('Table 7', N'Bàn Trống'),
('Table 8', N'Đã có khách'),
('Table 9', N'Bàn Trống'),
('Table 10', N'Bàn Trống'),
('Table 11', N'Bàn Trống'),
('Table 12', N'Bàn Trống');

INSERT INTO AccRole (RoleName)
VALUES
(N'Quản lý'),
(N'Pha chế'),
(N'Phục vụ');

INSERT INTO Account (DisplayName, UserName, PassWord, RoleName)
VALUES
('Admin ', 'admin', '1', 'Admin');



INSERT INTO Category (CategoryName)
VALUES
(N'Cà phê'),
(N'Trà sữa'),
(N'Thức uống đá xay'),
(N'Bánh & Snack'),
(N'Trà trái cây');


INSERT INTO Ingredient (IngredientName,SoLuong,PhanLoai, ImageURl, LastUpdated)
VALUES

(N'Coffee', 100, N'gói', '/img/nguyenlieu/coffee.png', '2024-08-05'),
(N'Sữa', 500, N'chai', '/img/nguyenlieu/sua.png', '2024-08-05'),
(N'Đường', 200, N'gói', '/img/nguyenlieu/duong.png', '2024-08-05'),
(N'Trà', 75, N'gói', '/img/nguyenlieu/tra.png', '2024-08-05'),
(N'Táo', 50, N'quả', '/img/nguyenlieu/tao.png', '2024-08-05'),
(N'Trân châu', 100, N'túi', '/img/nguyenlieu/tranchau.png', '2024-08-05'),
(N'Matcha', 120, N'gói', '/img/nguyenlieu/matcha.png', '2024-08-05'),
(N'Yến mạch', 130, N'gói', '/img/nguyenlieu/yenmach.png', '2024-08-05'),
(N'Caramel', 140, N'lọ', '/img/nguyenlieu/caramel.png', '2024-08-05'),
(N'Muối', 125, N'gói', '/img/nguyenlieu/muoi.png', '2024-08-05'),
(N'Hạnh nhân', 115, N'lọ', '/img/nguyenlieu/hanhnhan.png', '2024-08-05'),
(N'Bơ', 120, N'quả', '/img/nguyenlieu/bo.png', '2024-08-05'),
(N'Kem', 150, N'hộp', '/img/nguyenlieu/kem.png', '2024-08-05'),
(N'Choco Chip', 130, N'gói', '/img/nguyenlieu/choco_chip.png', '2024-08-05'),
(N'Mochi kem phúc bồn tử', 120, N'cái', '/img/nguyenlieu/mochi_kpbt.png', '2024-08-05'),
(N'Mochi kem việt quất', 120, N'cái', '/img/nguyenlieu/mochi_kvq.png', '2024-08-05'),
(N'Mochi kem chocolate', 120, N'cái', '/img/nguyenlieu/mochi_kemchoco.png', '2024-08-05'),
(N'Mousse gấu chocolate', 110, N'cái', '/img/nguyenlieu/mousse_gau.png', '2024-08-05'),
(N'Bánh mỳ', 150, N'ổ', '/img/nguyenlieu/banhmy.png', '2024-08-05'),
(N'Sữa đặc', 160, N'hộp', '/img/nguyenlieu/suadac.png', '2024-08-05'),
(N'Cam', 135, N'quả', '/img/nguyenlieu/cam.png', '2024-08-05'),
(N'Sả', 125, N'cây', '/img/nguyenlieu/sa.png', '2024-08-05'),
(N'Hạt sen', 115, N'túi', '/img/nguyenlieu/hatsen.png', '2024-08-05'),
(N'Vải', 140, N'quả', '/img/nguyenlieu/vai.png', '2024-08-05'),
(N'Mứt Yuzu', 120, N'lọ', '/img/nguyenlieu/yuzu.png', '2024-08-05'),
(N'Đào', 120, N'hộp', '/img/nguyenlieu/dao.png', '2024-08-05'),
(N'Bánh Gấu', 120, N'gói', '/img/nguyenlieu/banhgau.png', '2024-08-05'),
(N'Sương Sáo', 120, N'cốc', '/img/nguyenlieu/suongsao.png', '2024-08-05'),
(N'Dâu', 120, N'quả', '/img/nguyenlieu/dau.png', '2024-08-05'),
(N'Bim Bim Ngô', 120, N'gói', '/img/nguyenlieu/bimbimngo.png', '2024-08-05'),
(N'Bim Bim Sữa Dừa', 120, N'gói', '/img/nguyenlieu/bimbimsuadua.png', '2024-08-05');


INSERT INTO Food (FoodName, CategoryId, IngredientId, Price,ImageURL)
VALUES
(N'Trà xanh espresso marble', 1, 1, 45000,'/img/Cafe/traxanhespresso.png'),
(N'Bạc xỉu lắc sữa yến mạch', 1, 1, 50000,'/img/Cafe/bacxiulsyenmach.png'),
(N'Bạc xỉu lắc caramel muối', 1, 1, 55000,'/img/Cafe/bacxiulacmuoi.png'),
(N'Bạc xỉu lắc hạnh nhân nướng', 1, 1, 55000,'/img/Cafe/bacxiulachanhnhan.png'),
(N'Bơ arabica', 1, 1, 60000,'/img/Cafe/bo_arabica.png'),
(N'Đường đen sữa đá', 1, 1, 30000,'/img/Cafe/duongdensuada.png'),
(N'Cà phê sữa đá', 1, 1, 25000,'/img/Cafe/cafesuada.png'),
(N'Cà phê sữa nóng', 1, 1, 25000,'/img/Cafe/cafesuanong.png'),
(N'Bạc xỉu', 1, 1, 30000,'/img/Cafe/bacxiu.png'),
(N'Cà phê đen', 1, 1, 20000,'/img/Cafe/cafeden.png'),


(N'Trà sữa trân châu đường đen', 2, 2, 35000,'/img/trasua/tstcduongden.png'),
(N'Trà sữa olong', 2, 2, 30000,'/img/trasua/ts_olong.png'),
(N'Trà sữa olong tứ quý bơ', 2, 2, 35000,'/img/trasua/ts_olongtqbo.png'),
(N'Trà sữa olong nướng sương sáo', 2, 2, 35000,'/img/trasua/ts_olongss.png'),
(N'Trà đen macchito', 2, 2, 30000,'/img/trasua/tradenmacchiato.png'),
(N'Hồng trà sữa trân châu', 2, 2, 30000,'/img/trasua/hongtrasua.png'),

(N'Frosty phin-gato', 3, 3, 40000,'/img/tudx/frosty_phin.png'),
(N'Frosty cà phê đường đen', 3, 3, 40000,'/img/tudx/frosty_cfduongden.png'),
(N'Frosty bánh kem dâu', 3, 3, 45000,'/img/tudx/frosty_banhkemdau.png'),
(N'Frosty choco chip', 3, 3, 45000,'/img/tudx/frosty_choco.png'),
(N'Frosty caramel', 3, 3, 45000,'/img/tudx/frosty_caramel.png'),

(N'Butter croissant', 4, 4, 25000,'/img/banh/Butter_croissant.png'),
(N'Mochi kem phúc bồn tử', 4, 4, 30000,'/img/banh/mochi_phucbt.png'),
(N'Mochi kem việt quất', 4, 4, 30000,'/img/banh/mochi_vietquat.png'),
(N'Mochi kem chocolate', 4, 4, 30000,'/img/banh/mochi_kemchocolate.png'),
(N'Mousse gấu chocolate', 4, 4, 35000,'/img/banh/mousse_gauchoco.png'),
(N'Bánh mì Việt Nam', 4, 4, 15000,'/img/banh/banhmyvn.png'),
(N'Bim bim ngô', 4, 4, 10000,'/img/banh/bimbimngo.png'),
(N'Bim bim sữa dừa', 4, 4, 10000,'/img/banh/bimbimsuadua.png'),
(N'Bánh gấu', 4, 4, 15000,'/img/banh/banhgau.png'),


(N'Trà đào cam xả', 5, 5, 40000,'/img/ttc/tradaocamsa.png'),

(N'Olong tứ quý sen', 5, 5, 35000,'/img/ttc/olongtuquysen.png'),
(N'Đào kombucha', 5, 5, 45000,'/img/ttc/dao_kombucha.png'),
(N'Trà vải', 5, 5, 35000,'/img/ttc/travai.png'),
(N'Trà yuzu kombucha', 5, 5, 50000,'/img/ttc/yuzu_kombucha.png');

-- Ingredients for Trà xanh espresso marble
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(1, 1, 1), -- Coffee
(1, 2, 1), -- Milk
(1, 7, 1); -- Matcha

-- Ingredients for Bạc xỉu lắc sữa yến mạch
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(2, 1, 1), -- Coffee
(2, 2, 1), -- Milk
(2, 8, 1); -- Yến mạch

-- Ingredients for Bạc xỉu lắc caramel muối
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(3, 1, 1), -- Coffee
(3, 2, 1), -- Milk
(3, 9, 1), -- Caramel
(3, 10, 1); -- Salt

-- Ingredients for Bạc xỉu lắc hạnh nhân nướng
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(4, 1, 1), -- Coffee
(4, 2, 1), -- Milk
(4, 11, 1); -- Almond

-- Ingredients for Bơ arabica
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(5, 1, 1), -- Coffee
(5, 2, 1), -- Milk
(5, 12, 1); -- Butter

-- Ingredients for Đường đen sữa đá
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(6, 1, 1), -- Coffee
(6, 3, 1), -- Sugar
(6, 2, 1); -- Milk

-- Ingredients for Cà phê sữa đá
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(7, 1, 1), -- Coffee
(7, 2, 1); -- Milk

-- Ingredients for Cà phê sữa nóng
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(8, 1, 1), -- Coffee
(8, 2, 1); -- Milk

-- Ingredients for Bạc xỉu
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(9, 1, 1), -- Coffee
(9, 2, 1); -- Milk

-- Ingredients for Cà phê đen
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(10, 1, 1), -- Coffee
(10, 3, 1); -- Sugar

-- Ingredients for Trà sữa trân châu đường đen
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(11, 4, 1), -- Tea
(11, 2, 1), -- Milk
(11, 6, 1), -- Tapioca pearls
(11, 3, 1); -- Sugar

-- Ingredients for Trà sữa olong
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(12, 4, 1), -- Tea
(12, 2, 1); -- Milk

-- Ingredients for Trà sữa olong tứ quý bơ
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(13, 4, 1), -- Tea
(13, 2, 1), -- Milk
(13, 12, 1); -- Butter

-- Ingredients for Trà sữa olong nướng sương sáo
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(14, 4, 1), -- Tea
(14, 2, 1), -- Milk
(14, 28, 1); -- sương sáo

-- Ingredients for Trà đen macchito
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(15, 4, 1), -- Tea
(15, 2, 1); -- Milk

-- Ingredients for Hồng trà sữa trân châu
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(16, 4, 1), -- Tea
(16, 2, 1), -- Milk
(16, 6, 1); -- Tapioca pearls

-- Continue similarly for other food items in each category.

-- Ingredients for Frosty phin-gato
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(17, 4, 1), -- Tea
(17, 2, 1), -- Milk
(17, 13, 1), -- Cream
(17, 6, 1); -- Tapioca pearls

-- Ingredients for Frosty cà phê đường đen
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(18, 4, 1), -- Tea
(18, 2, 1), -- Milk
(18, 13, 1), -- Cream
(18, 3, 1); -- Sugar

-- Ingredients for Frosty bánh kem dâu
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(19, 4, 1), -- Tea
(19, 2, 1), -- Milk
(19, 13, 1), -- Cream
(19, 29, 1); -- Strawberry

-- Ingredients for Frosty choco chip
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(20, 4, 1), -- Tea
(20, 2, 1), -- Milk
(20, 13, 1), -- Cream
(20, 14, 1); -- Chocolate chip

-- Ingredients for Frosty caramel
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(21, 4, 1), -- Tea
(21, 2, 1), -- Milk
(21, 13, 1), -- Cream
(21, 9, 1); -- Caramel

-- Bánh & Snack (Pastry & Snack)

-- Ingredients for Butter croissant
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(22, 19, 1), -- Bread
(22, 20, 1); -- Condensed milk

-- Ingredients for Mochi kem phúc bồn tử
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(23, 15, 1); -- Mochi raspberry ice cream

-- Ingredients for Mochi kem việt quất
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(24, 16, 1); -- Mochi blueberry ice cream

-- Ingredients for Mochi kem chocolate
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(25, 17, 1); -- Mochi chocolate ice cream

-- Ingredients for Mousse gấu chocolate
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(26, 18, 1); -- Chocolate mousse bear

-- Ingredients for Bánh mì Việt Nam
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(27, 19, 1); -- Bread

-- Ingredients for Bim bim ngô
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(28, 30, 1); -- Corn snack

-- Ingredients for Bim bim sữa dừa
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(29, 31, 1); -- Coconut milk snack                                            

-- Ingredients for Bánh gấu
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(30, 27, 1); -- Bear biscuit

-- Trà trái cây (Fruit tea)

-- Ingredients for Trà đào cam xả
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(31, 4, 1), -- Tea
(31, 26, 1), -- đào
(31, 22, 1), -- Sả
(31, 21, 1); -- cam

-- Ingredients for Olong tứ quý sen
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(32, 4, 1), -- Tea
(32, 23, 1); -- Lotus seed

-- Ingredients for Đào kombucha
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(33, 4, 1), -- Tea
(33, 26, 1); -- Peach

-- Ingredients for Trà vải
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(34, 4, 1), -- Tea
(34, 24, 1); -- Lychee

-- Ingredients for Trà yuzu kombucha
INSERT INTO FoodIngredient (FoodId, IngredientId, Quantity)
VALUES 
(35, 4, 1), -- Tea
(35, 25, 1); -- Orange (Yuzu)




-- Insert vào Invoice
INSERT INTO Invoice (TableId, DateCheckIn, DateCheckOut, TrangThai)
VALUES
(1, '2024-08-06', '2024-08-06', 0),
(2, '2024-08-06', '2024-08-06', 0),
(5, '2024-08-06', '2024-08-06', 0),
(8, '2024-08-06', '2024-08-06', 0);

-- Insert vào InvoiceDetail
INSERT INTO InvoiceDetail (InvoiceId, FoodId, SoLuong, Price)
VALUES

    (1, 1, 2, 45000),
    (1, 2, 1, 50000),
    (2, 3, 1, 55000),
    (2, 4, 2, 55000),
    (3, 29, 3, 10000),
    (3, 1, 2, 45000),
    (3, 19, 1, 45000),
    (4, 9, 2, 30000),
    (4, 12, 2,30000);
	





-- Bảng Staff
INSERT INTO Staff (FullName, Phone, DateOfBirth, Email, Gender, AccountId, RoleId) 
VALUES
(N'Nguyen Hai Duong', '0985082004', '2004-08-05', 'billduongg@gmail.com', 'Nam', 1, 1),
(N'Nguyen Minh Hoang', '0985082005', '2004-08-06', 'staff2@gmail.com', 'Nam', null, 2),
(N'Nguyen Bao Han', '0985082029', '2004-08-16', 'staff1@gmail.com', 'Nam', null, 2),
(N'Nguyen Thi Hoai Suong', '0985082006', '2004-08-07', 'staff3@gmail.com', 'Nam', null, 3),
(N'Dieu Thuy Lien', '0985082007', '2004-08-08', 'staff4@gmail.com', 'Nam', null, 3),
(N'Vu Hoang Anh', '0985082008', '2004-08-09', 'staff5@gmail.com', 'Nam', null, 3),
(N'Vu Hoang Em', '0985082009', '2004-08-10', 'staff6@gmail.com', N'Nữ', null, 3);




-- Bảng Warehouse
INSERT INTO Warehouse (IngredientId, SoLuong, DateUpdate)
VALUES
(1, 100, GETDATE()),
(2, 50, GETDATE()),
(3, 200, GETDATE()),
(4, 75, GETDATE());

-- Bảng Orders
--INSERT INTO Orders (InvoiceDetailId, Status)
--VALUES
--(1, N'Hoàn thành'),
--(2, N'Chưa hoàn thành');










SELECT * FROM dbo.Account

SELECT * FROM Staff[QLSanpham]

SELECT s.StaffId, s.FullName, s.Phone, s.DateOfBirth, s.Email, s.Gender, a.RoleName, s.ImageStaff
FROM Staff s
INNER JOIN AccRole a ON s.RoleId = a.RoleId;

SELECT * FROM dbo.Food

select * from dbo.Ingredient

CREATE PROCEDURE GetFoodDetailsByName
    @FoodName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        FoodName, 
        Price, 
        ImageURL
    FROM 
        Food
    WHERE 
        FoodName = @FoodName;
END;
go
EXEC GetFoodDetailsByName @FoodName = N'Bạc xỉu';


select IngredientName, SoLuong, ImageURL from Ingredient



SELECT 
    FoodName, 
    Price, 
    ImageURL
FROM 
    Food
WHERE 
    FoodName = N'Bạc xỉu';

select * from staff

SELECT FoodName, Price, ImageURL FROM Food

CREATE PROCEDURE GetTop10FoodsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 100
        FoodName, 
        Price, 
        ImageURL
    FROM 
        Food
    WHERE 
        CategoryId = @CategoryId
END
GO

EXEC GetTop10FoodsByCategory @CategoryId = 1



--DROP PROCEDURE GetTop10FoodsByCategory;

select foodname, price, imageURL from food

CREATE PROCEDURE GetTableById
    @TableId INT
AS
BEGIN
    SELECT TableId, TableName, TrangThai
    FROM TableFood
    WHERE TableId = @TableId
END
go




exec GetTableById @TableId =1



select * from TableFood

CREATE PROCEDURE GetTableList
as select * from TableFood
go




exec GetTableList



select * from invoice where TableId = 2 and TrangThai = 0

select * from invoicedetail where InvoiceId = 2

select * from InvoiceDetail where InvoiceId = 1

select f.FoodName, bi.SoLuong, f.Price, f.price*bi.SoLuong as totalPrice from InvoiceDetail as bi, Invoice as b, Food as f
where bi.InvoiceDetailId = b.InvoiceId and bi.foodid = f.FoodId and b.TrangThai = 0 and b.TableId = 5

SELECT f.FoodName, bi.SoLuong, f.Price, f.Price * bi.SoLuong AS TotalPrice
FROM InvoiceDetail AS bi
INNER JOIN Invoice AS b ON bi.InvoiceId = b.InvoiceId AND b.TrangThai =0
INNER JOIN Food AS f ON bi.FoodId = f.FoodId

WHERE b.TableId = 5

SELECT f.FoodName, bi.SoLuong, f.Price, f.Price * bi.SoLuong AS TotalPrice
FROM InvoiceDetail AS bi
INNER JOIN Invoice AS b ON bi.InvoiceId = b.InvoiceId
INNER JOIN Food AS f ON bi.FoodId = f.FoodId
WHERE b.TrangThai = 0 AND b.TableId = 5;
 select * from Category where CategoryId = 1


 CREATE PROC Proc_InsertBill
@TableId int
as
begin
	insert Invoice
	(DateCheckIn, DateCheckOut, TableId, TrangThai)

	Values (GETdate(), null, @TableId, 0)

	end
	go


	--EXEC Proc_InsertBill @TableId = 1;




-- giai thich proc o tren 
---- Tạo hoặc thay đổi thủ tục Proc_InsertBillDetail
--ALTER PROC Proc_InsertBillDetail
--    @InvoiceId INT,       -- Tham số đầu vào: ID của hóa đơn
--    @FoodId INT,          -- Tham số đầu vào: ID của món ăn
--    @SoLuong INT          -- Tham số đầu vào: Số lượng món ăn cần thêm
--AS
--BEGIN
--    -- Khai báo biến để kiểm tra bản ghi tồn tại và số lượng hiện tại
--    DECLARE @isExitsBillInfo INT;     -- Biến để kiểm tra xem món ăn đã có trong hóa đơn hay chưa
--    DECLARE @foodCount INT = 0;       -- Biến để lưu số lượng hiện tại của món ăn trong hóa đơn

--    -- Kiểm tra xem bản ghi chi tiết hóa đơn với cùng InvoiceId và FoodId đã tồn tại hay chưa
--    SELECT @isExitsBillInfo = COUNT(*), @foodCount = SoLuong  -- lay  và lấy số lượng hiện tại
--    FROM InvoiceDetail 
--    WHERE InvoiceId = @InvoiceId AND FoodId = @FoodId;        -- Điều kiện kiểm tra hóa đơn và món ăn

--    -- Nếu bản ghi đã tồn tại trong bảng InvoiceDetail
--    IF (@isExitsBillInfo > 0)
--    BEGIN
--        -- Cập nhật số lượng món ăn trong hóa đơn hiện có
--        UPDATE InvoiceDetail 
--        SET SoLuong = @foodCount + @SoLuong                   -- Cộng thêm số lượng mới vào số lượng hiện tại
--        WHERE InvoiceId = @InvoiceId AND FoodId = @FoodId;    -- Cập nhật bản ghi cụ thể theo InvoiceId và FoodId
--    END
--    ELSE  -- Nếu bản ghi không tồn tại
--    BEGIN
--        -- Thêm một bản ghi mới vào bảng InvoiceDetail
--        INSERT INTO InvoiceDetail (InvoiceId, FoodId, SoLuong)  -- Thêm thông tin mới vào bảng
--        VALUES (@InvoiceId, @FoodId, @SoLuong);                 -- Gán giá trị cho các cột
--    END
--END
--GO
--Giải thích tổng quát:
--Kiểm tra bản ghi tồn tại: Dùng câu lệnh SELECT để xác định xem món ăn đã có trong hóa đơn chưa.
--Cập nhật số lượng: Nếu món ăn đã tồn tại, sử dụng UPDATE để cộng thêm số lượng mới.
--Thêm mới bản ghi: Nếu món ăn chưa tồn tại, sử dụng INSERT để thêm bản ghi mới vào bảng InvoiceDetail.


----kiem tra proc
--CREATE PROC Proc_InsertBill1
--    @TableId INT
--AS
--BEGIN
--    -- Chèn dữ liệu vào bảng Invoice
--    INSERT INTO Invoice (DateCheckIn, DateCheckOut, TableId, TrangThai)
--    VALUES (GETDATE(), NULL, @TableId, 0);

--    -- Trả về thông tin của hóa đơn vừa chèn
--    SELECT TOP 1 *
--    FROM Invoice
--    WHERE TableId = @TableId
--    ORDER BY DateCheckIn DESC;
--END
--GO
--EXEC Proc_InsertBill1 @TableId = 1;



Select max(Invoice.InvoiceId) from Invoice


SELECT Price FROM Food WHERE FoodId = 10;


CREATE PROCEDURE GetFoodsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        FoodId,
        FoodName,
        CategoryId,
        Price
       
    FROM 
        Food
    WHERE 
        CategoryId = @CategoryId;
END;







create PROCEDURE Proc_InsertBillDetail
@InvoiceId int,  
@FoodId int, 
@SoLuong int,
@Price decimal
AS
BEGIN
    DECLARE @isExitsBillInfo int;
    DECLARE @foodCount int = 0;

    -- Check if the record exists in InvoiceDetail
    SELECT @isExitsBillInfo = Invoiceid, @foodCount = InvoiceDetail.SoLuong 
    FROM InvoiceDetail 
    WHERE InvoiceId = @InvoiceId AND FoodId = @FoodId;

    IF (@isExitsBillInfo > 0)
    BEGIN
        DECLARE @newCount int = @foodCount + @SoLuong;
        
        -- If the new quantity is greater than 0, update the record
        IF (@newCount > 0)
        BEGIN
            UPDATE InvoiceDetail 
            SET SoLuong = @newCount, Price = @Price
            WHERE InvoiceId = @InvoiceId AND FoodId = @FoodId;
        END
        ELSE IF (@newCount <= 0) -- If the new quantity is 0 or negative
        BEGIN
            -- Delete if the new count is 0 or negative
            DELETE FROM InvoiceDetail WHERE InvoiceId = @InvoiceId AND FoodId = @FoodId;
        END
    END
    ELSE
    BEGIN
        -- Insert new record if not exists and SoLuong is positive
        IF @SoLuong > 0
        BEGIN
            INSERT INTO InvoiceDetail (InvoiceId, FoodId, SoLuong, Price)
            VALUES (@InvoiceId, @FoodId, @SoLuong, @Price);
        END
    END

    -- Return the updated InvoiceDetail for the given InvoiceId
    SELECT * FROM InvoiceDetail WHERE InvoiceId = @InvoiceId;
END



update Invoice set TrangThai = 1 where InvoiceId = 1;

--IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'Proc_InsertBillDetail')
--BEGIN
--    DROP PROCEDURE Proc_InsertBillDetail;
--END






CREATE PROCEDURE DeleteFoodById
    @FoodId INT
AS
BEGIN
    -- Xóa dữ liệu trong FoodIngredient
    DELETE FROM FoodIngredient
    WHERE FoodId = @FoodId;

    -- Xóa dữ liệu trong InvoiceDetail
    DELETE FROM InvoiceDetail
    WHERE FoodId = @FoodId;

    -- Xóa dữ liệu trong bảng Food
    DELETE FROM Food
    WHERE FoodId = @FoodId;
END;
GO

EXEC DeleteFoodById @FoodId = 1;



DELETE FROM InvoiceDetail WHERE InvoiceId = 4

delete InvoiceDetail

delete Invoice


CREATE TRIGGER updateInvoiceDetail
ON InvoiceDetail
FOR INSERT, UPDATE
AS
BEGIN
    DECLARE @InvoiceId INT;
    
    -- Lấy InvoiceId từ bảng inserted (bảng tạm trong trigger chứa các bản ghi mới được chèn/cập nhật)
    SELECT @InvoiceId = InvoiceId FROM inserted;
    
    DECLARE @TableId INT;

    -- Kiểm tra và lấy TableId từ bảng Invoice
    SELECT @TableId = TableId FROM Invoice WHERE InvoiceId = @InvoiceId AND TrangThai = 0;

	update dbo.TableFood set TrangThai = N'Đã có khách' where TableId = @TableId

    -- Bạn có thể thêm các thao tác khác ở đây nếu cần thiết
END
GO





--lần đầu phải chạy cái này trước mới alter đc nhé mng
--create trigger updateInvoice 
create trigger updateInvoice
on dbo.Invoice for update 
as
begin
		
		declare @InvoiceId int;

		select @InvoiceId = InvoiceId from inserted;

		declare @TableId int

		 SELECT @TableId = TableId FROM Invoice WHERE InvoiceId = @InvoiceId

		 declare @count int = 0

		 select  @count = Count(*) from Invoice where TableId = @TableId and TrangThai =0 

		 if(@count = 0)


			update TableFood set TrangThai = N'Bàn trống' where TableId = @TableId
end
go

SELECT SUM(TotalPrice) FROM Invoice WHERE TrangThai = 1


SELECT COUNT(*) FROM Invoice;
SELECT * FROM Invoice WHERE TrangThai = 1;

alter PROCEDURE USP_GetListInvoiceDetailsByDate
    @checkIn DATE,
    @checkOut DATE
AS
BEGIN
    SELECT 
        i.TableId,
        i.DateCheckIn,
        i.DateCheckOut,
        SUM(id.SoLuong * id.Price) AS TotalPrice
    FROM 
        Invoice i
    INNER JOIN 
        InvoiceDetail id ON i.InvoiceId = id.InvoiceId
    WHERE 
        i.DateCheckIn >= @checkIn AND i.DateCheckOut <= @checkOut AND i.TrangThai = 1
    GROUP BY 
        i.TableId, i.DateCheckIn, i.DateCheckOut
END
GO

EXEC USP_GetListInvoiceDetailsByDate @checkIn='2000-08-01', @checkOut='2025-08-07';

select * from invoice

create proc USP_GetListInvoiceByDate
@checkIn date,@checkOut date
as
begin
	select Invoice.TableId ,Invoice.DateCheckIn,Invoice.DateCheckOut,Invoice.TotalPrice,Invoice.TrangThai from Invoice
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and Invoice.TrangThai = 1
end
go
exec USP_GetListInvoiceByDate '2024-08-06','2024-08-07' 



select * from invoice


--select tf.TableName,  DateCheckIn , DateCheckOut
--from invoice as b, TableFood as tf , Food as f, InvoiceDetail bdt

--where DateCheckIn >= '20241201' and DateCheckOut<= '20241209'
--and b.TrangThai = 1 and b.TableId = tf.TableId and b.TableId = tf.TableId
--and bdt.FoodId = f.FoodId

--delete InvoiceDetail
--delete Invoice

--alter table invoice add totalprice float

 select * from invoice

SELECT 
    tf.TableName,  
    b.DateCheckIn, 
    b.DateCheckOut,
    SUM(bdt.Price * bdt.SoLuong) AS TotalAmount
FROM 
    invoice AS b
JOIN 
    TableFood AS tf ON b.TableId = tf.TableId
JOIN 
    InvoiceDetail AS bdt ON b.InvoiceId = bdt.InvoiceId
JOIN 
    Food AS f ON bdt.FoodId = f.FoodId
WHERE 
    b.DateCheckIn >= '2024-12-01' 
    AND b.DateCheckOut <= '2024-12-09'
    AND b.TrangThai = 1
GROUP BY 
    tf.TableName, b.DateCheckIn, b.DateCheckOut;




SELECT InvoiceDetailId, InvoiceId, FoodId, SoLuong, Price
    FROM InvoiceDetail
    WHERE InvoiceId = 1;

SELECT 
    tf.TableName,  
    b.DateCheckIn, 
    b.DateCheckOut,
    SUM(bdt.Price * bdt.SoLuong) AS TotalAmount
FROM 
    invoice AS b
JOIN 
    TableFood AS tf ON b.TableId = tf.TableId
JOIN 
    InvoiceDetail AS bdt ON b.InvoiceId = bdt.InvoiceId
JOIN 
    Food AS f ON bdt.FoodId = f.FoodId
WHERE 
    b.DateCheckIn >= '2024-12-01' 
    AND b.DateCheckOut <= '2024-12-09'
    AND b.TrangThai = 1
GROUP BY 
    tf.TableName, b.DateCheckIn, b.DateCheckOut;

DECLARE @InvoiceId INT;
SET @InvoiceId = <your_invoice_id_here>;

UPDATE Invoice 
SET 
    DateCheckOut = GETDATE(), 
    TrangThai = 1, 
    Total = (SELECT SUM(bdt.Price * bdt.SoLuong)
             FROM InvoiceDetail AS bdt
             WHERE bdt.InvoiceId = Invoice.InvoiceId)
WHERE InvoiceId = @InvoiceId;

ALTER TABLE Invoice
ADD Total DECIMAL(18, 3) DEFAULT 0;

go





CREATE PROCEDURE usp_GetBillByDate
    @datecheckin DATE,
    @datecheckout DATE
AS
BEGIN
    SELECT 
        tf.TableName,  
        b.DateCheckIn, 
        b.DateCheckOut,
        b.Total AS TongTien
    FROM 
        Invoice AS b
    JOIN 
        TableFood AS tf ON b.TableId = tf.TableId
    WHERE 
        b.DateCheckIn >= @datecheckin 
        AND b.DateCheckOut <= @datecheckout
        AND b.TrangThai = 1
    GROUP BY 
        tf.TableName, b.DateCheckIn, b.DateCheckOut, b.Total;
END;
GO

EXEC usp_GetBillByDate @datecheckin = '2024-12-01', @datecheckout = '2024-12-09';
-- Table: Invoice
CREATE TABLE Invoice1 (
    InvoiceId INT PRIMARY KEY IDENTITY,
    TableId INT,
    DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
    DateCheckOut DATE,
    TrangThai INT, -- 1 là thanh toán, 0 là chưa thanh toán
    Total INT, -- Sửa tên cột thành "Total"
    FOREIGN KEY (TableId) REFERENCES TableFood(TableId)
);
GO
INSERT INTO Invoice1 (TableId, DateCheckIn, DateCheckOut, TrangThai, Total)
VALUES
(1, '2024-12-01', '2024-12-01', 1, 100000),
(2, '2024-12-01', '2024-12-01', 1, 500000),
(3, '2024-12-01', '2024-12-01', 1, 120000),
(4, '2024-12-01', '2024-12-01', 1, 130000),
(5, '2024-12-01', '2024-12-01', 1, 103000),
(6, '2024-12-01', '2024-12-01', 1, 103000),
(7, '2024-12-01', '2024-12-01', 1, 110000),
(8, '2024-12-01', '2024-12-01', 1, 110000),
(1, '2024-12-02', '2024-12-02', 1, 100000),
(2, '2024-12-02', '2024-12-02', 1, 500000),
(3, '2024-12-02', '2024-12-02', 1, 120000),
(4, '2024-12-02', '2024-12-02', 1, 130000),
(5, '2024-12-03', '2024-12-03', 1, 103000),
(6, '2024-12-03', '2024-12-03', 1, 103000),
(7, '2024-12-04', '2024-12-04', 1, 110000),
(8, '2024-12-04', '2024-12-04', 1, 110000),
(5, '2024-12-04', '2024-12-04', 1, 103000),
(6, '2024-12-05', '2024-12-05', 1, 103000),
(2, '2024-12-06', '2024-12-06', 1, 110000),
(3, '2024-12-06', '2024-12-06', 1, 110000),
(4, '2024-12-07', '2024-12-07', 1, 110000),
(5, '2024-12-07', '2024-12-07', 1, 110000),
(7, '2024-12-08', '2024-12-08', 1, 110000),
(8, '2024-12-08', '2024-12-08', 1, 110000);
GO

SELECT * FROM Staff

