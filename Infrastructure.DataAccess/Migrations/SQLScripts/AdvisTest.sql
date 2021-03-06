/****** Script for SelectTopNRows command from SSMS  ******/
USE kitos
SELECT TOP (1000) 
      [Org].[Name] AS OrganisationNavn
	  ,[Contracts].[Name] AS KontraktNavn
	  ,[Advices].[Name] AS AdvisNavn
	  ,[Subject] AS Emne
      ,[StopDate] AS AfendelsesDato
  FROM [kitos].[dbo].[Advice] AS Advices
  INNER JOIN [dbo].[ItContract] AS Contracts
  ON [Contracts].[Id] = [Advices].[ObjectOwnerId]
  INNER JOIN [dbo].[Organization] AS Org
  ON [Contracts].[OrganizationId] = [Org].[Id]
  WHERE [Advices].StopDate >= Convert(datetime, '2017-02-08' ) AND [Advices].StopDate < Convert(datetime, '2017-03-08' )