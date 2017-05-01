/*
    Prepare the "order shard" tables
    Orders, Order Details
*/

-- Create the demo user, grant READ, WRITE

create user [demouser] for login [demouser];
go

alter role db_datareader add member [demouser];
go
alter role db_datawriter add member [demouser];
go

grant execute on [__ShardManagement].[spValidateShardMappingLocal] to [demouser];
go

-- ### Orders ###

CREATE TABLE [dbo].[Orders](
	[CustomerID] [int] NOT NULL,
	[OrderID] [uniqueidentifier] NOT NULL,
	[EmployeeID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[RequiredDate] [datetime] NULL,
	[ShippedDate] [datetime] NULL,
	[ShipVia] [int] NULL,
	[Freight] [money] NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ( [CustomerID] ASC, [OrderID] ASC )
);
GO

CREATE NONCLUSTERED INDEX [CustomerID] ON [dbo].[Orders] ( [CustomerID] ASC );
GO
CREATE NONCLUSTERED INDEX [EmployeeID] ON [dbo].[Orders] ( [EmployeeID] ASC );
GO
CREATE NONCLUSTERED INDEX [OrderDate] ON [dbo].[Orders] ( [OrderDate] ASC );
GO
CREATE NONCLUSTERED INDEX [ShippedDate] ON [dbo].[Orders] ( [ShippedDate] ASC );
GO
CREATE NONCLUSTERED INDEX [ShippersOrders] ON [dbo].[Orders] ( [ShipVia] ASC );
GO

ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [DF_Orders_OrderId] DEFAULT newid() FOR [OrderID];
GO


-- ### Order Details ###

CREATE TABLE [dbo].[Order Details](
    [CustomerID] [int] NOT NULL,
	[OrderID] [uniqueidentifier] NOT NULL,
	[ProductID] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[Discount] [real] NOT NULL,
    CONSTRAINT [PK_Order_Details] PRIMARY KEY CLUSTERED ( [CustomerID] ASC, [OrderID] ASC, [ProductID] ASC )
);
GO

CREATE NONCLUSTERED INDEX [CustomerID] ON [dbo].[Order Details] ( [CustomerID] ASC );
GO
CREATE NONCLUSTERED INDEX [OrderID] ON [dbo].[Order Details] ( [OrderID] ASC );
GO
CREATE NONCLUSTERED INDEX [ProductID] ON [dbo].[Order Details] ( [ProductID] ASC );
GO


-- ### Foreign Keys ###

-- Note: the only FK we currently need is Order Details -> Order (all other referential integrity is cross-database and impossible)
ALTER TABLE [dbo].[Order Details] ADD CONSTRAINT [FK_Order_Details_Orders] FOREIGN KEY ([CustomerID], [OrderID]) REFERENCES [dbo].[Orders] ([CustomerID], [OrderID]);
GO