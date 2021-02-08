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

    public class Isotope
    {
        private static readonly string[] _decayCodes = new string[] { "Stable", "α", "β+", "β−", "β−β−", "ε", "εε", "n", "γ", "IT", "IC", "SF" };

        public static string DecayChars => "apbBeEngTCF";
        public static ReadOnlyCollection<string> DecayCodes { get; } = new ReadOnlyCollection<string>(_decayCodes);

        public int A { get; private set; }
        public double? Abundance { get; private set; }
        public double? Halflife { get; private set; }
        public Decay DecayMode { get; private set; }

        public Isotope (int a, double abundance)
        {
            A = a;
            Abundance = abundance;
            Halflife = null;
            DecayMode = Decay.Stable;
        }

        public Isotope (int a, double? abundance, double halflife, Decay decayMode)
        {
            A = a;
            Abundance = abundance;
            Halflife = halflife;
            DecayMode = decayMode; 
        }

        public bool IsNatural => Abundance != null;
        public bool IsStable => Halflife == null;
        public int DecayBits => (int) DecayMode;

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

        public string FixedText()
        {
            var sb = new StringBuilder();
            var ts = A.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            ts = Abundance == null ? string.Empty : Abundance.Value.ToString ("F3");
            sb.Append(' ', 7 - ts.Length);
            sb.Append (ts);
            sb.Append(' ');
            ts = DecayCode;
            sb.Append (ts);
            sb.Append(' ', 5 - ts.Length);
            ts = Halflife.ToString();
            sb.Append (ts);
            //sb.Append (' ', 16 - ts.Length);
            return sb.ToString();
        }
    }
}
