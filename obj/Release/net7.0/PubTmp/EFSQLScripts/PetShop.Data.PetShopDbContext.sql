IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Category] (
        [Id] int NOT NULL IDENTITY,
        [SortOrder] int NOT NULL,
        [IsShowHome] bit NOT NULL,
        [ParentId] int NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Contact] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NULL,
        [Email] varchar(100) NOT NULL,
        [PhoneNumber] varchar(11) NOT NULL,
        [Message] nvarchar(255) NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Contact] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Language] (
        [Id] varchar(50) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [IsDefault] bit NOT NULL,
        CONSTRAINT [PK_Language] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Merchant] (
        [Id] int NOT NULL IDENTITY,
        [MerchantName] nvarchar(50) NULL,
        [MerchantWebLink] nvarchar(255) NULL,
        [MerchantIpnUrl] nvarchar(255) NULL,
        [MerchantReturnUrl] nvarchar(255) NULL,
        [SecretKey] nvarchar(50) NULL,
        [IsActive] bit NULL,
        CONSTRAINT [PK_Merchant] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [PaymentDestination] (
        [Id] int NOT NULL IDENTITY,
        [DesLogo] nvarchar(255) NULL,
        [DesShortName] nvarchar(50) NULL,
        [DesName] nvarchar(255) NULL,
        [DesSortIndex] int NULL,
        [ParentId] int NULL,
        [IsActive] bit NULL,
        [CreateBy] nvarchar(50) NULL,
        [CreateAt] datetime NULL,
        [LastUpdateBy] nvarchar(50) NULL,
        [LastUpdateAt] datetime NULL,
        CONSTRAINT [PK_PaymentDestination] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentDestination_PaymentDestination_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [PaymentDestination] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Promotion] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [FromDate] datetime NOT NULL,
        [ToDate] datetime NOT NULL,
        [ApplyForAll] bit NOT NULL,
        [DiscountPercent] int NULL,
        [DiscountAmount] int NULL,
        [ProductIds] nvarchar(4000) NULL,
        [ProductCategoryIds] nvarchar(4000) NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Promotion] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Roles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Users] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Address] nvarchar(500) NULL,
        [DateOfBirth] datetime NULL,
        [ImageUrl] nvarchar(max) NULL,
        [CreateDate] datetime NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Product] (
        [Id] int NOT NULL IDENTITY,
        [Price] decimal(19,2) NOT NULL,
        [OriginalPrice] decimal(19,2) NOT NULL,
        [Stock] int NOT NULL,
        [ViewCount] int NOT NULL,
        [DateCreated] datetime NOT NULL,
        [IsFeatured] bit NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Product_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [CategoryTranslation] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NULL,
        [SeoDescription] nvarchar(255) NULL,
        [SeoTitle] nvarchar(255) NULL,
        [SeoAlias] nvarchar(150) NULL,
        [LanguageId] varchar(50) NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_CategoryTranslation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CategoryTranslation_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CategoryTranslation_Language_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Language] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Cart] (
        [Id] int NOT NULL IDENTITY,
        [DateCreated] datetime NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Cart] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cart_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Order] (
        [Id] int NOT NULL IDENTITY,
        [OrderDate] datetime NOT NULL,
        [ShipName] nvarchar(150) NOT NULL,
        [ShipAddress] nvarchar(255) NOT NULL,
        [ShipEmail] varchar(100) NULL,
        [ShipPhoneNumber] varchar(11) NOT NULL,
        [TotalPrice] decimal(19,2) NOT NULL,
        [Status] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Order] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Order_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [UserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [UserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [ProductImage] (
        [Id] int NOT NULL IDENTITY,
        [ImagePath] nvarchar(4000) NULL,
        [Caption] nvarchar(255) NULL,
        [IsDefault] int NOT NULL,
        [DateCreated] datetime NOT NULL,
        [SortOrder] int NOT NULL,
        [FileSize] int NOT NULL,
        [ProductId] int NOT NULL,
        CONSTRAINT [PK_ProductImage] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductImage_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [ProductTranslation] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(60) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Details] nvarchar(255) NULL,
        [SeoDescription] nvarchar(500) NULL,
        [SeoTitle] nvarchar(200) NULL,
        [SeoAlias] nvarchar(255) NULL,
        [LanguageId] varchar(50) NOT NULL,
        [ProductId] int NOT NULL,
        CONSTRAINT [PK_ProductTranslation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductTranslation_Language_LanguageId] FOREIGN KEY ([LanguageId]) REFERENCES [Language] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductTranslation_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [CartItems] (
        [Id] int NOT NULL IDENTITY,
        [CartId] int NOT NULL,
        [ProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CartItems_Cart_CartId] FOREIGN KEY ([CartId]) REFERENCES [Cart] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CartItems_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [OrderDetail] (
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        [SubTotal] decimal(19,2) NOT NULL,
        CONSTRAINT [PK_OrderDetail] PRIMARY KEY ([OrderId], [ProductId]),
        CONSTRAINT [FK_OrderDetail_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetail_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [Payment] (
        [Id] int NOT NULL IDENTITY,
        [PaymentContent] nvarchar(255) NULL,
        [PaymentCurrency] nvarchar(10) NULL,
        [PaymentRefId] nvarchar(50) NULL,
        [RequiredAmount] decimal(19,2) NULL,
        [PaymentDate] datetime NULL,
        [ExpireDate] datetime NULL,
        [PaymentLanguage] nvarchar(10) NULL,
        [PaidAmount] decimal(19,2) NULL,
        [PaymentStatus] nvarchar(20) NULL,
        [PaymentLastMessage] nvarchar(255) NULL,
        [MerchantId] int NOT NULL,
        [PaymentDestinationId] int NOT NULL,
        [OrderId] int NOT NULL,
        CONSTRAINT [PK_Payment] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payment_Merchant_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Merchant] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Payment_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Payment_PaymentDestination_PaymentDestinationId] FOREIGN KEY ([PaymentDestinationId]) REFERENCES [PaymentDestination] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [PaymentNotification] (
        [Id] int NOT NULL IDENTITY,
        [PaymentRefId] nvarchar(50) NULL,
        [NotificationDate] nvarchar(50) NULL,
        [NotificationAmount] nvarchar(50) NULL,
        [NotificationContent] nvarchar(50) NULL,
        [NotificationMessage] nvarchar(50) NULL,
        [NotificationSignature] nvarchar(50) NULL,
        [NotificationStatus] nvarchar(50) NULL,
        [NotificationResponseDate] nvarchar(50) NULL,
        [PaymentId] int NOT NULL,
        CONSTRAINT [PK_PaymentNotification] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentNotification_Payment_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [PaymentSignature] (
        [Id] int NOT NULL IDENTITY,
        [SignValue] nvarchar(500) NULL,
        [SignAlgo] nvarchar(50) NULL,
        [SignDate] datetime NULL,
        [SignOwn] nvarchar(60) NULL,
        [IsValid] bit NOT NULL,
        [PaymentId] int NOT NULL,
        CONSTRAINT [PK_PaymentSignature] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentSignature_Payment_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE TABLE [PaymentTransaction] (
        [Id] int NOT NULL IDENTITY,
        [TranMessage] nvarchar(255) NULL,
        [TranPayload] nvarchar(500) NULL,
        [TranStatus] int NOT NULL,
        [TranAmount] decimal(19,2) NULL,
        [TranDate] datetime NULL,
        [PaymentId] int NOT NULL,
        CONSTRAINT [PK_PaymentTransaction] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentTransaction_Payment_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE UNIQUE INDEX [IX_Cart_UserId] ON [Cart] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_CartItems_CartId] ON [CartItems] ([CartId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_CartItems_ProductId] ON [CartItems] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_CategoryTranslation_CategoryId] ON [CategoryTranslation] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_CategoryTranslation_LanguageId] ON [CategoryTranslation] ([LanguageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_Order_UserId] ON [Order] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_OrderDetail_ProductId] ON [OrderDetail] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_Payment_MerchantId] ON [Payment] ([MerchantId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_Payment_OrderId] ON [Payment] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_Payment_PaymentDestinationId] ON [Payment] ([PaymentDestinationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_PaymentDestination_ParentId] ON [PaymentDestination] ([ParentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_PaymentNotification_PaymentId] ON [PaymentNotification] ([PaymentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_PaymentSignature_PaymentId] ON [PaymentSignature] ([PaymentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_PaymentTransaction_PaymentId] ON [PaymentTransaction] ([PaymentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_Product_CategoryId] ON [Product] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_ProductImage_ProductId] ON [ProductImage] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_ProductTranslation_LanguageId] ON [ProductTranslation] ([LanguageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_ProductTranslation_ProductId] ON [ProductTranslation] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [Email] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]) WHERE [UserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008082714_InitDb')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231008082714_InitDb', N'7.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231008121530_UpdateCategory')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231008121530_UpdateCategory', N'7.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Promotion]') AND [c].[name] = N'DiscountAmount');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Promotion] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Promotion] DROP COLUMN [DiscountAmount];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Promotion]') AND [c].[name] = N'DiscountPercent');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Promotion] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Promotion] DROP COLUMN [DiscountPercent];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Promotion]') AND [c].[name] = N'ProductCategoryIds');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Promotion] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Promotion] DROP COLUMN [ProductCategoryIds];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Promotion]') AND [c].[name] = N'ProductIds');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Promotion] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Promotion] DROP COLUMN [ProductIds];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payment]') AND [c].[name] = N'PaymentRefId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Payment] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Payment] DROP COLUMN [PaymentRefId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Promotion]') AND [c].[name] = N'Status');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Promotion] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Promotion] ALTER COLUMN [Status] int NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Promotion]') AND [c].[name] = N'Name');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Promotion] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Promotion] ALTER COLUMN [Name] nvarchar(200) NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    ALTER TABLE [Promotion] ADD [DiscountType] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    ALTER TABLE [Promotion] ADD [DiscountValue] decimal(19,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    CREATE TABLE [PromotionCategory] (
        [PromotionId] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_PromotionCategory] PRIMARY KEY ([PromotionId], [CategoryId]),
        CONSTRAINT [FK_PromotionCategory_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PromotionCategory_Promotion_PromotionId] FOREIGN KEY ([PromotionId]) REFERENCES [Promotion] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    CREATE TABLE [PromotionProduct] (
        [PromotionId] int NOT NULL,
        [ProductId] int NOT NULL,
        CONSTRAINT [PK_PromotionProduct] PRIMARY KEY ([PromotionId], [ProductId]),
        CONSTRAINT [FK_PromotionProduct_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PromotionProduct_Promotion_PromotionId] FOREIGN KEY ([PromotionId]) REFERENCES [Promotion] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    CREATE INDEX [IX_PromotionCategory_CategoryId] ON [PromotionCategory] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    CREATE INDEX [IX_PromotionProduct_ProductId] ON [PromotionProduct] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231009044550_AddPromotionTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231009044550_AddPromotionTable', N'7.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231011061741_AddcolumnPayment')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PaymentTransaction]') AND [c].[name] = N'TranStatus');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [PaymentTransaction] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [PaymentTransaction] ALTER COLUMN [TranStatus] nvarchar(10) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231011061741_AddcolumnPayment')
BEGIN
    ALTER TABLE [Payment] ADD [LastUpdateAt] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231011061741_AddcolumnPayment')
BEGIN
    ALTER TABLE [Payment] ADD [LastUpdateBy] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231011061741_AddcolumnPayment')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231011061741_AddcolumnPayment', N'7.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231011100019_updatelengthpaytrans')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PaymentTransaction]') AND [c].[name] = N'TranPayload');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [PaymentTransaction] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [PaymentTransaction] ALTER COLUMN [TranPayload] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231011100019_updatelengthpaytrans')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231011100019_updatelengthpaytrans', N'7.0.11');
END;
GO

COMMIT;
GO

