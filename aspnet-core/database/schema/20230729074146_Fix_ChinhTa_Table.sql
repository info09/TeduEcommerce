BEGIN TRANSACTION;
GO

EXEC sp_rename N'[AppProducts].[Visiblity]', N'Visibility', N'COLUMN';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230729074146_Fix_ChinhTa_Table', N'6.0.5');
GO

COMMIT;
GO

