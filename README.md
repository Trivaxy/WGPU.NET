# WGPU.NET
WGPU.NET is a library providing cross-platform raw bindings to [wgpu-native](https://github.com/gfx-rs/wgpu-native), allowing you to interact with your GPU using a flexible and efficient graphics API.

WGPU.NET runs on .NET 5.

# Usage
WGPU.NET is offered as a NuGet package [here](https://www.nuget.org/packages/WGPU.NET/0.1.0). Just install it and you're ready to go.

Note that this library is purely bindings only, so you cannot create windows with it. I recommend using [Silk.NET's GLFW bindings](https://www.nuget.org/packages/Silk.NET.GLFW/) if you need windowing, but you can use anything you like as long as it can provide raw window handles.

Bear in mind WGPU.NET is still a new project. If you run into problems using it, please open an issue.

# Building
`This repository contains 3 projects;
- BindingsGenerator: Responsible for generating bindings to wgpu-native automatically using its provided header files.
- WGPU: Contains the actual bindings. The output of BindingsGenerator is shoved in here, and everything is good to go.
- WGPU.Tests: Used to test the bindings and can be used for examples/references. Currently contains an example that draws a triangle to a window.

If you want to compile the bindings on your own, build and run BindingsGenerator, and copy its output source file into the WGPU project, then compile the WGPU project.