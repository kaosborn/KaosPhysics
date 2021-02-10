using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Kaos.Physics
{
    [Flags]
    public enum Decay
    {
        Stable=0,
        Alpha=1,
        BetaPlus=2, BetaMinus=4, Beta2=8,
        ElectronCap1=16, ElectronCap2=32,
        NeutronEmit=64,
        Gamma=128,
        IC=256, IT=512,
        Fission=1024
    }

    /// <summary>Variant of an element that differs by neutron number.</summary>
    public class Isotope
    {
        private static readonly string[] _decayCodes = new string[] { "α", "β+", "β−", "β−β−", "ε", "εε", "n", "γ", "IT", "IC", "SF" };

        /// <summary>
        /// Returns all the possible characters that the DecayCodes property may consist of.
        /// </summary>
        public static string DecayChars => "apbBeEngTCF";
        public static ReadOnlyCollection<string> DecayCodes { get; } = new ReadOnlyCollection<string>(_decayCodes);

        /// <summary>Nucleon count.</summary>
        public int A { get; private set; }

        /// <summary>Percent of Earth's total for this isotope. A null value indicates a synthetic isotope. A zero value indicates trace abundance.</summary>
        public double? Abundance { get; private set; }

        /// <summary>Time to decay by half. A null value indicates a stable isotope.</summary>
        public double? Halflife { get; private set; }

        public Decay DecayMode { get; private set; }

        /// <summary>Instantiate a stable isotope.</summary>
        /// <param name="a">Nucleon count</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        public Isotope (int a, double abundance)
        {
            A = a;
            Abundance = abundance;
            Halflife = null;
            DecayMode = Decay.Stable;
        }

        /// <summary>Instantiate a stable isotope.</summary>
        /// <param name="a">Nucleon count</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        /// <param name="halflife">Time to decay by half.</param>
        /// <param name="decayMode">Or'ed list of isotope mutations.</param>
        public Isotope (int a, double? abundance, double halflife, Decay decayMode)
        {
            A = a;
            Abundance = abundance;
            Halflife = halflife;
            DecayMode = decayMode;
        }

        /// <summary>Returns true if the isotope is not sythetic.</summary>
        public bool IsNatural => Abundance != null;

        /// <summary>Returns true if the isotope is not radioactive.</summary>
        public bool IsStable => Halflife == null;

        /// <summary>
        /// Returns a string containing a character for each decay mode of the isotope.
        /// This string provides a consise method of output in a human readable form.
        /// </summary>
        public string DecayCode
        {
            get
            {
                var result = string.Empty;
                for (int mask = 1, ix = 0; mask <= 1024; mask <<= 1)
                {
                    if ((((int) DecayMode) & mask) != 0)
                        result += DecayChars[ix];
                    ++ix;
                }
                return result;
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
            ts = DecayCode;
            sb.Append (ts);
            sb.Append (' ', 5 - ts.Length);
            ts = Halflife.ToString();
            sb.Append (ts);
            //sb.Append (' ', 16 - ts.Length);
            return sb.ToString();
        }

        /// <summary>Provide JSON format contents of isotope.</summary>
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
