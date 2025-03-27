using System.Collections.Generic;
using Atem.Core;

namespace Atem
{
    public class ViewHelper
    {
        private AtemRunner _atem;

        public ViewHelper(AtemRunner atem)
        {
            _atem = atem;
        }

        public ushort GetAddressOfNextInstruction()
        {
            return _atem.Bus.Processor.AddressOfNextInstruction;
        }

        public byte PeekAt(int address)
        {
            return _atem.Bus.Read((ushort)address);
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
                        yield return _atem.Bus.Read((ushort)addressToPeek);
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
                        yield return _atem.Bus.Read((ushort)addressToPeek);
                    }
                }
            }
        }
    }
}
