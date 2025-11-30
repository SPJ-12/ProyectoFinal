# Manual de Instalación - FreeDartsGame

## Requisitos Previos

### Software Necesario

1. **Visual Studio 2022** (versión 17.11 o superior)
   - Descarga desde: https://visualstudio.microsoft.com/es/downloads/
   - Durante la instalación, asegúrate de seleccionar las siguientes cargas de trabajo:
     - **Desarrollo para .NET Multiplataforma** (.NET MAUI)
     - **Desarrollo para escritorio con .NET** (si planeas ejecutar en Windows)

2. **.NET 8.0 SDK**
   - Generalmente se instala automáticamente con Visual Studio 2022
   - Verifica la instalación ejecutando en la terminal: `dotnet --version`
   - Debe mostrar la versión 8.0.x o superior

### Requisitos del Sistema

- **Windows 10** versión 19041.0 o superior (para desarrollo en Windows)
- **Android SDK** (si planeas desarrollar para Android)
- **Xcode** (si planeas desarrollar para iOS/macOS - solo en Mac)

## Pasos de Instalación

### 1. Clonar o Abrir el Proyecto

1. Abre **Visual Studio 2022**
2. Selecciona **Archivo → Abrir → Proyecto o solución**
3. Navega hasta la carpeta del proyecto y selecciona `FreeDartsGame`
4. Haz clic en **Abrir**

### 2. Configurar la Plataforma de Ejecución

1. En la barra de herramientas, selecciona la plataforma objetivo:
   - **net8.0-windows10.0.19041.0** (para Windows)
   - **net8.0-android** (para Android)
   - **net8.0-ios** (para iOS - requiere Mac)
   - **net8.0-maccatalyst** (para macOS)

2. Selecciona la configuración:
   - **Debug** (para desarrollo)
   - **Release** (para producción)

### 3. Compilar el Proyecto

1. Presiona **Ctrl + Shift + B** o
2. Ve a **Compilar → Compilar solución**
3. Verifica que no haya errores en la **Lista de errores**

### 4. Ejecutar la Aplicación

1. Presiona **F5** o haz clic en el botón **Iniciar** (▶️)
2. Si es la primera vez ejecutando en Windows, Visual Studio puede solicitar permisos para instalar el certificado de desarrollador
3. La aplicación se ejecutará en la plataforma seleccionada
