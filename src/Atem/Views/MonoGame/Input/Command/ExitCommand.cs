
namespace Atem.Views.MonoGame.Input.Command
{
    internal class ExitCommand : ICommand
    {
        public string Name => "Exit Program";

        public void Execute(View view, bool press)
        {
            if (press)
            {
                view.Exit();
            }
        }
    }
}
