using System.IO;

namespace Atem.Core.State
{
    public interface IStateful
    {
        public void GetState(BinaryWriter writer);

        public void SetState(BinaryReader reader);
    }
}
