using System;
using System.Collections.Generic;
using Kaos.Physics;

namespace AppMain
{
    class DotElements
    {
        static void Main (string[] args)
        {
            string lang = args.Length==1 ? args[0] : null;

            Console.WriteLine ("; Z,symbol,name,period,group,category,discoveryYear,discoveryIndex,stableCount,instabilityCode,block,occurrenceCode,lifeCode,atm0StateCode,melt,boil,weight");
            foreach (var nuc in Nuclide.Table)
                Console.WriteLine (nuc.ToFixedText (lang));

            Console.WriteLine ();
            Console.WriteLine ("; Z,symbol,A,abundance,decayChars,halflife");
            foreach (var nuc in Nuclide.Table)
            {
                double total = 0.0;
                foreach (var iso in nuc.Isotopes)
                {
                    Console.WriteLine ("{0,3} {1,-3} {2}", nuc.Z, nuc.Symbol, iso.ToFixedColumns());
                    if (iso.IsNatural)
                        total += iso.Abundance.Value;
                }
                //if ((total >= 0.1 && total <= 99.1) || total > 100.1)
                //    Console.WriteLine ("*** total = " + total);
            }

            Console.WriteLine ();
            Console.WriteLine ("; Translation totals");
            var langHits = new Dictionary<string,int>();
            foreach (var nuc in Nuclide.Table)
                foreach (var kv in nuc.NameMap)
                {
                    langHits.TryGetValue (kv.Key, out int v);
                    langHits[kv.Key] = v + 1;
                }
            foreach (var lg in langHits)
                Console.WriteLine (lg.Key + ": " + lg.Value);

            Console.WriteLine();
            foreach (var lx in Nuclide.GetLongTable())
                Console.WriteLine (lx);
            Console.WriteLine();
        }
    }
}
