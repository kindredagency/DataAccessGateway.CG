IF OBJECT_ID('_DataInsert', 'P') IS NOT NULL
DROP PROC _DataInsert

GO
IF OBJECT_ID('_DataUpdate', 'P') IS NOT NULL
DROP PROC _DataUpdate

GO
IF OBJECT_ID('_DataDeleteByKey', 'P') IS NOT NULL
DROP PROC _DataDeleteByKey

GO
IF OBJECT_ID('_DataGetByKey', 'P') IS NOT NULL
DROP PROC _DataGetByKey

GO
IF OBJECT_ID('_DataGetAll', 'P') IS NOT NULL
DROP PROC _DataGetAll

GO
IF OBJECT_ID('_Data_UDTT_Insert', 'P') IS NOT NULL
DROP PROC _Data_UDTT_Insert

GO
IF OBJECT_ID('_Data_UDTT_Update', 'P') IS NOT NULL
DROP PROC _Data_UDTT_Update

GO
IF OBJECT_ID('_SubTableInsert', 'P') IS NOT NULL
DROP PROC _SubTableInsert

GO
IF OBJECT_ID('_SubTableUpdate', 'P') IS NOT NULL
DROP PROC _SubTableUpdate

GO
IF OBJECT_ID('_SubTableDeleteByKey', 'P') IS NOT NULL
DROP PROC _SubTableDeleteByKey

GO
IF OBJECT_ID('_SubTableGetByKey', 'P') IS NOT NULL
DROP PROC _SubTableGetByKey

GO
IF OBJECT_ID('_SubTableGetAll', 'P') IS NOT NULL
DROP PROC _SubTableGetAll

GO
IF OBJECT_ID('_SubTableDeleteByBigInt', 'P') IS NOT NULL
DROP PROC _SubTableDeleteByBigInt

GO
IF OBJECT_ID('_SubTableGetByBigInt', 'P') IS NOT NULL
DROP PROC _SubTableGetByBigInt

GO
IF OBJECT_ID('_SubTableDeleteByData_Id', 'P') IS NOT NULL
DROP PROC _SubTableDeleteByData_Id

GO
IF OBJECT_ID('_SubTableGetByData_Id', 'P') IS NOT NULL
DROP PROC _SubTableGetByData_Id

GO
IF OBJECT_ID('_SubTableDeleteBy_BigInt_Data_Id', 'P') IS NOT NULL
DROP PROC _SubTableDeleteBy_BigInt_Data_Id

GO
IF OBJECT_ID('_SubTableGetBy_BigInt_Data_Id', 'P') IS NOT NULL
DROP PROC _SubTableGetBy_BigInt_Data_Id

GO
IF OBJECT_ID('_SubTable_UDTT_Insert', 'P') IS NOT NULL
DROP PROC _SubTable_UDTT_Insert

GO
IF OBJECT_ID('_SubTable_UDTT_Update', 'P') IS NOT NULL
DROP PROC _SubTable_UDTT_Update

GO
IF OBJECT_ID('_sysdiagramsInsert', 'P') IS NOT NULL
DROP PROC _sysdiagramsInsert

GO
IF OBJECT_ID('_sysdiagramsUpdate', 'P') IS NOT NULL
DROP PROC _sysdiagramsUpdate

GO
IF OBJECT_ID('_sysdiagramsDeleteByKey', 'P') IS NOT NULL
DROP PROC _sysdiagramsDeleteByKey

GO
IF OBJECT_ID('_sysdiagramsGetByKey', 'P') IS NOT NULL
DROP PROC _sysdiagramsGetByKey

GO
IF OBJECT_ID('_sysdiagramsGetAll', 'P') IS NOT NULL
DROP PROC _sysdiagramsGetAll

GO
IF OBJECT_ID('_sysdiagrams_UDTT_Insert', 'P') IS NOT NULL
DROP PROC _sysdiagrams_UDTT_Insert

GO
IF OBJECT_ID('_sysdiagrams_UDTT_Update', 'P') IS NOT NULL
DROP PROC _sysdiagrams_UDTT_Update

GO

IF TYPE_ID(N'_Data') IS NOT NULL
BEGIN
    DROP TYPE dbo._Data
END

GO
IF TYPE_ID(N'_SubTable') IS NOT NULL
BEGIN
    DROP TYPE dbo._SubTable
END

GO
IF TYPE_ID(N'_sysdiagrams') IS NOT NULL
BEGIN
    DROP TYPE dbo._sysdiagrams
END

GO



