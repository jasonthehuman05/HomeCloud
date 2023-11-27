namespace HomeCloud_Server.Services.MySqlInteractionToolkit
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class MySqlColumn : Attribute
    {
        private string _name = "";
        private bool _auto = true;

        public string Name { get => _name; set => _name = value; }
        public bool Auto { get => _auto; set => _auto = value; }

        public MySqlColumn(string name)
        {
            _name = name;
        }

        public MySqlColumn(string name, bool auto) : this(name)
        {
            _auto = auto;
        }
    }
}
