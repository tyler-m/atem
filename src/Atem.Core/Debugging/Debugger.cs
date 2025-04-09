using System.Collections.Generic;

namespace Atem.Core.Debugging
{
    public class Debugger : IDebugger
    {
        private readonly Dictionary<ushort, Breakpoint> _breakpointAddressHashset = [];
        private readonly List<Breakpoint> _breakpoints = [];
        private bool _active;

        public bool Active { get => _active; set => _active = value; }
        public int BreakpointCount { get => _breakpoints.Count; }

        public delegate void OnBreakpointEvent(ushort address);
        public event OnBreakpointEvent OnBreakpoint;

        public bool AddBreakpoint(Breakpoint breakpoint)
        {
            if (_breakpointAddressHashset.ContainsKey(breakpoint.Address))
            {
                return false;
            }

            _breakpoints.Add(breakpoint);
            _breakpoints.Sort();
            _breakpointAddressHashset.Add(breakpoint.Address, breakpoint);
            
            return true;
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
