using System.Collections.Generic;
using Atem.Core.Processing;
using Atem.Core;
using Atem.Core.Graphics;

namespace Atem
{
    public class ViewHelper
    {
        private Processor _processor;
        private Bus _bus;
        private GraphicsManager _graphics;

        public ViewHelper(Processor processor, Bus bus, GraphicsManager graphics)
        {
            _processor = processor;
            _bus = bus;
            _graphics = graphics;
        }

        public ushort GetAddressOfNextInstruction()
        {
            return _processor.AddressOfNextInstruction;
        }

        public byte PeekAt(int address)
        {
            return _bus.Read((ushort)address);
        }

        public IEnumerable<int> PeekBefore(int address, int count)
        {
            if (count > 0)
            {
                for (int addressToPeek = address - count; addressToPeek < address; addressToPeek++)
                {
                    if (addressToPeek < 0 || addressToPeek > ushort.MaxValue)
                    {
                        yield return -1;
                    }
                    else
                    {
                        yield return _bus.Read((ushort)addressToPeek);
                    }
                }
            }
        }

        public IEnumerable<int> PeekAfter(int address, int count)
        {
            if (count > 0)
            {
                for (int addressToPeek = address + 1; addressToPeek <= address + count; addressToPeek++)
                {
                    if (addressToPeek < 0 || addressToPeek > ushort.MaxValue)
                    {
                        yield return -1;
                    }
                    else
                    {
                        yield return _bus.Read((ushort)addressToPeek);
                    }
                }
            }
        }
    }
}
