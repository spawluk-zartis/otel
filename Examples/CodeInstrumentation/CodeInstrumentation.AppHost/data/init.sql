-- SQL Server init script

-- Create the [database] database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'database')
BEGIN
  CREATE DATABASE [database];
END;
GO

USE [database];
GO
