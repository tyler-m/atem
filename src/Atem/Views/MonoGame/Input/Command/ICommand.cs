
namespace Atem.Views.MonoGame.Input.Command
{
    public interface ICommand
    {
        public CommandType Type { get; }

        public void Execute(bool pressed);
    }
}
