using System;
using Atem.Core.State;

namespace Atem.Core.Input
{
    internal interface ISerialManager : IStateful
    {
        public byte SC { get; set; }
        public byte SB { get; set; }
        public bool TransferEnabled { get; set; }
        public bool Master { get; set; }

        public event EventHandler<EventArgs> OnTransferRequest;

        public event EventHandler<EventArgs> OnClock;
    }
}
