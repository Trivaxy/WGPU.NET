using Silk.NET.GLFW;
using System;
using System.IO;
using System.Runtime.InteropServices;
using WGPU.NET;

namespace WGPU.Tests
{
	public static class Test
	{
		public static unsafe void Main(string[] args)
		{
			Glfw glfw = GlfwProvider.GLFW.Value;

			if (!glfw.Init())
			{
				Console.WriteLine("GLFW failed to initialize");
				Console.ReadKey();
				return;
			}

			glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
			WindowHandle* window = glfw.CreateWindow(300, 300, "Wgpu.NET test", null, null);

			if (window == null)
			{
				Console.WriteLine("Failed to open window");
				glfw.Terminate();
				Console.ReadKey();
				return;
			}

			var wgpuInstance = new Wgpu.InstanceImpl();
			var nativeWindow = new GlfwNativeWindow(glfw, window).Win32.Value;
			var surfaceDescriptorInfo = new Wgpu.SurfaceDescriptorFromWindowsHWND() { hwnd = nativeWindow.Hwnd, hinstance = nativeWindow.HInstance, chain = new Wgpu.ChainedStruct() { sType = Wgpu.SType.SurfaceDescriptorFromWindowsHWND } };
			var surfaceDescriptor = new Wgpu.SurfaceDescriptor() { nextInChain = (IntPtr)(&surfaceDescriptorInfo) };
			var surface = Wgpu.InstanceCreateSurface(wgpuInstance, in surfaceDescriptor);

			var adapterOptions = new Wgpu.RequestAdapterOptions() { compatibleSurface = surface };
			Wgpu.AdapterImpl adapter = default;

			Wgpu.InstanceRequestAdapter(wgpuInstance, in adapterOptions, (s, a, m, u) => { adapter = a; }, IntPtr.Zero);

			var properties = new Wgpu.AdapterProperties();
			Wgpu.AdapterGetProperties(adapter, ref properties);

			var deviceExtras = new Wgpu.DeviceExtras() { chain = new Wgpu.ChainedStruct() { sType = (Wgpu.SType)Wgpu.NativeSType.STypeDeviceExtras }, label = "Device" };
			IntPtr deviceExtrasPtr = MarshalAndBox(deviceExtras);

			var requiredLimits = new Wgpu.RequiredLimits() { limits = new Wgpu.Limits() { maxBindGroups = 1 } };
			var deviceDescriptor = new Wgpu.DeviceDescriptor() { nextInChain = deviceExtrasPtr, requiredLimits = (IntPtr)(&requiredLimits) };

			Wgpu.DeviceImpl device = default;
			Wgpu.AdapterRequestDevice(adapter, in deviceDescriptor, (s, d, m, u) => { device = d; }, IntPtr.Zero);

			var shaderSource = File.ReadAllText("shader.wgsl");
			var wgslDescriptor = new Wgpu.ShaderModuleWGSLDescriptor() { chain = new Wgpu.ChainedStruct() { sType = Wgpu.SType.ShaderModuleWGSLDescriptor }, source = shaderSource };
			var wgslDescriptorPtr = MarshalAndBox(wgslDescriptor);
			var shaderDescriptor = new Wgpu.ShaderModuleDescriptor() { nextInChain = wgslDescriptorPtr, label = "shader.wgsl" };
			var shader = Wgpu.DeviceCreateShaderModule(device, in shaderDescriptor);

			var pipelineLayoutDescriptor = new Wgpu.PipelineLayoutDescriptor() { bindGroupLayoutCount = 0 };
			var pipelineLayout = Wgpu.DeviceCreatePipelineLayout(device, in pipelineLayoutDescriptor);

			var swapChainFormat = Wgpu.SurfaceGetPreferredFormat(surface, adapter);

			var blendState = new Wgpu.BlendState()
			{
				color = new Wgpu.BlendComponent()
				{
					srcFactor = Wgpu.BlendFactor.One,
					dstFactor = Wgpu.BlendFactor.Zero,
					operation = Wgpu.BlendOperation.Add
				},
				alpha = new Wgpu.BlendComponent()
				{
					srcFactor = Wgpu.BlendFactor.One,
					dstFactor = Wgpu.BlendFactor.Zero,
					operation = Wgpu.BlendOperation.Add
				}
			};

			var colorTargetState = new Wgpu.ColorTargetState()
			{
				format = swapChainFormat,
				blend = (IntPtr)(&blendState),
				writeMask = (uint)Wgpu.ColorWriteMask.All
			};

			var fragmentState = new Wgpu.FragmentState()
			{
				module = shader,
				entryPoint = "fs_main",
				targetCount = 1,
				targets = (IntPtr)(&colorTargetState),
			};
			var fragmentStatePtr = MarshalAndBox(fragmentState);

			var renderPipelineDescriptor = new Wgpu.RenderPipelineDescriptor()
			{
				label = "Render pipeline",
				layout = pipelineLayout,
				vertex = new Wgpu.VertexState()
				{
					module = shader,
					entryPoint = "vs_main",
					bufferCount = 0,
				},
				primitive = new Wgpu.PrimitiveState()
				{
					topology = Wgpu.PrimitiveTopology.TriangleList,
					stripIndexFormat = Wgpu.IndexFormat.Undefined,
					frontFace = Wgpu.FrontFace.CCW,
					cullMode = Wgpu.CullMode.None
				},
				multisample = new Wgpu.MultisampleState()
				{
					count = 1,
					mask = uint.MaxValue,
					alphaToCoverageEnabled = false
				},
				fragment = fragmentStatePtr
			};

			var renderPipeline = Wgpu.DeviceCreateRenderPipeline(device, in renderPipelineDescriptor);

			glfw.GetWindowSize(window, out int prevWidth, out int prevHeight);
			
			var swapChainDescriptor = new Wgpu.SwapChainDescriptor()
			{
				usage = (uint)Wgpu.TextureUsage.RenderAttachment,
				format = swapChainFormat,
				width = (uint)prevWidth,
				height = (uint)prevHeight,
				presentMode = Wgpu.PresentMode.Fifo
			};

			var swapChain = Wgpu.DeviceCreateSwapChain(device, surface, in swapChainDescriptor);

			while (!glfw.WindowShouldClose(window))
			{
				glfw.GetWindowSize(window, out int width, out int height);

				if (width != prevWidth || height != prevHeight)
				{
					prevWidth = width;
					prevHeight = height;
					swapChainDescriptor.width = (uint)width;
					swapChainDescriptor.height = (uint)height;
					swapChain = Wgpu.DeviceCreateSwapChain(device, surface, in swapChainDescriptor);
				}

				var nextTexture = Wgpu.SwapChainGetCurrentTextureView(swapChain);

				if (nextTexture.Handle == IntPtr.Zero)
				{
					Console.WriteLine("Could not acquire next swap chain texture");
					return;
				}

				var encoderDescriptor = new Wgpu.CommandEncoderDescriptor() { label = "Command Encoder" };
				var encoder = Wgpu.DeviceCreateCommandEncoder(device, in encoderDescriptor);

				var colorAttachment = new Wgpu.RenderPassColorAttachment()
				{
					view = nextTexture,
					resolveTarget = default,
					loadOp = Wgpu.LoadOp.Clear,
					storeOp = Wgpu.StoreOp.Store,
					clearColor = new Wgpu.Color() { r = 0, g = 1, b = 0, a = 1 }
				};

				var renderPassDescriptor = new Wgpu.RenderPassDescriptor()
				{
					colorAttachments = (IntPtr)(&colorAttachment),
					colorAttachmentCount = 1
				};

				var renderPass = Wgpu.CommandEncoderBeginRenderPass(encoder, in renderPassDescriptor);

				Wgpu.RenderPassEncoderSetPipeline(renderPass, renderPipeline);
				Wgpu.RenderPassEncoderDraw(renderPass, 3, 1, 0, 0);
				Wgpu.RenderPassEncoderEndPass(renderPass);

				var queue = Wgpu.DeviceGetQueue(device);
				var commandBufferDescriptor = new Wgpu.CommandBufferDescriptor();
				var commandBuffer = Wgpu.CommandEncoderFinish(encoder, in commandBufferDescriptor);

				Wgpu.QueueSubmit(queue, 1, ref commandBuffer);
				Wgpu.SwapChainPresent(swapChain);

				glfw.PollEvents();
			}

			glfw.DestroyWindow(window);
			glfw.Terminate();
		}

		private static IntPtr MarshalAndBox<T>(T structure)
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
			Marshal.StructureToPtr(structure, ptr, false);
			return ptr;
		}
	}
}