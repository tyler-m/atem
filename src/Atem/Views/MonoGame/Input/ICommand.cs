
namespace Atem.Views.MonoGame.Input
{
    public interface ICommand
    {
        public string Name { get; }

        public void Execute(View view, bool press);
    }
}
