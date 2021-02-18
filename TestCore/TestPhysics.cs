using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Kaos.Physics;

namespace TestCore
{
    [TestClass]
    public class Test_Nuclide
    {
        private static string[] langWhiteList = new string[]
        { "de", "en", "en-GB", "en-US", "es", "fr", "it", "nl", "pl", "pt", "ru" };

        private static string[] expectedTable = new string[]
        {
            "H . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . He",
            "LiBe. . . . . . . . . . . . . . . . . . . . . . . . B C N O F Ne",
            "NaMg. . . . . . . . . . . . . . . . . . . . . . . . AlSiP S ClAr",
            "K Ca. . . . . . . . . . . . . . ScTiV CrMnFeCoNiCuZnGaGeAsSeBrKr",
            "RbSr. . . . . . . . . . . . . . Y ZrNbMoTcRuRhPdAgCdInSnSbTeI Xe",
            "CsBaLaCePrNdPmSmEuGdTbDyHoErTmYbLuHfTaW ReOsIrPtAuHgTlPbBiPoAtRn",
            "FrRaAcThPaU NpPuAmCmBkCfEsFmMdNoLrRfDbSgBhHsMtDsRgCnNhFlMcLvTsOg"
        };

        [TestMethod]
        public void CheckTable()
        {
            int ix = 0;
            foreach (var lx in Nuclide.GetLongTable())
            {
                Assert.AreEqual (expectedTable[ix], lx);
                ++ix;
            }
        }

        [TestMethod]
        public void CheckGlobals()
        {
            Assert.IsTrue (Nuclide.Table.Count >= 119);

            int ix = 0;
            int expected = 0;
            foreach (var kv in Nuclide.CategoryNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.CategoryGroupNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.LifeDescriptions)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.OccurrenceNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.StabilityDescriptions)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.StateNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.ThemeNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);
        }

        [TestMethod]
        public void CheckProperties()
        {
            Assert.AreEqual ('P', Nuclide.Table[2].OccurrenceCode);
            Assert.AreEqual ('D', Nuclide.Table[43].OccurrenceCode);
            Assert.AreEqual ('S', Nuclide.Table[118].OccurrenceCode);
        }

        [TestMethod]
        public void CheckAllProperties()
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
            Assert.AreEqual ("Gas", Nuclide.StateNames["en"][Nuclide.Table[2].Atm0StateIndex]);

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
            Assert.IsTrue (Isotope.DecayCodes.Length > 0);
            Assert.AreEqual (Isotope.DecayCodes.Length, Isotope.DecaySymbols.Count);
        }

        [TestMethod]
        public void CheckProperties()
        {
            Assert.AreEqual (Origin.Synthetic, Nuclide.Table[117].Isotopes[0].Occurrence);
            Assert.AreEqual (Origin.Decay, Nuclide.Table[1].Isotopes[2].Occurrence);
            Assert.AreEqual (Origin.Primordial, Nuclide.Table[5].Isotopes[0].Occurrence);

            Assert.AreEqual (Origin.Primordial, Nuclide.Table[92][235].Occurrence);
            Assert.AreEqual (Origin.Decay, Nuclide.Table[94][244].Occurrence);
        }

        [TestMethod]
        public void CheckAllProperties()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Isotopes.Count > 0);
                foreach (Isotope iso in nuc.Isotopes)
                {
                    Assert.IsTrue (iso.A >= nuc.Z);
                    Assert.IsTrue (iso.Halflife == null || iso.Halflife > 0);
                    Assert.AreEqual (iso.DecayMode != Decay.Stable, iso.Halflife != null);
                    Assert.AreEqual (iso.DecayMode != Decay.Stable, iso.DecayModeCodes.Length > 0);
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
