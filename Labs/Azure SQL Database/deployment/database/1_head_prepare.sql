/*
    Prepare the "head" database tables
    Customers
    
    Remote tables: Products, Orders, Order Details 
*/

-- Create the demo user, grant READ

create user [demouser] for login [demouser];
GO

alter role db_datareader add member [demouser];
GO

grant execute on [__ShardManagement].[spGetAllShardMapsGlobal] to [demouser];
grant execute on [__ShardManagement].[spGetAllShardMappingsGlobal] to [demouser];
grant execute on [__ShardManagement].[spFindShardMappingByKeyGlobal] to [demouser];
GO

-- NOTE: username and password should be for an administrator SQL account

create master key encryption by password = '<password>';
create database scoped credential SqlAdminCred
	with identity = '<username>',
	secret = '<password>';

create external data source [ProductsDataSource] with (
    type = RDBMS,
    location = '<server>', 
    database_name = 'Products',
    credential = SqlAdminCred,
);

create external data source [OrdersDataSource] with (
    type = SHARD_MAP_MANAGER,
    location = '<server>',
    database_name = 'Head',
    credential = SqlAdminCred,
    shard_map_name = 'OrderShardMap'
);

-- EXTERNAL TABLES (Products)

CREATE EXTERNAL TABLE [dbo].[Products](
    [ProductID] [int] not null,
	[ProductName] [nvarchar](40) not null,
	[SupplierID] [int],
	[CategoryID] [int],
	[QuantityPerUnit] [nvarchar](20),
	[UnitPrice] [money],
	[UnitsInStock] [smallint],
	[UnitsOnOrder] [smallint],
	[ReorderLevel] [smallint],
	[Discontinued] [bit] not null,
)
WITH ( 
    data_source = ProductsDataSource
)
GO


-- EXTERNAL TABLES (Orders, Order Details)

CREATE EXTERNAL TABLE [dbo].[Orders](
	[CustomerID] [int] NOT NULL,
	[OrderID] [uniqueidentifier] NOT NULL,
	[EmployeeID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[RequiredDate] [datetime] NULL,
	[ShippedDate] [datetime] NULL,
	[ShipVia] [int] NULL,
	[Freight] [money] NULL
)
WITH ( 
    data_source = OrdersDataSource,
    distribution = SHARDED([CustomerID])
)
GO

CREATE EXTERNAL TABLE [dbo].[Order Details](
    [CustomerID] [int] NULL,
	[OrderID] [uniqueidentifier] NOT NULL,
	[ProductID] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[Discount] [real] NOT NULL
)
WITH ( 
    data_source = OrdersDataSource,
    distribution = SHARDED([CustomerID])
)
GO

-- LOCAL TABLES

-- ### Customers ###

CREATE TABLE [dbo].[Customers](
    [CustomerID] [int] IDENTITY(1, 1) NOT NULL,
    [CustomerCode] [nchar](5) NOT NULL,
    [CompanyName] [nvarchar](40) NOT NULL,
    [ContactName] [nvarchar](30) NULL,
    [ContactTitle] [nvarchar](30) NULL,
    [Address] [nvarchar](60) NULL,
    [City] [nvarchar](15) NULL,
    [Region] [nvarchar](15) NULL,
    [PostalCode] [nvarchar](10) NULL,
    [Country] [nvarchar](15) NULL,
    [Phone] [nvarchar](24) NULL,
    [Fax] [nvarchar](24) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ( [CustomerID] ASC )
)
GO

CREATE NONCLUSTERED INDEX [City] ON [dbo].[Customers]  ( [City] ASC )
GO
CREATE NONCLUSTERED INDEX [CompanyName] ON [dbo].[Customers] ( [CompanyName] ASC ) 
GO
CREATE NONCLUSTERED INDEX [PostalCode] ON [dbo].[Customers] ( [PostalCode] ASC )
GO
CREATE NONCLUSTERED INDEX [Region] ON [dbo].[Customers]  ( [Region] ASC )
GO

SET IDENTITY_INSERT [dbo].[Customers] ON
INSERT INTO [dbo].[Customers]
    ([CustomerID], [CustomerCode], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Country], [Phone], [Fax])
