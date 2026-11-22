-- Run this script after creating FinTrackerDb (or let Identity migrations create the database).
-- The ASP.NET Core Identity tables are created separately by EF Core migrations.
CREATE TABLE FinanceTransactions
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL CHECK (Type IN ('Income','Expense')),
    Amount DECIMAL(18,2) NOT NULL CHECK (Amount > 0),
    Category NVARCHAR(60) NOT NULL,
    TransactionDate DATE NOT NULL,
    Note NVARCHAR(300) NULL,
    UserId NVARCHAR(450) NOT NULL
);
CREATE INDEX IX_FinanceTransactions_UserId_Date
ON FinanceTransactions(UserId, TransactionDate DESC);
