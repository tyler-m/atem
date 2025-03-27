using Atem.Core.Processing;
using System.Collections.Generic;

namespace Atem.Core.Debugging
{
    public class Debugger
    {
        Dictionary<ushort, Breakpoint> _breakpointAddressHashset = [];
        List<Breakpoint> _breakpoints = [];

        public bool Active { get; set; }

        public delegate void OnBreakpointEvent(ushort address);
        public OnBreakpointEvent OnBreakpoint;

        public int BreakpointCount { get => _breakpoints.Count; }

        public Breakpoint AddBreakpoint(ushort address)
        {
            Breakpoint breakpoint = new Breakpoint(address);

            if (_breakpointAddressHashset.ContainsKey(address))
            {
                return null;
            }

            _breakpoints.Add(breakpoint);
            _breakpoints.Sort();
            _breakpointAddressHashset.Add(address, breakpoint);

            return breakpoint;
        }

        public bool RemoveBreakpoint(Breakpoint breakpoint)
        {
            _breakpoints.Remove(breakpoint);
            return _breakpointAddressHashset.Remove(breakpoint.Address);
        }

        public Breakpoint GetBreakpoint(int index)
        {
            return _breakpoints[index];
        }

        internal bool CheckBreakpoints(ushort programCounter)
        {
            if (_breakpointAddressHashset.TryGetValue(programCounter, out Breakpoint breakpoint)) 
            {
                if (breakpoint.Enabled)
                {
                    breakpoint.HitCount++;
                    OnBreakpoint?.Invoke(programCounter);
                    return true;
                }
            }

            return false;
        }
    }
}
