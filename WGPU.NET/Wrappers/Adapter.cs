using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public struct RequiredLimits
    {
        public Limits Limits;
    }

    public partial struct RequiredLimitsExtras
    {
        public uint MaxPushConstantSize;
    }

    public struct DeviceExtras
    {
        public string TracePath;
    }

    public class Adapter : IDisposable
    {
        internal AdapterImpl Impl;

        internal Adapter(AdapterImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Adapter));

            Impl = impl;
        }

        public FeatureName[] EnumerateFeatures()
        {
            ulong size;
            unsafe
            {
                size = AdapterEnumerateFeatures(Impl, ref *(FeatureName*)null);
            }

            FeatureName[] result = new FeatureName[size];
            AdapterEnumerateFeatures(Impl, ref result[0]);

            return result;
        }

        public bool GetLimits(out SupportedLimits limits)
        {
            limits = new SupportedLimits();

            return AdapterGetLimits(Impl, ref limits) == 1u;
        }

        public void GetProperties(out AdapterProperties properties)
        {
            properties = new AdapterProperties();

            AdapterGetProperties(Impl, ref properties);
        }

        public bool HasFeature(FeatureName feature) => AdapterHasFeature(Impl, feature) == 1u;

        public void RequestDevice(RequestDeviceCallback callback, string label, NativeFeature[] nativeFeatures, QueueDescriptor defaultQueue = default, 
            Limits? limits = null, RequiredLimitsExtras? limitsExtras = null, DeviceExtras? deviceExtras = null, DeviceLostCallback deviceLostCallback = null)
        {
            Wgpu.RequiredLimits requiredLimits = default;
            WgpuStructChain limitsExtrasChain = null;
            WgpuStructChain deviceExtrasChain = null;

            if (limitsExtras is { })
            {
                limitsExtrasChain = new WgpuStructChain()
                    .AddRequiredLimitsExtras(
                        limitsExtras.Value.MaxPushConstantSize);
            }

            if (limits is { })
            {
                requiredLimits = new Wgpu.RequiredLimits
                {
                    nextInChain = limitsExtras == null
                        ? IntPtr.Zero
                        : limitsExtrasChain.GetPointer(),
                    limits = limits.Value
                };
            }

            if (deviceExtras is { })
                deviceExtrasChain = new WgpuStructChain().AddDeviceExtras(deviceExtras.Value.TracePath);

            unsafe
            {
                fixed (NativeFeature* requiredFeatures = nativeFeatures)
                    AdapterRequestDevice(Impl, new DeviceDescriptor
                        {
                            defaultQueue = defaultQueue,
                            requiredLimits = limits is { } ? new IntPtr(&requiredLimits) : IntPtr.Zero,
                            requiredFeatureCount = (uint)nativeFeatures.Length,
                            requiredFeatures = new IntPtr(requiredFeatures),
                            label = label,
                            deviceLostCallback = (reason, message, _) => deviceLostCallback?.Invoke(reason, message),
                            nextInChain = deviceExtras is null ? IntPtr.Zero : deviceExtrasChain.GetPointer()
                        },
                        (s, d, m, _) => callback(s, new Device(d), m), IntPtr.Zero);
            }
            
            limitsExtrasChain?.Dispose();
            deviceExtrasChain?.Dispose();
        }

        public void Dispose() => AdapterRelease(Impl);
    }

    public delegate void RequestDeviceCallback(RequestDeviceStatus status, Device device, string message);
}
