using System;
using System.Collections.Generic;
using System.Text;

namespace WGPU.NET
{
    public class HandleDestroyedException : Exception
    {
        public HandleDestroyedException(string resourceType)
            : base($"Handle of this {resourceType} has been destroyed" +
                  $"therefore it can't be used anymore")
        {

        }
    }
}
