using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Kaos.Physics;

namespace TestPhysics
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
            Assert.IsNull (Nuclide.Table[5][15]);
            Assert.AreEqual (6, Nuclide.Table[5][11].N);

            Assert.AreEqual ('S', Nuclide.Table[118].OccurrenceCode);
            Assert.AreEqual ('D', Nuclide.Table[43].OccurrenceCode);
            Assert.AreEqual ('P', Nuclide.Table[2].OccurrenceCode);

            Assert.AreEqual (Origin.Decay, Nuclide.Table[86].Occurrence);
            Assert.AreEqual (2, Nuclide.Table[93].OccurrenceIndex);

            Assert.AreEqual (Category.NobleGas, Nuclide.Table[2].Category);
            Assert.AreEqual (1, Nuclide.Table[20].CategoryIndex);
            Assert.AreEqual (12.011, Nuclide.Table[6].Weight);
            Assert.AreEqual ("BT", Nuclide.Table[5].LifeCode);
            Assert.AreEqual (Nutrition.BulkEssential, Nuclide.Table[1].Life);
            Assert.AreEqual (1, Nuclide.Table[1].LifeIndex);

            Assert.AreEqual (32, Nuclide.Table[10].LongColumn);
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
        public void CheckStability()
        {
            Assert.AreEqual (1, Nuclide.Table[43].StabilityIndex);
            Assert.AreEqual (10, Nuclide.Table[50].StableCount);

            int ix = 0, expected = 0;
            foreach (var kv in Nuclide.StabilityDescriptions)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            var enStabilityCount = Nuclide.StabilityDescriptions["en"].Length;
            Assert.AreEqual (enStabilityCount - 1, Nuclide.StabilityIndexMax);
        }

        [TestMethod]
        public void CheckTemperatures()
        {
            Assert.AreEqual ("Gas", Nuclide.StateNames["en"][Nuclide.Table[18].StateAt0CIndex]);
            Assert.AreEqual ('L', Nuclide.Table[80].StateAt0CCode);

            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Melt == null || nuc.Boil == null || nuc.Melt <= nuc.Boil, "Z="+nuc.Z);
            }
        }

        [TestMethod]
        public void CheckWeight()
        {
            foreach (Nuclide nuc in Nuclide.Table)
                if (nuc.Occurrence < Origin.Decay)
                {
                    var expected = Math.Truncate (nuc.Weight);
                    Assert.AreEqual (expected, nuc.Weight, "Z="+nuc.Z);
                    double? maxHL = null;
                    int maxA = 0;
                    foreach (Isotope iso in nuc.Isotopes)
                    {
                        if (maxHL == null || maxHL < iso.Halflife)
                        { maxHL = iso.Halflife; maxA = iso.A; }
                    }
                    Assert.AreEqual (nuc.Weight, maxA, "Z="+nuc.Z);
                }
        }

        [TestMethod]
        public void CheckIsotopes()
        {
            foreach (Nuclide nuc in Nuclide.Table)
                foreach (Isotope iso in nuc.Isotopes)
                    Assert.AreEqual (nuc.Z, iso.Z);
        }
    }

    [TestClass]
    public class Test_Isotope
    {
        [TestMethod]
        public void TestCtor()
        {
            var iso1 = new Isotope (2, 5, 0.0);
            Assert.AreEqual (2, iso1.Z);
            Assert.AreEqual (5, iso1.A);
            Assert.AreEqual (null, iso1.Halflife);
            Assert.AreEqual (Decay.Stable, iso1.DecayMode);
            Assert.AreEqual (0, iso1.StabilityIndex);

            var iso2 = new Isotope (z: 118, a: 300, abundance: null, halflife: 0.01, decayMode: Decay.Alpha);
            Assert.AreEqual (300, iso2.A);
            Assert.AreEqual (Decay.Alpha, iso2.DecayMode);
            Assert.AreEqual (5, iso2.StabilityIndex);

            int productZ = iso2.Transmute (Decay.Alpha, out int productA);
            Assert.AreEqual (296, productA);
            Assert.AreEqual (116, productZ);
        }

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
            Assert.AreEqual (Origin.Cosmogenic, Nuclide.Table[1].Isotopes[2].Occurrence);
            Assert.AreEqual (Origin.Decay, Nuclide.Table[84][210].Occurrence);
            Assert.AreEqual (Origin.Primordial, Nuclide.Table[5].Isotopes[0].Occurrence);

            Assert.AreEqual (3, Nuclide.Table[92][235].OccurrenceIndex);
            Assert.AreEqual ('C', Nuclide.Table[94][244].OccurrenceCode);
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
