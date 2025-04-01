
namespace Atem.Views.MonoGame.Input.Command
{
    public class PauseCommand : ICommand
    {
        public string Name => "Pause";

        public void Execute(View view, bool press)
        {
            if (press)
            {
                view.Atem.Paused = !view.Atem.Paused;
            }
        }
    }
}
