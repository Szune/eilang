namespace eilang.Tokens
{
    public class Position
    {
        public Position(int line, int col)
        {
            Line = line;
            Col = col;
        }

        public int Line { get; }
        public int Col { get; }
    }
}