CREATE TYPE dbo._Data AS TABLE
( 
Id UniqueIdentifier,
BigInt BigInt,
Binary Binary,
Bit Bit,
Char Char,
Date Date,
DateTime DateTime,
DateTime2 DateTime2,
SmallDateTime SmallDateTime,
DateTimeOffset DateTimeOffset,
Decimal Decimal,
Float Float,
Image Image,
Int Int,
Money Money,
NChar NChar,
NText NText,
NVarChar NVarChar,
Real Real,
SmallMoney SmallMoney,
Text Text,
VarBinary VarBinary,
VarChar VarChar
)

GO

CREATE TYPE dbo._SubTable AS TABLE
( 
Id Int,
BigInt BigInt,
Data_Id UniqueIdentifier
)

GO

CREATE TYPE dbo._sysdiagrams AS TABLE
( 
name NVarChar,
principal_id Int,
diagram_id Int,
version Int,
definition VarBinary
)

GO


CREATE PROCEDURE [_DataInsert]
(
@Id UniqueIdentifier,
@BigInt BigInt,
@Binary Binary = null,
@Bit Bit = null,
@Char Char = null,
@Date Date = null,
@DateTime DateTime = null,
@DateTime2 DateTime2 = null,
@SmallDateTime SmallDateTime = null,
@DateTimeOffset DateTimeOffset = null,
@Decimal Decimal = null,
@Float Float = null,
@Image Image = null,
@Int Int = null,
@Money Money = null,
@NChar NChar = null,
@NText NText = null,
@NVarChar NVarChar = null,
@Real Real = null,
@SmallMoney SmallMoney = null,
@Text Text = null,
@VarBinary VarBinary = null,
@VarChar VarChar = null 
)
AS
BEGIN
INSERT INTO [Data] ([Id],[BigInt],[Binary],[Bit],[Char],[Date],[DateTime],[DateTime2],[SmallDateTime],[DateTimeOffset],[Decimal],[Float],[Image],[Int],[Money],[NChar],[NText],[NVarChar],[Real],[SmallMoney],[Text],[VarBinary],[VarChar]) VALUES (@Id,@BigInt,@Binary,@Bit,@Char,@Date,@DateTime,@DateTime2,@SmallDateTime,@DateTimeOffset,@Decimal,@Float,@Image,@Int,@Money,@NChar,@NText,@NVarChar,@Real,@SmallMoney,@Text,@VarBinary,@VarChar)  END

GO

CREATE PROCEDURE [_DataUpdate]
(
@Id UniqueIdentifier,
@BigInt BigInt,
@Binary Binary = null,
@Bit Bit = null,
@Char Char = null,
@Date Date = null,
@DateTime DateTime = null,
@DateTime2 DateTime2 = null,
@SmallDateTime SmallDateTime = null,
@DateTimeOffset DateTimeOffset = null,
@Decimal Decimal = null,
@Float Float = null,
@Image Image = null,
@Int Int = null,
@Money Money = null,
@NChar NChar = null,
@NText NText = null,
@NVarChar NVarChar = null,
@Real Real = null,
@SmallMoney SmallMoney = null,
@Text Text = null,
@VarBinary VarBinary = null,
@VarChar VarChar = null,@_Id UniqueIdentifier,
@_BigInt BigInt 
)
AS
BEGIN
UPDATE [Data] SET [Id] = @Id,
[BigInt] = @BigInt,
[Binary] = @Binary,
[Bit] = @Bit,
[Char] = @Char,
[Date] = @Date,
[DateTime] = @DateTime,
[DateTime2] = @DateTime2,
[SmallDateTime] = @SmallDateTime,
[DateTimeOffset] = @DateTimeOffset,
[Decimal] = @Decimal,
[Float] = @Float,
[Image] = @Image,
[Int] = @Int,
[Money] = @Money,
[NChar] = @NChar,
[NText] = @NText,
[NVarChar] = @NVarChar,
[Real] = @Real,
[SmallMoney] = @SmallMoney,
[Text] = @Text,
[VarBinary] = @VarBinary,
[VarChar] = @VarChar WHERE [Id] = @_Id and
[BigInt] = @_BigInt
END

GO

CREATE PROCEDURE [_DataDeleteByKey]
(
@Id UniqueIdentifier,
@BigInt BigInt 
)
AS
BEGIN
DELETE FROM [Data] WHERE [Id] = @Id and 
[BigInt] = @BigInt
END

GO

CREATE PROCEDURE [_DataGetByKey]
(
@Id UniqueIdentifier,
@BigInt BigInt 
)
AS
BEGIN
SELECT 
[Id],[BigInt],[Binary],[Bit],[Char],[Date],[DateTime],[DateTime2],[SmallDateTime],[DateTimeOffset],[Decimal],[Float],[Image],[Int],[Money],[NChar],[NText],[NVarChar],[Real],[SmallMoney],[Text],[VarBinary],[VarChar]
FROM [Data] WHERE [Id] = @Id and 
[BigInt] = @BigInt
END

