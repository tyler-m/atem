
namespace Atem.Core.State
{
    public enum BootMode
    {
        DMG,
        CGB
    }

    public interface IBootable
    {
        void Boot(BootMode mode);
    }
}
