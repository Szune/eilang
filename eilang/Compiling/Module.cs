namespace eilang.Compiling
{
    public class Module
    {
        //public Dictionary<string, Function> Functions {get;} = new Dictionary<string, Function>();
        //public Dictionary<string, Class> Classes {get;} = new Dictionary<string, Class>();
        public string Name { get; }
        public Module(string name)
        {
            Name = name;
        }
    }
}