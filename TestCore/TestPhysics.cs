﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using Kaos.Physics;

namespace TestPhysics
{
    [TestClass]
    public class Test_Nuclide
    {
        private static string[] expectedASCII = new string[]
        {
            "H . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . He",
            "LiBe. . . . . . . . . . . . . . . . . . . . . . . . B C N O F Ne",
            "NaMg. . . . . . . . . . . . . . . . . . . . . . . . AlSiP S ClAr",
            "K Ca. . . . . . . . . . . . . . ScTiV CrMnFeCoNiCuZnGaGeAsSeBrKr",
            "RbSr. . . . . . . . . . . . . . Y ZrNbMoTcRuRhPdAgCdInSnSbTeI Xe",
            "CsBaLaCePrNdPmSmEuGdTbDyHoErTmYbLuHfTaW ReOsIrPtAuHgTlPbBiPoAtRn",
            "FrRaAcThPaU NpPuAmCmBkCfEsFmMdNoLrRfDbSgBhHsMtDsRgCnNhFlMcLvTsOg"
        };

        private static string[] langWhiteList = new string[]
        { "de", "en", "en-GB", "en-US", "es", "fr", "it", "nl", "pl", "pt", "ru" };

        [TestMethod]
        public void TestNuclide_GetLongTable()
        {
            int ix = 0;
            foreach (string actualASCII in Nuclide.GetLongTable())
            {
                Assert.AreEqual (expectedASCII[ix], actualASCII);
                ++ix;
            }
        }

        [TestMethod]
        public void TestNuclide_CheckGlobals()
        {
            Assert.IsTrue (Nuclide.Table.Count >= 119);
            Assert.IsTrue (Nuclide.MaxNameLengths["en"] > 8);

            int ix = 0;
            int expected = 0;
            foreach (var kv in Nuclide.DecayModeNames)
            {
                foreach (var nm in kv.Value)
                    Assert.IsFalse (nm.Contains ('\''), "nm="+nm);

                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);
            }

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.ThemeNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);