VALUES
    (1, 'ALFKI', 'Alfreds Futterkiste', 'Maria Anders', 'Sales Representative', 'Obere Str. 57', 'Berlin', null, '12209', 'Germany', '030-0074321', '030-0076545'),
    (2, 'ANATR', 'Ana Trujillo Emparedados y helados', 'Ana Trujillo', 'Owner', 'Avda. de la Constitución 2222', 'México D.F.', null, '05021', 'Mexico', '(5) 555-4729', '(5) 555-3745'),
    (3, 'ANTON', 'Antonio Moreno Taquería', 'Antonio Moreno', 'Owner', 'Mataderos  2312', 'México D.F.', null, '05023', 'Mexico', '(5) 555-3932', null),
    (4, 'AROUT', 'Around the Horn', 'Thomas Hardy', 'Sales Representative', '120 Hanover Sq.', 'London', null, 'WA1 1DP', 'UK', '(171) 555-7788', '(171) 555-6750'),
    (5, 'BERGS', 'Berglunds snabbköp', 'Christina Berglund', 'Order Administrator', 'Berguvsvägen  8', 'Luleå', null, 'S-958 22', 'Sweden', '0921-12 34 65', '0921-12 34 67'),
    (6, 'BLAUS', 'Blauer See Delikatessen', 'Hanna Moos', 'Sales Representative', 'Forsterstr. 57', 'Mannheim', null, '68306', 'Germany', '0621-08460', '0621-08924'),
    (7, 'BLONP', 'Blondesddsl père et fils', 'Frédérique Citeaux', 'Marketing Manager', '24, place Kléber', 'Strasbourg', null, '67000', 'France', '88.60.15.31', '88.60.15.32'),
    (8, 'BOLID', 'Bólido Comidas preparadas', 'Martín Sommer', 'Owner', 'C/ Araquil, 67', 'Madrid', null, '28023', 'Spain', '(91) 555 22 82', '(91) 555 91 99'),
    (9, 'BONAP', 'Bon app', 'Laurence Lebihan', 'Owner', '12, rue des Bouchers', 'Marseille', null, '13008', 'France', '91.24.45.40', '91.24.45.41'),
    (10, 'BOTTM', 'Bottom-Dollar Markets', 'Elizabeth Lincoln', 'Accounting Manager', '23 Tsawassen Blvd.', 'Tsawassen', 'BC', 'T2F 8M4', 'Canada', '(604) 555-4729', '(604) 555-3745'),
    (11, 'BSBEV', 'Bs Beverages', 'Victoria Ashworth', 'Sales Representative', 'Fauntleroy Circus', 'London', null, 'EC2 5NT', 'UK', '(171) 555-1212', null),
    (12, 'CACTU', 'Cactus Comidas para llevar', 'Patricio Simpson', 'Sales Agent', 'Cerrito 333', 'Buenos Aires', null, '1010', 'Argentina', '(1) 135-5555', '(1) 135-4892'),
    (13, 'CENTC', 'Centro comercial Moctezuma', 'Francisco Chang', 'Marketing Manager', 'Sierras de Granada 9993', 'México D.F.', null, '05022', 'Mexico', '(5) 555-3392', '(5) 555-7293'),
    (14, 'CHOPS', 'Chop-suey Chinese', 'Yang Wang', 'Owner', 'Hauptstr. 29', 'Bern', null, '3012', 'Switzerland', '0452-076545', null),
    (15, 'COMMI', 'Comércio Mineiro', 'Pedro Afonso', 'Sales Associate', 'Av. dos Lusíadas, 23', 'Sao Paulo', 'SP', '05432-043', 'Brazil', '(11) 555-7647', null),
    (16, 'CONSH', 'Consolidated Holdings', 'Elizabeth Brown', 'Sales Representative', 'Berkeley Gardens 12  Brewery', 'London', null, 'WX1 6LT', 'UK', '(171) 555-2282', '(171) 555-9199'),
    (17, 'DRACD', 'Drachenblut Delikatessen', 'Sven Ottlieb', 'Order Administrator', 'Walserweg 21', 'Aachen', null, '52066', 'Germany', '0241-039123', '0241-059428'),
    (18, 'DUMON', 'Du monde entier', 'Janine Labrune', 'Owner', '67, rue des Cinquante Otages', 'Nantes', null, '44000', 'France', '40.67.88.88', '40.67.89.89'),
    (19, 'EASTC', 'Eastern Connection', 'Ann Devon', 'Sales Agent', '35 King George', 'London', null, 'WX3 6FW', 'UK', '(171) 555-0297', '(171) 555-3373'),
    (20, 'ERNSH', 'Ernst Handel', 'Roland Mendel', 'Sales Manager', 'Kirchgasse 6', 'Graz', null, '8010', 'Austria', '7675-3425', '7675-3426'),
    (21, 'FAMIA', 'Familia Arquibaldo', 'Aria Cruz', 'Marketing Assistant', 'Rua Orós, 92', 'Sao Paulo', 'SP', '05442-030', 'Brazil', '(11) 555-9857', null),
    (22, 'FISSA', 'FISSA Fabrica Inter. Salchichas S.A.', 'Diego Roel', 'Accounting Manager', 'C/ Moralzarzal, 86', 'Madrid', null, '28034', 'Spain', '(91) 555 94 44', '(91) 555 55 93'),
    (23, 'FOLIG', 'Folies gourmandes', 'Martine Rancé', 'Assistant Sales Agent', '184, chaussée de Tournai', 'Lille', null, '59000', 'France', '20.16.10.16', '20.16.10.17'),
    (24, 'FOLKO', 'Folk och fä HB', 'Maria Larsson', 'Owner', 'Åkergatan 24', 'Bräcke', null, 'S-844 67', 'Sweden', '0695-34 67 21', null),
    (25, 'FRANK', 'Frankenversand', 'Peter Franken', 'Marketing Manager', 'Berliner Platz 43', 'München', null, '80805', 'Germany', '089-0877310', '089-0877451'),
    (26, 'FRANR', 'France restauration', 'Carine Schmitt', 'Marketing Manager', '54, rue Royale', 'Nantes', null, '44000', 'France', '40.32.21.21', '40.32.21.20'),
    (27, 'FRANS', 'Franchi S.p.A.', 'Paolo Accorti', 'Sales Representative', 'Via Monte Bianco 34', 'Torino', null, '10100', 'Italy', '011-4988260', '011-4988261'),
    (28, 'FURIB', 'Furia Bacalhau e Frutos do Mar', 'Lino Rodriguez', 'Sales Manager', 'Jardim das rosas n. 32', 'Lisboa', null, '1675', 'Portugal', '(1) 354-2534', '(1) 354-2535'),
    (29, 'GALED', 'Galería del gastrónomo', 'Eduardo Saavedra', 'Marketing Manager', 'Rambla de Cataluña, 23', 'Barcelona', null, '08022', 'Spain', '(93) 203 4560', '(93) 203 4561'),
    (30, 'GODOS', 'Godos Cocina Típica', 'José Pedro Freyre', 'Sales Manager', 'C/ Romero, 33', 'Sevilla', null, '41101', 'Spain', '(95) 555 82 82', null),
    (31, 'GOURL', 'Gourmet Lanchonetes', 'André Fonseca', 'Sales Associate', 'Av. Brasil, 442', 'Campinas', 'SP', '04876-786', 'Brazil', '(11) 555-9482', null),
    (32, 'GREAL', 'Great Lakes Food Market', 'Howard Snyder', 'Marketing Manager', '2732 Baker Blvd.', 'Eugene', 'OR', '97403', 'USA', '(503) 555-7555', null),
    (33, 'GROSR', 'GROSELLA-Restaurante', 'Manuel Pereira', 'Owner', '5ª Ave. Los Palos Grandes', 'Caracas', 'DF', '1081', 'Venezuela', '(2) 283-2951', '(2) 283-3397'),
    (34, 'HANAR', 'Hanari Carnes', 'Mario Pontes', 'Accounting Manager', 'Rua do Paço, 67', 'Rio de Janeiro', 'RJ', '05454-876', 'Brazil', '(21) 555-0091', '(21) 555-8765'),
    (35, 'HILAA', 'HILARION-Abastos', 'Carlos Hernández', 'Sales Representative', 'Carrera 22 con Ave. Carlos Soublette #8-35', 'San Cristóbal', 'Táchira', '5022', 'Venezuela', '(5) 555-1340', '(5) 555-1948'),
    (36, 'HUNGC', 'Hungry Coyote Import Store', 'Yoshi Latimer', 'Sales Representative', 'City Center Plaza 516 Main St.', 'Elgin', 'OR', '97827', 'USA', '(503) 555-6874', '(503) 555-2376'),
    (37, 'HUNGO', 'Hungry Owl All-Night Grocers', 'Patricia McKenna', 'Sales Associate', '8 Johnstown Road', 'Cork', 'Co. Cork', null, 'Ireland', '2967 542', '2967 3333'),
    (38, 'ISLAT', 'Island Trading', 'Helen Bennett', 'Marketing Manager', 'Garden House Crowther Way', 'Cowes', 'Isle of Wight', 'PO31 7PJ', 'UK', '(198) 555-8888', null),
    (39, 'KOENE', 'Königlich Essen', 'Philip Cramer', 'Sales Associate', 'Maubelstr. 90', 'Brandenburg', null, '14776', 'Germany', '0555-09876', null),
    (40, 'LACOR', 'La corne dabondance', 'Daniel Tonini', 'Sales Representative', '67, avenue de lEurope', 'Versailles', null, '78000', 'France', '30.59.84.10', '30.59.85.11'),
    (41, 'LAMAI', 'La maison dAsie', 'Annette Roulet', 'Sales Manager', '1 rue Alsace-Lorraine', 'Toulouse', null, '31000', 'France', '61.77.61.10', '61.77.61.11'),
    (42, 'LAUGB', 'Laughing Bacchus Wine Cellars', 'Yoshi Tannamuri', 'Marketing Assistant', '1900 Oak St.', 'Vancouver', 'BC', 'V3F 2K1', 'Canada', '(604) 555-3392', '(604) 555-7293'),
    (43, 'LAZYK', 'Lazy K Kountry Store', 'John Steel', 'Marketing Manager', '12 Orchestra Terrace', 'Walla Walla', 'WA', '99362', 'USA', '(509) 555-7969', '(509) 555-6221'),
    (44, 'LEHMS', 'Lehmanns Marktstand', 'Renate Messner', 'Sales Representative', 'Magazinweg 7', 'Frankfurt a.M.', null, '60528', 'Germany', '069-0245984', '069-0245874'),
    (45, 'LETSS', 'Lets Stop N Shop', 'Jaime Yorres', 'Owner', '87 Polk St. Suite 5', 'San Francisco', 'CA', '94117', 'USA', '(415) 555-5938', null),
    (46, 'LILAS', 'LILA-Supermercado', 'Carlos González', 'Accounting Manager', 'Carrera 52 con Ave. Bolívar #65-98 Llano Largo', 'Barquisimeto', 'Lara', '3508', 'Venezuela', '(9) 331-6954', '(9) 331-7256'),
    (47, 'LINOD', 'LINO-Delicateses', 'Felipe Izquierdo', 'Owner', 'Ave. 5 de Mayo Porlamar', 'I. de Margarita', 'Nueva Esparta', '4980', 'Venezuela', '(8) 34-56-12', '(8) 34-93-93'),
    (48, 'LONEP', 'Lonesome Pine Restaurant', 'Fran Wilson', 'Sales Manager', '89 Chiaroscuro Rd.', 'Portland', 'OR', '97219', 'USA', '(503) 555-9573', '(503) 555-9646'),
    (49, 'MAGAA', 'Magazzini Alimentari Riuniti', 'Giovanni Rovelli', 'Marketing Manager', 'Via Ludovico il Moro 22', 'Bergamo', null, '24100', 'Italy', '035-640230', '035-640231'),
    (50, 'MAISD', 'Maison Dewey', 'Catherine Dewey', 'Sales Agent', 'Rue Joseph-Bens 532', 'Bruxelles', null, 'B-1180', 'Belgium', '(02) 201 24 67', '(02) 201 24 68'),
    (51, 'MEREP', 'Mère Paillarde', 'Jean Fresnière', 'Marketing Assistant', '43 rue St. Laurent', 'Montréal', 'Québec', 'H1J 1C3', 'Canada', '(514) 555-8054', '(514) 555-8055'),
    (52, 'MORGK', 'Morgenstern Gesundkost', 'Alexander Feuer', 'Marketing Assistant', 'Heerstr. 22', 'Leipzig', null, '04179', 'Germany', '0342-023176', null),
    (53, 'NORTS', 'North/South', 'Simon Crowther', 'Sales Associate', 'South House 300 Queensbridge', 'London', null, 'SW7 1RZ', 'UK', '(171) 555-7733', '(171) 555-2530'),
    (54, 'OCEAN', 'Océano Atlántico Ltda.', 'Yvonne Moncada', 'Sales Agent', 'Ing. Gustavo Moncada 8585 Piso 20-A', 'Buenos Aires', null, '1010', 'Argentina', '(1) 135-5333', '(1) 135-5535'),
    (55, 'OLDWO', 'Old World Delicatessen', 'Rene Phillips', 'Sales Representative', '2743 Bering St.', 'Anchorage', 'AK', '99508', 'USA', '(907) 555-7584', '(907) 555-2880'),
    (56, 'OTTIK', 'Ottilies Käseladen', 'Henriette Pfalzheim', 'Owner', 'Mehrheimerstr. 369', 'Köln', null, '50739', 'Germany', '0221-0644327', '0221-0765721'),
    (57, 'PARIS', 'Paris spécialités', 'Marie Bertrand', 'Owner', '265, boulevard Charonne', 'Paris', null, '75012', 'France', '(1) 42.34.22.66', '(1) 42.34.22.77'),
    (58, 'PERIC', 'Pericles Comidas clásicas', 'Guillermo Fernández', 'Sales Representative', 'Calle Dr. Jorge Cash 321', 'México D.F.', null, '05033', 'Mexico', '(5) 552-3745', '(5) 545-3745'),
    (59, 'PICCO', 'Piccolo und mehr', 'Georg Pipps', 'Sales Manager', 'Geislweg 14', 'Salzburg', null, '5020', 'Austria', '6562-9722', '6562-9723'),
    (60, 'PRINI', 'Princesa Isabel Vinhos', 'Isabel de Castro', 'Sales Representative', 'Estrada da saúde n. 58', 'Lisboa', null, '1756', 'Portugal', '(1) 356-5634', null),
    (61, 'QUEDE', 'Que Delícia', 'Bernardo Batista', 'Accounting Manager', 'Rua da Panificadora, 12', 'Rio de Janeiro', 'RJ', '02389-673', 'Brazil', '(21) 555-4252', '(21) 555-4545'),
    (62, 'QUEEN', 'Queen Cozinha', 'Lúcia Carvalho', 'Marketing Assistant', 'Alameda dos Canàrios, 891', 'Sao Paulo', 'SP', '05487-020', 'Brazil', '(11) 555-1189', null),
    (63, 'QUICK', 'QUICK-Stop', 'Horst Kloss', 'Accounting Manager', 'Taucherstraße 10', 'Cunewalde', null, '01307', 'Germany', '0372-035188', null),
    (64, 'RANCH', 'Rancho grande', 'Sergio Gutiérrez', 'Sales Representative', 'Av. del Libertador 900', 'Buenos Aires', null, '1010', 'Argentina', '(1) 123-5555', '(1) 123-5556'),
    (65, 'RATTC', 'Rattlesnake Canyon Grocery', 'Paula Wilson', 'Assistant Sales Representative', '2817 Milton Dr.', 'Albuquerque', 'NM', '87110', 'USA', '(505) 555-5939', '(505) 555-3620'),
    (66, 'REGGC', 'Reggiani Caseifici', 'Maurizio Moroni', 'Sales Associate', 'Strada Provinciale 124', 'Reggio Emilia', null, '42100', 'Italy', '0522-556721', '0522-556722'),
    (67, 'RICAR', 'Ricardo Adocicados', 'Janete Limeira', 'Assistant Sales Agent', 'Av. Copacabana, 267', 'Rio de Janeiro', 'RJ', '02389-890', 'Brazil', '(21) 555-3412', null),
    (68, 'RICSU', 'Richter Supermarkt', 'Michael Holz', 'Sales Manager', 'Grenzacherweg 237', 'Genève', null, '1203', 'Switzerland', '0897-034214', null),
    (69, 'ROMEY', 'Romero y tomillo', 'Alejandra Camino', 'Accounting Manager', 'Gran Vía, 1', 'Madrid', null, '28001', 'Spain', '(91) 745 6200', '(91) 745 6210'),
    (70, 'SANTG', 'Santé Gourmet', 'Jonas Bergulfsen', 'Owner', 'Erling Skakkes gate 78', 'Stavern', null, '4110', 'Norway', '07-98 92 35', '07-98 92 47'),
    (71, 'SAVEA', 'Save-a-lot Markets', 'Jose Pavarotti', 'Sales Representative', '187 Suffolk Ln.', 'Boise', 'ID', '83720', 'USA', '(208) 555-8097', null),
    (72, 'SEVES', 'Seven Seas Imports', 'Hari Kumar', 'Sales Manager', '90 Wadhurst Rd.', 'London', null, 'OX15 4NB', 'UK', '(171) 555-1717', '(171) 555-5646'),
    (73, 'SIMOB', 'Simons bistro', 'Jytte Petersen', 'Owner', 'Vinbæltet 34', 'Kobenhavn', null, '1734', 'Denmark', '31 12 34 56', '31 13 35 57'),
    (74, 'SPECD', 'Spécialités du monde', 'Dominique Perrier', 'Marketing Manager', '25, rue Lauriston', 'Paris', null, '75016', 'France', '(1) 47.55.60.10', '(1) 47.55.60.20'),
    (75, 'SPLIR', 'Split Rail Beer & Ale', 'Art Braunschweiger', 'Sales Manager', 'P.O. Box 555', 'Lander', 'WY', '82520', 'USA', '(307) 555-4680', '(307) 555-6525'),
    (76, 'SUPRD', 'Suprêmes délices', 'Pascale Cartrain', 'Accounting Manager', 'Boulevard Tirou, 255', 'Charleroi', null, 'B-6000', 'Belgium', '(071) 23 67 22 20', '(071) 23 67 22 21'),
    (77, 'THEBI', 'The Big Cheese', 'Liz Nixon', 'Marketing Manager', '89 Jefferson Way Suite 2', 'Portland', 'OR', '97201', 'USA', '(503) 555-3612', null),
    (78, 'THECR', 'The Cracker Box', 'Liu Wong', 'Marketing Assistant', '55 Grizzly Peak Rd.', 'Butte', 'MT', '59801', 'USA', '(406) 555-5834', '(406) 555-8083'),
    (79, 'TOMSP', 'Toms Spezialitäten', 'Karin Josephs', 'Marketing Manager', 'Luisenstr. 48', 'Münster', null, '44087', 'Germany', '0251-031259', '0251-035695'),
    (80, 'TORTU', 'Tortuga Restaurante', 'Miguel Angel Paolino', 'Owner', 'Avda. Azteca 123', 'México D.F.', null, '05033', 'Mexico', '(5) 555-2933', null),
    (81, 'TRADH', 'Tradição Hipermercados', 'Anabela Domingues', 'Sales Representative', 'Av. Inês de Castro, 414', 'Sao Paulo', 'SP', '05634-030', 'Brazil', '(11) 555-2167', '(11) 555-2168'),
    (82, 'TRAIH', 'Trails Head Gourmet Provisioners', 'Helvetius Nagy', 'Sales Associate', '722 DaVinci Blvd.', 'Kirkland', 'WA', '98034', 'USA', '(206) 555-8257', '(206) 555-2174'),
    (83, 'VAFFE', 'Vaffeljernet', 'Palle Ibsen', 'Sales Manager', 'Smagsloget 45', 'Århus', null, '8200', 'Denmark', '86 21 32 43', '86 22 33 44'),
    (84, 'VICTE', 'Victuailles en stock', 'Mary Saveley', 'Sales Agent', '2, rue du Commerce', 'Lyon', null, '69004', 'France', '78.32.54.86', '78.32.54.87'),
    (85, 'VINET', 'Vins et alcools Chevalier', 'Paul Henriot', 'Accounting Manager', '59 rue de lAbbaye', 'Reims', null, '51100', 'France', '26.47.15.10', '26.47.15.11'),
    (86, 'WANDK', 'Die Wandernde Kuh', 'Rita Müller', 'Sales Representative', 'Adenauerallee 900', 'Stuttgart', null, '70563', 'Germany', '0711-020361', '0711-035428'),
    (87, 'WARTH', 'Wartian Herkku', 'Pirkko Koskitalo', 'Accounting Manager', 'Torikatu 38', 'Oulu', null, '90110', 'Finland', '981-443655', '981-443655'),
    (88, 'WELLI', 'Wellington Importadora', 'Paula Parente', 'Sales Manager', 'Rua do Mercado, 12', 'Resende', 'SP', '08737-363', 'Brazil', '(14) 555-8122', null),
    (89, 'WHITC', 'White Clover Markets', 'Karl Jablonski', 'Owner', '305 - 14th Ave. S. Suite 3B', 'Seattle', 'WA', '98128', 'USA', '(206) 555-4112', '(206) 555-4115'),
    (90, 'WILMK', 'Wilman Kala', 'Matti Karttunen', 'Owner/Marketing Assistant', 'Keskuskatu 45', 'Helsinki', null, '21240', 'Finland', '90-224 8858', '90-224 8858'),
    (91, 'WOLZA', 'Wolski  Zajazd', 'Zbyszek Piestrzeniewicz', 'Owner', 'ul. Filtrowa 68', 'Warszawa', null, '01-012', 'Poland', '(26) 642-7012', '(26) 642-7012');
SET IDENTITY_INSERT [dbo].[Customers] OFF