GO

CREATE PROCEDURE [_DataGetAll]
AS
BEGIN
SELECT 
[Id],[BigInt],[Binary],[Bit],[Char],[Date],[DateTime],[DateTime2],[SmallDateTime],[DateTimeOffset],[Decimal],[Float],[Image],[Int],[Money],[NChar],[NText],[NVarChar],[Real],[SmallMoney],[Text],[VarBinary],[VarChar]
FROM [Data]
END

GO

CREATE PROCEDURE [_Data_UDTT_Insert]
(
@Data as _Data READONLY
)
AS
BEGIN
INSERT INTO [Data] ([Id],[BigInt],[Binary],[Bit],[Char],[Date],[DateTime],[DateTime2],[SmallDateTime],[DateTimeOffset],[Decimal],[Float],[Image],[Int],[Money],[NChar],[NText],[NVarChar],[Real],[SmallMoney],[Text],[VarBinary],[VarChar]) Select [Id],[BigInt],[Binary],[Bit],[Char],[Date],[DateTime],[DateTime2],[SmallDateTime],[DateTimeOffset],[Decimal],[Float],[Image],[Int],[Money],[NChar],[NText],[NVarChar],[Real],[SmallMoney],[Text],[VarBinary],[VarChar] from @Data
END

GO

CREATE PROCEDURE [_Data_UDTT_Update]
(
@Data as _Data READONLY
)
AS
BEGIN
UPDATE [Data] SET Data.[Id] = TVP.Id,
Data.[BigInt] = TVP.BigInt,
Data.[Binary] = TVP.Binary,
Data.[Bit] = TVP.Bit,
Data.[Char] = TVP.Char,
Data.[Date] = TVP.Date,
Data.[DateTime] = TVP.DateTime,
Data.[DateTime2] = TVP.DateTime2,
Data.[SmallDateTime] = TVP.SmallDateTime,
Data.[DateTimeOffset] = TVP.DateTimeOffset,
Data.[Decimal] = TVP.Decimal,
Data.[Float] = TVP.Float,
Data.[Image] = TVP.Image,
Data.[Int] = TVP.Int,
Data.[Money] = TVP.Money,
Data.[NChar] = TVP.NChar,
Data.[NText] = TVP.NText,
Data.[NVarChar] = TVP.NVarChar,
Data.[Real] = TVP.Real,
Data.[SmallMoney] = TVP.SmallMoney,
Data.[Text] = TVP.Text,
Data.[VarBinary] = TVP.VarBinary,
Data.[VarChar] = TVP.VarChar FROM  Data, @Data as TVP WHERE Data.[Id] = TVP.Id and
Data.[BigInt] = TVP.BigInt
END

GO

CREATE PROCEDURE [_SubTableInsert]
(
@BigInt BigInt,
@Data_Id UniqueIdentifier 
)
AS
BEGIN
INSERT INTO [SubTable] ([BigInt],[Data_Id]) VALUES (@BigInt,@Data_Id) IF SCOPE_IDENTITY() IS NOT NULL
BEGIN
	SELECT SCOPE_IDENTITY()
END
ELSE
BEGIN
	SELECT -1
END END

GO

CREATE PROCEDURE [_SubTableUpdate]
(
@BigInt BigInt,
@Data_Id UniqueIdentifier,@_Id Int 
)
AS
BEGIN
UPDATE [SubTable] SET [BigInt] = @BigInt,
[Data_Id] = @Data_Id WHERE [Id] = @_Id
END

GO

CREATE PROCEDURE [_SubTableDeleteByKey]
(
@Id Int 
)
AS
BEGIN
DELETE FROM [SubTable] WHERE [Id] = @Id
END

GO

CREATE PROCEDURE [_SubTableGetByKey]
(
@Id Int 
)
AS
BEGIN
SELECT 
[Id],[BigInt],[Data_Id]
FROM [SubTable] WHERE [Id] = @Id
END

GO

CREATE PROCEDURE [_SubTableGetAll]
AS
BEGIN
SELECT 
[Id],[BigInt],[Data_Id]
FROM [SubTable]
END

GO

CREATE PROCEDURE [_SubTableDeleteByBigInt]
(
@BigInt BigInt
)
AS
BEGIN
DELETE FROM [SubTable] WHERE [BigInt] = @BigInt
END

GO

