IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.sp_request_currency_rate'))
   DROP PROCEDURE sp_request_currency_rate; 
GO

CREATE PROCEDURE [dbo].[sp_request_currency_rate]
@num_code nvarchar(3),
@date_req datetime
AS
BEGIN

SELECT        CurrencyRates.NumCode, CurrencyRates.DateTime, CurrencyRates.Rate, Currencies.Name
FROM            Currencies LEFT OUTER JOIN
                         CurrencyRates ON Currencies.NumCode = CurrencyRates.NumCode
WHERE			CurrencyRates.DateTime = @date_req AND Currencies.CharCode = @num_code
						 
END
GO