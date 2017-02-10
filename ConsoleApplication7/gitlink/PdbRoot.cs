namespace GitLink.Pdb
{
    using System.Collections.Generic;

    internal class PdbRoot
    {
        internal PdbRoot(PdbStream stream)
        {
            Stream = stream;
            Streams = new List<PdbStream>();
        }

        internal PdbStream Stream { get; set; }

        internal List<PdbStream> Streams { get; private set; }
    }
}