            ix = 0;
            expected = 0;
            foreach (var kv in Nuclide.IsotopesHeadings)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);
        }

        [TestMethod]
        public void TestNuclide_CheckProperties1()
        {
            Assert.IsNull (Nuclide.Table[5][15]);
            Assert.AreEqual (6, Nuclide.Table[5][11].N);
            Assert.AreEqual (32, Nuclide.Table[10].LongColumn);
        }

        [TestMethod]
        public void TestNuclide_CheckProperties2()
        {
            for (int nx = 0; nx < Nuclide.Table.Count; ++nx)
            {
                var nuc = Nuclide.Table[nx];

                Assert.AreEqual (nx, nuc.Z);
                Assert.IsTrue (nuc.Symbol.Length >= 1 && nuc.Symbol.Length <= 3, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.Period >= 1 && nuc.Period <= 7, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.Block == 'f' || nuc.Group >= 1 && nuc.Group <= 18, "Z="+ nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.Block == 's' || nuc.Block == 'p' || nuc.Block == 'd' || nuc.Block == 'f', "Z="+nuc.Z);
                Assert.IsTrue (nuc.Z == 0 || nuc.LongColumn >= 1 && nuc.LongColumn <= 32, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Known >= 0 && nuc.Known <= 2030, "Z="+nuc.Z);
                Assert.IsTrue (nuc.KnownIndex >= 0 && nuc.KnownIndex <= 6, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Naming.Length > 0, "Z="+nuc.Z);
                Assert.IsTrue (nuc.Known == 0 || nuc.Credit.Length > 0, "Z="+nuc.Z);
                Assert.IsTrue (nuc.ToString().Length > 0);
                Assert.IsTrue (nuc.ToFixedWidthString().Length > 0);
                Assert.IsTrue (nuc.ToJsonString("").Length > 0);
            }
        }

        [TestMethod]
        public void TestNuclide_CheckLangs()
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
        public void TestNuclide_Category()
        {
            Assert.AreEqual (Category.NobleGas, Nuclide.Table[2].Category);
            Assert.AreEqual (1, Nuclide.Table[20].CategoryIndex);
            Assert.AreEqual ("NobleGas", Nuclide.Table[18].CategoryAbbr);

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
        }

        [TestMethod]
        public void TestNuclide_Life()
        {
            Assert.AreEqual ("BT", Nuclide.Table[5].LifeCode);
            Assert.AreEqual (Nutrition.BulkEssential, Nuclide.Table[1].Life);
            Assert.AreEqual (1, Nuclide.Table[1].LifeIndex);
            Assert.AreEqual ("A", Nuclide.LifeCodes[Nuclide.Table[82].LifeIndex]);

            int ix = 0;
            int expected = 0;
            foreach (var kv in Nuclide.LifeDescriptions)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);
        }

        [TestMethod]
        public void TestNuclide_Occurrence()
        {
            Assert.AreEqual ('S', Nuclide.Table[118].OccurrenceCode);
            Assert.AreEqual ('D', Nuclide.Table[43].OccurrenceCode);
            Assert.AreEqual ('P', Nuclide.Table[2].OccurrenceCode);

            Assert.AreEqual (Origin.Decay, Nuclide.Table[86].Occurrence);
            Assert.AreEqual (2, Nuclide.Table[93].OccurrenceIndex);
            Assert.AreEqual ('S', Nuclide.OccurrenceCodes[Nuclide.Table[118].OccurrenceIndex]);

            int ix = 0;
            int expected = 0;
            foreach (var kv in Nuclide.OccurrenceNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);
        }

        [TestMethod]
        public void TestNuclide_Stability()
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
        public void TestNuclide_State()
        {
            Assert.AreEqual ("Gas", Nuclide.StateNames["en"][Nuclide.Table[18].StateAt0CIndex]);
            Assert.AreEqual ('L', Nuclide.Table[80].StateAt0CCode);
            Assert.AreEqual (State.Solid, Nuclide.Table[15].StateAt0C);
            Assert.AreEqual (State.Liquid, Nuclide.Table[30].GetState(500+273.15));
            Assert.AreEqual ('S', Nuclide.StateCodes[(int) Nuclide.Table[6].GetState(3000)]);

            foreach (Nuclide nuc in Nuclide.Table)
                Assert.IsTrue (nuc.Melt == null || nuc.Boil == null || nuc.Melt <= nuc.Boil, "Z="+nuc.Z);

            int ix = 0;
            int expected = 0;
            foreach (var kv in Nuclide.StateNames)
                if (ix++ == 0)
                    expected = kv.Value.Length;
                else
                    Assert.AreEqual (expected, kv.Value.Length, "ix="+ix);
        }

        [TestMethod]
        public void TestNuclide_Weight()
        {
            Assert.AreEqual (12.011, Nuclide.Table[6].Weight);

            foreach (Nuclide nuc in Nuclide.Table)
                if (nuc.Occurrence < Origin.Decay)
                {
                    var expected = Math.Truncate (nuc.Weight);
                    Assert.AreEqual (expected, nuc.Weight, "Z="+nuc.Z);
                    double? maxHL = null;
                    int maxA = 0;
                    foreach (Isotope iso in nuc.Isotopes)
                    {
                        if (maxHL == null || maxHL < iso.HalflifeSeconds)
                        { maxHL = iso.HalflifeSeconds; maxA = iso.A; }
                    }
                    Assert.AreEqual (nuc.Weight, maxA, "Z="+nuc.Z);
                }
        }

        [TestMethod]
        public void TestNuclide_GetBySymbol()
        {
            Nuclide actual1 = Nuclide.Table.FirstOrDefault (x => x.Symbol == "W");
            Assert.AreEqual (74, actual1.Z); 

            Nuclide actual2 = Nuclide.GetBySymbol ("W");
            Assert.AreEqual (74, actual2.Z);

            Nuclide actual3 = Nuclide.GetBySymbol ("Xy");
            Assert.IsNull (actual3);
        }

        [TestMethod]
        public void TestNuclide_CheckAll()
        {
            int expected = 0;

            foreach (Nuclide nuc in Nuclide.Table)
                foreach (Isotope iso in nuc.Isotopes)
                {
                    Assert.AreEqual (nuc.Z, iso.Z);
                    ++expected;
                }

            Assert.AreEqual (expected, Nuclide.GetIsotopes().Count() + 1);
        }

        [TestMethod]
        public void TestNuclide_CheckMethods()
        {
            Assert.AreEqual (Nuclide.Table.Count - 1, Nuclide.GetElements().Count());
            Assert.IsTrue (Nuclide.GetLongTable().Count() >= 7);
            Assert.AreEqual ("Kalium", Nuclide.Table[19].GetName("de"));
            Assert.AreEqual ("Yodo", Nuclide.Table[53].GetName (new CultureInfo ("es")));
            Assert.AreEqual ("Sulfur", Nuclide.Table[16].GetName (new CultureInfo ("en")));
            Assert.AreEqual ("Sulphur", Nuclide.Table[16].GetName (new CultureInfo ("en-GB")));
            Assert.AreEqual ("Caesium", Nuclide.Table[55].GetName (new CultureInfo ("en")));
            Assert.AreEqual ("Cesium", Nuclide.Table[55].GetName (new CultureInfo ("en-US")));
        }
    }

    [TestClass]
    public class Test_Isotope
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashIsotope_Ctor5_ArgumentException()
        {
            var _ = new Isotope (2, 5, 0.0, Decay.Alpha, 1.2, '!');
        }

        [TestMethod]
        public void TestIsotope_Ctor()
        {
            var iso1 = new Isotope (2, 5, 0.0);
            Assert.AreEqual (2, iso1.Z);
            Assert.AreEqual (5, iso1.A);
            Assert.AreEqual (Decay.None, iso1.DecayMode);
            Assert.AreEqual (0, iso1.StabilityIndex);
            Assert.IsTrue (iso1.IsStable);

            var iso2 = new Isotope (z: 118, a: 300, abundance: null, decayMode: Decay.Alpha, halflife: 0.01, timeUnit: 's');
            Assert.AreEqual (300, iso2.A);
            Assert.AreEqual (Decay.Alpha, iso2.DecayMode);
            Assert.AreEqual (5, iso2.StabilityIndex);
        }

        [TestMethod]
        public void TestIsotope_CheckGlobals()
        {
            Assert.AreEqual (Nuclide.DecayModeCount, Nuclide.DecayModeCodes.Length);
            Assert.AreEqual (Nuclide.DecayModeCount, Nuclide.DecayModeSymbols.Count);
        }

        [TestMethod]
        public void TestIsotope_CheckProperties1()
        {
            Assert.AreEqual (Origin.Synthetic, Nuclide.Table[117].Isotopes[0].Occurrence);
            Assert.AreEqual (Origin.Cosmogenic, Nuclide.Table[1].Isotopes[2].Occurrence);
            Assert.AreEqual (Origin.Decay, Nuclide.Table[84][210].Occurrence);
            Assert.AreEqual (Origin.Primordial, Nuclide.Table[5].Isotopes[0].Occurrence);

            Assert.AreEqual (3, Nuclide.Table[92][235].OccurrenceIndex);
            Assert.AreEqual ('C', Nuclide.Table[94][244].OccurrenceCode);

            Assert.AreEqual (82, Nuclide.Table[82][210].Nuclide.Z);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void TestIsotope_Nuclide_ArgumentOutOfRange()
        {
            var badIsotope = new Isotope (200, 400, 0.0);
            Nuclide _ = badIsotope.Nuclide;
        }

        [TestMethod]
        public void TestIsotope_CheckProperties2()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                Assert.IsTrue (nuc.Isotopes.Count > 0);
                foreach (Isotope iso in nuc.Isotopes)
                {
                    Assert.IsTrue (iso.A >= nuc.Z);
                    Assert.AreEqual (iso.IsStable, iso.DecayModeCodes.Length == 0);
                    Assert.AreEqual (iso.IsStable, iso.DecayMode == Decay.None);
                    Assert.AreEqual (iso.IsStable, iso.StabilityIndex == 0);
                    Assert.IsTrue (iso.IsStable || iso.Halflife > 0.0);
                    Assert.IsTrue (iso.ToString().Length > 0);
                    Assert.IsTrue (iso.ToFixedWidthString().Length > 0);
                    Assert.IsTrue (iso.ToFixedWidthString (new CultureInfo ("it")).Length > 0);
                    Assert.IsTrue (iso.ToJsonString("").Length > 0);
                    Assert.IsTrue (iso.IsStable || "ydhmstin".Contains (iso.TimeUnit));
                }
            }
        }

        [TestMethod]
        public void TestIsotope_Abundance()
        {
            foreach (Nuclide nuc in Nuclide.Table)
            {
                double total = 0.0;
                foreach (Isotope iso in nuc.Isotopes)
                {
                    Assert.IsTrue (iso.Abundance == null || (iso.Abundance >= 0 && iso.Abundance <= 100));
                    Assert.AreEqual (iso.IsNatural, iso.Abundance != null);
                    if (iso.Abundance != null)
                        total += iso.Abundance.Value;
                }

                Assert.IsTrue (total < 0.05 || (total > 99.5 && total < 101.0), "Z="+nuc.Z+", total="+total);
            }
        }

        [TestMethod]
        public void TestIsotope_GetDecayIndexes()
        {
            var actualBe9 = 0;
            foreach (int decayIndex in Nuclide.Table[4][9].GetDecayIndexes())
                ++actualBe9;
            Assert.AreEqual (0, actualBe9);

            var actualK40 = 0;
            foreach (int decayIndex in Nuclide.Table[19][40].GetDecayIndexes())
                ++actualK40;
            Assert.AreEqual (3, actualK40);
        }

        [TestMethod]
        public void TestNuclide_HalflifeString()
        {
            var h3 = Nuclide.Table[1][3];
            var be7 = Nuclide.Table[4][7];
            var c11 = Nuclide.Table[6][11];
            var u235 = Nuclide.Table[92][235];
            var na24 = Nuclide.Table[11][24];
            var ar41 = Nuclide.Table[18][41];

            Assert.AreEqual ("12.32 y", h3.HalflifeText);
            Assert.AreEqual ("12.32 y", h3.GetHalflifeText (new CultureInfo ("en-US")));
            Assert.AreEqual ("704000000 y", u235.GetHalflifeText (new CultureInfo ("en-US")));

            Assert.AreEqual ("164.3 ns", Nuclide.Table[84][214].HalflifeText);
            Assert.AreEqual ("32.3 ms", Nuclide.Table[85][217].HalflifeText);
            Assert.AreEqual ("700 μs", Nuclide.Table[118][294].HalflifeText);

            Assert.AreEqual ("109,34 ans", ar41.GetHalflifeText (new CultureInfo ("fr")));
            Assert.AreEqual ("20 min", c11.GetHalflifeText (new CultureInfo ("es")));
            Assert.AreEqual ("14,96 ore", na24.GetHalflifeText (new CultureInfo ("it")));
            Assert.AreEqual ("53,12 j", be7.GetHalflifeText (new CultureInfo ("fr")));
            Assert.AreEqual ("12,32 a", h3.GetHalflifeText (new CultureInfo ("de-DE")));
        }

        [TestMethod]
        public void TestIsotope_Transmute()
        {
            int z1 = 1, a1 = 4;
            var iso1 = new Isotope (z1, a1, null, Decay.NEmit, 0.001, 's');
            int z1p = iso1.Transmute (Decay.NEmit, out int a1p);
            Assert.AreEqual (a1 - 1, a1p);
            Assert.AreEqual (z1, z1p);

            int z2 = 118, a2 = 300;
            var iso2 = new Isotope (z: z2, a: a2, abundance: null, decayMode: Decay.Alpha, halflife: 0.02, timeUnit: 's');
            int z2p = iso2.Transmute (Decay.Alpha, out int a2p);
            Assert.AreEqual (a2 - 4, a2p);
            Assert.AreEqual (z2 - 2, z2p);

            int z3 = 3, a3 = 8;
            var iso3 = new Isotope (z3, a3, null, Decay.BetaMinus, 0.03, 's');
            int z3p = iso3.Transmute (Decay.BetaMinus, out int a3p);
            Assert.AreEqual (a3, a3p);
            Assert.AreEqual (z3 + 1, z3p);

            int z4 = 4, a4 = 11;
            var iso4 = new Isotope (z4, a4, null, Decay.BetaPlus, 0.04, 's');
            int z4p = iso4.Transmute (Decay.BetaPlus, out int a4p);
            Assert.AreEqual (a4, a4p);
            Assert.AreEqual (z4 - 1, z4p);

            int z5 = 6, a5 = 15;
            var iso5 = new Isotope (z5, a5, null, Decay.Beta2, 0.05, 's');
            int z5p = iso5.Transmute (Decay.Beta2, out int a5p);
            Assert.AreEqual (a5, a5p);
            Assert.AreEqual (z5 + 2, z5p);

            int z6 = 6, a6 = 15;
            var iso6 = new Isotope (z6, a6, null, Decay.ECap1, 0.06, 's');
            int z6p = iso6.Transmute (Decay.ECap1, out int a6p);
            Assert.AreEqual (a6, a6p);
            Assert.AreEqual (z6 - 1, z6p);

            int z7 = 7, a7 = 17;
            var iso7 = new Isotope (z7, a7, null, Decay.ECap2, 0.07, 's');
            int z7p = iso7.Transmute (Decay.ECap2, out int a7p);
            Assert.AreEqual (a7, a7p);
            Assert.AreEqual (z7 - 2, z7p);

            int z8 = 8, a8 = 18;
            var iso8 = new Isotope (z8, a8, null, Decay.Gamma, 0.08, 's');
            int z8p = iso8.Transmute (Decay.Gamma, out int a8p);
            Assert.AreEqual (a8, a8p);
            Assert.AreEqual (z8, z8p);
        }
    }
}
