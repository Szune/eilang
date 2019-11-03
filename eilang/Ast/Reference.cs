namespace eilang.Ast
{
    public class Reference
    {
        public string Ident { get; }
        public bool IsModule { get; }

        public Reference(string ident, bool isModule)
        {
            Ident = ident;
            IsModule = isModule;
        }
    }
}