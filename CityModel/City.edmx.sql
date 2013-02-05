
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/02/2011 20:01:57
-- Generated from EDMX file: D:\programming\visual studio 10\projects\CitySilverlight\CityModel\City.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CITY];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CreditCardPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_CreditCardTransactions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];
GO
IF OBJECT_ID(N'[dbo].[FK_PoiTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_PoiTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_PoiPoiType_Poi]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PoiPoiType] DROP CONSTRAINT [FK_PoiPoiType_Poi];
GO
IF OBJECT_ID(N'[dbo].[FK_PoiPoiType_PoiType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PoiPoiType] DROP CONSTRAINT [FK_PoiPoiType_PoiType];
GO
IF OBJECT_ID(N'[dbo].[FK_CreditCardCurrency]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreditCards] DROP CONSTRAINT [FK_CreditCardCurrency];
GO
IF OBJECT_ID(N'[dbo].[FK_PersonDemand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Demands] DROP CONSTRAINT [FK_PersonDemand];
GO
IF OBJECT_ID(N'[dbo].[FK_DemandPoiType_Demand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DemandPoiType] DROP CONSTRAINT [FK_DemandPoiType_Demand];
GO
IF OBJECT_ID(N'[dbo].[FK_DemandPoiType_PoiType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DemandPoiType] DROP CONSTRAINT [FK_DemandPoiType_PoiType];
GO
IF OBJECT_ID(N'[dbo].[FK_CreditCardBank]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreditCards] DROP CONSTRAINT [FK_CreditCardBank];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_AddressPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_DistrictAddress]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Addresses] DROP CONSTRAINT [FK_DistrictAddress];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationDistrict]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Districts] DROP CONSTRAINT [FK_ConfigurationDistrict];
GO
IF OBJECT_ID(N'[dbo].[FK_TransactionDemand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_TransactionDemand];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationPoiType_Configuration]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConfigurationPoiType] DROP CONSTRAINT [FK_ConfigurationPoiType_Configuration];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationPoiType_PoiType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConfigurationPoiType] DROP CONSTRAINT [FK_ConfigurationPoiType_PoiType];
GO
IF OBJECT_ID(N'[dbo].[FK_DistrictMapPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MapPoints] DROP CONSTRAINT [FK_DistrictMapPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_PersonPersonTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_PersonPersonTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressMapPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Addresses] DROP CONSTRAINT [FK_AddressMapPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_PoiMapPoint]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pois] DROP CONSTRAINT [FK_PoiMapPoint];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationSimulationMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Configurations] DROP CONSTRAINT [FK_ConfigurationSimulationMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationPois]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pois] DROP CONSTRAINT [FK_ConfigurationPois];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Districts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Districts];
GO
IF OBJECT_ID(N'[dbo].[People]', 'U') IS NOT NULL
    DROP TABLE [dbo].[People];
GO
IF OBJECT_ID(N'[dbo].[CreditCards]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CreditCards];
GO
IF OBJECT_ID(N'[dbo].[MapPoints]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MapPoints];
GO
IF OBJECT_ID(N'[dbo].[Banks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Banks];
GO
IF OBJECT_ID(N'[dbo].[Currencies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Currencies];
GO
IF OBJECT_ID(N'[dbo].[PoiTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PoiTypes];
GO
IF OBJECT_ID(N'[dbo].[Transactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Transactions];
GO
IF OBJECT_ID(N'[dbo].[Demands]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Demands];
GO
IF OBJECT_ID(N'[dbo].[Configurations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configurations];
GO
IF OBJECT_ID(N'[dbo].[PersonTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PersonTemplates];
GO
IF OBJECT_ID(N'[dbo].[Pois]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Pois];
GO
IF OBJECT_ID(N'[dbo].[Addresses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Addresses];
GO
IF OBJECT_ID(N'[dbo].[SimulationMethods]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SimulationMethods];
GO
IF OBJECT_ID(N'[dbo].[PoiPoiType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PoiPoiType];
GO
IF OBJECT_ID(N'[dbo].[DemandPoiType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DemandPoiType];
GO
IF OBJECT_ID(N'[dbo].[ConfigurationPoiType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConfigurationPoiType];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Districts'
CREATE TABLE [dbo].[Districts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Population] int  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Configuration_Id] int  NOT NULL
);
GO

-- Creating table 'People'
CREATE TABLE [dbo].[People] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Address_Id] int  NULL,
    [Template_Id] int  NOT NULL
);
GO

-- Creating table 'CreditCards'
CREATE TABLE [dbo].[CreditCards] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Balance] float  NOT NULL,
    [Limit] float  NOT NULL,
    [Number] nvarchar(max)  NOT NULL,
    [Owner_Id] int  NOT NULL,
    [Currency_Id] int  NOT NULL,
    [Bank_Id] int  NOT NULL
);
GO

-- Creating table 'MapPoints'
CREATE TABLE [dbo].[MapPoints] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Lat] float  NOT NULL,
    [Lng] float  NOT NULL,
    [DistrictMapPoint_MapPoint_Id] int  NULL
);
GO

-- Creating table 'Banks'
CREATE TABLE [dbo].[Banks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Currencies'
CREATE TABLE [dbo].[Currencies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PoiTypes'
CREATE TABLE [dbo].[PoiTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Transactions'
CREATE TABLE [dbo].[Transactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Value] float  NOT NULL,
    [CreditCard_Id] int  NOT NULL,
    [Poi_Id] int  NOT NULL,
    [Demand_Id] int  NOT NULL
);
GO

-- Creating table 'Demands'
CREATE TABLE [dbo].[Demands] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Fulfilled] bit  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Person_Id] int  NOT NULL
);
GO

-- Creating table 'Configurations'
CREATE TABLE [dbo].[Configurations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [AreaRange] float  NOT NULL,
    [AreaLat] float  NOT NULL,
    [AreaLng] float  NOT NULL,
    [SimulationStartDate] datetime  NOT NULL,
    [SimulationEndDate] datetime  NOT NULL,
    [PersonDemandsPerDay] int  NOT NULL,
    [SimulationMethod_Id] int  NOT NULL
);
GO

-- Creating table 'PersonTemplates'
CREATE TABLE [dbo].[PersonTemplates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [IsMale] bit  NOT NULL
);
GO

-- Creating table 'Pois'
CREATE TABLE [dbo].[Pois] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Vicinity] nvarchar(max)  NOT NULL,
    [MapPoint_Id] int  NOT NULL,
    [Configuration_Id] int  NOT NULL
);
GO

-- Creating table 'Addresses'
CREATE TABLE [dbo].[Addresses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AddressLine] nvarchar(max)  NULL,
    [District_Id] int  NOT NULL,
    [MapPoint_Id] int  NOT NULL
);
GO

-- Creating table 'SimulationMethods'
CREATE TABLE [dbo].[SimulationMethods] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PoiPoiType'
CREATE TABLE [dbo].[PoiPoiType] (
    [PoiPoiType_PoiType_Id] int  NOT NULL,
    [Type_Id] int  NOT NULL
);
GO

-- Creating table 'DemandPoiType'
CREATE TABLE [dbo].[DemandPoiType] (
    [DemandPoiType_PoiType_Id] int  NOT NULL,
    [PoiTypes_Id] int  NOT NULL
);
GO

-- Creating table 'ConfigurationPoiType'
CREATE TABLE [dbo].[ConfigurationPoiType] (
    [ConfigurationPoiType_PoiType_Id] int  NOT NULL,
    [PoiTypes_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Districts'
ALTER TABLE [dbo].[Districts]
ADD CONSTRAINT [PK_Districts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'People'
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [PK_People]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CreditCards'
ALTER TABLE [dbo].[CreditCards]
ADD CONSTRAINT [PK_CreditCards]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MapPoints'
ALTER TABLE [dbo].[MapPoints]
ADD CONSTRAINT [PK_MapPoints]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Banks'
ALTER TABLE [dbo].[Banks]
ADD CONSTRAINT [PK_Banks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Currencies'
ALTER TABLE [dbo].[Currencies]
ADD CONSTRAINT [PK_Currencies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PoiTypes'
ALTER TABLE [dbo].[PoiTypes]
ADD CONSTRAINT [PK_PoiTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [PK_Transactions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Demands'
ALTER TABLE [dbo].[Demands]
ADD CONSTRAINT [PK_Demands]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Configurations'
ALTER TABLE [dbo].[Configurations]
ADD CONSTRAINT [PK_Configurations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PersonTemplates'
ALTER TABLE [dbo].[PersonTemplates]
ADD CONSTRAINT [PK_PersonTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Pois'
ALTER TABLE [dbo].[Pois]
ADD CONSTRAINT [PK_Pois]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [PK_Addresses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SimulationMethods'
ALTER TABLE [dbo].[SimulationMethods]
ADD CONSTRAINT [PK_SimulationMethods]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [PoiPoiType_PoiType_Id], [Type_Id] in table 'PoiPoiType'
ALTER TABLE [dbo].[PoiPoiType]
ADD CONSTRAINT [PK_PoiPoiType]
    PRIMARY KEY NONCLUSTERED ([PoiPoiType_PoiType_Id], [Type_Id] ASC);
GO

-- Creating primary key on [DemandPoiType_PoiType_Id], [PoiTypes_Id] in table 'DemandPoiType'
ALTER TABLE [dbo].[DemandPoiType]
ADD CONSTRAINT [PK_DemandPoiType]
    PRIMARY KEY NONCLUSTERED ([DemandPoiType_PoiType_Id], [PoiTypes_Id] ASC);
GO

-- Creating primary key on [ConfigurationPoiType_PoiType_Id], [PoiTypes_Id] in table 'ConfigurationPoiType'
ALTER TABLE [dbo].[ConfigurationPoiType]
ADD CONSTRAINT [PK_ConfigurationPoiType]
    PRIMARY KEY NONCLUSTERED ([ConfigurationPoiType_PoiType_Id], [PoiTypes_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Owner_Id] in table 'CreditCards'
ALTER TABLE [dbo].[CreditCards]
ADD CONSTRAINT [FK_CreditCardPerson]
    FOREIGN KEY ([Owner_Id])
    REFERENCES [dbo].[People]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CreditCardPerson'
CREATE INDEX [IX_FK_CreditCardPerson]
ON [dbo].[CreditCards]
    ([Owner_Id]);
GO

-- Creating foreign key on [CreditCard_Id] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_CreditCardTransactions]
    FOREIGN KEY ([CreditCard_Id])
    REFERENCES [dbo].[CreditCards]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CreditCardTransactions'
CREATE INDEX [IX_FK_CreditCardTransactions]
ON [dbo].[Transactions]
    ([CreditCard_Id]);
GO

-- Creating foreign key on [Poi_Id] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_PoiTransaction]
    FOREIGN KEY ([Poi_Id])
    REFERENCES [dbo].[Pois]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PoiTransaction'
CREATE INDEX [IX_FK_PoiTransaction]
ON [dbo].[Transactions]
    ([Poi_Id]);
GO

-- Creating foreign key on [PoiPoiType_PoiType_Id] in table 'PoiPoiType'
ALTER TABLE [dbo].[PoiPoiType]
ADD CONSTRAINT [FK_PoiPoiType_Poi]
    FOREIGN KEY ([PoiPoiType_PoiType_Id])
    REFERENCES [dbo].[Pois]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Type_Id] in table 'PoiPoiType'
ALTER TABLE [dbo].[PoiPoiType]
ADD CONSTRAINT [FK_PoiPoiType_PoiType]
    FOREIGN KEY ([Type_Id])
    REFERENCES [dbo].[PoiTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PoiPoiType_PoiType'
CREATE INDEX [IX_FK_PoiPoiType_PoiType]
ON [dbo].[PoiPoiType]
    ([Type_Id]);
GO

-- Creating foreign key on [Currency_Id] in table 'CreditCards'
ALTER TABLE [dbo].[CreditCards]
ADD CONSTRAINT [FK_CreditCardCurrency]
    FOREIGN KEY ([Currency_Id])
    REFERENCES [dbo].[Currencies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CreditCardCurrency'
CREATE INDEX [IX_FK_CreditCardCurrency]
ON [dbo].[CreditCards]
    ([Currency_Id]);
GO

-- Creating foreign key on [Person_Id] in table 'Demands'
ALTER TABLE [dbo].[Demands]
ADD CONSTRAINT [FK_PersonDemand]
    FOREIGN KEY ([Person_Id])
    REFERENCES [dbo].[People]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PersonDemand'
CREATE INDEX [IX_FK_PersonDemand]
ON [dbo].[Demands]
    ([Person_Id]);
GO

-- Creating foreign key on [DemandPoiType_PoiType_Id] in table 'DemandPoiType'
ALTER TABLE [dbo].[DemandPoiType]
ADD CONSTRAINT [FK_DemandPoiType_Demand]
    FOREIGN KEY ([DemandPoiType_PoiType_Id])
    REFERENCES [dbo].[Demands]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [PoiTypes_Id] in table 'DemandPoiType'
ALTER TABLE [dbo].[DemandPoiType]
ADD CONSTRAINT [FK_DemandPoiType_PoiType]
    FOREIGN KEY ([PoiTypes_Id])
    REFERENCES [dbo].[PoiTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DemandPoiType_PoiType'
CREATE INDEX [IX_FK_DemandPoiType_PoiType]
ON [dbo].[DemandPoiType]
    ([PoiTypes_Id]);
GO

-- Creating foreign key on [Bank_Id] in table 'CreditCards'
ALTER TABLE [dbo].[CreditCards]
ADD CONSTRAINT [FK_CreditCardBank]
    FOREIGN KEY ([Bank_Id])
    REFERENCES [dbo].[Banks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CreditCardBank'
CREATE INDEX [IX_FK_CreditCardBank]
ON [dbo].[CreditCards]
    ([Bank_Id]);
GO

-- Creating foreign key on [Address_Id] in table 'People'
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [FK_AddressPerson]
    FOREIGN KEY ([Address_Id])
    REFERENCES [dbo].[Addresses]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressPerson'
CREATE INDEX [IX_FK_AddressPerson]
ON [dbo].[People]
    ([Address_Id]);
GO

-- Creating foreign key on [District_Id] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [FK_DistrictAddress]
    FOREIGN KEY ([District_Id])
    REFERENCES [dbo].[Districts]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DistrictAddress'
CREATE INDEX [IX_FK_DistrictAddress]
ON [dbo].[Addresses]
    ([District_Id]);
GO

-- Creating foreign key on [Configuration_Id] in table 'Districts'
ALTER TABLE [dbo].[Districts]
ADD CONSTRAINT [FK_ConfigurationDistrict]
    FOREIGN KEY ([Configuration_Id])
    REFERENCES [dbo].[Configurations]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationDistrict'
CREATE INDEX [IX_FK_ConfigurationDistrict]
ON [dbo].[Districts]
    ([Configuration_Id]);
GO

-- Creating foreign key on [Demand_Id] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_TransactionDemand]
    FOREIGN KEY ([Demand_Id])
    REFERENCES [dbo].[Demands]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TransactionDemand'
CREATE INDEX [IX_FK_TransactionDemand]
ON [dbo].[Transactions]
    ([Demand_Id]);
GO

-- Creating foreign key on [ConfigurationPoiType_PoiType_Id] in table 'ConfigurationPoiType'
ALTER TABLE [dbo].[ConfigurationPoiType]
ADD CONSTRAINT [FK_ConfigurationPoiType_Configuration]
    FOREIGN KEY ([ConfigurationPoiType_PoiType_Id])
    REFERENCES [dbo].[Configurations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [PoiTypes_Id] in table 'ConfigurationPoiType'
ALTER TABLE [dbo].[ConfigurationPoiType]
ADD CONSTRAINT [FK_ConfigurationPoiType_PoiType]
    FOREIGN KEY ([PoiTypes_Id])
    REFERENCES [dbo].[PoiTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationPoiType_PoiType'
CREATE INDEX [IX_FK_ConfigurationPoiType_PoiType]
ON [dbo].[ConfigurationPoiType]
    ([PoiTypes_Id]);
GO

-- Creating foreign key on [DistrictMapPoint_MapPoint_Id] in table 'MapPoints'
ALTER TABLE [dbo].[MapPoints]
ADD CONSTRAINT [FK_DistrictMapPoint]
    FOREIGN KEY ([DistrictMapPoint_MapPoint_Id])
    REFERENCES [dbo].[Districts]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DistrictMapPoint'
CREATE INDEX [IX_FK_DistrictMapPoint]
ON [dbo].[MapPoints]
    ([DistrictMapPoint_MapPoint_Id]);
GO

-- Creating foreign key on [Template_Id] in table 'People'
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [FK_PersonPersonTemplate]
    FOREIGN KEY ([Template_Id])
    REFERENCES [dbo].[PersonTemplates]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PersonPersonTemplate'
CREATE INDEX [IX_FK_PersonPersonTemplate]
ON [dbo].[People]
    ([Template_Id]);
GO

-- Creating foreign key on [MapPoint_Id] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [FK_AddressMapPoint]
    FOREIGN KEY ([MapPoint_Id])
    REFERENCES [dbo].[MapPoints]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressMapPoint'
CREATE INDEX [IX_FK_AddressMapPoint]
ON [dbo].[Addresses]
    ([MapPoint_Id]);
GO

-- Creating foreign key on [MapPoint_Id] in table 'Pois'
ALTER TABLE [dbo].[Pois]
ADD CONSTRAINT [FK_PoiMapPoint]
    FOREIGN KEY ([MapPoint_Id])
    REFERENCES [dbo].[MapPoints]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PoiMapPoint'
CREATE INDEX [IX_FK_PoiMapPoint]
ON [dbo].[Pois]
    ([MapPoint_Id]);
GO

-- Creating foreign key on [SimulationMethod_Id] in table 'Configurations'
ALTER TABLE [dbo].[Configurations]
ADD CONSTRAINT [FK_ConfigurationSimulationMethod]
    FOREIGN KEY ([SimulationMethod_Id])
    REFERENCES [dbo].[SimulationMethods]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationSimulationMethod'
CREATE INDEX [IX_FK_ConfigurationSimulationMethod]
ON [dbo].[Configurations]
    ([SimulationMethod_Id]);
GO

-- Creating foreign key on [Configuration_Id] in table 'Pois'
ALTER TABLE [dbo].[Pois]
ADD CONSTRAINT [FK_ConfigurationPois]
    FOREIGN KEY ([Configuration_Id])
    REFERENCES [dbo].[Configurations]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationPois'
CREATE INDEX [IX_FK_ConfigurationPois]
ON [dbo].[Pois]
    ([Configuration_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------