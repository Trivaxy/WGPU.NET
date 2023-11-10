using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WGPU.NET
{
    internal static class Util
    {
		public static IntPtr AllocHStruct<T>(T structure)
			where T : struct
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
			Marshal.StructureToPtr(structure, ptr, false);
  
			return ptr;
		}
  
		public static unsafe IntPtr AllocHArray(byte[] arr)
		{
			IntPtr ptr = Marshal.AllocHGlobal(arr.Length);
  
			Span<byte> span = new Span<byte>((void*)ptr, arr.Length);
  
			new Span<byte>(arr).CopyTo(span);
  
			return ptr;
		}
  
		public static unsafe IntPtr AllocHArray<T>(T[] arr)
			where T : struct
		{
			int size = sizeof(T);
  
			IntPtr ptr = Marshal.AllocHGlobal(size * arr.Length);
  
			Span<T> span = new Span<T>((void*)ptr, arr.Length);
  
            for (int i = 0; i < arr.Length; i++)
            {
				span[i] = arr[i];
            }
  
			return ptr;
        }
  
		public static unsafe IntPtr AllocHArray<T>(int count, IEnumerable<T> items)
			where T : struct
		{
			int size = sizeof(T);
  
			IntPtr ptr = Marshal.AllocHGlobal(size * count);
  
			Span<T> span = new Span<T>((void*)ptr, count);
  
			int i = 0;
  
            foreach (var item in items)
            {
				span[i] = item;
  
				i++;
			}
  
			if (i != count)
				throw new ArgumentException($"{nameof(count)} is larger then the supplied enumerable");
  
			return ptr;
		}
  
		public static IntPtr Optional<T>(T? optional)
			where T : struct
        {
			if (optional == null) return IntPtr.Zero;
  
			return AllocHStruct(optional.Value);
        }
  
		public static void FreePtr(IntPtr ptr) => Marshal.FreeHGlobal(ptr);
  //
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
		where TValue : new()
		{
			if (self.TryGetValue(key, out TValue value))
			{
				return value;
			}
  
			TValue newVal = new TValue();
  
			self[key] = newVal;
  
			return newVal;
		}
  
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, Func<TValue> creator)
		{
			if (self.TryGetValue(key, out TValue value))
			{
				return value;
			}
  
			TValue newVal = creator();
  
			self[key] = newVal;
  
			return newVal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Wgpu.BackendType ToBackend(this Wgpu.InstanceBackend type) => type switch
		{
			Wgpu.InstanceBackend.All => Wgpu.BackendType.Undefined,
			Wgpu.InstanceBackend.Vulkan => Wgpu.BackendType.Vulkan,
			Wgpu.InstanceBackend.GL => Wgpu.BackendType.OpenGLES,
			Wgpu.InstanceBackend.Metal => Wgpu.BackendType.Metal,
			Wgpu.InstanceBackend.DX12 => Wgpu.BackendType.D3D12,
			Wgpu.InstanceBackend.DX11 => Wgpu.BackendType.D3D11,
			Wgpu.InstanceBackend.BrowserWebGPU => Wgpu.BackendType.WebGPU,
			Wgpu.InstanceBackend.Primary => Wgpu.BackendType.Vulkan,
			Wgpu.InstanceBackend.Secondary => Wgpu.BackendType.OpenGLES,
			Wgpu.InstanceBackend.Force32 => Wgpu.BackendType.Force32,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Wgpu.InstanceBackend ToInstanceBackend(this Wgpu.BackendType type) => type switch
		{
			Wgpu.BackendType.Undefined => Wgpu.InstanceBackend.All,
			Wgpu.BackendType.Null => Wgpu.InstanceBackend.All,
			Wgpu.BackendType.WebGPU => Wgpu.InstanceBackend.BrowserWebGPU,
			Wgpu.BackendType.D3D11 => Wgpu.InstanceBackend.DX11,
			Wgpu.BackendType.D3D12 => Wgpu.InstanceBackend.DX12,
			Wgpu.BackendType.Metal => Wgpu.InstanceBackend.Metal,
			Wgpu.BackendType.Vulkan => Wgpu.InstanceBackend.Vulkan,
			Wgpu.BackendType.OpenGL => Wgpu.InstanceBackend.GL,
			Wgpu.BackendType.OpenGLES => Wgpu.InstanceBackend.GL,
			Wgpu.BackendType.Force32 => Wgpu.InstanceBackend.Force32,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
    }
}
