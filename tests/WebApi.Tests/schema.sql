-- Schema usado apenas pelos testes de integração.

CREATE TABLE `Users` (
    `Id` char(36) NOT NULL,
    `Active` tinyint(1) NOT NULL,
    `Name` longtext NOT NULL,
    `Email` longtext NOT NULL,
    `Password` longtext NOT NULL,
    PRIMARY KEY (`Id`)
);
