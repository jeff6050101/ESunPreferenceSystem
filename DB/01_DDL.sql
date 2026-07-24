IF DB_ID('ESunPreferenceDB') IS NULL
    CREATE DATABASE ESunPreferenceDB;
GO

USE ESunPreferenceDB;
GO

DROP TABLE IF EXISTS LikeList;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS [User];
GO

CREATE TABLE [User]
(
    UserID VARCHAR(20) NOT NULL ,
    UserName NVARCHAR(50) NOT NULL,
    Email	VARCHAR(100) NOT NULL,
    Account	VARCHAR(20) NOT NULL,
	CONSTRAINT PK_User PRIMARY KEY (UserID)
);
GO

CREATE TABLE Product
(
    No	INT IDENTITY(1,1) NOT NULL,	
    ProductName	NVARCHAR(100)	NOT NULL,	
    Price	DECIMAL(18,4)	NOT NULL, 
    FeeRate	DECIMAL(5,4)	NOT NULL,
	CONSTRAINT PK_Product PRIMARY KEY (No),
	CONSTRAINT CK_Product_Price   CHECK (Price >= 0),
    CONSTRAINT CK_Product_FeeRate CHECK (FeeRate >= 0 AND FeeRate <= 1)
);
GO

CREATE TABLE LikeList
(
    SN	INT	 IDENTITY(1,1) NOT NULL,	
    UserID	VARCHAR(20)	NOT NULL,	
    ProductNo	INT NOT NULL,	
    PurchaseQuantity INT NOT NULL, 
    Account	VARCHAR(20)	NOT NULL,	
    TotalFee	DECIMAL(18,4)	NOT NULL,	
    TotalAmount	DECIMAL(18,4)	NOT NULL,

	CONSTRAINT PK_LikeList PRIMARY KEY (SN),
	CONSTRAINT FK_LikeList_User FOREIGN KEY (UserID) REFERENCES [User](UserID),
	CONSTRAINT FK_LikeList_Product FOREIGN KEY (ProductNo) REFERENCES Product(No),
	CONSTRAINT CK_LikeList_Qty CHECK (PurchaseQuantity > 0)
	

);
go

CREATE INDEX IX_LikeList_UserID    ON LikeList(UserID);
CREATE INDEX IX_LikeList_ProductNo ON LikeList(ProductNo);
GO




