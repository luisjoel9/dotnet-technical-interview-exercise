DECLARE @User1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User2Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[User] ([Id], [Username], [Email], [Password], [CreatedAt])
VALUES 
    (@User1Id, 'cris', 'cris@asdf.com', '$2a$12$tsLR7n0zLUbZswlJ9nfV9.4XOg07KCU.yaTD.eNZ7EwhZcZvpUvlq', GETDATE()),
    (@User2Id, 'pepe', 'pepe@asdf.com', '$2a$12$yzSlRlsohg3ufAnxL2ybwuTuB3NmKuf0C5wA79KOjnL4E3kkHtdmy', GETDATE());
	
INSERT INTO [dbo].[Reminder] ([Id], [UserId], [Title], [Description], [TargetDateTime], [Status])
VALUES 
    (NEWID(), @User1Id, 'Reunión de equipo', 'Sincronización semanal sobre el proyecto de recordatorios.', DATEADD(day, 2, GETDATE()), 0),
    (NEWID(), @User1Id, 'Pagar servicio de internet', 'Evitar corte del servicio, último día de pago.', DATEADD(day, 5, GETDATE()), 1),
    (NEWID(), @User1Id, 'Comprar víveres', NULL, DATEADD(hour, 4, GETDATE()), 1),

    (NEWID(), @User2Id, 'Cita médica - Dentista', 'Revisión anual y limpieza.', DATEADD(day, 7, GETDATE()), 0),
    (NEWID(), @User2Id, 'Entrega de reporte financiero', 'Enviar al director ejecutivo por correo.', DATEADD(day, -1, GETDATE()), 2);

	
