
namespace Atem.Views.MonoGame.Input.Command
{
    public class ContinueCommand : ICommand
    {
        public string Name => "Continue";

        public void Execute(View view, bool press)
        {
            if (press)
            {
                view.Atem.Continue();
            }
        }
    }
}
