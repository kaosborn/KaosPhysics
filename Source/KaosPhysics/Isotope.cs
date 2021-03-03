using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Kaos.Physics
{
    /// <summary>Variant of an element that differs by neutron count.</summary>
    /// <remarks>
    /// <para>
    /// Use the <see cref="Z"/>, <see cref="A"/>, and <see cref="N"/> properties for their respective nucleon counts.
    /// The <see cref="Z"/> property may serve as an indexer into the <see cref="Nuclide.Table"/> collection.
    /// </para>
    /// <para>
    /// Use the <see cref="Abundance"/> property for the natural abundance (as a per cent) of the isotope.
    /// A <b>null</b> value indicates that the isotope has been created synthetically but has not been detected in nature.
    ///
    /// Use the <see cref="Occurrence"/> property for the natural origin of the isotope.
    /// This value is calculated from the properties of the other nuclides in the nuclides table.
    /// </para>
    /// <para>
    /// Use the <see cref="IsNatural"/> and <see cref="IsStable"/> properties
    /// to determine if the isotope is not synthetic and not radioactive.
    ///
    /// Use the <see cref="Halflife"/> property for the decay time in seconds.
    /// A <b>null</b> value indicates a stable isotope.
    ///
    /// Use the <see cref="StabilityIndex"/> property for the scale of radioactivity of the isotope.
    /// </para>
    /// <para>
    /// Use the <see cref="GetDecayIndexes"/> method to iterate thru all the possible decay modes of the isotope.
    /// </para>
    /// <para>
    /// Use the <see cref="ToFixedWidthString"/> method to get a string representation of the nuclide
    /// with narrow, fixed-width columns. Supply a language code for localized nuclide names.
    /// </para>
    /// </remarks>
    public class Isotope
    {
        /// <summary>Create a stable isotope.</summary>
        /// <param name="z">Proton count.</param>
        /// <param name="a">Nucleon count.</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes.</param>
        public Isotope (int z, int a, double abundance)
        {
            Z = z;
            A = a;
            Abundance = abundance;
            Halflife = null;
            DecayMode = Decay.None;
            StabilityIndex = 0;
        }

        /// <summary>Create a radioactive isotope.</summary>
        /// <param name="z">Proton count.</param>
        /// <param name="a">Nucleon count.</param>
        /// <param name="abundance">Percentage of the isotope to all the element's isotopes or <b>null</b> for synthetics.</param>
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
                Halflife >= 24.0 * 60.0 * 60.0 ? 3 :
                Halflife >= 10.0 * 60.0 ? 4 : 5;
        }

        /// <summary>Atomic number (proton count) of this isotope.</summary>
        public int Z { get; private set; }

        /// <summary>Mass number (nucleon count) of this isotope.</summary>
        public int A { get; private set; }

        /// <summary>Neutron count (<em>A</em>-<em>Z</em>) of this isotope.</summary>
        public int N => A - Z;

        /// <summary>Percent of Earth's total for this isotope.</summary>
        /// <remarks>A <b>null</b> value indicates a synthetic isotope. A 0.0 value indicates trace abundance.</remarks>
        public double? Abundance { get; private set; }

        /// <summary>Ored bitflags of all possible transmutations.</summary>
        public Decay DecayMode { get; private set; }

        /// <summary>Time to decay mass by 50% in seconds. Stable isotopes are indicated by <b>null</b>.</summary>
        /// <remarks>Divide by the <see cref="Nuclide.SecondsPerYear"/> constant to convert to years.</remarks>
        public double? Halflife { get; private set; }

        /// <summary>Returns <b>true</b> if the isotope is not synthetic, else <b>false</b>.</summary>
        public bool IsNatural => Abundance != null;

        /// <summary>Returns <b>false</b> if the isotope is radioactive, else <b>true</b>.</summary>
        public bool IsStable => StabilityIndex == 0;

        /// <summary>Returns the <see cref="Nuclide"/> of this isotope.</summary>
        /// <exception cref="ArgumentException">When <em>Z</em> is not a valid nucleon count.</exception>
        public Nuclide Nuclide => Nuclide.Table[Z];

        private Origin? _occurrence = null;

        /// <summary>Returns origin in nature, or synthetic.</summary>
        /// <remarks>
        /// <para>
        /// For more information, see:
        /// </para>
        /// <para>
        /// <em>https://en.wikipedia.org/wiki/Nucleosynthesis</em>
        /// </para>
        /// </remarks>
        public Origin Occurrence
        {
            get
            {
                if (_occurrence == null)
                    _occurrence = GetOrigin();
                return _occurrence.Value;
            }
        }

        /// <summary>Returns scaler value (0-3) of <see cref="Occurrence"/>.</summary>
        public int OccurrenceIndex => (int) Occurrence;

        /// <summary>Returns character code of <see cref="Occurrence"/>.</summary>
        public char OccurrenceCode => Nuclide.OccurrenceCodes[(int) Occurrence];

        /// <summary>Returns a string containing a character for each possible decay mode of the isotope.</summary>
        /// <remarks>The returned value provides concise output in a human-readable form.</remarks>
        public string DecayModeCodes
        {
            get
            {
                var result = string.Empty;
                for (int ix = 0, mask = 1; ix <= Nuclide.DecayModeCount; ++ix)
                {
                    if ((((int) DecayMode) & mask) != 0)
                        result += Nuclide.DecayModeCodes[ix];
                    mask <<= 1;
                }
                return result;
            }
        }

        /// <summary>Returns scaler value (0-5) of radioactivity.</summary>
        /// <remarks>
        /// A value of 0 indicates a stable isotope and 5 the most radioactive.
        /// Use <see cref="Nuclide.StabilityDescriptions"/> for related strings.
        /// </remarks>
        public int StabilityIndex { get; private set; }

        /// <summary>Returns an enumerator that iterates thru the possible decay mode indexes of the isotope.</summary>
        /// <returns>An enumerator that iterates thru the possible indexes.</returns>
        public IEnumerable<int> GetDecayIndexes()
        {
            for (int ix = 0, mask = 1; ix <= Nuclide.DecayModeCount; ++ix)
            {
                if (((int) DecayMode & mask) != 0)
                    yield return ix;
                mask <<= 1;
            }
        }

        private Origin GetOrigin()
        {
            if (Abundance == null)
                return Origin.Synthetic;
            if (Halflife == null || Halflife > Nuclide.PrimordialCutoff)
                return Origin.Primordial;

            if (Z > 0)
                foreach (var iso in Nuclide.Table[Z - 1].Isotopes)
                    if (iso.IsNatural && ! iso.IsStable)
                        foreach (var dx in iso.GetDecayIndexes())
                        {
                            var tryZ = iso.Transmute (dx, out int tryA);
                            if (tryZ == Z && tryA == A)
                               return Origin.Decay;
                        }

            if (Z < Nuclide.Table.Count - 1)
                foreach (var iso in Nuclide.Table[Z + 1].Isotopes)
                    if (iso.IsNatural && ! iso.IsStable)
                        foreach (var dx in iso.GetDecayIndexes())
                        {
                            var tryZ = iso.Transmute (dx, out int tryA);
                            if (tryZ == Z && tryA == A)
                               return Origin.Decay;
                        }

            if (Z < Nuclide.Table.Count - 2)
                foreach (var iso in Nuclide.Table[Z + 2].Isotopes)
                    if (iso.IsNatural && ! iso.IsStable)
                        foreach (var dx in iso.GetDecayIndexes())
                        {
                            var tryZ = iso.Transmute (dx, out int tryA);
                            if (tryZ == Z && tryA == A)
                               return Origin.Decay;
                        }

            // Uranium SF exceptions:
            if (Z == 43 || Z == 93)
                return Origin.Decay;

            return Origin.Cosmogenic;
        }

        /// <summary>Provide isotope contents in fixed-width form.</summary>
        /// <returns>A string holding fixed-width columns.</returns>
        public string ToFixedWidthString()
        {
            var sb = new StringBuilder();
            var ts = A.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            ts = Abundance == null ? string.Empty : Abundance.Value.ToString ("F4");
            sb.Append (' ', 8 - ts.Length);
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

        /// <summary>Provide isotope contents in JSON form.</summary>
        /// <param name="quote">Name delimiter, or empty string for no delimiter.</param>
        /// <returns>A string defining a JSON object.</returns>
        /// <remarks>
        /// Pass an empty string for JavaScript results
        /// or pass a string containing a quote character for JSON results.
        /// </remarks>
        public string ToJsonString (string quote)
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

        /// <summary>Provide isotope contents in short form.</summary>
        /// <returns>Returns the key of the isotope.</returns>
        public override string ToString() => "Z="+Z+", A="+A;

        /// <summary>Convert this isotope to another.</summary>
        /// <param name="decayModeBitflag">Bitflag of an isotope mutation.</param>
        /// <param name="nucleonCount">Nucleon count (<em>A</em>) of the product.</param>
        /// <returns>Proton count (<em>Z</em>) of the product. The number of nucleons of the product is returned in <b>nucleonCount</b>.</returns>
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

        /// <summary>Convert this isotope to another.</summary>
        /// <param name="decayIndex">Index of an isotope mutation.</param>
        /// <param name="nucleonCount">Nucleon count (<em>A</em>) of the product.</param>
        /// <returns>Proton count (<em>Z</em>) of the product. The number of nucleons of the product is returned in <b>nucleonCount</b>.</returns>
        public int Transmute (int decayIndex, out int nucleonCount)
         => Transmute ((Decay) (1<<decayIndex), out nucleonCount);
    }
}
