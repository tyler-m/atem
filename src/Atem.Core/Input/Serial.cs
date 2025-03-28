using Atem.Core.State;
using System.IO;

namespace Atem.Core.Input
{
    public class Serial : IStateful
    {
        public byte SB, SC;

        public void GetState(BinaryWriter writer)
        {
            writer.Write(SB);
            writer.Write(SC);
        }

        public void SetState(BinaryReader reader)
        {
            SB = reader.ReadByte();
            SC = reader.ReadByte();
        }
    }
}
