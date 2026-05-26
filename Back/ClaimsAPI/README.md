# InterDavid - Backend de Claims

## Descripción
**InterDavid** es el servicio de backend encargado de la gestión de siniestros (Claims) dentro del ecosistema de seguros. Este servicio proporciona una API REST robusta para gestionar el ciclo de vida de un siniestro, desde su creación hasta su resolución, incluyendo un sistema de comentarios y auditoría detallada.

## Arquitectura
El proyecto sigue los principios de **Arquitectura Limpia (Clean Architecture)** y está desarrollado con **ASP.NET Core 9.0**. Se divide en los siguientes proyectos:

- **Claims.Api**: Capa de presentación (REST API). Contiene los controladores, la configuración de la aplicación y el manejo de peticiones HTTP.
- **Claims.Application**: Capa de aplicación. Contiene la lógica de negocio, interfaces, servicios, DTOs y validaciones.
- **Claims.Domain**: Capa de dominio. Contiene las entidades principales (`Claim`, `Comment`, `Audit`) y la lógica pura del negocio.
- **Claims.Infrastructure**: Capa de infraestructura. Implementa el acceso a datos mediante **Entity Framework Core (SQL Server)** y la comunicación con servicios externos (como Policies) utilizando **Refit**.

## Características Principales
1.  **Gestión de Siniestros**: CRUD completo de claims con estados (SUBMITTED, UNDER_REVIEW, APPROVED, REJECTED, etc.).
2.  **Sistema de Comentarios**: Permite añadir, listar y eliminar comentarios asociados a cada siniestro.
3.  **Auditoría Automática**: Registro histórico de cambios. Cada vez que un campo de un siniestro se modifica, se guarda quién lo hizo, qué cambió y los valores anterior y nuevo.
4.  **Integración con Policies**: Validación de la existencia y vigencia de pólizas antes de permitir la creación de un siniestro.
5.  **Documentación**: Soporte para Swagger y archivo `.http` para pruebas rápidas.

## Requisitos Técnicos
- **.NET 9.0 SDK**
- **SQL Server** (LocalDB o Instancia de servidor)
- **NuGet**: Dependencias principales como `Refit`, `EntityFrameworkCore.SqlServer` y `Newtonsoft.Json`.

## Cómo empezar
1.  **Configuración**: Ajusta la cadena de conexión `ClaimsDb` en `Claims.Api/appsettings.json`.
2.  **Base de Datos**: El sistema utiliza migraciones de EF Core. Asegúrate de que la base de datos esté actualizada.
3.  **Ejecución**:
    ```bash
    dotnet run --project Claims.Api
    ```
4.  **Pruebas**: Puedes usar el archivo `Claims.Api/Claims.Api.http` con la extensión REST Client de VS Code para probar los endpoints.

---
*Desarrollado como parte del proyecto de prácticas de InternApp.*
