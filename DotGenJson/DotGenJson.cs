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
            string quote = "";

            Console.WriteLine ('{');
            Console.Write ($"  {quote}categoryNames{quote}: [ ");
            for (int ix = 0; ix < Nuclide.CategoryNames.Count; ++ix)
            {
                if (ix != 0)
                    Console.Write (", ");
                Console.Write ($"\"{Nuclide.CategoryNames[ix]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.Write ($"  {quote}occurrenceNames{quote}: [ ");
            var occurrenceNames = Enum.GetNames (typeof (Origin));
            for (int ix = 0; ix < occurrenceNames.Length; ++ix)
            {
                if (ix != 0)
                    Console.Write (", ");
                Console.Write ($"\"{occurrenceNames[ix]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.Write ($"  {quote}stateNames{quote}: [ ");
            var stateNames = Enum.GetNames (typeof (State));
            for (int ix = 0; ix < stateNames.Length; ++ix)
            {
                if (ix != 0)
                    Console.Write (", ");
                Console.Write ($"\"{stateNames[ix]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.WriteLine ($"  {quote}decayChars{quote}: \"{Isotope.DecayChars}\",");
            Console.WriteLine();

            Console.Write ($"  {quote}decayCodes{quote}: [ ");
            for (int dx = 1; dx < Isotope.DecayCodes.Count; ++dx)
            {
                if (dx > 1)
                    Console.Write (", ");
                Console.Write ($"\"{Isotope.DecayCodes[dx]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.Write ($"  {quote}biologyCodes{quote}: [ ");
            for (var bx = 0; bx < Nuclide.LifeCodes.Count; ++bx)
            {
                if (bx != 0)
                    Console.Write (", ");
                Console.Write ($"\"{Nuclide.LifeCodes[bx]}\"");
            }
            Console.WriteLine (" ],");
            Console.WriteLine ();

            Console.WriteLine ($"  {quote}biologyDescriptions{quote}:");
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

            Console.WriteLine ($"  {quote}stabilityDescriptions{quote}:");
            Console.WriteLine ("  [");
            for (int ix = 0; ix < stabilityDescriptions.Count; ++ix)
            {
                Console.Write ($"    \"{stabilityDescriptions[ix]}\"");
                if (ix + 1 < stabilityDescriptions.Count)
                    Console.Write (',');
                Console.WriteLine();
            }
            Console.WriteLine ("  ],");
            Console.WriteLine ();

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
                Console.Write ($"  {quote}lang-{allLangs[lx]}{quote}");
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

            Console.WriteLine ($"  {quote}nuclides{quote}:");
            Console.WriteLine ("  [");
            for (int nx = 0; nx < Nuclide.Table.Count; ++nx)
            {
                if (nx != 0)
                    Console.WriteLine (",");
                Console.Write ("    { ");
                Console.Write (Nuclide.Table[nx].ToJson (quote));
                Console.Write ("] }");
            }
            Console.WriteLine ();
            Console.WriteLine ("  ]");

            Console.WriteLine ("}");
        }
    }
}
