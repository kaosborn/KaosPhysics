using System;
using System.Collections.ObjectModel;
using Kaos.Physics;

namespace AppMain
{
    class DotGenJson
    {
        static readonly string[] allLangs = new string[] { "de", "en", "en-GB", "en-US", "es", "fr", "it", "ru" };

        static readonly ReadOnlyCollection<string> nutritionDescriptions = new ReadOnlyCollection<string> (new string[]
        {
            "Not absorbed",
            "Essential element (> .1% by mass)",
            "Essential element (< .1% by mass)",
            "Beneficial trace element",
            "Nonbeneficial trace element"
        });

        static readonly ReadOnlyCollection<string> stabilityDescriptions = new ReadOnlyCollection<string>(new string[]
        {
            "Stable",
            "Slightly radioactive",
            "Somewhat radioactive",
            "Significantly radioactive",
            "Highly radioactive",
            "Extremely radioactive"
        });

        static void Main()
        {
            Console.WriteLine ("{");
            Console.Write ("  \"categoryNames\": [ ");
            for (int ix = 0; ix < Nuclide.CategoryNames.Count; ++ix)
            {
                if (ix != 0)
                    Console.Write (", ");
                Console.Write ($"\"{Nuclide.CategoryNames[ix]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.Write ("  \"occurrenceNames\": [ ");
            var occurrenceNames = Enum.GetNames (typeof (Origin));
            for (int ix = 0; ix < occurrenceNames.Length; ++ix)
            {
                if (ix != 0)
                    Console.Write (", ");
                Console.Write ($"\"{occurrenceNames[ix]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.Write ("  \"stateNames\": [ ");
            var stateNames = Enum.GetNames (typeof (State));
            for (int ix = 0; ix < stateNames.Length; ++ix)
            {
                if (ix != 0)
                    Console.Write (", ");
                Console.Write ($"\"{stateNames[ix]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.WriteLine ("  \"decayChars\": \"" + Isotope.DecayChars + "\",");
            Console.WriteLine();

            Console.Write ("  \"decayCodes\": [ ");
            for (int dx = 1; dx < Isotope.DecayCodes.Count; ++dx)
            {
                if (dx > 1)
                    Console.Write (", ");
                Console.Write ($"\"{Isotope.DecayCodes[dx]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.Write ("  \"biologyCodes\": [ ");
            for (var bx = 0; bx < Nuclide.LifeCodes.Count; ++bx)
            {
                if (bx != 0)
                    Console.Write (", ");
                Console.Write ($"\"{Nuclide.LifeCodes[bx]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.WriteLine ("  \"biologyDescriptions\":");
            Console.WriteLine ("  [");
            for (int ix = 0; ix < nutritionDescriptions.Count; ++ix)
            {
                Console.Write ($"    \"{nutritionDescriptions[ix]}\"");
                if (ix+1 < nutritionDescriptions.Count)
                    Console.Write (",");
                Console.WriteLine();
            }
            Console.WriteLine ("  ],");
            Console.WriteLine ();

            Console.WriteLine("  \"stabilityDescriptions\":");
            Console.WriteLine("  [");
            for (int ix = 0; ix < stabilityDescriptions.Count; ++ix)
            {
                Console.Write($"    \"{stabilityDescriptions[ix]}\"");
                if (ix + 1 < stabilityDescriptions.Count)
                    Console.Write(",");
                Console.WriteLine();
            }
            Console.WriteLine("  ],");
            Console.WriteLine();

            var maxLens = new int[Nuclide.Table.Count];
            for (var ex = 0; ex < Nuclide.Table.Count; ++ex)
                for (var lx = 0; lx < allLangs.Length; ++lx)
                {
                    var nm = Nuclide.Table[ex].GetName (allLangs[lx]);
                    if (maxLens[ex] < nm.Length)
                        maxLens[ex] = nm.Length;
                }
            for (var lx = 0; lx < allLangs.Length; ++lx)
            {
                Console.Write ($"  \"lang-{allLangs[lx]}\"");
                Console.Write (new string (' ', 5 - allLangs[lx].Length));
                Console.Write (": [ ");
                for (int ex = 0; ex < Nuclide.Table.Count; ++ex)
                {
                    var nm = Nuclide.Table[ex].GetName (allLangs[lx]);
                    Console.Write ($"\"{nm}\"");
                    if (ex+1 < Nuclide.Table.Count)
                        Console.Write (',');
                    Console.Write (new string (' ', maxLens[ex] - nm.Length + 1));
                }
                Console.WriteLine ("],");
            }
            Console.WriteLine();

            Console.WriteLine ("  \"nuclides\":");
            Console.WriteLine ("  [");

            foreach (var nuc in Nuclide.Table)
            {
                if (nuc.Z > 0)
                    Console.WriteLine (",");
                Console.Write ("    { \"z\":");
                string ts = nuc.Z.ToString();
                Console.Write (new string (' ', 3 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"symbol\": \"" + nuc.Symbol + "\"");
                Console.Write (',');
                Console.Write (new string (' ', 3 - nuc.Symbol.Length));
                Console.Write ("\"period\": ");
                Console.Write (nuc.Period);
                Console.Write(", \"group\":");
                ts = nuc.Group.ToString();
                Console.Write (new string (' ', 2 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"categoryIndex\": ");
                Console.Write (nuc.CategoryIndex);
                Console.Write (", \"block\": \"" + nuc.Block + "\"");
                Console.Write (", \"occurrenceIndex\": ");
                Console.Write (nuc.OccurrenceIndex);
                Console.Write (", \"lifeIndex\": ");
                Console.Write (nuc.LifeIndex);
                Console.Write (", \"discoveryYear\":");
                ts = nuc.Known.ToString();
                Console.Write (new string (' ', 5 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"discoveryIndex\": ");
                Console.Write (nuc.KnownIndex);
                Console.Write (", \"atm0StateIndex\": ");
                Console.Write (nuc.Atm0StateIndex);
                Console.Write (", \"melt\":");
                ts = nuc.Melt == null ? "null" : nuc.Melt.Value.ToString ("F4");
                Console.Write (new string (' ', 9 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"boil\":");
                ts = nuc.Boil == null ? "null" : nuc.Boil.Value.ToString ("F4");
                Console.Write (new string (' ', 9 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"weight\":");
                ts = nuc.Weight.ToString ("F4");
                Console.Write (new string (' ', 9 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"stableCount\":");
                ts = nuc.StableCount.ToString();
                Console.Write (new string (' ', 2 - ts.Length));
                Console.Write (ts);
                Console.Write (", \"stabilityIndex\": ");
                Console.Write (nuc.StabilityIndex);
                Console.Write (", \"isotopes\": [");

                for (var ix = 0; ix < nuc.Isotopes.Count; ++ix)
                {
                    var iso = nuc.Isotopes[ix];
                    if (ix != 0)
                        Console.Write (',');
                    Console.Write ('[');
                    Console.Write (iso.A);
                    Console.Write (',');
                    Console.Write (iso.IsNatural ? iso.Abundance.ToString() : "null");
                    if (iso.DecayBits != 0 || iso.Halflife != null)
                    {
                        Console.Write (',');
                        Console.Write (iso.DecayBits);
                        Console.Write (',');
                        Console.Write (iso.Halflife.Value);
                    }
                    Console.Write (']');
                }

                Console.Write ("] }");
            }

            Console.WriteLine();
            Console.WriteLine("  ]");
            Console.WriteLine("}");
        }
    }
}
