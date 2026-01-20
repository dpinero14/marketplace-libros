# Arquitectura del Proyecto

## Visión General

Biblioteca Personal es una Blazor Web App progresiva (PWA) diseñada para gestionar bibliotecas personales con un enfoque mobile-first.

## Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                      Blazor Web App                          │
│                     (Cliente + Servidor)                     │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Components (UI Layer)                                        │
│  ├── Pages/                                                   │
│  │   ├── Home.razor          (Página de inicio)             │
│  │   ├── Biblioteca.razor    (Listado + búsqueda)           │
│  │   └── Agregar.razor       (Formulario de carga)          │
│  └── Layout/                                                  │
│      ├── MainLayout.razor    (Layout principal)              │
│      └── NavMenu.razor       (Navegación)                    │
│                                                               │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Services (Business Logic Layer)                              │
│  └── BookService              (Lógica de negocio)            │
│      ├── GetAllBooksAsync()                                  │
│      ├── SearchBooksAsync()                                  │
│      ├── CreateBookAsync()                                   │
│      ├── UpdateBookAsync()                                   │
│      └── DeleteBookAsync()                                   │
│                                                               │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  API Layer (Minimal APIs)                                     │
│  └── /api/books              (Endpoints REST)                │
│      ├── GET    /api/books                                   │
│      ├── GET    /api/books/{id}                              │
│      ├── POST   /api/books                                   │
│      ├── PUT    /api/books/{id}                              │
│      ├── DELETE /api/books/{id}                              │
│      └── GET    /api/books/search?query=...                  │
│                                                               │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Data Layer                                                   │
│  ├── BibliotecaDbContext      (EF Core Context)              │
│  └── Models/                                                  │
│      └── Book                 (Entidad principal)            │
│                                                               │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Database                                                     │
│  └── SQLite (biblioteca.db)                                   │
│                                                               │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                      PWA Features                            │
├─────────────────────────────────────────────────────────────┤
│  ├── manifest.json           (Metadata de la PWA)            │
│  ├── service-worker.js       (Cache offline)                 │
│  └── Instalable en móviles                                   │
└─────────────────────────────────────────────────────────────┘
```

## Capas de la Aplicación

### 1. Capa de Presentación (Components)
**Responsabilidad:** Interfaz de usuario e interacción.

- **Blazor Server Mode:** Renderizado en servidor con SignalR para actualizaciones en tiempo real
- **Componentes Razor:** Pages y Layout components
- **Styling:** CSS mobile-first en `wwwroot/css/app.css`

### 2. Capa de Servicios (Services)
**Responsabilidad:** Lógica de negocio y orquestación.

- `IBookService`: Interface para abstracción
- `BookService`: Implementación con lógica de negocio
  - Validaciones
  - Transformaciones
  - Búsqueda case-insensitive

### 3. Capa de API (Minimal APIs)
**Responsabilidad:** Endpoints HTTP para futura integración con apps móviles nativas o clientes externos.

Endpoints en `Program.cs`:
```csharp
/api/books           → GET todos los libros
/api/books/{id}      → GET libro específico
/api/books           → POST crear libro
/api/books/{id}      → PUT actualizar libro
/api/books/{id}      → DELETE eliminar libro
/api/books/search    → GET buscar libros
```

### 4. Capa de Datos (Data)
**Responsabilidad:** Acceso a datos y persistencia.

- **EF Core:** ORM para interactuar con SQLite
- **DbContext:** `BibliotecaDbContext` con configuración de entidades
- **Migraciones:** Code-first approach

### 5. Capa de Base de Datos
**Responsabilidad:** Almacenamiento persistente.

- **SQLite:** Base de datos embebida
- **Ventajas:**
  - Sin configuración de servidor
  - Archivo único (`biblioteca.db`)
  - Ideal para desarrollo y deploy inicial
  - Fácil migración futura a PostgreSQL/SQL Server

## Modelo de Datos

### Entidad Book
```csharp
public class Book
{
    Guid Id                    // PK
    string Title               // NOT NULL, indexed
    string Authors             // NOT NULL, indexed
    string? Publisher
    int? PublicationYear
    string? Isbn13            // Indexed
    string? Language
    string? Format
    string? Notes
    decimal Confidence        // 0.0 a 1.0 (para IA)
    BookStatus Status         // Enum: Keep/Sell/Swap, indexed
    DateTime CreatedAt        // NOT NULL
}
```

### Índices
- `Title`: Para búsquedas rápidas
- `Authors`: Para filtrado por autor
- `Isbn13`: Para búsqueda exacta
- `Status`: Para filtrado por estado

## Flujo de Datos

### Búsqueda de Libros
```
Usuario escribe en search box
    ↓
