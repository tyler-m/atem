namespace Atem.Input.Command
{
    public interface ICommand
    {
        public CommandType Type { get; }

        public void Execute(bool pressed);
    }
}
