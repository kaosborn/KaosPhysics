// Usage:
//   DotElements.exe [lang]
// Purpose:
// * Output elements and isotopes as fixed-column lists.
// * Provide element name for the supplied language. (default="en").

using System;
using System.Collections.Generic;
using Kaos.Physics;

namespace AppMain
{
    class DotElements
    {
        static void Main (string[] args)
        {
            string lang = args.Length == 0 || ! Nuclide.MaxNameLengths.ContainsKey (args[0]) ? "en-US" : args[0];

            Console.WriteLine ("; Z,symbol,name,period,group,category,discoveryYear,discoveryIndex,stableCount,stabilityIndex,block,occurrenceCode,lifeCode,atm0StateCode,melt,boil,weight");
            foreach (var nuc in Nuclide.Table)
                Console.WriteLine (nuc.ToFixedText (lang));

            Console.WriteLine ();
            Console.WriteLine ("; Z,symbol,A,stabilityIndex,decayModeCode,daughterZ,daughterSymbol,daughterA");
            foreach (var nuc in Nuclide.Table)
                foreach (var iso in nuc.Isotopes)
                {
                    var part1 = $"{nuc.Z,3} {nuc.Symbol,-3}{iso.A,3}";
                    if (iso.DecayMode == Decay.Stable)
                        Console.WriteLine (part1);
                    else
                    {
                        int mask = 1;
                        for (var decayIndex = 0; decayIndex < Isotope.DecayModeCount; ++decayIndex)
                        {
                            if (((int) iso.DecayMode & mask) != 0)
                            {
                                var decayCode = Isotope.DecayCodes[decayIndex];
                                var daughterA = iso.A;
                                var daughter = nuc.Transmute ((Decay) mask, ref daughterA);
                                Console.Write (part1);
                                Console.WriteLine ($" {iso.StabilityIndex}{decayCode} {daughter.Z,3} {daughter.Symbol,-3}{daughterA,3}");
                            }
                            mask <<= 1;
                        }
                    }
                }

            Console.WriteLine ();
            Console.WriteLine ("; Z,symbol,A,abundance,occurrenceCode,decayModeCodes,halflife");
            foreach (var nuc in Nuclide.Table)
            {
                double total = 0.0;
                foreach (var iso in nuc.Isotopes)
                {
                    Console.WriteLine ($"{nuc.Z,3} {nuc.Symbol,-3}{iso.ToFixedColumns()}");
                    if (iso.IsNatural)
                        total += iso.Abundance.Value;
                }
                //if ((total >= 0.1 && total <= 99.1) || total > 100.1)
                //    Console.WriteLine ("*** total = " + total);
            }

            Console.WriteLine ();
            Console.WriteLine ("; language,totalTranslations");
            var langHits = new Dictionary<string,int>();
            foreach (var nuc in Nuclide.Table)
                foreach (var kv in nuc.NameMap)
                {
                    langHits.TryGetValue (kv.Key, out int v);
                    langHits[kv.Key] = v + 1;
                }
            foreach (var lg in langHits)
                Console.WriteLine ($"{lg.Key,-5}{lg.Value,4}");

            Console.WriteLine();
            foreach (var lx in Nuclide.GetLongTable())
                Console.WriteLine (lx);
        }
    }
}