Componente Biblioteca.razor (@bind + @onkeyup)
    ↓
BookService.SearchBooksAsync(query)
    ↓
EF Core query con .Where() + .Contains()
    ↓
SQLite busca en índices
    ↓
Resultados retornan al componente
    ↓
UI se actualiza (Blazor re-render)
```

### Crear Libro
```
Usuario completa formulario
    ↓
EditForm OnValidSubmit
    ↓
BookService.CreateBookAsync(book)
    ↓
EF Core .Add() + .SaveChangesAsync()
    ↓
SQLite INSERT
    ↓
Navegación a /biblioteca
```

## PWA Architecture

### Service Worker Strategy
- **Development:** Network First con fallback a Cache
- **Production:** Cache First para assets estáticos

### Cacheable Resources
- HTML principal
- CSS
- JavaScript framework
- Manifest
- Iconos

### Offline Capabilities
- ✅ UI funcional offline
- ✅ Assets estáticos cacheados
- ⚠️ Operaciones de datos requieren conexión (por ahora)
- 🔮 Futuro: IndexedDB para sync offline

## Escalabilidad Futura

### Fase 1 (Actual): MVP
- Un usuario (sin autenticación)
- SQLite local
- Deploy en Cloud Run

### Fase 2: Multi-usuario
- Agregar ASP.NET Core Identity
- Migrar a PostgreSQL (Cloud SQL)
- Separar frontend/backend si es necesario

### Fase 3: Marketplace
- Sistema de matching (libros en venta/intercambio)
- Mensajería entre usuarios
- Sistema de calificaciones
- Geolocalización para intercambios locales

### Fase 4: IA Avanzada
- Integración con Google Vertex AI (Gemini)
- OCR para extracción de datos desde imágenes
- Recomendaciones personalizadas
- Búsqueda semántica

## Tecnologías y Dependencias

### Backend
- .NET 8
- Entity Framework Core 8.0
- SQLite

### Frontend
- Blazor Server Components
- CSS vanilla (sin frameworks)
- Progressive Web App APIs

### DevOps
- Docker
- Google Cloud Run
- Cloud Build

## Convenciones de Código

### Nombres
- **Clases:** PascalCase (`BookService`)
- **Métodos:** PascalCase + Async suffix (`GetAllBooksAsync`)
- **Variables:** camelCase (`searchQuery`)
- **Constantes:** PascalCase o UPPER_SNAKE_CASE

### Estructura de archivos
```
/Models        → Entidades y DTOs
/Services      → Interfaces + Implementaciones
/Data          → DbContext y configuraciones
/Components    → UI (Pages + Layout)
/wwwroot       → Assets estáticos
```

### Async/Await
- Todos los métodos de datos son async
- Usar `Task<T>` para operaciones asíncronas
- Suffix `Async` en nombres de métodos

## Seguridad

### Estado Actual
- ⚠️ Sin autenticación (MVP)
- ⚠️ Sin autorización
- ✅ CORS configurado para mismo origen
- ✅ HTTPS en producción (Cloud Run)

### Próximas Mejoras
- Implementar ASP.NET Core Identity
- JWT para API
- Rate limiting
- Input validation mejorada
- XSS protection
