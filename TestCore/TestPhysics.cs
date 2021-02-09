using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Kaos.Physics;

namespace TestCore
{
    [TestClass]
    public class TestPhysics
    {
        private static string[] langWhiteList = new string[] { "de", "en", "en-GB", "en-US", "es", "fr", "it", "nl", "pl", "pt", "ru" };

        [TestMethod]
        public void TestElementProps()
        {
            for (int ex = 0; ex < Nuclide.Table.Count; ++ex)
            {
                var nuc = Nuclide.Table[ex];
                Assert.AreEqual (ex, nuc.Z);
                Assert.IsTrue (nuc.Symbol.Length >= 1 && nuc.Symbol.Length <= 3, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Group >= 0 && nuc.Group <= 18, "Z="+ nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.Period >= 1 && nuc.Period <= 7, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.Block == 's' || nuc.Block == 'p' || nuc.Block == 'd' || nuc.Block == 'f', "Z="+nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.LongColumn >= 1 && nuc.LongColumn <= 32, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Known >= 0 && nuc.Known <= 2030, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Naming.Length > 0, "Z="+nuc.Z);
            }
        }

        [TestMethod]
        public void CheckLangs()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Name.Length >= 1, "Z="+nuc.Z);

                foreach (var kv in nuc.NameMap)
                {
                    Assert.IsTrue (langWhiteList.Any (x => x == kv.Key), "Z="+nuc.Z+", lang="+kv.Key);
                    Assert.IsTrue (kv.Value.Length > 0 && kv.Value != nuc.Name, "Z="+nuc.Z);
                }
            }
        }

        [TestMethod]
        public void CheckTemps()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Melt == null || nuc.Boil == null || nuc.Melt <= nuc.Boil, "Z="+nuc.Z);
            }
        }
    }

    [TestClass]
    public class TestIsotope
    {
        [TestMethod]
        public void CheckIsotopes()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Isotopes.Count > 0);

                double total = 0.0;

                foreach (Isotope iso in nuc.Isotopes)
                {
                    Assert.IsTrue (iso.Halflife == null || iso.Halflife > 0);
                    Assert.IsTrue ((iso.Halflife == null) == (iso.DecayMode == Decay.Stable));
                    Assert.IsTrue (iso.A >= nuc.Z);
                    Assert.IsTrue (iso.Abundance == null || (iso.Abundance >= 0 && iso.Abundance <= 100));
                    Assert.IsTrue ((iso.Abundance != null) == iso.IsNatural);

                    if (iso.Abundance != null)
                        total += iso.Abundance.Value;
                }

                Assert.IsTrue (total < 0.05 || (total > 99.5 && total < 101.0), "Z="+nuc.Z+", total="+total);
            }
        }
    }
}
