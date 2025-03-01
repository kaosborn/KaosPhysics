﻿// Usage:
//   DotElements.exe [languageCode]
// Purpose:
// * Output elements and isotopes in concise, fixed-width lists.
// * Provide element and decay mode names for the supplied language (default en-US).

using System;
using System.Collections.Generic;
using System.Globalization;
using Kaos.Physics;

namespace AppMain
{
    class DotElements
    {
        static void Main (string[] args)
        {
            var culture = new CultureInfo (args.Length == 0 ? "en" : args[0]);

            Console.WriteLine ("; z,symbol,name,period,group,category,discoveryYear,discoveryIndex,stableCount,stabilityIndex,block,occurrenceCode,lifeCode,stateCode,melt,boil,weight");
            foreach (var nuc in Nuclide.Table)
                Console.WriteLine (nuc.ToFixedWidthString (culture));

            Console.WriteLine ();
            Console.WriteLine ("; decayModeCode,decayModeSymbol,decayModeName");
            for (var ix = 0; ix < Nuclide.DecayModeSymbols.Count; ++ix)
                Console.WriteLine ($"{Nuclide.DecayModeCodes[ix]} {Nuclide.DecayModeSymbols[ix],-5}{Nuclide.DecayModeNames[culture.TwoLetterISOLanguageName][ix]}");

            Console.WriteLine ();
            Console.WriteLine ("; z,symbol,z,occurrenceCode,stabilityIndex,decayModeCode,productZ,productSymbol,productA");
            foreach (var nuc in Nuclide.Table)
                foreach (var iso in nuc.Isotopes)
                {
                    var part1 = $"{nuc.Z,3} {nuc.Symbol,-3}{iso.A,3}";
                    if (iso.DecayMode == Decay.None)
                        Console.WriteLine (part1);
                    else
                    {
                        var orgChar = iso.Occurrence == Origin.Synthetic ? ' ' : Nuclide.OccurrenceCodes[(int) iso.Occurrence];
                        foreach (var decayIndex in iso.GetDecayIndexes())
                        {
                            var productZ = iso.Transmute (decayIndex, out int productA);
                            Console.WriteLine ($"{part1} {orgChar}{iso.StabilityIndex}{Nuclide.DecayModeCodes[decayIndex]} {productZ,3} {Nuclide.Table[productZ].Symbol,-3}{productA,3}");
                        }
                    }
                }

            Console.WriteLine ();
            Console.WriteLine ("; z,symbol,a,abundance,occurrenceCode,decayModeCodes,halflife");
            foreach (var nuc in Nuclide.Table)
            {
                double total = 0.0;
                foreach (var iso in nuc.Isotopes)
                {
                    Console.WriteLine ($"{nuc.Z,3} {nuc.Symbol,-3}{iso.ToFixedWidthString(culture)}");
                    if (iso.IsNatural)
                        total += iso.Abundance.Value;
                }
                //if ((total >= 0.1 && total <= 99.1) || total > 100.1)
                //    Console.WriteLine ("*** total = " + total);
            }

            Console.WriteLine ();
            Console.WriteLine ("; languageCode,totalDiffs");
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
