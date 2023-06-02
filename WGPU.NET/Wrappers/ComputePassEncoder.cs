using System;
using System.Runtime.CompilerServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class ComputePassEncoder
    {
        internal ComputePassEncoderImpl Impl;

        internal ComputePassEncoder(ComputePassEncoderImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(ComputePassEncoder));

            Impl = impl;
        }

        public void BeginPipelineStatisticsQuery(QuerySet querySet, uint queryIndex)
            => ComputePassEncoderBeginPipelineStatisticsQuery(Impl, querySet.Impl, queryIndex);

        public void DispatchWorkgroups(uint workgroupCountX, uint workgroupCountY, uint workgroupCountZ)
            => ComputePassEncoderDispatchWorkgroups(Impl, workgroupCountX, workgroupCountY, workgroupCountZ);

        public void DispatchWorkgroupsIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => ComputePassEncoderDispatchWorkgroupsIndirect(Impl, indirectBuffer.Impl, indirectOffset);

        public void End() => ComputePassEncoderEnd(Impl);

        public void EndPipelineStatisticsQuery() => ComputePassEncoderEndPipelineStatisticsQuery(Impl);

        public void InsertDebugMarker(string markerLabel)
            => ComputePassEncoderInsertDebugMarker(Impl, markerLabel);

        public void PushDebugGroup(string groupLabel)
            => ComputePassEncoderPushDebugGroup(Impl, groupLabel);

        public void PopDebugGroup(string groupLabel) => ComputePassEncoderPopDebugGroup(Impl);

        public unsafe void SetBindGroup(uint groupIndex, BindGroup group, uint[] dynamicOffsets)
        {
            ComputePassEncoderSetBindGroup(Impl, groupIndex,
                group.Impl,
                (uint)dynamicOffsets.Length,
                ref Unsafe.AsRef<uint>((void*)Util.AllocHArray(dynamicOffsets))
            );
        }

        public void SetPipeline(ComputePipeline pipeline) => ComputePassEncoderSetPipeline(Impl, pipeline.Impl);
    }
}
