# Comandos Útiles

## Desarrollo

### Ejecutar la aplicación
```bash
dotnet run --project BibliotecaPersonal
```

### Ejecutar con hot reload
```bash
dotnet watch --project BibliotecaPersonal
```

### Limpiar y reconstruir
```bash
dotnet clean
dotnet restore
dotnet build
```

## Entity Framework

### Crear migración
```bash
dotnet ef migrations add NombreDeLaMigracion --project BibliotecaPersonal
```

### Aplicar migraciones
```bash
dotnet ef database update --project BibliotecaPersonal
```

### Revertir última migración
```bash
dotnet ef database update PreviousMigrationName --project BibliotecaPersonal
```

### Eliminar última migración
```bash
dotnet ef migrations remove --project BibliotecaPersonal
```

### Ver script SQL de migraciones
```bash
dotnet ef migrations script --project BibliotecaPersonal
```

### Eliminar base de datos
```bash
dotnet ef database drop --project BibliotecaPersonal
```

## Testing

### Ejecutar tests (cuando se agreguen)
```bash
dotnet test
```

### Con cobertura
```bash
dotnet test /p:CollectCoverage=true
```

## NuGet

### Agregar paquete
```bash
dotnet add BibliotecaPersonal package NombreDelPaquete
```

### Actualizar paquetes
```bash
dotnet list BibliotecaPersonal package --outdated
dotnet add BibliotecaPersonal package NombreDelPaquete
```

### Eliminar paquete
```bash
dotnet remove BibliotecaPersonal package NombreDelPaquete
```

## Docker

### Construir imagen
```bash
docker build -t biblioteca-personal:latest .
```

### Ejecutar contenedor
```bash
docker run -p 8080:8080 --name biblioteca biblioteca-personal:latest
```

### Ver logs
```bash
docker logs biblioteca
```

### Detener contenedor
```bash
docker stop biblioteca
```

### Eliminar contenedor
```bash
docker rm biblioteca
```

### Limpiar imágenes sin usar
```bash
docker system prune -a
```

## Google Cloud

### Configurar proyecto
```bash
gcloud config set project [PROJECT-ID]
gcloud config set run/region us-central1
```

### Build y deploy
```bash
# Build
gcloud builds submit --tag gcr.io/[PROJECT-ID]/biblioteca-personal

# Deploy
gcloud run deploy biblioteca-personal \
  --image gcr.io/[PROJECT-ID]/biblioteca-personal \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --memory 512Mi \
  --cpu 1
```

### Ver servicios
```bash
gcloud run services list
```

### Ver logs
```bash
gcloud run services logs read biblioteca-personal --limit=50
```

### Eliminar servicio
```bash
gcloud run services delete biblioteca-personal
```

## Git

### Inicializar repositorio
```bash
git init
git add .
git commit -m "Initial commit: Blazor PWA biblioteca personal"
```

### Crear rama de feature
```bash
git checkout -b feature/nombre-feature
```

### Merge a main
```bash
git checkout main
git merge feature/nombre-feature
```

### Tag de versión
```bash
git tag -a v1.0.0 -m "Versión 1.0.0 - MVP"
git push origin v1.0.0
```

## PWA

### Generar service worker assets (producción)
```bash
dotnet publish -c Release
```

### Probar PWA localmente
1. Ejecutar: `dotnet run`
2. Abrir Chrome DevTools → Application → Service Workers
3. Verificar que se registra correctamente
4. Application → Manifest → Ver manifest.json
5. Lighthouse → Generar reporte PWA

## Troubleshooting

### Puerto en uso
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID [PID] /F

# Linux/Mac
lsof -ti:5000 | xargs kill -9
```

### Resetear base de datos
```bash
rm BibliotecaPersonal/biblioteca.db
dotnet run --project BibliotecaPersonal
```

### Limpiar cache de NuGet
```bash
dotnet nuget locals all --clear
```

### Ver información del SDK
```bash
dotnet --info
```

### Listar templates disponibles
```bash
dotnet new list
```

## Productividad

### Crear alias útiles (PowerShell)
Agregar al profile (`$PROFILE`):
```powershell
function Run-Biblioteca { dotnet run --project BibliotecaPersonal }
function Watch-Biblioteca { dotnet watch --project BibliotecaPersonal }
function Clean-Biblioteca { 
    dotnet clean
    Remove-Item BibliotecaPersonal/biblioteca.db -ErrorAction SilentlyContinue
}

Set-Alias rb Run-Biblioteca
Set-Alias wb Watch-Biblioteca
Set-Alias cb Clean-Biblioteca
```

### Crear alias útiles (Bash/Zsh)
Agregar al `.bashrc` o `.zshrc`:
```bash
alias rb="dotnet run --project BibliotecaPersonal"
alias wb="dotnet watch --project BibliotecaPersonal"
alias cb="dotnet clean && rm -f BibliotecaPersonal/biblioteca.db"
```

## VS Code

### Extensiones recomendadas
- C# (Microsoft)
- C# Dev Kit
- REST Client (para probar APIs)
- SQLite Viewer
- Docker
- GitLens

### Tasks.json recomendado
Ver archivo `.vscode/tasks.json` en el proyecto.

### Launch.json recomendado
Ver archivo `.vscode/launch.json` en el proyecto.
