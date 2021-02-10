using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Kaos.Physics;

namespace TestCore
{
    [TestClass]
    public class Test_Nuclide
    {
        private static string[] langWhiteList = new string[] { "de", "en", "en-GB", "en-US", "es", "fr", "it", "nl", "pl", "pt", "ru" };

        [TestMethod]
        public void CheckGlobals()
        {
            Assert.IsTrue (Nuclide.Table.Count >= 119);
        }

        [TestMethod]
        public void CheckProperties()
        {
            for (int nx = 0; nx < Nuclide.Table.Count; ++nx)
            {
                var nuc = Nuclide.Table[nx];

                Assert.AreEqual (nx, nuc.Z);
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
        public void CheckTemperatures()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Melt == null || nuc.Boil == null || nuc.Melt <= nuc.Boil, "Z="+nuc.Z);
            }
        }
    }

    [TestClass]
    public class Test_Isotope
    {
        [TestMethod]
        public void CheckGlobals()
        {
            Assert.IsTrue (Isotope.DecayChars.Length > 0);
            Assert.AreEqual (Isotope.DecayChars.Length, Isotope.DecayCodes.Count);
        }

        [TestMethod]
        public void CheckProperties()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Isotopes.Count > 0);
                foreach (Isotope iso in nuc.Isotopes)
                {
                    Assert.IsTrue (iso.A >= nuc.Z);
                    Assert.IsTrue (iso.Halflife == null || iso.Halflife > 0);
                    Assert.AreEqual (iso.DecayMode != Decay.Stable, iso.Halflife != null);
                    Assert.AreEqual (iso.DecayMode != Decay.Stable, iso.DecayCode.Length > 0);
                    Assert.AreEqual (iso.DecayMode != Decay.Stable, ! iso.IsStable);
                }
            }
        }

        [TestMethod]
        public void CheckAbundance()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                double total = 0.0;
                foreach (Isotope iso in nuc.Isotopes)
                {
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
