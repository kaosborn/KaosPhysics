// Usage:
//   DotGenJson.exe [-js]
// Purpose:
//   Output JSON data (media type application/json) of KaosPhysics model.
//   Or output JavaScript of same with -js command line switch.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Kaos.Physics;

namespace AppMain
{
    class DotGenJson
    {
        static int Main (string[] args)
        {
            string quote = (args.Length == 1 && args[0] == "-js") ? "" : "\"";
            string binop = (args.Length == 1 && args[0] == "-js") ? " =" : ":";

            Console.WriteLine ('{');

            Console.Write (toJsonSB2 ("categoryGroupNames", Nuclide.CategoryGroupNames).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.Write (toJsonSB2 ("categoryNames", Nuclide.CategoryNames).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.WriteLine ($"  {quote}decayCodes{quote}{binop} \"{Isotope.DecayCodes}\",");
            Console.WriteLine ();

            Console.Write (toJsonSB1 ("decaySymbols", Isotope.DecaySymbols).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.Write (toJsonSB1 ("biologyCodes", Nuclide.LifeCodes).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.Write (toJsonSB2 ("biologyDescriptions", Nuclide.LifeDescriptions).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.WriteLine ($"  {quote}occurrenceCodes{quote}{binop} \"{Nuclide.OccurrenceCodes}\",");
            Console.WriteLine ();

            Console.Write (toJsonSB2 ("occurrenceNames", Nuclide.OccurrenceNames).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.Write (toJsonSB2 ("stabilityDescriptions", Nuclide.StabilityDescriptions).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.WriteLine ($"  {quote}stateCodes{quote}{binop} \"{Nuclide.StateCodes}\",");
            Console.WriteLine ();

            Console.Write (toJsonSB2 ("stateNames", Nuclide.StateNames).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            Console.Write (toJsonSB2 ("themeNames", Nuclide.ThemeNames).ToString());
            Console.WriteLine (',');
            Console.WriteLine ();

            // Output fixed-width element names by language:

            var maxLens = new int[Nuclide.Table.Count];
            for (var nx = 0; nx < Nuclide.Table.Count; ++nx)
                foreach (var lg in Nuclide.MaxNameLengths.Keys)
                {
                    var nm = Nuclide.Table[nx].GetName (lg);
                    if (maxLens[nx] < nm.Length)
                        maxLens[nx] = nm.Length;
                }
            Console.WriteLine ($"  {quote}nuclideNames{quote}{binop} {{");
            int c1 = 0;
            foreach (var lg in Nuclide.MaxNameLengths.Keys)
            {
                var lg2 = lg.Length != 5 ? lg : lg.Substring (0, 2) + lg.Substring (3);
                if (c1++ != 0) Console.WriteLine (',');
                Console.Write ($"    \"{lg2}\"");
                Console.Write (new string (' ', 4 - lg2.Length));
                Console.Write ($": [ ");
                for (int nx = 0; nx < Nuclide.Table.Count; ++nx)
                {
                    var nm = Nuclide.Table[nx].GetName (lg);
                    Console.Write ($"\"{nm}\"");
                    if (nx+1 < Nuclide.Table.Count)
                        Console.Write (',');
                    Console.Write (new string (' ', maxLens[nx] - nm.Length + 1));
                }
                Console.Write (']');
            }
            Console.WriteLine ();
            Console.WriteLine ("  },");
            Console.WriteLine ();

            // Output the nuclides:

            Console.WriteLine ($"  {quote}nuclides{quote}{binop}");
            Console.WriteLine ("  [");
            for (int nx = 0; nx < Nuclide.Table.Count; ++nx)
            {
                if (nx != 0)
                    Console.WriteLine (',');
                Console.Write ("    { ");
                Console.Write (Nuclide.Table[nx].ToJsonSB (quote).ToString());
                Console.Write (" }");
            }
            Console.WriteLine ();
            Console.WriteLine ("  ]");

            Console.WriteLine ('}');
            return 0;

            StringBuilder toJsonSB1 (string propertyName, IList<string> values)
            {
                var sb = new StringBuilder();
                sb.Append ("  ");
                sb.Append (quote);
                sb.Append (propertyName);
                sb.Append (quote);
                sb.Append (binop);
                sb.Append (" [ ");
                for (int ix = 0; ix < values.Count; ++ix)
                {
                    if (ix != 0)
                        sb.Append (", ");
                    sb.Append ('\"');
                    sb.Append (values[ix]);
                    sb.Append ('\"');
                }
                sb.Append (" ]");
                return sb;
            }

            StringBuilder toJsonSB2 (string propertyName, ReadOnlyDictionary<string,string[]> vals)
            {
                var sb = new StringBuilder();
                sb.Append ("  ");
                sb.Append (quote);
                sb.Append (propertyName);
                sb.Append (quote);
                sb.Append (binop);
                sb.Append (" {");
                sb.Append (Environment.NewLine);
                int cx1 = 0;
                foreach (var kv in vals)
                {
                    if (cx1++ != 0) { sb.Append (','); sb.Append (Environment.NewLine); }
                    sb.Append ("    \"");
                    sb.Append (kv.Key);
                    sb.Append ("\": [");
                    int cx2 = 0;
                    foreach (var nm in kv.Value)
                    {
                        if (cx2++ != 0) sb.Append (", ");
                        sb.Append ('\"');
                        sb.Append (nm);
                        sb.Append ('\"');
                    }
                    sb.Append (" ]");
                }
                sb.Append (Environment.NewLine);
                sb.Append ("  }");
                return sb;
            }
        }
    }
}
