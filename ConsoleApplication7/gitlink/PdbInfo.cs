// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbInfo.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;

    internal class PdbInfo
    {
        internal PdbInfo()
        {
            Guid = default(Guid);
            PdbNames = new List<PdbName>();
        }

        internal int Version { get; set; }

        internal int Signature { get; set; }

        internal Guid Guid { get; set; }

        internal int Age { get; set; }

        internal int FlagIndexMax { get; set; }

        internal int FlagCount { get; set; }

        internal List<PdbName> PdbNames { get; private set; }

        internal void AddName(PdbName name)
        {
            PdbNames.Add(name);
        }
    }
}