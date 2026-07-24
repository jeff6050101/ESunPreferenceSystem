USE ESunPreferenceDB;
GO


DELETE FROM LikeList;
DELETE FROM Product;
DELETE FROM [User];
GO

IF EXISTS (SELECT 1 FROM sys.identity_columns
           WHERE object_id = OBJECT_ID('Product') AND last_value IS NOT NULL)
    DBCC CHECKIDENT ('Product', RESEED, 0) WITH NO_INFOMSGS;

IF EXISTS (SELECT 1 FROM sys.identity_columns
           WHERE object_id = OBJECT_ID('LikeList') AND last_value IS NOT NULL)
    DBCC CHECKIDENT ('LikeList', RESEED, 0) WITH NO_INFOMSGS;
GO


INSERT INTO [User] (UserID, UserName, Email, Account) VALUES
    ('A1236456789', N'王小明', 'wang@email.com',  '1111999666'),
    ('B2234567890', N'李大華', 'lee@email.com',   '2222888555'),
    ('C3345678901', N'陳芳芳', 'chen@email.com',  '3333777444');
GO


INSERT INTO Product (ProductName, Price, FeeRate) VALUES
    (N'安聯台灣智慧基金',         15.8000, 0.0300),   
    (N'富邦全球債券基金',         15.2000, 0.0150),   
    (N'元大台灣50 ETF連結基金',  185.5000, 0.0100),   
    (N'美元定期存款',            32.5000, 0.0050),   
    (N'黃金存摺',              2450.0000, 0.0080);   
GO

INSERT INTO LikeList (UserID, ProductNo, PurchaseQuantity, Account, TotalFee, TotalAmount)
SELECT  v.UserID, p.No, v.Qty, v.Account, v.TotalFee, v.TotalAmount
FROM (VALUES
       
        ('A1236456789', N'安聯台灣智慧基金',        1000, '1111999666', 474.0000, 16274.0000),

       
        ('A1236456789', N'元大台灣50 ETF連結基金',   100, '1111000123', 185.5000, 18735.5000),

        ('B2234567890', N'美元定期存款',           1000, '2222888555', 162.5000, 32662.5000),

        
        ('B2234567890', N'黃金存摺',                  5, '2222888555',  98.0000, 12348.0000),

        
        ('C3345678901', N'富邦全球債券基金',        2000, '3333777444', 456.0000, 30856.0000)
     ) AS v (UserID, ProductName, Qty, Account, TotalFee, TotalAmount)
INNER JOIN Product p ON p.ProductName = v.ProductName;
GO


SELECT  l.SN                AS 序號,
        u.UserName          AS 使用者,
        u.Email             AS 聯絡信箱,
        p.ProductName       AS 產品名稱,
        p.Price             AS 產品價格,
        p.FeeRate           AS 手續費率,
        l.PurchaseQuantity  AS 購買數量,
        l.Account           AS 扣款帳號,
        l.TotalFee          AS 總手續費,
        l.TotalAmount       AS 預計扣款總金額
FROM        LikeList l
INNER JOIN  [User]   u ON l.UserID    = u.UserID
INNER JOIN  Product  p ON l.ProductNo = p.No
ORDER BY    l.SN;
GO
