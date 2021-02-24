using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Kaos.Physics
{
    [Flags]
    public enum Decay
    {
        Stable=0,
        Alpha=1,
        BetaPlus=2, BetaMinus=4, Beta2=8,
        ECap1=16, ECap2=32,
        NEmit=64, Gamma=128,
        IC=256, IT=512, Fission=1024
    }

    /// <summary>Variant of an element that differs by neutron number.</summary>
    public class Isotope
    {
        /// <summary>Returns the number of valid decay mode flags.</summary>
        public static int DecayModeCount { get; } = Enum.GetValues (typeof (Decay)).Cast<int>().Max();

        /// <summary>Returns all the possible characters that may be returned by the DecayCodes property.</summary>
        public static string DecayCodes => "apbBeEngTCF";

        public static ReadOnlyCollection<string> DecaySymbols { get; } = new ReadOnlyCollection<string>(new string[]
        { "α", "β+", "β−", "β−β−", "ε", "εε", "n", "γ", "IT", "IC", "SF" });

        /// <summary>Atomic number (proton count).</summary>
        public int Z { get; private set; }

        /// <summary>Mass number (nucleon count).</summary>
        public int A { get; private set; }

        /// <summary>Percent of Earth's total for this isotope. A value of <b>null</b> indicates a synthetic isotope. A zero value indicates trace abundance.</summary>
        public double? Abundance { get; private set; }

        /// <summary>Time to decay by 50%. Stable isotopes are indicated by <b>null</b>.</summary>
        public double? Halflife { get; private set; }

        /// <summary>Ored bitflags of all possible transmutations.</summary>
        public Decay DecayMode { get; private set; }

        /// <summary>Returns index of radioactivity from 0 to 5.</summary>
        /// <remarks>A value of 0 indicates a stable isotope and 5 the most radioactive.</remarks>
        public int StabilityIndex { get; private set; }

        /// <summary>Instantiate a stable isotope.</summary>
        /// <param name="z">Proton count.</param>
        /// <param name="a">Nucleon count.</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        public Isotope (int z, int a, double abundance)
        {
            Z = z;
            A = a;
            Abundance = abundance;
            Halflife = null;
            DecayMode = Decay.Stable;
            StabilityIndex = 0;
        }

        /// <summary>Instantiate an unstable isotope.</summary>
        /// <param name="z">Proton count.</param>
        /// <param name="a">Nucleon count.</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        /// <param name="halflife">Time to decay by half.</param>
        /// <param name="decayMode">Ored bitflags of possible isotope mutations.</param>
        public Isotope (int z, int a, double? abundance, double halflife, Decay decayMode)
        {
            Z = z;
            A = a;
            Abundance = abundance;
            Halflife = halflife;
            DecayMode = decayMode;
            StabilityIndex =
                Halflife >= 2000000.0 * Nuclide.SecondsPerYear ?  1 :
                Halflife >= 800.0 * Nuclide.SecondsPerYear ? 2 :
                Halflife >= 24.0 * 60.00 * 60.0 ? 3 :
                Halflife >= 10.0 * 60.0 ? 4 : 5;
        }

        /// <summary>Returns <b>true</b> if the isotope is not synthetic.</summary>
        public bool IsNatural => Abundance != null;

        /// <summary>Returns the neutron count (A - Z).</summary>
        public int N => A - Z;

        private Origin? _occurrence = null;

        /// <summary>Returns origin in nature, or synthetic.</summary>
        public Origin Occurrence
        {
            get
            {
                if (_occurrence == null)
                    _occurrence = Isotope.CalcOrigin (Nuclide.Table[Z], this);
                return _occurrence.Value;
            }
        }

        /// <summary>Returns index value (0-3) of <see cref="Occurrence"/>.</summary>
        public int OccurrenceIndex => (int) Occurrence;

        /// <summary>Returns letter code of <see cref="Occurrence"/>.</summary>
        public char OccurrenceCode => Nuclide.OccurrenceCodes[(int) Occurrence];

        /// <summary>Returns <b>true</b> if the isotope is not radioactive.</summary>
        public bool IsStable => Halflife == null;

        /// <summary>Returns a string containing a letter for each possible decay mode of the isotope.</summary>
        /// <remarks>The returned value provides concise output in a human-readable form.</remarks>
        public string DecayModeCodes
        {
            get
            {
                var result = string.Empty;
                for (int mask = 1, ix = 0; mask <= DecayModeCount; mask <<= 1)
                {
                    if ((((int) DecayMode) & mask) != 0)
                        result += DecayCodes[ix];
                    ++ix;
                }
                return result;
            }
        }

        private static Origin CalcOrigin (Nuclide nuc, Isotope iso)
        {
            if (iso.Abundance == null)
                return Origin.Synthetic;
            if (iso.Halflife == null || iso.Halflife > Nuclide.PrimordialCutoff)
                return Origin.Primordial;

            if (nuc.Z > 0)
                foreach (var iso2 in Nuclide.Table[nuc.Z - 1].Isotopes)
                    if (iso2.IsNatural && ! iso2.IsStable)
                        foreach (var dx in iso2.GetDecayIndexes())
                        {
                            var tryZ = iso2.Transmute (dx, out int tryA);
                            if (tryZ == nuc.Z && tryA == iso.A)
                               return Origin.Decay;
                        }

            if (nuc.Z < Nuclide.Table.Count - 1)
                foreach (var iso2 in Nuclide.Table[nuc.Z + 1].Isotopes)
                    if (iso2.IsNatural && ! iso2.IsStable)
                        foreach (var dx in iso2.GetDecayIndexes())
                        {
                            var tryZ = iso2.Transmute (dx, out int tryA);
                            if (tryZ == nuc.Z && tryA == iso.A)
                               return Origin.Decay;
                        }

            if (nuc.Z < Nuclide.Table.Count - 2)
                foreach (var iso2 in Nuclide.Table[nuc.Z + 2].Isotopes)
                    if (iso2.IsNatural && ! iso2.IsStable)
                        foreach (var dx in iso2.GetDecayIndexes())
                        {
                            var tryZ = iso2.Transmute (dx, out int tryA);
                            if (tryZ == nuc.Z && tryA == iso.A)
                               return Origin.Decay;
                        }

            // Uranium SF exceptions:
            if (nuc.Z == 43 || nuc.Z == 93)
                return Origin.Decay;

            return Origin.Cosmogenic;
        }


        /// <summary>Provide fixed-width contents isotope.</summary>
        /// <returns>Fixed-column formatted string.</returns>
        public string ToFixedColumns()
        {
            var sb = new StringBuilder();
            var ts = A.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            ts = Abundance == null ? string.Empty : Abundance.Value.ToString ("F3");
            sb.Append (' ', 7 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            sb.Append (OccurrenceCode);
            sb.Append (' ');
            ts = DecayModeCodes;
            sb.Append (ts);
            sb.Append (' ', 5 - ts.Length);
            ts = Halflife.ToString();
            sb.Append (ts);
            //sb.Append (' ', 16 - ts.Length);
            return sb.ToString();
        }

        /// <summary>Provide JSON format contents of the isotope.</summary>
        /// <param name="quote">Typically the quote character or empty string.</param>
        /// <returns>JSON formatted string.</returns>
        public string ToJson (string quote)
        {
            var sb = new StringBuilder();
            sb.Append ('{');
            sb.Append ($"{quote}z{quote}:");
            sb.Append (Z);
            sb.Append ($",{quote}a{quote}:");
            sb.Append (A);
            sb.Append ($",{quote}abundance{quote}:");
            sb.Append (IsNatural ? Abundance.ToString() : "null");
            sb.Append ($",{quote}occurrenceIndex{quote}:");
            sb.Append (OccurrenceIndex);
            if (DecayMode != 0 || Halflife != null)
            {
                sb.Append ($",{quote}stabilityIndex{quote}:");
                sb.Append (StabilityIndex);
                sb.Append ($",{quote}decayFlags{quote}:");
                sb.Append ((int) DecayMode);
                sb.Append ($",{quote}halflife{quote}:");
                sb.Append (Halflife.Value);
            }
            sb.Append ('}');
            return sb.ToString();
        }

        public override string ToString() => "Z,A="+Z+","+A;

        /// <summary>Convert this isotope to another.</summary>
        /// <param name="decayModeBitflag">Bitflag of isotope mutation.</param>
        /// <param name="nucleonCount">Nucleon count (A) of product.</param>
        /// <returns>Proton count (Z) of product.</returns>
        public int Transmute (Decay decayModeBitflag, out int nucleonCount)
        {
            nucleonCount = A;
            if (decayModeBitflag == Decay.Alpha)
            {
                nucleonCount -= 4;
                return Z - 2;
            }
            else if (decayModeBitflag == Decay.ECap2)
                return Z - 2;
            else if (decayModeBitflag == Decay.BetaPlus || decayModeBitflag == Decay.ECap1)
                return Z - 1;
            else if (decayModeBitflag == Decay.BetaMinus)
                return Z + 1;
            else if (decayModeBitflag == Decay.Beta2)
                return Z + 2;
            else if (decayModeBitflag == Decay.NEmit)
                --nucleonCount;
            return Z;
        }

        public int Transmute (int decayIndex, out int nucleonCount)
         => Transmute ((Decay) (1<<decayIndex), out nucleonCount);


        /// <summary>Returns an enumerator that iterates thru the possible decay mode indexes of the isotope.</summary>
        /// <returns>An enumerator that iterates thru the possible indexes.</returns>
        public IEnumerable<int> GetDecayIndexes()
        {
            int mask = 1;
            for (var decayIndex = 0; decayIndex < Isotope.DecayModeCount; ++decayIndex)
            {
                if (((int) DecayMode & mask) != 0)
                    yield return decayIndex;
                mask <<= 1;
            }
        }
    }
}
