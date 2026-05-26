# Guía de Migraciones — Entity Framework Core

Esta guía explica cómo gestionar los cambios en la base de datos utilizando EF Core Migrations en este proyecto.

---

## 1. ¿Qué son las Migraciones?

En este proyecto utilizamos el enfoque **Code-First**. Esto significa que las clases de C# en el proyecto `.Domain` son la "fuente de la verdad".

*   **El Modelo manda:** Si cambias una clase (ej. añades una propiedad a `Policy`), la base de datos debe actualizarse para reflejar ese cambio.
*   **La Migración es el puente:** Es un archivo de código que EF Core genera comparando tu modelo actual con la última versión de la base de datos.
*   **Historial:** EF Core guarda una tabla llamada `__EFMigrationsHistory` en la base de datos para saber qué cambios se han aplicado ya.

---

## 2. Requisitos previos

Para ejecutar estos comandos, debes tener instalada la herramienta global de EF Core. Si no la tienes, ejecuta:

```bash
dotnet tool install --global dotnet-ef
```

---

## 3. Comandos Principales

Debes ejecutar estos comandos desde la raíz de la solución (`PoliciesService/`).

### A. Crear una nueva migración
Cada vez que hagas un cambio en las clases del proyecto `.Domain`, ejecuta:

```bash
dotnet ef migrations add NombreDeLaMigracion --project PoliciesService.Infrastructure --startup-project PoliciesService.Api
```

*   **NombreDeLaMigracion:** Usa algo descriptivo como `AddUserAddress` o `UpdatePolicyConstraints`.
*   **--project:** Indica dónde se guardará el código de la migración (`Infrastructure`).
*   **--startup-project:** Indica dónde está la configuración de la base de datos (`Api`).

### B. Aplicar cambios a la Base de Datos
Para que los cambios se reflejen en SQL Server, ejecuta:

```bash
dotnet ef database update --project PoliciesService.Infrastructure --startup-project PoliciesService.Api
```

### C. Deshacer el último cambio (Solo en local)
Si aplicaste una migración y te diste cuenta de un error **antes de subir el código**:

```bash
# 1. Revierte la base de datos a la migración anterior
dotnet ef database update NombreDeMigracionAnterior --project PoliciesService.Infrastructure --startup-project PoliciesService.Api

# 2. Borra el archivo de la migración errónea
dotnet ef migrations remove --project PoliciesService.Infrastructure --startup-project PoliciesService.Api
```

---

## 4. Flujo de Trabajo Recomendado

1.  **Modifica** tus entidades en `PoliciesService.Domain`.
2.  **Compila** la solución para asegurar que no hay errores (`dotnet build`).
3.  **Genera** la migración (Punto 3.A).
4.  **Revisa** el archivo generado en `Infrastructure/Migrations` por si acaso.
5.  **Actualiza** la base de datos (Punto 3.B).

---

## 5. Notas Importantes

*   **No borres la tabla `__EFMigrationsHistory`**: Si lo haces, EF intentará crear todas las tablas de nuevo y fallará.
*   **Triggers manuales**: Si necesitas añadir triggers o lógica SQL pura que EF no detecta, puedes escribirla dentro del método `Up` de la migración generada usando `migrationBuilder.Sql("TU SQL AQUÍ");`.
