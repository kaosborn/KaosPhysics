using System;
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

        /// <summary>Mass number (nucleon count).</summary>
        public int A { get; private set; }

        /// <summary>Percent of Earth's total for this isotope. A null value indicates a synthetic isotope. A zero value indicates trace abundance.</summary>
        public double? Abundance { get; private set; }

        /// <summary>Time to decay by 50%. Stable isotopes are indicated by a null value.</summary>
        public double? Halflife { get; private set; }

        /// <summary>Or'ed bitflags of all possible transmutations.</summary>
        public Decay DecayMode { get; private set; }

        /// <summary>Instantiate a stable isotope.</summary>
        /// <param name="a">Nucleon count.</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        public Isotope (int a, double abundance)
        {
            A = a;
            Abundance = abundance;
            Halflife = null;
            DecayMode = Decay.Stable;
        }

        /// <summary>Instantiate a stable isotope.</summary>
        /// <param name="a">Nucleon count.</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        /// <param name="halflife">Time to decay by half.</param>
        /// <param name="decayMode">Ored bitflags of isotope mutations.</param>
        public Isotope (int a, double? abundance, double halflife, Decay decayMode)
        {
            A = a;
            Abundance = abundance;
            Halflife = halflife;
            DecayMode = decayMode;
        }

        /// <summary>Returns <b>true</b> if the isotope is not synthetic.</summary>
        public bool IsNatural => Abundance != null;

        /// <summary>Returns abundance in nature, or synthetic.</summary>
        public Origin Occurrence => Abundance == null ? Origin.Synthetic : Halflife < Nuclide.PrimordialCutoff ? Origin.Decay : Origin.Primordial;

        /// <summary>Returns index value for Occurrence.</summary>
        public int OccurrenceIndex => (int) Occurrence;

        /// <summary>Returns letter code for Occurrence.</summary>
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

        /// <summary>Returns index of radioactivity from 0 to 5.</summary>
        /// <remarks>A value of 0 indicates a stable isotope and 5 the least stable.</remarks>
        public int StabilityIndex
        {
            get
            {
                if (Halflife == null)
                    return 0;
                if (Halflife >= 2000000.0*31556952.0)
                    return 1;
                if (Halflife >= 800.0*31556952.0)
                    return 2;
                if (Halflife >= 86400.0)
                    return 3;
                if (Halflife >= 600.0)
                    return 4;
                return 5;
            }
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
        /// <returns>JSON formatted string.</returns>
        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append ('[');
            sb.Append (A);
            sb.Append (',');
            sb.Append (IsNatural ? Abundance.ToString() : "null");
            if (DecayMode != 0 || Halflife != null)
            {
                sb.Append (',');
                sb.Append ((int) DecayMode);
                sb.Append (',');
                sb.Append (Halflife.Value);
            }
            sb.Append (']');
            return sb.ToString();
        }
    }
}
