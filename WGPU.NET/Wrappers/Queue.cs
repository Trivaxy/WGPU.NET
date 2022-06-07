using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Queue
    {
        private static Dictionary<QueueImpl, Queue> instances =
            new Dictionary<QueueImpl, Queue>();

        internal QueueImpl Impl;

        private Queue(QueueImpl impl)
        {
            Impl = impl;
        }

        internal static Queue For(QueueImpl impl)
            => impl.Handle == IntPtr.Zero ? null : instances.GetOrCreate(impl, () => new Queue(impl));

        public void OnSubmittedWorkDone(QueueWorkDoneCallback callback)
        {
            QueueOnSubmittedWorkDone(Impl,
                (s, d) => callback(s), 
                IntPtr.Zero
            );
        }

        public unsafe void Submit(CommandBuffer[] commands)
        {
            QueueSubmit(Impl, (uint)commands.Length,
                ref Unsafe.AsRef<CommandBufferImpl>(
                    (void*)Util.AllocHArray(commands.Length, commands.Select(x=>x.Impl))
                )
            );
        }

        public unsafe void WriteBuffer<T>(Buffer buffer, ulong bufferOffset, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            ulong structSize = (ulong)sizeof(T);


            QueueWriteBuffer(Impl, buffer.Impl, bufferOffset,
                (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(data)), 
                (ulong)data.Length * structSize);
        }

        public unsafe void WriteTexture<T>(ImageCopyTexture destination, ReadOnlySpan<T> data, 
            in TextureDataLayout dataLayout, in Extent3D writeSize)
            where T : unmanaged
        {
            ulong structSize = (ulong)Marshal.SizeOf<T>();


            QueueWriteTexture(Impl, destination,
                (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(data)),
                (ulong)data.Length * structSize,
                dataLayout, in writeSize);
        }
    }
}
