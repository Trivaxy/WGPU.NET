using System;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class ComputePipeline
    {
        private ComputePipelineImpl _impl;

        internal ComputePipelineImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(ComputePipeline));

                return _impl;
            }

            private set => _impl = value;
        }

        internal ComputePipeline(ComputePipelineImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(ComputePipeline));

            Impl = impl;
        }

        public BindGroupLayout GetBindGroupLayout(uint groupIndex)
            => BindGroupLayout.For(ComputePipelineGetBindGroupLayout(Impl, groupIndex));

        public void SetLabel(string label) => ComputePipelineSetLabel(Impl, label);
        
        /// <summary>
        /// Signals to the underlying rust API that this <see cref="RenderBundle"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            ComputePipelineDrop(Impl);
            Impl = default;
        }
    }

    public class PipelineLayout
    {
        private PipelineLayoutImpl _impl;

        internal PipelineLayoutImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(PipelineLayout));

                return _impl;
            }

            private set => _impl = value;
        }

        internal PipelineLayout(PipelineLayoutImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(PipelineLayout));

            Impl = impl;
        }

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="FreeHandle"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            PipelineLayoutDrop(Impl);
            Impl = default;
        }
    }

    public class QuerySet
    {
        private QuerySetImpl _impl;

        internal QuerySet(QuerySetImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(QuerySet));

            Impl = impl;
        }

        internal QuerySetImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(QuerySet));

                return _impl;
            }

            private set => _impl = value;
        }

        /// <summary>
        /// Destroys the GPU Resource associated to this <see cref="QuerySet"/>
        /// </summary>
        public void DestroyResource()
        {
            QuerySetDestroy(Impl);
            Impl = default;
        }

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="QuerySet"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            QuerySetDrop(Impl);
            Impl = default;
        }
    }

    public class RenderPipeline
    {
        private RenderPipelineImpl _impl;

        internal RenderPipelineImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(RenderPipeline));

                return _impl;
            }

            private set => _impl = value;
        }

        internal RenderPipeline(RenderPipelineImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderPipeline));

            Impl = impl;
        }

        public BindGroupLayout GetBindGroupLayout(uint groupIndex)
            => BindGroupLayout.For(RenderPipelineGetBindGroupLayout(Impl, groupIndex));

        public void SetLabel(string label) => RenderPipelineSetLabel(Impl, label);

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="RenderPipeline"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            RenderPipelineDrop(Impl);
            Impl = default;
        }
    }

    public class Sampler
    {
        private SamplerImpl _impl;

        internal SamplerImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Sampler));

                return _impl;
            }

            private set => _impl = value;
        }

        internal Sampler(SamplerImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Sampler));

            Impl = impl;
        }

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="Sampler"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            SamplerDrop(Impl);
            Impl = default;
        }
    }

    public class ShaderModule
    {
        private ShaderModuleImpl _impl;

        internal ShaderModuleImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(ShaderModule));

                return _impl;
            }

            private set => _impl = value;
        }

        internal ShaderModule(ShaderModuleImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(ShaderModule));

            Impl = impl;
        }

        public unsafe void GetCompilationInfo(CompilationInfoCallback callback)
        {
            ShaderModuleGetCompilationInfo(Impl,
                (CompilationInfoRequestStatus s, in Wgpu.CompilationInfo c, IntPtr _) =>
                {
                    callback(s,
                        new ReadOnlySpan<CompilationMessage>((void*)c.messages, (int)c.messages)
                    );

                }, IntPtr.Zero);
        }

        public void SetLabel(string label) => ShaderModuleSetLabel(Impl, label);

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="ShaderModule"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            ShaderModuleDrop(Impl);
            Impl = default;
        }
    }

    public delegate void CompilationInfoCallback(CompilationInfoRequestStatus status,
        ReadOnlySpan<CompilationMessage> messages);

    public class SwapChain
    {
        internal SwapChainImpl Impl;

        internal SwapChain(SwapChainImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(SwapChain));

            Impl = impl;
        }

        public TextureView GetCurrentTextureView()
            => TextureView.For(SwapChainGetCurrentTextureView(Impl));

        public void Present()
            => SwapChainPresent(Impl);
    }

    public delegate void QueueWorkDoneCallback(QueueWorkDoneStatus status); 

    public class CommandBuffer
    {
        private CommandBufferImpl _impl;

        internal CommandBufferImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(CommandBuffer));

                return _impl;
            }

            private set => _impl = value;
        }

        internal CommandBuffer(CommandBufferImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(CommandBuffer));

            Impl = impl;
        }

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="CommandBuffer"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            CommandBufferDrop(Impl);
            Impl = default;
        }
    }

    public class RenderBundle
    {
        private RenderBundleImpl _impl;

        internal RenderBundleImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(RenderBundle));

                return _impl;
            }

            private set => _impl = value;
        }

        internal RenderBundle(RenderBundleImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderBundle));
            Impl = impl;
        }

        /// <summary>
        /// Signals to the underlying rust API that this <see cref="RenderBundle"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            RenderBundleDrop(Impl);
            Impl = default;
        }
    }
}
