using System;
using Physics;

namespace AppMain
{
    class ConGenHtml
    {
        static void Main()
        {
            string[] cMap = new string[]
            { "AlkCat", "AEaCat", "LanCat", "ActCat", "TMeCat", "PtMCat", "MetCat", "NMeCat", "HalCat", "NobCat" };

            foreach (var e in Nuclide.GetElements())
            {
                Console.Write ("<td onclick=\"cellClick()\">");
                Console.Write ("<div class=\"" + cMap[e.CategoryIndex] + "\">");

                Console.Write ("<div class=\"Nm\"><span>" + e.Name + "</span>");
                foreach (var word in e.NameMap)
                    Console.Write ("<span style=\"display:none\" lang=\"" + word.Key + "\">" + word.Value + "</span>");
                Console.Write ("</div>");

                Console.Write ("<a><div class=\"Sb\">" + e.Symbol + "</div></a>");
                Console.Write ("<div class=\"An\">" + e.Z + "</div>");
                Console.WriteLine ("</div></td>");
            }
        }
    }
}
