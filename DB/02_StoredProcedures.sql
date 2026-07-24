

USE ESunPreferenceDB;
GO


CREATE OR ALTER PROCEDURE dbo.usp_User_GetList
AS
BEGIN
    
    SET NOCOUNT ON;

    SELECT  UserID,
            UserName,
            Email,
            Account
    FROM    [User]
    ORDER BY UserID;
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_Preference_GetList
    @UserID VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  l.SN,
            l.UserID,
            u.UserName,
            u.Email,                    
            l.ProductNo,
            p.ProductName,              
            p.Price,
            p.FeeRate,
            l.PurchaseQuantity,
            l.Account,  
			l.TotalFee,                 
            l.TotalAmount			
    FROM        LikeList l
    INNER JOIN  [User]   u ON l.UserID    = u.UserID
    INNER JOIN  Product  p ON l.ProductNo = p.No
    WHERE       l.UserID = @UserID
    ORDER BY    l.SN;
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_Preference_GetById
    @SN INT,
	@UserID VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  l.SN,
            l.UserID,
            u.UserName,
            u.Email,
            l.ProductNo,
            p.ProductName,
            p.Price,
            p.FeeRate,
            l.PurchaseQuantity,
            l.Account,
            l.TotalFee,
            l.TotalAmount
    FROM        LikeList l
    INNER JOIN  [User]   u ON l.UserID    = u.UserID
    INNER JOIN  Product  p ON l.ProductNo = p.No
    WHERE       l.SN = @SN AND l.UserID = @UserID;
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_Preference_Add
    @UserID	VARCHAR(20),
	@ProductName	NVARCHAR(100),
	@Price	DECIMAL(18,4),
	@FeeRate	DECIMAL(5,4),
	@PurchaseQuantity	INT,
	@Account	VARCHAR(20),
	@TotalFee	DECIMAL(18,4),
	@TotalAmount	DECIMAL(18,4)

	
AS
BEGIN
    SET NOCOUNT ON;
	SET XACT_ABORT ON;
	DECLARE @NewProductNo INT;
	DECLARE @NewSN INT;
	
	BEGIN TRY
		BEGIN TRANSACTION
		
			Insert into Product (ProductName, Price, FeeRate)
			Values (@ProductName,@Price,@FeeRate);
			SET @NewProductNo = CAST(SCOPE_IDENTITY() AS INT);
			INSERT INTO LikeList (UserID, ProductNo, PurchaseQuantity, Account, TotalFee, TotalAmount)
			Values (@UserID,@NewProductNo,@PurchaseQuantity,@Account,@TotalFee,@TotalAmount)
			SET @NewSN = CAST(SCOPE_IDENTITY() AS INT);
			
		
		COMMIT TRANSACTION;
		SELECT @NewSN AS SN;  
		
	END TRY
	BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Preference_Update
    @UserID	VARCHAR(20),
	@ProductName	NVARCHAR(100),
	@SN INT,
	@Price	DECIMAL(18,4),
	@FeeRate	DECIMAL(5,4),
	@PurchaseQuantity	INT,
	@Account	VARCHAR(20),
	@TotalFee	DECIMAL(18,4),
	@TotalAmount	DECIMAL(18,4)
	
AS
BEGIN
    SET NOCOUNT ON;
	SET XACT_ABORT ON;
	DECLARE @ProductNo INT;
	
	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @ProductNo = ProductNo
			FROM LikeList WITH (UPDLOCK)
			WHERE SN = @SN AND UserID = @UserID

			IF @ProductNo IS NULL
			BEGIN
				-- 此路徑未異動任何資料，提交空交易即可釋放 UPDLOCK。
				-- 不使用 ROLLBACK：呼叫端若處於 INSERT-EXEC 情境會觸發錯誤 3915。
				COMMIT TRANSACTION;
				SELECT 0 AS AffectedRows;
				RETURN;
			END

			UPDATE Product
			SET ProductName = @ProductName, Price = @Price, FeeRate = @FeeRate
			WHERE No = @ProductNo;
			
			UPDATE LikeList
			SET PurchaseQuantity = @PurchaseQuantity , Account = @Account, TotalFee =@TotalFee , TotalAmount = @TotalAmount
			WHERE SN = @SN
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		THROW;
    END CATCH
	
	SELECT 1 AS AffectedRows;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Preference_Delete
	@SN INT,
	@UserID VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
	SET XACT_ABORT ON;
	DECLARE @ProductNo INT;
	
	BEGIN TRY
		BEGIN TRANSACTION
		
			SELECT @ProductNo = ProductNo
			FROM LikeList WITH (UPDLOCK)
			WHERE SN = @SN AND UserID = @UserID

			IF @ProductNo IS NULL
			BEGIN
				-- 此路徑未異動任何資料，提交空交易即可釋放 UPDLOCK。
				-- 不使用 ROLLBACK：呼叫端若處於 INSERT-EXEC 情境會觸發錯誤 3915。
				COMMIT TRANSACTION;
				SELECT 0 AS AffectedRows;
				RETURN;
			END

			DELETE FROM LikeList
			WHERE SN = @SN;
			
			DELETE FROM Product 
			Where No = @ProductNo;
			
		COMMIT TRANSACTION;
		
	END TRY
	BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		THROW;
    END CATCH
	
	SELECT 1 AS AffectedRows; 
END
GO




