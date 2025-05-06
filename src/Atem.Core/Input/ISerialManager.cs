using System;
using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core.Input
{
    public interface ISerialManager : IAddressable, IBootable, IStateful
    {
        byte SC { get; set; }
        byte SB { get; set; }
        bool TransferEnabled { get; set; }
        bool Master { get; set; }
        event EventHandler<EventArgs> OnTransferRequest;
        event EventHandler<EventArgs> OnClock;
        void Clock();
        void RequestInterrupt();
    }
}
