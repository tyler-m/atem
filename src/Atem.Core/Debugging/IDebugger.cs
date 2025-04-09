
namespace Atem.Core.Debugging
{
    public interface IDebugger
    {
        public bool Active { get; set; }
        public bool AddBreakpoint(Breakpoint breakpoint);
        public bool RemoveBreakpoint(Breakpoint breakpoint);
        public Breakpoint GetBreakpoint(int index);
        public int BreakpointCount { get; }
    }
}
