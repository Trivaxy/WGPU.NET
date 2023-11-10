using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Queue : IDisposable
    {
        private readonly static Dictionary<QueueImpl, Queue> instances = new Dictionary<QueueImpl, Queue>();
        
        private QueueImpl _impl;

        internal Queue(QueueImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Queue));

            _impl = impl;
        }
        
        internal static Queue For(QueueImpl impl)
            => impl.Handle == IntPtr.Zero ? null : instances.GetOrCreate(impl, () => new Queue(impl));

        public void OnSubmittedWorkDone(QueueWorkDoneCallback callback)
        {
            QueueOnSubmittedWorkDone(_impl,
                (s, d) => callback(s), 
                IntPtr.Zero
            );
        }

        public unsafe void Submit(CommandBuffer[] commands)
        {
            Span<CommandBufferImpl> commandBufferImpls = stackalloc CommandBufferImpl[commands.Length];

            for (int i = 0; i < commands.Length; i++)
                commandBufferImpls[i] = commands[i].Impl;
            
            QueueSubmit(_impl, (uint)commands.Length, ref commandBufferImpls.GetPinnableReference());
        }

        public unsafe void WriteBuffer<T>(Buffer buffer, ulong bufferOffset, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            ulong structSize = (ulong)sizeof(T);


            QueueWriteBuffer(_impl, buffer.Impl, bufferOffset,
                (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(data)), 
                (ulong)data.Length * structSize);
        }

        public unsafe void WriteTexture<T>(ImageCopyTexture destination, ReadOnlySpan<T> data, 
            in TextureDataLayout dataLayout, in Extent3D writeSize)
            where T : unmanaged
        {
            ulong structSize = (ulong)Marshal.SizeOf<T>();


            QueueWriteTexture(_impl, destination,
                (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(data)),
                (ulong)data.Length * structSize,
                dataLayout, in writeSize);
        }
        
        /// <summary>
        /// This function will be called automatically when this Queue's associated Device is disposed.
        /// </summary>
        public void Dispose()
        {
            QueueRelease(_impl);
        }
    }
}
