-- Banco de dados e tabela de alertas de dengue por semana epidemiológica.
-- Executar antes de iniciar a aplicação pela primeira vez.

IF DB_ID('alerta_dengue') IS NULL
    CREATE DATABASE alerta_dengue;
GO

USE alerta_dengue;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'alertas')
BEGIN
    CREATE TABLE alertas (
        id                INT IDENTITY(1,1) NOT NULL,
        ano               INT               NOT NULL,
        semana            INT               NOT NULL,
        casos_estimados   DECIMAL(10,2)     NOT NULL,
        casos_notificados INT               NOT NULL,
        nivel_alerta      INT               NOT NULL,
        data_registro_utc DATETIME2(3)      NOT NULL,

        CONSTRAINT pk_alertas        PRIMARY KEY (id),
        CONSTRAINT uq_alertas_semana UNIQUE (ano, semana),
        CONSTRAINT ck_alertas_semana CHECK (semana BETWEEN 1 AND 53),
        CONSTRAINT ck_alertas_nivel  CHECK (nivel_alerta BETWEEN 1 AND 4),
        CONSTRAINT ck_alertas_casos  CHECK (casos_estimados >= 0 AND casos_notificados >= 0)
    );
END
GO