using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public struct BindGroupEntry
    {
        public uint Binding;
        public Buffer Buffer;
        public ulong Offset;
        public ulong Size;
        public Sampler Sampler;
        public TextureView TextureView;
    }

    public struct ProgrammableStageDescriptor
    {
        public ShaderModule Module;
        public string EntryPoint;
    }

    public struct VertexState
    {
        public ShaderModule Module;
        public string EntryPoint;
        public VertexBufferLayout[] bufferLayouts;
    }

    public partial struct VertexBufferLayout
    {
        public ulong ArrayStride;

        public Wgpu.VertexStepMode StepMode;

        public VertexAttribute[] Attributes;
    }

    public struct FragmentState
    {
        public ShaderModule Module;
        public string EntryPoint;
        public ColorTargetState[] colorTargets;
    }

    public struct ColorTargetState
    {
        public Wgpu.TextureFormat Format;

        public BlendState? BlendState;

        public uint WriteMask;
    }

    public class Device
    {
        private DeviceImpl _impl;

        public DeviceImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Device));

                return _impl;
            }

            private set => _impl = value;
        }

        internal Device(DeviceImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Device));

            Impl = impl;
        }

        public BindGroup CreateBindGroup(string label, BindGroupLayout layout, BindGroupEntry[] entries)
        {
            return new BindGroup(
                DeviceCreateBindGroup(Impl, new BindGroupDescriptor
                {
                    label = label,
                    layout = layout.Impl,
                    entries = Util.AllocHArray(entries.Length,
                    entries.Select(x => new Wgpu.BindGroupEntry()
                    {
                        binding = x.Binding,
                        buffer = x.Buffer?.Impl ?? default,
                        offset = x.Offset,
                        size = x.Size,
                        sampler = x.Sampler?.Impl ?? default,
                        textureView = x.TextureView?.Impl ?? default
                    })),
                    entryCount = (uint)entries.Length
                })
            );
        }

        public BindGroupLayout CreateBindgroupLayout(string label, BindGroupLayoutEntry[] entries)
        {
            return BindGroupLayout.For(
                DeviceCreateBindGroupLayout(Impl, new BindGroupLayoutDescriptor
                {
                    label = label,
                    entries = Util.AllocHArray(entries),
                    entryCount = (uint)entries.Length
                })
            );
        }

        public Buffer CreateBuffer(string label, bool mappedAtCreation, ulong size, BufferUsage usage)
        {
            var desc = new BufferDescriptor
            {
                label = label,
                mappedAtCreation = mappedAtCreation,
                size = size,
                usage = (uint)usage
            };

            return new Buffer(DeviceCreateBuffer(Impl, desc), desc);
        }

        public CommandEncoder CreateCommandEncoder(string label)
        {
            return new CommandEncoder(
                DeviceCreateCommandEncoder(Impl, new CommandEncoderDescriptor
                {
                    label = label
                })
            );
        }

        public ComputePipeline CreateComputePipeline(string label, ProgrammableStageDescriptor compute)
        {
            return new ComputePipeline(
                DeviceCreateComputePipeline(Impl, new ComputePipelineDescriptor
                {
                    label = label,
                    compute = new Wgpu.ProgrammableStageDescriptor
                    {
                        module = compute.Module.Impl,
                        entryPoint = compute.EntryPoint
                    }
                })
            );
        }

        public void CreateComputePipelineAsync(string label, CreateComputePipelineAsyncCallback callback, ProgrammableStageDescriptor compute)
        {
            DeviceCreateComputePipelineAsync(Impl, new ComputePipelineDescriptor
                {
                    label = label,
                    compute = new Wgpu.ProgrammableStageDescriptor
                    {
                        module = compute.Module.Impl,
                        entryPoint = compute.EntryPoint
                    }
                }, (s, p, m, _) => callback(s, new ComputePipeline(p), m), IntPtr.Zero
            );
        }

        public delegate void CreateComputePipelineAsyncCallback(CreatePipelineAsyncStatus status, ComputePipeline pipeline, string message);

        public PipelineLayout CreatePipelineLayout(string label, BindGroupLayout[] bindGroupLayouts)
        {
            return new PipelineLayout(
                DeviceCreatePipelineLayout(Impl, new PipelineLayoutDescriptor
                {
                    label = label,
                    bindGroupLayouts = Util.AllocHArray(
                        bindGroupLayouts.Length,
                        bindGroupLayouts.Select(x => x.Impl)),

                    bindGroupLayoutCount = (uint)bindGroupLayouts.Length
                })
            );
        }

        public QuerySet CreateQuerySet(string label, QueryType queryType, uint count, PipelineStatisticName[] pipelineStatistics)
        {
            return new QuerySet(
                DeviceCreateQuerySet(Impl, new QuerySetDescriptor
                {
                    label = label,
                    type = queryType,
                    count = count,
                    pipelineStatistics = Util.AllocHArray(pipelineStatistics),
                    pipelineStatisticsCount = (uint)pipelineStatistics.Length
                })
            );
        }

        public RenderBundleEncoder CreateRenderBundleEncoder(string label, TextureFormat[] colorFormats, TextureFormat depthStencilFormat,
            uint sampleCount, bool depthReadOnly, bool stencilReadOnly)
        {
            return new RenderBundleEncoder(
                DeviceCreateRenderBundleEncoder(Impl, new RenderBundleEncoderDescriptor
                {
                    label = label,
                    colorFormats = Util.AllocHArray(colorFormats),
                    colorFormatsCount = (uint)colorFormats.Length,
                    depthStencilFormat = depthStencilFormat,
                    sampleCount = sampleCount,
                    depthReadOnly = depthReadOnly,
                    stencilReadOnly = stencilReadOnly
                })
            );
        }

        public RenderPipeline CreateRenderPipeline(string label, PipelineLayout layout,
            VertexState vertexState, PrimitiveState primitiveState, MultisampleState multisampleState, 
            DepthStencilState? depthStencilState = null, FragmentState? fragmentState = null)
        {
            RenderPipelineDescriptor desc = CreateRenderPipelineDescriptor(label, layout, vertexState, primitiveState, multisampleState, depthStencilState, fragmentState);

            return new RenderPipeline(DeviceCreateRenderPipeline(Impl, desc));
        }

        public void CreateRenderPipelineAsync(string label, CreateRenderPipelineAsyncCallback callback, PipelineLayout layout,
            VertexState vertexState, PrimitiveState primitiveState, MultisampleState multisampleState, 
            DepthStencilState? depthStencilState = null, FragmentState? fragmentState = null)
        {
            RenderPipelineDescriptor desc = CreateRenderPipelineDescriptor(label, layout, vertexState, primitiveState, multisampleState, depthStencilState, fragmentState);

            DeviceCreateRenderPipelineAsync(Impl, desc, (s, p, m, _) => callback(s, new RenderPipeline(p), m), IntPtr.Zero);
        }

        public delegate void CreateRenderPipelineAsyncCallback(CreatePipelineAsyncStatus status, RenderPipeline pipeline, string message);

        private static RenderPipelineDescriptor CreateRenderPipelineDescriptor(string label, PipelineLayout layout, VertexState vertexState, 
            PrimitiveState primitiveState, MultisampleState multisampleState, DepthStencilState? depthStencilState, 
            FragmentState? fragmentState)
        {
            return new RenderPipelineDescriptor
            {
                label = label,
                layout = layout.Impl,
                vertex = new Wgpu.VertexState
                {
                    module = vertexState.Module.Impl,
                    entryPoint = vertexState.EntryPoint,
                    buffers = Util.AllocHArray(vertexState.bufferLayouts.Length,
                        vertexState.bufferLayouts.Select(x => new Wgpu.VertexBufferLayout
                        {
                            arrayStride = x.ArrayStride,
                            stepMode = x.StepMode,
                            attributes = Util.AllocHArray(x.Attributes),
                            attributeCount = (uint)x.Attributes.Length
                        })
                    ),
                    bufferCount = (uint)vertexState.bufferLayouts.Length
                },
                primitive = primitiveState,
                depthStencil = Util.Optional(depthStencilState),
                multisample = multisampleState,
                fragment = fragmentState == null ? IntPtr.Zero : Util.AllocHStruct(new Wgpu.FragmentState
                {
                    module = fragmentState.Value.Module.Impl,
                    entryPoint = fragmentState.Value.EntryPoint,
                    targets = Util.AllocHArray(fragmentState.Value.colorTargets.Length,
                        fragmentState.Value.colorTargets.Select(x => new Wgpu.ColorTargetState
                        {
                            format = x.Format,
                            blend = Util.Optional(x.BlendState),
                            writeMask = x.WriteMask
                        })
                    ),
                    targetCount = (uint)fragmentState.Value.colorTargets.Length
                })
            };
        }

        public Sampler CreateSampler(string label, AddressMode addressModeU, AddressMode addressModeV, AddressMode addressModeW,
            FilterMode magFilter, FilterMode minFilter, MipmapFilterMode mipmapFilter, 
            float lodMinClamp, float lodMaxClamp, CompareFunction compare, ushort maxAnisotropy)
        {
            return new Sampler(
                DeviceCreateSampler(Impl, new SamplerDescriptor
                {
                    label = label,
                    addressModeU = addressModeU,
                    addressModeV = addressModeV,
                    addressModeW = addressModeW,
                    magFilter = magFilter,
                    minFilter = minFilter,
                    mipmapFilter = mipmapFilter,
                    lodMinClamp = lodMinClamp,
                    lodMaxClamp = lodMaxClamp,
                    compare = compare,
                    maxAnisotropy = maxAnisotropy
                })
            );
        }

        public ShaderModule CreateSprivShaderModule(string label, byte[] spirvCode)
        {
            return new ShaderModule(
                DeviceCreateShaderModule(Impl, new ShaderModuleDescriptor
                {
                    label = label,
                    nextInChain = new WgpuStructChain()
                    .Add_ShaderModuleSPIRVDescriptor(spirvCode)
                    .GetPointer()
                })
            );
        }

        public ShaderModule CreateWgslShaderModule(string label, string wgslCode)
        {
            return new ShaderModule(
                DeviceCreateShaderModule(Impl, new ShaderModuleDescriptor
                {
                    label = label,
                    nextInChain = new WgpuStructChain()
                    .Add_ShaderModuleWGSLDescriptor(wgslCode)
                    .GetPointer()
                })
            );
        }

        public SwapChain CreateSwapChain(string label, Surface surface, TextureUsage usage,
            TextureFormat format, uint width, uint height, PresentMode presentMode)
        {
            return new SwapChain(
                DeviceCreateSwapChain(Impl, surface.Impl,
                new SwapChainDescriptor
                {
                    label = label,
                    usage = (uint)usage,
                    format = format,
                    width = width,
                    height = height,
                    presentMode = presentMode
                })
            );
        }

        public SwapChain CreateSwapChain(Surface surface, in SwapChainDescriptor descriptor)
        {
            return new SwapChain(
                DeviceCreateSwapChain(Impl, surface.Impl, descriptor)
            );
        }

        public Texture CreateTexture(string label, TextureUsage usage,
            TextureDimension dimension, Extent3D size, TextureFormat format,
            uint mipLevelCount, uint sampleCount)
        {
            var desc = new TextureDescriptor
            {
                label = label,
                usage = (uint)usage,
                dimension = dimension,
                size = size,
                format = format,
                mipLevelCount = mipLevelCount,
                sampleCount = sampleCount,
            };

            return CreateTexture(in desc);
        }

        public Texture CreateTexture(in TextureDescriptor descriptor)
        {
            return new Texture(DeviceCreateTexture(Impl, descriptor), descriptor);
        }

        public unsafe FeatureName[] EnumerateFeatures()
        {
            FeatureName features = default;

            ulong size = DeviceEnumerateFeatures(Impl, ref features);

            var featuresSpan = new Span<FeatureName>(Unsafe.AsPointer(ref features), (int)size);

            FeatureName[] result = new FeatureName[size];

            featuresSpan.CopyTo(result);

            return result;
        }

        public bool GetLimits(out SupportedLimits limits)
        {
            limits = new SupportedLimits();

            return DeviceGetLimits(Impl, ref limits);
        }

        public Queue GetQueue() => Queue.For(DeviceGetQueue(Impl));

        public bool HasFeature(FeatureName feature) => DeviceHasFeature(Impl, feature);

        public void PushErrorScope(ErrorFilter filter) => DevicePushErrorScope(Impl, filter);
        public void PopErrorScope(ErrorCallback callback)
        {
            DevicePopErrorScope(Impl,
                (t, m, _) => callback(t, m),
                IntPtr.Zero);
        }

        public void SetDeviceLostCallback(DeviceLostCallback callback)
        {
            DeviceSetDeviceLostCallback(Impl,
                (t, m, _) => callback(t, m),
                IntPtr.Zero);
        }

        private static readonly List<Wgpu.ErrorCallback> s_errorCallbacks = 
            new List<Wgpu.ErrorCallback>();

        public void SetUncapturedErrorCallback(ErrorCallback callback)
        {
            Wgpu.ErrorCallback errorCallback = (t, m, _) => callback(t, m);

            s_errorCallbacks.Add(errorCallback);

            DeviceSetUncapturedErrorCallback(Impl,
                errorCallback,
                IntPtr.Zero);
        }



        /// <summary>
        /// Destroys the GPU Resource associated to this <see cref="Device"/>
        /// </summary>
        public void DestroyResource()
        {
            DeviceDestroy(Impl);
            Impl = default;
        }
        
        /// <summary>
        /// Signals to the underlying rust API that this <see cref="Device"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            DeviceDrop(Impl);
            Impl = default;
        }
    }

    public delegate void ErrorCallback(ErrorType type, string message);
    public delegate void DeviceLostCallback(DeviceLostReason reason, string message);
}
