using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace Atem
{
    internal class ViewHelper
    {
        private CPU _cpu;
        private Bus _bus;

        public ViewHelper(CPU cpu, Bus bus)
        {
            _cpu = cpu;
            _bus = bus;
        }

        public ushort GetAddressOfNextOperation()
        {
            return _cpu.AddressOfNextOperation;
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