CREATE PROCEDURE [_SubTableGetByBigInt]
(
@BigInt BigInt
)
AS
BEGIN
SELECT 
[Id],[BigInt],[Data_Id]
FROM [SubTable] WHERE [BigInt] = @BigInt
END

GO

CREATE PROCEDURE [_SubTableDeleteByData_Id]
(
@Data_Id UniqueIdentifier
)
AS
BEGIN
DELETE FROM [SubTable] WHERE [Data_Id] = @Data_Id
END

GO

CREATE PROCEDURE [_SubTableGetByData_Id]
(
@Data_Id UniqueIdentifier
)
AS
BEGIN
SELECT 
[Id],[BigInt],[Data_Id]
FROM [SubTable] WHERE [Data_Id] = @Data_Id
END

GO

CREATE PROCEDURE [_SubTableDeleteBy_BigInt_Data_Id]
(
@BigInt BigInt,
@Data_Id UniqueIdentifier
)
AS
BEGIN
DELETE FROM [SubTable] WHERE [BigInt] = @BigInt and 
[Data_Id] = @Data_Id
END

GO

CREATE PROCEDURE [_SubTableGetBy_BigInt_Data_Id]
(
@BigInt BigInt,
@Data_Id UniqueIdentifier
)
AS
BEGIN
SELECT
[Id],[BigInt],[Data_Id]
FROM [SubTable] WHERE [BigInt] = @BigInt and 
[Data_Id] = @Data_Id
END

GO

CREATE PROCEDURE [_SubTable_UDTT_Insert]
(
@SubTable as _SubTable READONLY
)
AS
BEGIN
INSERT INTO [SubTable] ([Id],[BigInt],[Data_Id]) Select [Id],[BigInt],[Data_Id] from @SubTable
END

GO

CREATE PROCEDURE [_SubTable_UDTT_Update]
(
@SubTable as _SubTable READONLY
)
AS
BEGIN
UPDATE [SubTable] SET SubTable.[BigInt] = TVP.BigInt,
SubTable.[Data_Id] = TVP.Data_Id FROM  SubTable, @SubTable as TVP WHERE SubTable.[Id] = TVP.Id
END

GO

CREATE PROCEDURE [_sysdiagramsInsert]
(
@name NVarChar,
@principal_id Int,
@version Int = null,
@definition VarBinary = null 
)
AS
BEGIN
INSERT INTO [sysdiagrams] ([name],[principal_id],[version],[definition]) VALUES (@name,@principal_id,@version,@definition) IF SCOPE_IDENTITY() IS NOT NULL
BEGIN
	SELECT SCOPE_IDENTITY()
END
ELSE
BEGIN
	SELECT -1
END END

GO

CREATE PROCEDURE [_sysdiagramsUpdate]
(
@name NVarChar,
@principal_id Int,
@version Int = null,
@definition VarBinary = null,@_diagram_id Int 
)
AS
BEGIN
UPDATE [sysdiagrams] SET [name] = @name,
[principal_id] = @principal_id,
[version] = @version,
[definition] = @definition WHERE [diagram_id] = @_diagram_id
END

GO

CREATE PROCEDURE [_sysdiagramsDeleteByKey]
(
@diagram_id Int 
)
AS
BEGIN
DELETE FROM [sysdiagrams] WHERE [diagram_id] = @diagram_id
END

GO

CREATE PROCEDURE [_sysdiagramsGetByKey]
(
@diagram_id Int 
)
AS
BEGIN
SELECT 
[name],[principal_id],[diagram_id],[version],[definition]
FROM [sysdiagrams] WHERE [diagram_id] = @diagram_id
END

GO

CREATE PROCEDURE [_sysdiagramsGetAll]
AS
BEGIN
SELECT 
[name],[principal_id],[diagram_id],[version],[definition]
FROM [sysdiagrams]
END

GO

CREATE PROCEDURE [_sysdiagrams_UDTT_Insert]
(
@sysdiagrams as _sysdiagrams READONLY
)
AS
BEGIN
INSERT INTO [sysdiagrams] ([name],[principal_id],[diagram_id],[version],[definition]) Select [name],[principal_id],[diagram_id],[version],[definition] from @sysdiagrams
END

GO

CREATE PROCEDURE [_sysdiagrams_UDTT_Update]
(
@sysdiagrams as _sysdiagrams READONLY
)
AS
BEGIN
UPDATE [sysdiagrams] SET sysdiagrams.[name] = TVP.name,
sysdiagrams.[principal_id] = TVP.principal_id,
sysdiagrams.[version] = TVP.version,
sysdiagrams.[definition] = TVP.definition FROM  sysdiagrams, @sysdiagrams as TVP WHERE sysdiagrams.[diagram_id] = TVP.diagram_id
END

GO


