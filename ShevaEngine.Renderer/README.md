# ShevaEngine.Renderer

This is the new renderer module for ShevaEngine, designed to replace the legacy rendering system.

## Purpose

This project provides a modern, clean implementation of the rendering pipeline that will eventually replace the existing `RenderingPipeline` class in `ShevaEngine.Core.Graphics`.

## Features

- Clean separation of concerns with `IRenderer` interface
- Support for both opaque and transparent rendering
- Instanced rendering for better performance
- Material profile support (Default, Shadows)
- Light management
- Camera support

## Usage

```csharp
// Create and initialize the renderer
var renderer = new Renderer();
renderer.Initialize();

// Set up camera and game time
renderer.Camera = camera;
renderer.GameTime = gameTime;

// Begin rendering
renderer.Begin();

// Add objects to render
renderer.AddObject(model, worldMatrix);
renderer.AddObject(model, worldMatrices); // Multiple instances

// Add lights
renderer.AddLight(directionalLight);

// Render all queued objects
renderer.Draw();

// End rendering
renderer.End();

// Dispose when done
renderer.Dispose();
```

## Architecture

The renderer is designed with the following principles:

1. **Interface-first**: The `IRenderer` interface defines the contract, making it easy to mock or swap implementations
2. **State management**: Clear separation between state setup (Begin/End) and rendering (Draw)
3. **Performance**: Uses instanced rendering where possible
4. **Extensibility**: Can be extended with new features without breaking existing code

## Migration from Legacy Renderer

The new renderer is designed to be API-compatible with the legacy `RenderingPipeline` class. Migration should be straightforward:

1. Replace `RenderingPipeline` with `Renderer`
2. Replace method calls accordingly
3. The new renderer provides the same functionality with a cleaner interface

## Dependencies

- ShevaEngine.Core
- MonoGame.Framework.Native

## Future Work

- Shader management system
- Render target management
- Post-processing effects integration
- Shadow mapping improvements
- Performance optimizations
