using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WGPU.NET
{
    public class WgpuStructChain : IDisposable
	{
		private readonly List<IntPtr> _pointers = new List<IntPtr>();
		private readonly List<IntPtr> _trackedAllocatedData = new List<IntPtr>();
        private IntPtr _pointer = IntPtr.Zero;

        public IntPtr GetPointer()
        {
            return _pointer;
        }

        public WgpuStructChain AddPrimitiveDepthClipControl(bool unclippedDepth = default)
        {
			AddStruct(new Wgpu.PrimitiveDepthClipControl()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.PrimitiveDepthClipControl },
				unclippedDepth = unclippedDepth
			});

			return this;
		}

		public WgpuStructChain AddShaderModuleSPIRVDescriptor(byte[] code)
		{
			AddStruct(new Wgpu.ShaderModuleSPIRVDescriptor()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.ShaderModuleSPIRVDescriptor },
				code = TrackAllocatedData(Util.AllocHArray(code)),
				codeSize = (uint)code.Length
			});

			return this;
		}

		public WgpuStructChain AddShaderModuleWGSLDescriptor(string code)
		{
			AddStruct(new Wgpu.ShaderModuleWGSLDescriptor()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.ShaderModuleWGSLDescriptor },
				code = code
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromAndroidNativeWindow(IntPtr window = default)
        {
			AddStruct(new Wgpu.SurfaceDescriptorFromAndroidNativeWindow()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromAndroidNativeWindow },
				window = window
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromCanvasHTMLSelector(string selector = default)
		{
			AddStruct(new Wgpu.SurfaceDescriptorFromCanvasHTMLSelector()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromCanvasHTMLSelector },
				selector = selector
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromMetalLayer(IntPtr layer = default)
		{
			AddStruct(new Wgpu.SurfaceDescriptorFromMetalLayer()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromMetalLayer },
				layer = layer
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromWaylandSurface(IntPtr display = default, IntPtr surface = default)
		{
			AddStruct(new Wgpu.SurfaceDescriptorFromWaylandSurface()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromWaylandSurface },
				display = display,
				surface = surface
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromWindowsHWND(IntPtr hinstance = default, IntPtr hwnd = default)
		{
			AddStruct(new Wgpu.SurfaceDescriptorFromWindowsHWND()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromWindowsHWND },
				hinstance = hinstance,
				hwnd = hwnd
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromXcbWindow(IntPtr connection = default, uint window = default)
		{
			AddStruct(new Wgpu.SurfaceDescriptorFromXcbWindow()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromXcbWindow },
				connection = connection,
				window = window
			});

			return this;
		}

		public WgpuStructChain AddSurfaceDescriptorFromXlibWindow(IntPtr display = default, uint window = default)
		{
			AddStruct(new Wgpu.SurfaceDescriptorFromXlibWindow()
			{
				chain = new Wgpu.ChainedStruct { sType = Wgpu.SType.SurfaceDescriptorFromXlibWindow },
				display = display,
				window = window
			});

			return this;
		}

		public WgpuStructChain AddDeviceExtras(string tracePath = default)
		{
			AddStruct(new Wgpu.DeviceExtras()
			{
				chain = new Wgpu.ChainedStruct { sType = (Wgpu.SType)Wgpu.NativeSType.STypeDeviceExtras },
				tracePath = tracePath
			});

			return this;
		}

        public WgpuStructChain AddRequiredLimitsExtras(uint maxPushConstantSize = default)
		{
			AddStruct(new Wgpu.RequiredLimitsExtras()
			{
				chain = new Wgpu.ChainedStruct { sType = (Wgpu.SType)Wgpu.NativeSType.STypeRequiredLimitsExtras },
				maxPushConstantSize = maxPushConstantSize
			});

			return this;
		}

		public WgpuStructChain AddPipelineLayoutExtras(Wgpu.PushConstantRange[] pushConstantRanges)
		{
			AddStruct(new Wgpu.PipelineLayoutExtras()
			{
				chain = new Wgpu.ChainedStruct { sType = (Wgpu.SType)Wgpu.NativeSType.STypePipelineLayoutExtras },
				pushConstantRangeCount = (uint)pushConstantRanges.Length,
				pushConstantRanges = TrackAllocatedData(Util.AllocHArray(pushConstantRanges))
			});

			return this;
		}

		private void AddStruct<T>(T structure)
			where T : struct
		{
			IntPtr ptr = Util.AllocHStruct(structure);
			
			if(GetPointer() == IntPtr.Zero)
                _pointer = ptr;
            
			if(_pointers.Count!=0)
            {
				//write this struct into the "next" field of the last struct
				//this only works because next is guaranteed to be the first field of every ChainedStruct
				Marshal.StructureToPtr(ptr, _pointers[_pointers.Count - 1], false);
            }
		}

		private IntPtr TrackAllocatedData(IntPtr ptr)
		{
			_trackedAllocatedData.Add(ptr);
			return ptr;
		}

        public void Dispose()
        {
            foreach (var pointer in _pointers)
	            Util.FreePtr(pointer);
            
            foreach (var pointer in _trackedAllocatedData)
	            Util.FreePtr(pointer);
            
            _pointers.Clear();
            _trackedAllocatedData.Clear();
        }
    }
}