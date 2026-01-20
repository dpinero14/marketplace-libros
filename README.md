# 📚 Biblioteca Personal

Aplicación PWA para gestionar tu biblioteca personal de libros. Consulta rápidamente si ya tienes un libro desde tu celular.

## 🎯 Objetivo

Este es el primer producto de un futuro marketplace de libros usados. El foco inicial es:
- Digitalizar bibliotecas físicas
- Consultar inventario en segundos ("¿Lo tengo?")
- Marcar libros para vender/intercambiar/conservar

## 🛠️ Stack Tecnológico

- **Frontend:** Blazor Web App (.NET 8)
- **Backend:** Minimal APIs
- **Base de datos:** SQLite con Entity Framework Core
- **PWA:** Habilitada para instalación en dispositivos móviles
- **Diseño:** Mobile-first

## 📱 Features del MVP

1. **Mi Biblioteca**
   - Tabla con búsqueda instantánea por título, autor o ISBN
   - Filtros por estado (Conservar/Vender/Intercambiar)

2. **Agregar Libro**
   - Subir 1-3 imágenes (tapa/contratapa/página legal)
   - Formulario editable con datos del libro

3. **Gestión de Estado**
   - Marcar libros como: Conservar, Vender o Intercambiar

## 🚀 Cómo ejecutar

### Requisitos
- .NET 8 SDK
- Visual Studio 2022 o VS Code

### Pasos

1. Clonar el repositorio:
```bash
git clone <repo-url>
cd marketplace-libros
```

2. Restaurar paquetes:
```bash
cd BibliotecaPersonal
dotnet restore
```

3. Ejecutar la aplicación:
```bash
dotnet run
```

4. Abrir en el navegador:
```
http://localhost:5000
```

## 📦 Estructura del Proyecto

```
BibliotecaPersonal/
├── Components/
│   ├── Layout/         # Layouts y navegación
│   ├── Pages/          # Páginas Razor
│   └── _Imports.razor
├── Data/
│   └── BibliotecaDbContext.cs
├── Models/
│   └── Book.cs
├── Services/
│   ├── IBookService.cs
│   └── BookService.cs
├── wwwroot/
│   ├── css/
│   ├── manifest.json
│   └── service-worker.js
└── Program.cs
```

## 🐳 Docker

Construir imagen:
```bash
docker build -t biblioteca-personal .
```

Ejecutar contenedor:
```bash
docker run -p 8080:8080 biblioteca-personal
```

## ☁️ Deploy en Google Cloud Run

```bash
# Construir y pushear a GCP
gcloud builds submit --tag gcr.io/[PROJECT-ID]/biblioteca-personal

# Deploy
gcloud run deploy biblioteca-personal \
  --image gcr.io/[PROJECT-ID]/biblioteca-personal \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

## 🔮 Próximas Features

- Integración con Google Vertex AI (Gemini) para extracción automática de datos desde imágenes
- Autenticación de usuarios
- Compartir biblioteca con amigos
- Marketplace para vender/intercambiar libros

## 📄 Licencia

Este proyecto es privado y está en desarrollo.

## 👨‍💻 Autor

Desarrollado como MVP para digitalización de bibliotecas personales.
