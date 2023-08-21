# WGPU.NET

![Nuget](https://img.shields.io/nuget/dt/WGPU.NET)
![GitHub forks](https://img.shields.io/github/forks/Trivaxy/WGPU.NET?color=%23770a7f)
![GitHub Repo stars](https://img.shields.io/github/stars/Trivaxy/WGPU.NET)

WGPU.NET is a library providing cross-platform raw bindings and optional wrappers for [wgpu-native](https://github.com/gfx-rs/wgpu-native), allowing you to interact with your GPU using a flexible and efficient graphics API.

WGPU.NET targets .NET Standard 2.0.

# Usage
WGPU.NET is offered as a NuGet package [here](https://www.nuget.org/packages/WGPU.NET/). Just install it and you're ready to go.

This library is purely graphics-oriented and does not provide utilities for creating windows. I recommend using [Silk.NET's GLFW bindings](https://www.nuget.org/packages/Silk.NET.GLFW/) if you need windowing, but you can use anything you like as long as it can provide raw window handles.

Check out the [usage example here](https://github.com/Trivaxy/WGPU.NET/tree/master/WGPU.Tests) for a demonstration on using WGPU.NET.

If you run into problems using WGPU.NET, please open an issue.

# Building
This repository contains 3 projects:
- BindingsGenerator: Responsible for generating bindings to wgpu-native automatically using its provided header files.
- WGPU: Contains the actual bindings. The output of BindingsGenerator is shoved in here, and everything is good to go.
- WGPU.Tests: Used to test the bindings and can be used for examples/references. Currently contains an example that draws a triangle to a window.

If you want to compile the bindings on your own, build and run BindingsGenerator, and copy its output source file into the WGPU project, then compile the WGPU project.