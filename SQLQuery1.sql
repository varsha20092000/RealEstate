-- Drop old table and recreate with Guid ID
DROP TABLE [PropertyImages];

CREATE TABLE [PropertyImages] (
    [Id] uniqueidentifier NOT NULL DEFAULT newsequentialid(),
    [PropertyId] uniqueidentifier NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL DEFAULT '',
    [IsPrimary] bit NOT NULL DEFAULT 0,
    [DisplayOrder] int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_PropertyImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PropertyImages_Properties_PropertyId] 
        FOREIGN KEY ([PropertyId]) 
        REFERENCES [Properties]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PropertyImages_PropertyId] ON [PropertyImages]([PropertyId]);

-- Also add migration record so EF knows it's done
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260311114910_ChangePropertyImageIdToGuid', '9.0.0');