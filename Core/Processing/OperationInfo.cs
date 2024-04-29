
namespace Atem
{
    internal class OperationInfo
    {
        public string Name;
        public string[] Operands;
        public string Text;

        public string Operand1 { get { return Operands[0]; } }

        public string Operand2 { get { return Operands[1]; } }

        public OperationInfo(string operation)
        {
            Text = operation;
            string[] s = operation.Split(' ');
            Name = s[0];
            if (s.Length > 1)
            {
                Operands = s[1].Split(',');
            }
            else
            {
                Operands = new string[0];
            }
        }
    }
}
