# Guía de Instalación y Configuración

## Requisitos Previos

### Software necesario
1. **.NET 8 SDK** - [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Editor de código:**
   - Visual Studio 2022 (Community o superior)
   - O Visual Studio Code con extensión C#

### Verificar instalación de .NET
```bash
dotnet --version
# Debe mostrar 8.0.x
```

## Instalación Paso a Paso

### 1. Clonar el repositorio
```bash
git clone <tu-repo-url>
cd marketplace-libros
```

### 2. Restaurar dependencias
```bash
cd BibliotecaPersonal
dotnet restore
```

### 3. Crear la base de datos
La base de datos SQLite se creará automáticamente la primera vez que ejecutes la aplicación.

### 4. Ejecutar la aplicación
```bash
dotnet run
```

La aplicación estará disponible en: `http://localhost:5000`

### 5. Probar como PWA

#### En Chrome/Edge (Desktop):
1. Abre `http://localhost:5000`
2. Haz clic en el icono de instalación en la barra de direcciones
3. Confirma la instalación

#### En dispositivo móvil:
1. Abre la URL en el navegador móvil
2. Menú → "Agregar a pantalla de inicio"
3. La app se instalará como PWA

## Desarrollo

### Ejecutar en modo watch (recarga automática)
```bash
dotnet watch run
```

### Aplicar migraciones manualmente (si es necesario)
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Limpiar y reconstruir
```bash
dotnet clean
dotnet build
```

## Solución de Problemas

### Error: "No .NET SDKs were found"
- Instala .NET 8 SDK desde el enlace oficial
- Reinicia tu terminal después de la instalación

### La base de datos no se crea
- Verifica que tienes permisos de escritura en la carpeta del proyecto
- Elimina `biblioteca.db` y reinicia la aplicación

### Errores de compilación
```bash
# Limpiar solución
dotnet clean
# Restaurar paquetes
dotnet restore
# Reconstruir
dotnet build
```

## Configuración Adicional

### Cambiar el puerto
Edita [launchSettings.json](BibliotecaPersonal/Properties/launchSettings.json):
```json
"applicationUrl": "http://localhost:TU_PUERTO"
```

### Configurar cadena de conexión
Edita [appsettings.json](BibliotecaPersonal/appsettings.json):
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=TU_RUTA/biblioteca.db"
}
```

## Próximos Pasos

1. ✅ Proyecto creado y configurado
2. 📝 Agregar libros de prueba
3. 🔍 Probar búsqueda y filtros
4. 🚀 Preparar para deploy en Cloud Run
