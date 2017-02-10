namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;

    public class PdbInfo
    {
        public PdbInfo()
        {
            Guid = default(Guid);
            PdbNames = new List<PdbName>();
        }

        public int Version { get; set; }

        public int Signature { get; set; }

        public Guid Guid { get; set; }

        public int Age { get; set; }

        public int FlagIndexMax { get; set; }

        public int FlagCount { get; set; }

        public List<PdbName> PdbNames { get; private set; }

        public void AddName(PdbName name)
        {
            PdbNames.Add(name);
        }
    }
}