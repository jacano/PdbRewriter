namespace GitLink.Pdb
{
    public class PdbName
    {
        public PdbName(string name = "")
        {
            Name = name;
        }

        public string Name { get; set; }

        public long Stream { get;  set; }
    }
}