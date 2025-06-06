﻿using System.Collections.Generic;

namespace Atem.Core.Memory
{
    public interface IAddressable
    {
        byte Read(ushort address, bool ignoreAccessRestrictions = false);
        public void Write(ushort address, byte value, bool ignoreRenderMode = false);
        IEnumerable<(ushort Start, ushort End)> GetAddressRanges();
    }
}
