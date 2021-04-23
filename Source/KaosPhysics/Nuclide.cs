using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Kaos.Physics
{
    /// <summary>Region of the periodic table with similar traits.</summary>
    public enum Category
    {
        /// <summary>Alkali metal.</summary>
        AlMetal,
        /// <summary>Alkaline earth metal.</summary>
        AlEMetal,
        /// <summary>Lanthanoid.</summary>
        Lanthan,
        /// <summary>Acitinoid.</summary>
        Actin,
        /// <summary>Transitional metal.</summary>
        TMetal,
        /// <summary>Post-transitional metal.</summary>
        PtMetal,
        /// <summary>Metalloid.</summary>
        Metaloid,
        /// <summary>Nonmetal.</summary>
        Nonmetal,
        /// <summary>Halogen.</summary>
        Halogen,
        /// <summary>Noble gas.</summary>
        NobleGas
    }

    /// <summary>Bitflags that are bitwise ored together for each isotope.</summary>
    /// <remarks>
    /// <para>
    /// For more information about decay modes, see:
    /// </para>
    /// <para>
    /// <em>https://en.wikipedia.org/wiki/Radioactive_decay#Modes</em>
    /// </para>
    /// </remarks>
    [Flags]
    public enum Decay
    {
        /// <summary>No decay.</summary>
        None=0,
        /// <summary>Alpha decay: Emit an alpha particle.</summary>
        Alpha=1,
        /// <summary>Beta plus decay: Emit a positron.</summary>
        BetaPlus=2,
        /// <summary>Beta minus decay: Emit an electron.</summary>
        BetaMinus=4,
        /// <summary>Double beta decay: emit 2 electrons.</summary>
        Beta2=8,
        /// <summary>Electron capture: Nucleus absorbs its own electron.</summary>
        ECap1=16,
        /// <summary>Double electron capture: Nucleus absorbs 2 of its own electrons.</summary>
        ECap2=32,
        /// <summary>Neutron emission: Emit a neutron.</summary>
        NEmit=64,
        /// <summary>Gamma decay: Emit a gamma ray.</summary>
        Gamma=128,
        /// <summary>Internal conversion: Eject orbital electron.</summary>
        IC=256,
        /// <summary>Isomeric transition: Emit a gamma ray.</summary>
        IT=512,
        /// <summary>Spontaneous fission: Emit a nucleus heavier than an alpha particle.</summary>
        SF=1024
    }

    /// <summary>Natural origin.</summary>
    public enum Origin
    {
        /// <summary>Isotopes that have never been detected terrestrially.</summary>
        /// <remarks>Also includes decayed isotopes that are too transient to detect.</remarks>
        /// <remarks>Also includes isotopes that are the result of fallout.</remarks>
        Synthetic,
        /// <summary>Isotope produced from cosmic ray interaction.</summary>
        Cosmogenic,
        /// <summary>Isotope is radiogenic and has been detected in nature.</summary>
        Decay,
        /// <summary>Isotope has existed since Earth formation.</summary>
        Primordial
    }

    /// <summary>Biological significance.</summary>
    public enum Nutrition
    {
        /// <summary>No role in biology.</summary>
        None,
        /// <summary>One of the 11 elements that make up 99.85% of body mass.</summary>
        BulkEssential,
        /// <summary>One of the elements that make up the other 0.15% of body mass.</summary>
        TraceEssential, 
        /// <summary>Trace element with possible function as suggested by deprivation effects.</summary>
        Beneficial,
        /// <summary>Absorbed trace element with no known benefit.</summary>
        Absorbed
    }

    /// <summary>Distinct forms which matter can take.</summary>
    public enum State
    {
        /// <summary>State cannot be determined.</summary>
        Unknown,
        /// <summary>Matter where particles cannot move freely.</summary>
        Solid,
        /// <summary>A noncompressible fluid.</summary>
        Liquid,
        /// <summary>A compressible fluid.</summary>
        Gas
    }

    /// <summary>Represents a chemical element or the neutron.</summary>
    /// <remarks>
    /// <para>
    /// This class cannot be directly instantiated.
    /// To access the elements, use their class variables or use the <see cref="Nuclide.Table"/> collection.
    /// Class variables use the same names as the element names in world-english.
    /// </para>
    /// <para>
    /// The <see cref="Nuclide.Table"/> collection is an array of the nuclides.
    /// This section describes the API of its chemical element items.
    /// </para>
    /// <para>
    /// Use the <see cref="Z"/>, <see cref="Symbol"/>, <see cref="Period"/>,
    /// <see cref="Group"/>, <see cref="Block"/>, <see cref="Weight"/>
    /// properties for basic information about the element.
    /// </para>
    /// <para>
    /// To get the name of the element in world-English, use the <see cref="Name"/> property.
    /// To get the name of the element in any language, use the <see cref="GetName(string)"/> method.
    /// </para>
    /// <para>
    /// To get the melting and boiling points of the element,
    /// use the <see cref="Melt"/> and <see cref="Boil"/> properties.
    /// To get the state of matter at a given temperature, supply <see cref="GetState(double)"/> in kelvins.
    /// </para>
    /// <para>
    /// Use the <see cref="P:Kaos.Physics.Nuclide.Item(System.Int32)">indexer</see>
    /// for items of the nuclide's <see cref="Isotope"/> collection by <em>A</em>.
    ///
    /// Use the <see cref="GetElements"/> enumerator to exclude the neutron
    /// and iterate thru the elements in <em>Z</em> order.
    ///
    /// Use the <see cref="GetIsotopes"/> enumerator
    /// to iterate thru all isotopes in <em>Z</em>, <em>A</em> order.
    /// </para>
    /// <para>
    /// Use the <see cref="ToFixedWidthString(CultureInfo)"/> method to get a string representation of the nuclide
    /// with narrow, fixed-width columns. Supply a language code for localized nuclide names.
    ///
    /// Use the <see cref="ToJsonString(string)"/> method to get a string representation of the nuclide
    /// in either JSON or JavaScript form.
    /// </para>
    /// </remarks>
    public class Nuclide
    {
        /// <summary>Number of seconds in a year based on years with 365.2425 days.</summary>
        public const double SecondsPerYear = 31556952.0;

        /// <summary>Divide line between <see cref="Origin"/> of <b>Primoridal</b> and <b>Cosmogenic</b>.</summary>
        /// <remarks>
        /// The primordial isotope with shortest half-life is Uranium-235.
        /// The radiogenic isotope with longest half-life is Plutonium-244.
        /// </remarks>
        public const double PrimordialCutoff = 100000000.0 * SecondsPerYear;

        /// <summary>Nuclide Z=0, A=1.</summary>
        public static Nuclide Neutron { get; } = new Nuclide
        (
            0, "n", nameof (Neutron), Category.Nonmetal, 0, 0,
            melt: null, boil: null,
            weight: 1,
            known: 1932, credit: "James Chadwick",
            naming: "After Latin neuter, meaning neutral",
            isotopes: new Isotope[] { new Isotope (0, 1, 0.0, Decay.BetaMinus, 610.1, 's') },
            nameMap: new Dictionary<string,string> { { "es","Neutrón" }, { "it","Neutrone" }, { "ru","Водород" } }
        );

        /// <summary>Chemical element 1.</summary>
        public static Nuclide Hydrogen { get; } = new Nuclide
        (
            z: 1, symbol: "H", name: nameof (Hydrogen),
            category: Category.Nonmetal,
            period: 1, group: 1,
            melt: 13.99, boil: 20.271,
            weight: 1.008,
            life: Nutrition.BulkEssential,
            known: 1766, credit: "Henry Cavindish",
            naming: "From Greek, meaning water-former",
            isotopes: new Isotope[] { new Isotope (1, 1, 99.985), new Isotope (1, 2, 0.015), new Isotope (1, 3, 0.0, Decay.BetaMinus, 12.32, 'y') },
            nameMap: new Dictionary<string,string> { { "de","Wasserstoff" }, { "es","Hidrógeno" }, { "fr","Hydrogène" }, { "it","Hydrogène" }, { "ru","Нейтрон" } }
        );

        /// <summary>Chemical element 2.</summary>
        public static Nuclide Helium { get; } = new Nuclide
        (
            2, "He", nameof (Helium), Category.NobleGas, 1, 18,
            melt: 0.95, boil: 4.222,
            weight: 4.0026,
            known: 1868, credit: "Pierre Janssen, Norman Lockyer",
            naming: "After Helios, Greek god of the Sun",
            isotopes: new Isotope[] { new Isotope (2, 3, 0.0002), new Isotope (2, 4, 99.9998) },
            nameMap: new Dictionary<string,string> { { "es","Helio" }, { "fr","Hélium" }, { "it","Elio" }, { "ru","Гелий" } }
        );

        /// <summary>Chemical element 3.</summary>
        public static Nuclide Lithium { get; } = new Nuclide
        (
            3, "Li", nameof (Lithium), Category.AlMetal, 2, 1,
            melt: 453.65, boil: 1603.0,
            weight: 6.94,
            life: Nutrition.TraceEssential,
            known: 1817, credit: "Johan August Arfwedson",
            naming: "From the Greek λιθoς (lithos), meaning stone",
            isotopes: new Isotope[] { new Isotope (3, 6, 7.59), new Isotope (3, 7, 92.41) },
            nameMap: new Dictionary<string,string> { { "es","Litio" }, { "it","Litio" }, { "ru","Литий" } }
        );

        /// <summary>Chemical element 4.</summary>
        public static Nuclide Beryllium { get; } = new Nuclide
        (
            4, "Be", nameof (Beryllium), Category.AlEMetal, 2, 2,
            melt: 1560.0, boil: 2742.0,
            weight: 9.0122,
            known: 1798, credit: "Louis Nicolas Vauquelin",
            naming: "From the Greek beryllos, meaning beryl mineral",
            isotopes: new Isotope[] { new Isotope (4, 7, 0.0, Decay.ECap1|Decay.Gamma, 53.12, 'd'), new Isotope (4, 9, 100.0), new Isotope (4, 10, 0.0, Decay.BetaMinus, 1.39E6, 'y') },
            nameMap: new Dictionary<string,string> { { "es","Berilio" }, { "fr","Béryllium" }, { "it","Berillio" }, { "ru","Бериллий" } }
        );

        /// <summary>Chemical element 5.</summary>
        public static Nuclide Boron { get; } = new Nuclide
        (
            5, "B", nameof (Boron), Category.Metaloid, 2, 13,
            melt: 2349.0, boil: 4200.0,
            weight: 10.810,
            life: Nutrition.Beneficial,
            known: 1808, credit: "Joseph Louis Gay-Lussac, Louis Jacques Thénard",
            naming: "From the mineral borax",
            isotopes: new Isotope[] { new Isotope (5, 10, 20.0), new Isotope (5, 11, 80.0) },
            nameMap: new Dictionary<string,string> { { "de","Bor" }, { "es","Boro" }, { "fr","Bore" }, { "it","Boro" }, { "ru","Бор" } }
        );

        /// <summary>Chemical element 6.</summary>
        public static Nuclide Carbon { get; } = new Nuclide
        (
            6, "C", nameof (Carbon), Category.Nonmetal, period: 2, group: 14,
            melt: 3915.0, boil: 3915.0,
            weight: 12.011,
            life: Nutrition.BulkEssential,
            known: 1789, credit: "Antoine Lavoisier",
            naming: "From the Latin carbo, meaning coal",
            isotopes: new Isotope[] { new Isotope (6, 11, null, Decay.BetaPlus, 20.0, 'm'), new Isotope (6, 12, 98.9), new Isotope (6, 13, 1.1), new Isotope (6, 14, 0.0, Decay.BetaMinus, 5730, 'y') },
            nameMap: new Dictionary<string,string> { { "de","Kohlensto" }, { "es","Carbono" }, { "fr","Carbone" }, { "it","Carbonio" }, { "ru","Углерод" } }
        );

        /// <summary>Chemical element 7.</summary>
        public static Nuclide Nitrogen { get; } = new Nuclide
        (
            7, "N", nameof (Nitrogen), Category.Nonmetal, 2, 15,
            melt: 63.23, boil: 77.355,
            weight: 14.007,
            life: Nutrition.BulkEssential,
            known: 1772, credit: "Daniel Rutherford",
            naming: "From the French nitrogène, meaning nitre-producing",
            isotopes: new Isotope[] { new Isotope (7, 14, 99.6), new Isotope (7, 15, 0.4) },
            nameMap: new Dictionary<string,string> { { "de","Stickstoff" }, { "es","Nitrógeno" }, { "fr","Azote" }, { "it","Azoto" }, { "ru","Азот" } }
        );

        /// <summary>Chemical element 8.</summary>
        public static Nuclide Oxygen { get; } = new Nuclide
        (
            8, "O", nameof (Oxygen), Category.Nonmetal, 2, 16,
            melt: 54.36, boil: 90.188,
            weight: 15.999,
            life: Nutrition.BulkEssential,
            naming: "From the Greek ὀξύς (oxys) and -γενής (-genēs), meaning acid-producer",
            known: 1771, credit: "Carl Wilhelm Scheele",
            isotopes: new Isotope[] { new Isotope (8, 16, 99.76), new Isotope (8, 17, 0.04), new Isotope (8, 18, 0.20) },
            nameMap: new Dictionary<string,string> { { "de","Sauerstoff" }, { "es","Oxígeno" }, { "fr","Oxygène" }, { "it","Ossigeno" }, { "ru","Кислород" } }
        );

        /// <summary>Chemical element 9.</summary>
        public static Nuclide Fluorine { get; } = new Nuclide
        (
            9, "F", nameof (Fluorine), Category.Halogen, 2, 17,
            melt: 53.48, boil: 85.03,
            weight: 18.998,
            life: Nutrition.Beneficial,
            known: 1810, credit: "André-Marie Ampère",
            naming: "From fluoric acid",
            isotopes: new Isotope[] { new Isotope (9, 18, 0.0, Decay.BetaPlus|Decay.ECap1, 109.8, 's'), new Isotope (9, 19, 100.0) },
            nameMap: new Dictionary<string,string> { { "de","Fluor" }, { "es","Fluor" }, { "fr","Fluor" }, { "it","Fluoro" }, { "ru","Фтор" } }
         );

        /// <summary>Chemical element 10.</summary>
        public static Nuclide Neon { get; } = new Nuclide
        (
            10, "Ne", nameof (Neon), Category.NobleGas, 2, 18,
            melt: 24.56, boil: 27.104,
            weight: 20.180,
            known: 1898, credit: "William Ramsay, Morris Travers",
            naming: "From Latin novum via Greek, meaning new",
            isotopes: new Isotope[] { new Isotope (10, 20, 90.48), new Isotope (10, 21, 0.27), new Isotope (10, 22, 9.25) },
            nameMap: new Dictionary<string,string> { { "es","Neón" }, { "fr","Néon" }, { "ru","Неон" } }
        );

        /// <summary>Chemical element 11.</summary>
        public static Nuclide Sodium { get; } = new Nuclide
        (
            11, "Na", nameof (Sodium), Category.AlMetal, 3, 1,
            melt: 370.944, boil: 1156.090,
            weight: 22.990,
            life: Nutrition.BulkEssential,
            known: 1807, credit: "Humphry Davy",
            naming: "Possibly from the Arabic suda, meaning headache",
            isotopes: new Isotope[] { new Isotope (11, 22, 0.0, Decay.BetaPlus, 2.602, 'y'), new Isotope (11, 23, 100.0), new Isotope (11, 24, 0.0, Decay.BetaMinus, 14.96, 'h') },
            nameMap: new Dictionary<string,string> { { "de","Natrium" }, { "es","Sodio" }, { "it","Sodio" }, { "ru","Натрий" } }
        );

        /// <summary>Chemical element 12.</summary>
        public static Nuclide Magnesium { get; } = new Nuclide
        (
            12, "Mg", nameof (Magnesium), Category.AlEMetal, 3, 2,
            melt: 923.0, boil: 1363.0,
            weight: 24.305,
            life: Nutrition.BulkEssential,
            known: 1755, credit: "Joseph Black",
            naming: "From the Greek word for Magnesia (now in Turkey)",
            isotopes: new Isotope[] { new Isotope (12, 24, 79.0), new Isotope (12, 25, 10.0), new Isotope (12, 26, 11.0) },
            nameMap: new Dictionary<string,string> { { "es","Magnesio" }, { "fr","Magnésium" }, { "it", "Magnesio" }, { "ru","Калий" } }
        );

        /// <summary>Chemical element 13.</summary>
        public static Nuclide Aluminium { get; } = new Nuclide
        (
            13, "Al", nameof (Aluminium), Category.PtMetal, 3, 13,
            melt: 933.47, boil: 2743.0,
            weight: 26.982,
            life: Nutrition.Absorbed,
            known: 1824, credit: "Hans Christian Ørsted",
            naming: "From the Latin alum, the mineral from which it was isolated",
            isotopes: new Isotope[] { new Isotope (13, 26, 0.0, Decay.BetaPlus|Decay.ECap1|Decay.Gamma, 7.17E5, 'y'), new Isotope (13, 27, 100.0) },
            nameMap: new Dictionary<string,string> { { "en-US","Aluminum" }, { "es","Aluminio" }, { "it","Alluminio" }, { "ru","Алюминий" } }
        );

        /// <summary>Chemical element 14.</summary>
        public static Nuclide Silicon { get; } = new Nuclide
        (
            14, "Si", nameof (Silicon), Category.Metaloid, 3, 14,
            melt: 1687, boil: 3538,
            weight: 28.085,
            life: Nutrition.Beneficial,
            known: 1823, credit: "Jöns Jacob Berzelius",
            naming: "From the Latin silicis, meaning flint",
            isotopes: new Isotope[] { new Isotope (14, 28, 92.2), new Isotope (14, 29, 4.7), new Isotope (14, 30, 3.1), new Isotope (14, 31, 0.0, Decay.BetaMinus, 2.62, 'h'), new Isotope (14, 32, 0.0, Decay.BetaMinus, 1.53, 'y') },
            nameMap: new Dictionary<string,string> { { "de","Silicium" }, { "es","Silicio" }, { "fr","Silicium" }, { "it","Silicio" }, { "ru","Кремний" } }
        );

        /// <summary>Chemical element 15.</summary>
        public static Nuclide Phosphorus { get; } = new Nuclide
        (
            15, "P", nameof (Phosphorus), Category.Nonmetal, 3, 15,
            melt: 317.3, boil: 553.7, // white phosphorus
            weight: 30.974,
            life: Nutrition.BulkEssential,
            known: 1669, credit: "Hennig Brand",
            naming: "From the Greek φῶς and -φέρω, meaning light-bringer",
            isotopes: new Isotope[] { new Isotope (15, 30, null, Decay.BetaPlus, 2.498, 'm'), new Isotope (15, 31, 100.0), new Isotope (15, 32, 0.0, Decay.BetaMinus, 14.28, 'd'), new Isotope (15, 33, 0.0, Decay.BetaMinus, 25.3, 'd') },
            nameMap: new Dictionary<string,string> { { "de","Phosphor" }, { "es","Fósforo" }, { "fr","Phosphore" }, { "it","Fosforo" }, { "ru","Фосфор" } }
        );

        /// <summary>Chemical element 16.</summary>
        public static Nuclide Sulfur { get; } = new Nuclide
        (
            16, "S", nameof (Sulfur), Category.Nonmetal, 3, 16,
            melt: 388.36, boil: 717.8,
            weight: 32.06,
            life: Nutrition.BulkEssential,
            known: 0,
            naming: "From the Latin sulpur",
            isotopes: new Isotope[] { new Isotope (16, 32, 94.99), new Isotope (16, 33, 0.75), new Isotope (16, 34, 4.25), new Isotope (16, 35, 0.0, Decay.BetaMinus, 87.37, 'd'), new Isotope (16, 36, 0.01) },
            nameMap: new Dictionary<string,string> { { "de","Schwefel" }, { "en-GB","Sulphur" }, { "es","Azufre" }, { "fr","Soufre" }, { "it","Zolfo" }, { "ru","Сера" } }
        );

        /// <summary>Chemical element 17.</summary>
        public static Nuclide Chlorine { get; } = new Nuclide
        (
            17, "Cl", nameof (Chlorine), Category.Halogen, 3, 17,
            melt: 171.6, boil: 239.11,
            weight: 35.45,
            life: Nutrition.BulkEssential,
            known: 1774, credit: "Carl Wilhelm Scheele",
            naming: "From the Greek χλωρος (chlōros), meaning green-yellow",
            isotopes: new Isotope[] { new Isotope (17, 35, 76.0), new Isotope (17, 36, 0.0, Decay.BetaMinus|Decay.ECap1, 3.01E5, 'y'), new Isotope (17, 37, 24.0) },
            nameMap: new Dictionary<string,string> { { "de","Chlor" }, { "es","Cloro" }, { "fr","Chlore" }, { "it","Cloro" }, { "ru","Хлор" } }
        );

        /// <summary>Chemical element 18.</summary>
        public static Nuclide Argon { get; } = new Nuclide
        (
            18, "Ar", nameof (Argon), Category.NobleGas, 3, 18,
            melt: 83.81, boil: 87.302,
            weight: 39.95,
            known: 1894, credit: "Lord Rayleigh, William Ramsay",
            naming: "From the Greek ἀργόν, meaning inactive",
            isotopes: new Isotope[] { new Isotope (18, 36, 0.334), new Isotope (18, 37, null, Decay.ECap1, 35, 'd'), new Isotope (18, 38, 0.063), new Isotope (18, 39, 0.0, Decay.BetaMinus, 269, 'y'), new Isotope (18, 40, 99.604), new Isotope (18, 41, null, Decay.BetaMinus, 109.34, 's'), new Isotope (18, 42, null, Decay.BetaMinus, 32.9, 'y') },
            nameMap: new Dictionary<string,string> { { "es","Argón" }, { "ru","Аргон" } }
        );

        /// <summary>Chemical element 19.</summary>
        public static Nuclide Potassium { get; } = new Nuclide
        (
            19, "K", nameof (Potassium), Category.AlMetal, 4, 1,
            melt: 336.7, boil: 1032.0,
            weight: 39.098,
            life: Nutrition.BulkEssential,
            known: 1807, credit: "Humphry Davy",
            naming: "From placing in a pot the ash of burnt wood",
            isotopes: new Isotope[] { new Isotope (19, 39, 93.258), new Isotope (19, 40, 0.0117, Decay.BetaMinus|Decay.ECap1|Decay.BetaPlus, 1.248E9, 'y'), new Isotope (19, 41, 6.730) },
            nameMap: new Dictionary<string,string> { { "de","Kalium" }, { "es","Potasio" }, { "it","Potassio" }, { "ru","Калий" } }
        );

        /// <summary>Chemical element 20.</summary>
        public static Nuclide Calcium { get; } = new Nuclide
        (
            20, "Ca", nameof (Calcium), Category.AlEMetal, 4, 2,
            melt: 1115.0, boil: 1757.0,
            weight: 40.078,
            life: Nutrition.BulkEssential,
            known: 1808, credit: "Humphry Davy",
            naming: "From the Latin calx, meaning lime",
            isotopes: new Isotope[] { new Isotope (20, 40, 96.941), new Isotope (20, 41, 0.0, Decay.ECap1, 9.94E4, 'y'), new Isotope (20, 42, 0.647), new Isotope (20, 43, 0.135), new Isotope (20, 44, 2.086), new Isotope (20, 45, null, Decay.BetaMinus, 162.6, 'd'), new Isotope (20, 46, 0.004), new Isotope (20, 47, null, Decay.BetaMinus|Decay.Gamma, 4.5, 'd'), new Isotope (20, 48, 0.187, Decay.Beta2, 6.4E19, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Calcio" }, { "it","Calcio" }, { "ru","Кальций" } }
        );
        /// <summary>Chemical element 21.</summary>
        public static Nuclide Scandium { get; } = new Nuclide
        (
            21, "Sc", nameof (Scandium), Category.TMetal, 4, 3,
            melt: 1814.0, boil: 3109.0,
            weight: 44.956,
            known: 1879, credit: "Lars Fredrik Nilson",
            naming: "From the Latin Scandia, meaning Scandinavia",
            isotopes: new Isotope[] { new Isotope (21, 44, null, Decay.IC|Decay.Gamma|Decay.ECap1, 58.61, 'h'), new Isotope (21, 45, 100.0), new Isotope (21, 46, null, Decay.BetaMinus|Decay.Gamma, 83.79, 'd'), new Isotope (21, 47, null, Decay.BetaMinus|Decay.Gamma, 80.38, 'd'), new Isotope (21, 48, null, Decay.BetaMinus|Decay.Gamma, 43.67, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Escandio" }, { "it","Scandio" }, { "ru","Скандий" } }
        );

        /// <summary>Chemical element 22.</summary>
        public static Nuclide Titanium { get; } = new Nuclide
        (
            22, "Ti", nameof (Titanium), Category.TMetal, 4, 4,
            melt: 1941.0, boil: 3560.0,
            weight: 47.867,
            known: 1791, credit: "William Gregor",
            naming: "For the Titans of Greek mythology",
            isotopes: new Isotope[] { new Isotope (22, 44, null, Decay.ECap1|Decay.Gamma, 63, 'y'), new Isotope (22, 46, 8.25), new Isotope (22, 47, 7.44), new Isotope (22, 48, 73.72), new Isotope (22, 49, 5.41), new Isotope (22, 50, 5.18) },
            nameMap: new Dictionary<string,string>() { { "de","Titan" }, { "es","Titanio" }, { "fr","Titane" }, { "it","Titanio" }, { "ru","Титан" } }
        );

        /// <summary>Chemical element 23.</summary>
        public static Nuclide Vanadium { get; } = new Nuclide
        (
            23, "V", nameof (Vanadium), Category.TMetal, 4, 5,
            melt: 2183.0, boil: 3680.0,
            weight: 50.942,
            life: Nutrition.Beneficial,
            known: 1867, credit: "Henry Enfield Roscoe",
            naming: "For the Scandinavian goddess of beauty and fertility, Vanadís",
            isotopes: new Isotope[] { new Isotope (23, 48, null, Decay.BetaPlus, 16.0, 'd'), new Isotope (23, 49, null, Decay.ECap1, 330.0, 'd'), new Isotope (23, 50, 0.25, Decay.ECap1|Decay.BetaMinus, 1.5E17, 'y'), new Isotope (23, 51, 99.75) },
            nameMap: new Dictionary<string,string>() { { "es","Vanadio" }, { "it","Vanadio" }, { "ru","Ванадий" } }
        );

        /// <summary>Chemical element 24.</summary>
        public static Nuclide Chromium { get; } = new Nuclide
        (
            24, "Cr", nameof (Chromium), Category.TMetal, 4, 6,
            melt: 2180.0, boil: 2944.0,
            weight: 51.996,
            life: Nutrition.Absorbed,
            known: 1797, credit: "Louis Nicolas Vauquelin",
            naming: "From the Greek Chroma, meaing color",
            isotopes: new Isotope[] { new Isotope (24, 50, 4.345), new Isotope (24, 51, null, Decay.ECap1|Decay.Gamma, 27.7025, 'd'), new Isotope (24, 52, 83.789), new Isotope (24, 53, 9.501), new Isotope (24, 54, 2.365) },
            nameMap: new Dictionary<string,string>() { { "de","Chrom" }, { "es","Cromo" }, { "fr","Chrome" }, { "it","Cromo" }, { "ru","Хром" } }
        );

        /// <summary>Chemical element 25.</summary>
        public static Nuclide Manganese { get; } = new Nuclide
        (
            25, "Mn", nameof (Manganese), Category.TMetal, 4, 7,
            melt: 1519.0, boil: 2334.0,
            weight: 54.938,
            life: Nutrition.TraceEssential,
            known: 1774, credit: "Johann Gottlieb Gahn",
            naming: "From the Greek word for Magnesia (now in Turkey)",
            isotopes: new Isotope[] { new Isotope (25, 52, null, Decay.ECap1|Decay.BetaPlus|Decay.Gamma, 5.6, 'd'), new Isotope (25, 53, 0.0, Decay.ECap1, 3.74E6, 'y'), new Isotope (25, 54, null, Decay.ECap1|Decay.Gamma, 312.03, 'd'), new Isotope (25, 55, 100.0) },
            nameMap: new Dictionary<string,string>() { { "de","Mangan" }, { "es","Manganeso" }, { "fr","Manganèse" }, { "ru","Марганец" } }
        );

        /// <summary>Chemical element 26.</summary>
        public static Nuclide Iron { get; } = new Nuclide
        (
            26, "Fe", nameof (Iron), Category.TMetal, 4, 8,
            melt: 1811.0, boil: 3134.0,
            weight: 55.845,
            life: Nutrition.TraceEssential,
            known: 0,
            naming: "From proto-Germanic isarnan",
            isotopes: new Isotope[] { new Isotope (26, 54, 5.85), new Isotope (26, 55, null, Decay.ECap1, 2.73, 'y'), new Isotope (26, 56, 91.75), new Isotope (26, 57, 2.12), new Isotope (26, 58, 0.28), new Isotope (26, 59, null, Decay.BetaMinus, 44.6, 'd'), new Isotope (26, 60, 0.0, Decay.BetaMinus, 2.6E6, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Eisen" }, { "es","Hierro" }, { "fr","Fer" }, { "it","Ferro" }, { "ru","Железо" } }
        );

        /// <summary>Chemical element 27.</summary>
        public static Nuclide Cobalt { get; } = new Nuclide
        (
            27, "Co", nameof (Cobalt), Category.TMetal, 4, 9,
            melt: 1768.0, boil: 2723.0,
            weight: 58.933,
            life: Nutrition.TraceEssential,
            known: 1735, credit: "Georg Brandt",
            naming: "From the German kobold, meaning goblin",
            isotopes: new Isotope[] { new Isotope (27, 56, null, Decay.ECap1, 77.27, 'd'), new Isotope (27, 57, null, Decay.ECap1, 271.79, 'd'), new Isotope (27, 58, null, Decay.ECap1, 70.86, 'd'), new Isotope (27, 59, 100.0), new Isotope (27, 60, null, Decay.BetaMinus|Decay.Gamma, 5.2714, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Cobalto" }, { "it","Cobalto" }, { "ru","Кобальт" } }
        );

        /// <summary>Chemical element 28.</summary>
        public static Nuclide Nickel { get; } = new Nuclide
        (
            28, "Ni", nameof (Nickel), Category.TMetal, 4, 10,
            melt: 1728.0, boil: 3003.0,
            weight: 58.693,
            life: Nutrition.Absorbed,
            known: 1751, credit: "Axel Fredrik Cronstedt",
            naming: "After a mischievous sprite of German mythology, Nickel",
            isotopes: new Isotope[] { new Isotope (28, 58, 68.077), new Isotope (28, 59, 0.0, Decay.ECap1, 7.6E4, 'y'), new Isotope (28, 60, 26.223), new Isotope (28, 61, 1.140), new Isotope (28, 62, 3.635), new Isotope (28, 63, null, Decay.BetaMinus, 100, 'y'), new Isotope (28, 64, 0.926) },
            nameMap: new Dictionary<string,string>() { { "es","Niquel" }, { "it","Nichel" }, { "ru","Никель" } }
        );

        /// <summary>Chemical element 29.</summary>
        public static Nuclide Copper { get; } = new Nuclide
        (
            29, "Cu", nameof (Copper), Category.TMetal, 4, 11,
            melt: 1357.77, boil: 2835.0,
            weight: 63.546,
            life: Nutrition.TraceEssential,
            known: 0,
            naming: "From the Latin Cyprium, an alloy from Cyprus",
            isotopes: new Isotope[] { new Isotope (29, 63, 69.15), new Isotope (29, 64, null, Decay.ECap1|Decay.BetaMinus, 12.70, 'h'), new Isotope (29, 65, 30.85), new Isotope (29, 67, null, Decay.BetaMinus, 61.83, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Kupfer" }, { "es","Cobre" }, { "fr","Cuivre" }, { "it","Rame" }, { "ru","Медь" } }
        );

        /// <summary>Chemical element 30.</summary>
        public static Nuclide Zinc { get; } = new Nuclide
        (
            30, "Zn", nameof (Zinc), Category.TMetal, 4, 12,
            melt: 692.68, boil: 1180.0,
            weight: 65.38,
            life: Nutrition.TraceEssential,
            known: 1746, credit: "Andreas Sigismund Marggraf",
            naming: "Probably from the German zinke, meaning pointed or jagged",
            isotopes: new Isotope[] { new Isotope (30, 64, 49.2), new Isotope (30, 65, null, Decay.ECap1|Decay.Gamma, 244.0, 'd'), new Isotope (30, 66, 27.7), new Isotope (30, 67, 4.0), new Isotope (30, 68, 18.5), new Isotope (30, 69, null, Decay.BetaMinus, 56.0, 'm'), new Isotope (30, 70, 0.6), new Isotope (30, 71, null, Decay.BetaMinus, 2.4, 'm'), new Isotope (30, 72, null, Decay.BetaMinus, 46.5, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Zink" }, { "it","Zinco" }, { "ru","Цинк" } }
        );

        /// <summary>Chemical element 31.</summary>
        public static Nuclide Gallium { get; } = new Nuclide
        (
            31, "Ga", nameof (Gallium), Category.PtMetal, 4, 13,
            melt: 302.9146, boil: 2673.0,
            weight: 69.723,
            known: 1875, credit: "Lecoq de Boisbaudran",
            naming: "From Latin Gallia, meaning Gaul",
            isotopes: new Isotope[] { new Isotope (31, 66, null, Decay.BetaPlus, 95, 'h'), new Isotope (31, 67, null, Decay.ECap1, 3.3, 'd'), new Isotope (31, 68, null, Decay.BetaPlus, 1.2, 'h'), new Isotope (31, 69, 60.11), new Isotope (31, 70, null, Decay.BetaMinus|Decay.ECap1, 21.0, 'm'), new Isotope (31, 71, 39.89), new Isotope (31, 72, null, Decay.BetaMinus, 14.1, 'h'), new Isotope (31, 73, null, Decay.BetaMinus, 4.9, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Galio" }, { "it","Gallio" }, { "ru","Галлий" } }
        );

        /// <summary>Chemical element 32.</summary>
        public static Nuclide Germanium { get; } = new Nuclide
        (
            32, "Ge", nameof (Germanium), Category.Metaloid, 4, 14,
            melt: 1211.40, boil: 3106.0,
            weight: 72.630,
            known: 1886, credit: "Clemens Winkler",
            naming: "From the Latin Germania, meaning Germany",
            isotopes: new Isotope[] { new Isotope (32, 68, null, Decay.ECap1, 270.95, 'd'), new Isotope (32, 70, 20.52), new Isotope (32, 71, null, Decay.ECap1, 11.3, 'd'), new Isotope (32, 72, 27.45), new Isotope (32, 73, 7.76), new Isotope (32, 74, 36.7), new Isotope (32, 76, 7.75, Decay.Beta2, 1.78E21, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Germanio" }, { "it","Germanio" }, { "ru","Германий" } }
        );

        /// <summary>Chemical element 33.</summary>
        public static Nuclide Arsenic { get; } = new Nuclide
        (
            33, "As", nameof (Arsenic), Category.Metaloid, 4, 15,
            melt: 887.0, boil: 887.0,
            weight: 74.922,
            life: Nutrition.Absorbed,
            known: 0,
            naming: "From Arabic al-zarnīḵ, meaning yellow orpiment",
            isotopes: new Isotope[] { new Isotope (33, 73, null, Decay.ECap1|Decay.Gamma, 80.3, 'd'), new Isotope (33, 74, null, Decay.ECap1|Decay.BetaPlus|Decay.Gamma|Decay.BetaMinus, 17.8, 'd'), new Isotope (33, 75, 100.0), new Isotope (33, 76, null, Decay.BetaMinus|Decay.ECap1, 1.0942, 'd') },
            nameMap: new Dictionary<string,string>() { { "de","Arsen" }, { "es","Arsénico" }, { "it","Arsenico" }, { "ru","Мышьяк" } }
        );

        /// <summary>Chemical element 34.</summary>
        public static Nuclide Selenium { get; } = new Nuclide
        (
            34, "Se", nameof (Selenium), Category.Nonmetal, 4, 16,
            melt: 494.0, boil: 958.0,
            weight: 78.971,
            life: Nutrition.TraceEssential,
            known: 1817, credit: "Jöns Jakob Berzelius, Johann Gottlieb Gahn",
            naming: "From Greek σελήνη (selḗnē), meaning Moon",
            isotopes: new Isotope[] { new Isotope (34, 72, null, Decay.ECap1|Decay.Gamma, 8.4, 'd'), new Isotope (34, 74, 0.86), new Isotope (34, 75, null, Decay.ECap1|Decay.Gamma, 119.8, 'd'), new Isotope (34, 76, 9.23), new Isotope (34, 77, 7.60), new Isotope (34, 78, 23.69), new Isotope (34, 79, 0.0, Decay.BetaMinus, 3.27E5, 'y'), new Isotope (34, 80, 49.80), new Isotope (34, 82, 8.82, Decay.Beta2, 1.08E20, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Selen" }, { "es","Selenio" }, { "fr","Sélénium" }, { "it","Selenio" }, { "ru","Селен" } }
        );

        /// <summary>Chemical element 35.</summary>
        public static Nuclide Bromine { get; } = new Nuclide
        (
            35, "Br", nameof (Bromine), Category.Halogen, 4, 17,
            melt: 265.8, boil: 332.0,
            weight: 79.904,
            life: Nutrition.Absorbed,
            known: 1825, credit: "Antoine Jérôme Balard, Carl Jacob Löwig",
            naming: "From the Greek βρῶμος, meaning stench",
            isotopes: new Isotope[] { new Isotope (35, 79, 51.0), new Isotope (35, 81, 49.0), new Isotope (35, 82, null, Decay.BetaMinus, 35.282, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Brom" }, { "es","Bromo" }, { "fr","Brome" }, { "it","Bromo" }, { "ru","Бром" } }
        );

        /// <summary>Chemical element 36.</summary>
        public static Nuclide Krypton { get; } = new Nuclide
        (
            36, "Kr", nameof (Krypton), Category.NobleGas, 4, 18,
            melt: 115.78, boil: 119.93,
            weight: 83.798,
            known: 1898, credit: "William Ramsay, Morris Travers",
            naming: "From the Greek kryptos, meaning hidden",
            isotopes: new Isotope[] { new Isotope (36, 78, 0.36, Decay.ECap2, 9.2E21, 'y'), new Isotope (36, 79, null, Decay.ECap1|Decay.BetaPlus|Decay.Gamma, 35, 'h'), new Isotope (36, 80, 2.29), new Isotope (36, 81, 0.0, Decay.ECap1|Decay.Gamma, 2.3E5, 'y'), new Isotope (36, 82, 11.59), new Isotope (36, 83, 11.50), new Isotope (36, 84, 56.99), new Isotope (36, 85, null, Decay.BetaMinus, 11, 'y'), new Isotope (36, 86, 17.28) },
            nameMap: new Dictionary<string,string>() { { "es","Kriptón" }, { "it","Kripton" }, { "ru","Криптон" } }
        );

        /// <summary>Chemical element 37.</summary>
        public static Nuclide Rubidium { get; } = new Nuclide
        (
            37, "Rb", nameof (Rubidium), Category.AlMetal, 5, 1,
            melt: 312.45, boil: 961.0,
            weight: 85.468,
            life: Nutrition.Absorbed,
            known: 1861, credit: "Robert Bunsen, Gustav Kirchhoff",
            naming: "From the Latin rubidus, meaning deep red",
            isotopes: new Isotope[] { new Isotope (37, 83, null, Decay.ECap1|Decay.Gamma, 86.2, 'd'), new Isotope (37, 84, null, Decay.ECap1|Decay.BetaPlus|Decay.Gamma|Decay.BetaMinus, 32.9, 'd'), new Isotope (37, 85, 72.17), new Isotope (37, 86, null, Decay.BetaMinus|Decay.Gamma, 18.7, 'd'), new Isotope (37, 87, 27.83, Decay.BetaMinus, 4.9E10, 'y'), },
            nameMap: new Dictionary<string,string>() { { "es","Rubidoo" }, { "it","Rubidoo" }, { "ru","Рубидий" } }
        );

        /// <summary>Chemical element 38.</summary>
        public static Nuclide Strontium { get; } = new Nuclide
        (
            38, "Sr", nameof (Strontium), Category.AlEMetal, 5, 2,
            melt: 1050.0, boil: 1650.0,
            weight: 87.62,
            life: Nutrition.Absorbed,
            known: 1787, credit: "William Cruickshank",
            naming: "From the Scottish village of Strontian",
            isotopes: new Isotope[] { new Isotope (38, 82, null, Decay.ECap1, 25.36, 'd'), new Isotope (38, 83, null, Decay.ECap1|Decay.BetaPlus|Decay.Gamma, 1.35, 'd'), new Isotope (38, 84, 0.56), new Isotope (38, 85, null, Decay.ECap1|Decay.Gamma, 64.84, 'd'), new Isotope (38, 86, 9.86), new Isotope (38, 87, 7.0), new Isotope (38, 88, 82.58), new Isotope (38, 89, null, Decay.ECap1|Decay.BetaMinus, 50.52, 'd'), new Isotope (38, 90, 0.0, Decay.BetaMinus, 28.90, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Estronzio" }, { "it","Stronzio" }, { "ru","Стронций" } }
        );

        /// <summary>Chemical element 39.</summary>
        public static Nuclide Yttrium { get; } = new Nuclide
        (
            39, "Y", nameof (Yttrium), Category.TMetal, 5, 3,
            melt: 1799.0, boil: 3203.0,
            weight: 88.906,
            known: 1794, credit: "Johan Gadolin",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (39, 87, null, Decay.ECap1|Decay.Gamma, 3.4, 'd'), new Isotope (39, 88, null, Decay.ECap1|Decay.Gamma, 106.6, 'd'), new Isotope (39, 89, 100.0), new Isotope (39, 90, null, Decay.BetaMinus|Decay.Gamma, 2.7, 'd'), new Isotope (39, 91, null, Decay.BetaMinus|Decay.Gamma, 58.5, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Itrio" }, { "it","Ittrio" }, { "ru","Иттрий" } }
        );

        /// <summary>Chemical element 40.</summary>
        public static Nuclide Zirconium { get; } = new Nuclide
        (
            40, "Zr", nameof (Zirconium), Category.TMetal, 5, 4,
            melt: 2128.0, boil: 4650.0,
            weight: 91.224,
            known: 1789, credit: "Martin Heinrich Klaproth",
            naming: "From the mineral zircon",
            isotopes: new Isotope[] { new Isotope (40, 88, null, Decay.ECap1|Decay.Gamma, 83.4, 'd'), new Isotope (40, 89, null, Decay.ECap1|Decay.BetaPlus|Decay.Gamma, 78.4, 'h'), new Isotope (40, 90, 51.45), new Isotope (40, 91, 11.22), new Isotope (40, 92, 17.15), new Isotope (40, 93, 0.0, Decay.BetaMinus, 1.53E6, 'y'), new Isotope (40, 94, 17.38), new Isotope (40, 96, 2.8, Decay.Beta2, 2E19, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Zirconio" }, { "it","Zirconio" }, { "ru","Цирконий" } }
        );

        /// <summary>Chemical element 41.</summary>
        public static Nuclide Niobium { get; } = new Nuclide
        (
            41, "Nb", nameof (Niobium), Category.TMetal, 5, 5,
            melt: 2750.0, boil: 5017.0,
            weight: 92.906,
            known: 1801, credit: "Charles Hatchett",
            naming: "From Niobe, the daughter of Tantalus",
            isotopes: new Isotope[] { new Isotope (41, 90, null, Decay.BetaPlus, 15, 'h'), new Isotope (41, 91, null, Decay.ECap1, 680, 'y'), new Isotope (41, 92, 0.0, Decay.ECap1|Decay.Gamma, 3.47E7, 'y'), new Isotope (41, 93, 100.0), new Isotope (41, 94, 0.0, Decay.BetaMinus|Decay.Gamma, 20.3E3, 'y'), new Isotope (41, 95, null, Decay.BetaMinus|Decay.Gamma, 35.0, 'd'), new Isotope (41, 96, null, Decay.BetaMinus, 24.0, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Niob" }, { "es","Niobo" }, { "it","Niobio" }, { "ru","Ниобий" } }
        );

        /// <summary>Chemical element 42.</summary>
        public static Nuclide Molybdenum { get; } = new Nuclide
        (
            42, "Mo", nameof (Molybdenum), Category.TMetal, 5, 6,
            melt: 2896.0, boil: 4912.0,
            weight: 95.95,
            known: 1778, credit: "Carl Wilhelm Scheele",
            life: Nutrition.TraceEssential,
            naming: "From the ore molybdena",
            isotopes: new Isotope[] { new Isotope (42, 92, 14.65), new Isotope (42, 93, null, Decay.ECap1, 4.0E3, 'y'), new Isotope (42, 94, 9.19), new Isotope (42, 95, 15.87), new Isotope (42, 96, 16.67), new Isotope (42, 97, 9.58), new Isotope (42, 98, 24.29), new Isotope (42, 99, null, Decay.BetaMinus|Decay.Gamma, 65.94, 'h'), new Isotope (42, 100, 9.74, Decay.Beta2, 7.8E18, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Molybdän" }, { "es","Molibdeno" }, { "fr","Molybdène" }, { "it","Molibdeno" }, { "ru","Молибден" } }
        );

        /// <summary>Chemical element 43.</summary>
        public static Nuclide Technetium { get; } = new Nuclide
        (
            43, "Tc", nameof (Technetium), Category.TMetal, 5, 7,
            melt: 2430.0, boil: 4538.0,
            weight: 97.0,
            known: 1937, credit: "Emilio Segrè, Carlo Perrier",
            naming: "From the Greek τεχνητός, meaning artificial",
            isotopes: new Isotope[] { new Isotope (43, 95, null, Decay.ECap1|Decay.Gamma|Decay.IC, 61.0, 'd'), new Isotope (43, 96, null, Decay.ECap1|Decay.Gamma, 4.3, 'd'), new Isotope (43, 97, null, Decay.ECap1, 4.21E6, 'y'), new Isotope (43, 98, null, Decay.BetaMinus|Decay.Gamma, 4.2E6, 'y'), new Isotope (43, 99, 0.0, Decay.BetaMinus, 2.111E5, 'y'), new Isotope (43, 100, null, Decay.BetaMinus|Decay.ECap1, 15.8, 's') },
            nameMap: new Dictionary<string,string>() { { "fr","Technétium" }, { "es","Tecnecio" }, { "it","Tecnezio" }, { "ru","Технеций" } }
        );

        /// <summary>Chemical element 44.</summary>
        public static Nuclide Ruthenium { get; } = new Nuclide
        (
            44, "Ru", nameof (Ruthenium), Category.TMetal, 5, 8,
            melt: 2607.0, boil: 4423.0,
            weight: 101.07,
            known: 1844, credit: "Karl Ernst Claus",
            naming: "From the Latin Ruthenia, meaning Russia",
            isotopes: new Isotope[] { new Isotope (44, 96, 5.54), new Isotope (44, 97, null, Decay.ECap1|Decay.Gamma, 2.9, 'd'), new Isotope (44, 98, 1.87), new Isotope (44, 99, 12.76), new Isotope (44, 100, 12.60), new Isotope (44, 101, 17.06), new Isotope (44, 102, 31.55), new Isotope (44, 103, null, Decay.BetaMinus|Decay.Gamma, 39.26, 'd'), new Isotope (44, 104, 18.62), new Isotope (44, 106, null, Decay.BetaMinus, 373.59, 'd') },
            nameMap: new Dictionary<string,string>() { { "fr","Ruthénium" }, { "es","Rutenio" }, { "it","Rutenio" }, { "ru","Рутений" } }
        );

        /// <summary>Chemical element 45.</summary>
        public static Nuclide Rhodium { get; } = new Nuclide
        (
            45, "Rh", nameof (Rhodium), Category.TMetal, 5, 9,
            melt: 2237.0, boil: 3968.0,
            weight: 102.91,
            known: 1804, credit: "William Hyde Wollaston",
            naming: "From Greek ῥόδον (rhodon), meaning rose",
            isotopes: new Isotope[] { new Isotope (45, 99, null, Decay.ECap1|Decay.Gamma, 16.1, 'd'), new Isotope (45, 101, null, Decay.ECap1|Decay.IC|Decay.Gamma, 4.34, 'd'), new Isotope (45, 101, null, Decay.ECap1|Decay.Gamma, 3.3, 'y'), new Isotope (45, 102, null, Decay.ECap1|Decay.Gamma, 3.7, 'y'), new Isotope (45, 102, null, Decay.ECap1|Decay.BetaPlus|Decay.BetaMinus|Decay.Gamma, 207.0, 'd'), new Isotope (45, 103, 100.0), new Isotope (45, 105, null, Decay.BetaMinus|Decay.Gamma, 35.36, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Rodio" }, { "it","Rodio" }, { "ru","Родий" } }
        );

        /// <summary>Chemical element 46.</summary>
        public static Nuclide Palladium { get; } = new Nuclide
        (
            46, "Pd", nameof (Palladium), Category.TMetal, 5, 10,
            melt: 1828.05, boil: 3236.0,
            weight: 106.42,
            known: 1802, credit: "William Hyde Wollaston",
            naming: "After the asteroid 2 Pallas",
            isotopes: new Isotope[] { new Isotope (46, 100, null, Decay.ECap1|Decay.Gamma, 3.63, 'd'), new Isotope (46, 102, 1.02), new Isotope (46, 103, null, Decay.ECap1, 16.991, 'd'), new Isotope (46, 104, 11.14), new Isotope (46, 105, 22.33), new Isotope (46, 106, 27.33), new Isotope (46, 107, 0.0, Decay.BetaMinus, 6.5E6, 'y'), new Isotope (46, 108, 26.46), new Isotope (46, 110, 11.72) },
            nameMap: new Dictionary<string,string>() { { "es","Paladio" }, { "it","Palladio" }, { "ru","Палладий" } }
        );

        /// <summary>Chemical element 47.</summary>
        public static Nuclide Silver { get; } = new Nuclide
        (
            47, "Ag", nameof (Silver), Category.TMetal, 5, 11,
            melt: 1234.93, boil: 2435.0,
            weight: 107.87,
            known: 0,
            naming: "From Proto-Germanic silubra",
            isotopes: new Isotope[] { new Isotope (47, 105, null, Decay.ECap1|Decay.Gamma, 41.2, 'd'), new Isotope (47, 106, null, Decay.ECap1|Decay.Gamma, 8.28, 'd'), new Isotope (47, 107, 51.839), new Isotope (47, 108, null, Decay.ECap1|Decay.IT|Decay.Gamma, 418, 'y'), new Isotope (47, 109, 48.161), new Isotope (47, 110, null, Decay.BetaMinus|Decay.Gamma, 249.95, 'd'), new Isotope (47, 111, null, Decay.BetaMinus|Decay.Gamma, 7.45, 'd') },
            nameMap: new Dictionary<string,string>() { { "de","Silber" }, { "es","Plata" }, { "fr","Argent" }, { "it","Argento" }, { "ru","Серебро" } }
        );

        /// <summary>Chemical element 48.</summary>
        public static Nuclide Cadmium { get; } = new Nuclide
        (
            48, "Cd", nameof (Cadmium), Category.TMetal, 5, 12,
            weight: 112.41,
            melt: 594.22, boil: 1040.0,
            known: 1817, credit: "Karl Samuel Leberecht Hermann, Friedrich Stromeyer",
            naming: "From the mineral calamine named after the Greek mythological character Κάδμος (Cadmus)",
            isotopes: new Isotope[] { new Isotope (48, 106, 1.25), new Isotope (48, 107, null, Decay.ECap1, 6.5, 'h'), new Isotope (48, 108, 0.89), new Isotope (48, 109, null, Decay.ECap1, 462.6, 'd'), new Isotope (48, 110, 12.47), new Isotope (48, 111, 12.80), new Isotope (48, 112, 24.11), new Isotope (48, 113, 12.23, Decay.BetaMinus, 7.7E15, 'y'), new Isotope (48, 113, null, Decay.BetaMinus|Decay.IT, 14.1, 'y'), new Isotope (48, 114, 28.75), new Isotope (48, 115, null, Decay.BetaMinus, 53.46, 'h'), new Isotope (48, 116, 7.51, Decay.Beta2, 3.1E19, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Cadmio" }, { "it","Cadmio" }, { "ru","Кадмий" } }
        );

        /// <summary>Chemical element 49.</summary>
        public static Nuclide Indium { get; } = new Nuclide
        (
            49, "In", nameof (Indium), Category.PtMetal, 5, 13,
            melt: 429.7485, boil: 2345.0,
            weight: 114.82,
            known: 1863, credit: "Ferdinand Reich, Hieronymous Theodor Richter",
            naming: "From the indigo color seen in its spectrum, after the Latin indicum, meaning 'of India'",
            isotopes: new Isotope[] { new Isotope (49, 111, null, Decay.ECap1, 2.8, 'd'), new Isotope (49, 113, 4.28), new Isotope (49, 115, 95.72, Decay.BetaMinus, 4.41E14, 'y'), new Isotope (49, 116, null, Decay.BetaMinus|Decay.ECap1, 14.1, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Indio" }, { "it","Indio" }, { "ru","Индий" } }
        );

        /// <summary>Chemical element 50.</summary>
        public static Nuclide Tin { get; } = new Nuclide
        (
            50, "Sn", nameof (Tin), Category.PtMetal, 5, 14,
            melt: 505.08, boil: 2875.0,
            weight: 118.710,
            life: Nutrition.Absorbed,
            known: 0,
            naming: "From proto-Germanic 'tin-om'",
            isotopes: new Isotope[] { new Isotope (50, 112, 0.97), new Isotope (50, 114, 0.66), new Isotope (50, 115, 0.34), new Isotope (50, 116, 14.54), new Isotope (50, 117, 7.68), new Isotope (50, 118, 24.22), new Isotope (50, 119, 8.59), new Isotope (50, 120, 32.58), new Isotope (50, 122, 4.63), new Isotope (50, 124, 5.79), new Isotope (50, 126, 0.0, Decay.BetaMinus, 2.3E5, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Zinn" }, { "es","Estaño" }, { "fr","Etain" }, { "it","Stagno" }, { "ru","Олово" } }
        );

        /// <summary>Chemical element 51.</summary>
        public static Nuclide Antimony { get; } = new Nuclide
        (
            51, "Sb", nameof (Antimony), Category.Metaloid, 5, 15,
            melt: 903.78, boil: 1908.0,
            weight: 121.760,
            known: 0,
            naming: "From Latin antimonium",
            isotopes: new Isotope[] { new Isotope (51, 121, 57.21), new Isotope (51, 123, 42.79), new Isotope (51, 125, null, Decay.BetaMinus, 2.7582, 'y'), new Isotope (51, 126, null, Decay.BetaMinus, 12.35, 'd') },
            nameMap: new Dictionary<string,string>() { { "de","Antimon" }, { "es","Antimonio" }, { "fr","Antimoine" }, { "it","Antimonio" }, { "ru","Сурьма" } }
        );

        /// <summary>Chemical element 52.</summary>
        public static Nuclide Tellurium { get; } = new Nuclide
        (
            52, "Te", nameof (Tellurium), Category.Metaloid, 5, 16,
            melt: 722.66, boil: 1261.0,
            weight: 127.60,
            known: 1782, credit: "Franz-Joseph Müller von Reichenstein",
            naming: "From Latin tellus, meaning 'earth'",
            isotopes: new Isotope[] { new Isotope (52, 120, 0.09), new Isotope (52, 121, null, Decay.ECap1, 16.78, 'd'), new Isotope (52, 122, 2.55), new Isotope (52, 123, 0.89), new Isotope (52, 124, 4.74), new Isotope (52, 125, 7.07), new Isotope (52, 126, 18.84), new Isotope (52, 127, null, Decay.BetaMinus, 9.35, 'h'), new Isotope (52, 128, 31.74, Decay.Beta2, 2.2E24, 'y'), new Isotope (52, 129, null, Decay.BetaMinus, 69.6, 'm'), new Isotope (52, 130, 34.08, Decay.Beta2, 7.9E20, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Tellur" }, { "es","Teluro" }, { "fr","Tellure" }, { "it","Tellurio" }, { "ru","Теллур" } }
        );

        /// <summary>Chemical element 53.</summary>
        public static Nuclide Iodine { get; } = new Nuclide
        (
            53, "I", nameof (Iodine), Category.Halogen, 5, 17,
            melt: 386.85, boil: 457.4,
            weight: 126.9,
            known: 1811, credit: "Bernard Courtois",
            life: Nutrition.TraceEssential,
            naming: "From the Greek ἰοειδής (ioeidēs), meaning 'violet'",
            isotopes: new Isotope[] { new Isotope (53, 123, null, Decay.ECap1|Decay.Gamma, 13.0, 'h'), new Isotope (53, 124, null, Decay.ECap1, 4.176, 'd'), new Isotope (53, 125, null, Decay.ECap1, 59.40, 'd'), new Isotope (53, 127, 100.0), new Isotope (53, 128, null, Decay.BetaMinus|Decay.BetaPlus, 24.99, 'm'), new Isotope (53, 129, 0.0, Decay.BetaMinus, 1.57E7, 'y'), new Isotope (53, 130, null, Decay.BetaMinus, 12.36, 'h'), new Isotope (53, 131, null, Decay.BetaMinus|Decay.Gamma, 8.02070, 'd'), new Isotope (53, 135, null, Decay.BetaMinus, 6.57, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Iod" }, { "es","Yodo" }, { "fr","Iode" }, { "it","Iodio" }, { "ru","Иод" } }
        );

        /// <summary>Chemical element 54.</summary>
        public static Nuclide Xenon { get; } = new Nuclide
        (
            54, "Xe", nameof (Xenon), Category.NobleGas, 5, 18,
            melt: 161.40, boil: 165.051,
            weight: 131.29,
            known: 1898, credit: "William Ramsay, Morris Travers",
            naming: "From the Greek ξένον (xénon), meaning 'foreigner'",
            isotopes: new Isotope[] { new Isotope (54, 124, 0.095, Decay.ECap2, 1.8E22, 'y'), new Isotope (54, 125, null, Decay.ECap1, 16.9, 'h'), new Isotope (54, 126, 0.89), new Isotope (54, 127, null, Decay.ECap1, 36.345, 'd'), new Isotope (54, 128, 1.910), new Isotope (54, 129, 26.401), new Isotope (54, 130, 4.071), new Isotope (54, 131, 21.232), new Isotope (54, 132, 26.909), new Isotope (54, 133, null, Decay.BetaMinus, 5.247, 'd'), new Isotope (54, 134, 10.436), new Isotope (54, 135, null, Decay.BetaMinus, 9.14, 'h'), new Isotope (54, 136, 8.857, Decay.Beta2, 2.165E21, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Xénon" }, { "fr","Xénon" }, { "ru","Ксенон" } }
        );

        /// <summary>Chemical element 55.</summary>
        public static Nuclide Caesium { get; } = new Nuclide
        (
            55, "Cs", nameof (Caesium), Category.AlMetal, 6, 1,
            melt: 301.7, boil: 944.0,
            weight: 132.91,
            known: 1860, credit: "Robert Bunsen, Gustav Kirchhoff",
            naming: "From the Latin word caesius, meaning 'sky-blue'",
            isotopes: new Isotope[] { new Isotope (55, 133, 100.0), new Isotope (55, 134, null, Decay.ECap1|Decay.BetaMinus, 2.0648, 'y'), new Isotope (55, 135, 0.0, Decay.BetaMinus, 2.3E6, 'y'), new Isotope (55, 136, null, Decay.BetaMinus, 13.16, 'd'), new Isotope (55, 137, null, Decay.BetaMinus, 30.17, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Cäsium" }, { "en-US","Cesium" }, { "es","Cesio" }, { "fr","Césium" }, { "it","Cesio" }, { "ru","Цезий" } }
        );

        /// <summary>Chemical element 56.</summary>
        public static Nuclide Barium { get; } = new Nuclide
        (
            56, "Ba", nameof (Barium), Category.AlEMetal, 6, 2,
            melt: 1000.0, boil: 2118.0,
            weight: 137.33,
            known: 1772, credit: "Carl Wilhelm Scheele",
            naming: "After the mineral 'baryta'",
            isotopes: new Isotope[] { new Isotope (56, 130, 0.11, Decay.ECap2, 0.5E21, 'y'), new Isotope (56, 132, 0.10), new Isotope (56, 133, null, Decay.ECap1, 10.51, 'y'), new Isotope (56, 134, 2.42), new Isotope (56, 135, 6.59), new Isotope (56, 136, 7.85), new Isotope (56, 137, 11.23), new Isotope (56, 138, 71.70) },
            nameMap: new Dictionary<string,string>() { { "es","Bario" }, { "fr","Baryum" }, { "it","Bario" }, { "ru","Барий" } }
        );

        /// <summary>Chemical element 57.</summary>
        public static Nuclide Lanthanum { get; } = new Nuclide
        (
            57, "La", nameof (Lanthanum), Category.Lanthan, 6, 0,
            melt: 1193.0, boil: 3737.0,
            weight: 138.91,
            known: 1838, credit: "Carl Gustaf Mosander",
            naming: "From Greek λανθάνειν (lanthanein), meaning to lie hidden",
            isotopes: new Isotope[] { new Isotope (57, 137, null, Decay.ECap1, 6E4, 'y'), new Isotope (57, 138, 0.089, Decay.ECap1|Decay.BetaMinus, 1.05E11, 'y'), new Isotope (57, 139, 99.911) },
            nameMap: new Dictionary<string,string>() { { "de","Lanthan" }, { "es","Lantano" }, { "fr","Lanthane" }, { "it","Lantanio" }, { "ru","Лантан" } }
        );

        /// <summary>Chemical element 58.</summary>
        public static Nuclide Cerium { get; } = new Nuclide
        (
            58, "Ce", nameof (Cerium), Category.Lanthan, 6, 0,
            melt: 1068.0, boil: 3716.0,
            weight: 140.12,
            known: 1803, credit: "Martin Heinrich Klaproth, Jöns Jakob Berzelius, Wilhelm Hisinger",
            naming: "After the dwarf planet Ceres",
            isotopes: new Isotope[] { new Isotope (58, 134, null, Decay.ECap1, 3.16, 'd'), new Isotope (58, 136, 0.186), new Isotope (58, 138, 0.251), new Isotope (58, 139, null, Decay.ECap1, 137.640, 'd'), new Isotope (58, 140, 88.449), new Isotope (58, 141, null, Decay.BetaMinus, 32.501, 'd'), new Isotope (58, 142, 11.114), new Isotope (58, 143, null, Decay.BetaMinus, 33.039, 'h'), new Isotope (58, 144, null, Decay.BetaMinus, 284.893, 'd') },
            nameMap: new Dictionary<string,string>() { { "de","Cer" }, { "es","Cerio" }, { "fr","Cérium" }, { "it","Cerio" }, { "ru","Церий" } }
        );

        /// <summary>Chemical element 59.</summary>
        public static Nuclide Praseodymium { get; } = new Nuclide
        (
            59, "Pr", nameof (Praseodymium), Category.Lanthan, 6, 0,
            melt: 1208.0, boil: 3403.0,
            weight: 140.91,
            known: 1885, credit: "Carl Auer von Welsbach",
            naming: "From the Greek πρασιος, meaning 'leek green'",
            isotopes: new Isotope[] { new Isotope (59, 141, 100.0), new Isotope (59, 142, null, Decay.BetaMinus|Decay.ECap1, 19.12, 'h'), new Isotope (59, 143, null, Decay.BetaMinus, 13.57, 'd') },
            nameMap: new Dictionary<string,string>() { { "de","Praseodym" }, { "es","Praseodimio" }, { "fr","Raséodyme" }, { "it","Praseodimio" }, { "ru","Празеодим" } }
        );

        /// <summary>Chemical element 60.</summary>
        public static Nuclide Neodymium { get; } = new Nuclide
        (
            60, "Nd", nameof (Neodymium), Category.Lanthan, 6, 0,
            melt: 1297.0, boil: 3347.0,
            weight: 144.24,
            known: 1885, credit: "Carl Auer von Welsbach",
            naming: "From the Greek νέος (neos) and διδύμος (didymos), meaning 'new twin'",
            isotopes: new Isotope[] { new Isotope (60, 142, 27.2), new Isotope (60, 143, 12.2), new Isotope (60, 144, 23.8, Decay.Alpha, 2.29E15, 'y'), new Isotope (60, 145, 8.3), new Isotope (60, 146, 17.2), new Isotope (60, 148, 5.8, Decay.Beta2, 2.7E18, 'y'), new Isotope (60, 150, 5.6, Decay.Beta2, 21.0E18, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Neodym" }, { "es","Neodimio" }, { "fr","Néodyme" }, { "it","Neodimio" }, { "ru","Неодим" } }
        );

        /// <summary>Chemical element 61.</summary>
        public static Nuclide Promethium { get; } = new Nuclide
        (
            61, "Pm", nameof (Promethium), Category.Lanthan, 6, 0,
            melt: 1315.0, boil: 3273.0,
            weight: 145,
            known: 1942, credit: "Chien Shiung Wu, Emilio Segrè, Hans Bethe",
            naming: "After Prometheus, a Titan in Greek mythology",
            isotopes: new Isotope[] { new Isotope (61, 145, 0.0, Decay.ECap1, 17.7, 'y'), new Isotope (61, 146, null, Decay.ECap1|Decay.BetaMinus, 5.53, 'y'), new Isotope (61, 147, 0.0, Decay.BetaMinus, 2.6234, 'y'), new Isotope (61, 148, null, Decay.BetaMinus, 5.368, 'd'), new Isotope (61, 149, null, Decay.BetaMinus, 53.08, 'h'), new Isotope (61, 150, null, Decay.BetaMinus, 2.68, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Prometio" }, { "fr","Prométhium" }, { "it","Promezio" }, { "ru","Прометий" } }
        );

        /// <summary>Chemical element 62.</summary>
        public static Nuclide Samarium { get; } = new Nuclide
        (
            62, "Sm", nameof (Samarium), Category.Lanthan, 6, 0,
            melt: 1345.0, boil: 2173.0,
            weight: 150.36,
            known: 1879, credit: "Lecoq de Boisbaudran",
            naming: "After the mineral samarskite",
            isotopes: new Isotope[] { new Isotope (62, 144, 3.08), new Isotope (62, 145, null, Decay.ECap1, 340.0, 'd'), new Isotope (62, 146, null, Decay.Alpha, 6.8E7, 'y'), new Isotope (62, 147, 15.0, Decay.Alpha, 1.06E11, 'y'), new Isotope (62, 148, 11.25, Decay.Alpha, 7E15, 'y'), new Isotope (62, 149, 13.82), new Isotope (62, 150, 7.37), new Isotope (62, 151, null, Decay.BetaMinus, 90, 'y'), new Isotope (62, 152, 26.74), new Isotope (62, 153, null, Decay.BetaMinus, 46.284, 'h'), new Isotope (62, 154, 22.74) },
            nameMap: new Dictionary<string,string>() { { "es","Samario" }, { "it","Samario" }, { "ru","Самарий" } }
        );

        /// <summary>Chemical element 63.</summary>
        public static Nuclide Europium { get; } = new Nuclide
        (
            63, "Eu", nameof (Europium), Category.Lanthan, 6, 0,
            melt: 1099.0, boil: 1802.0,
            weight: 151.96,
            known: 1896, credit: "Eugène-Anatole Demarçay",
            naming: "After the continent of Europe",
            isotopes: new Isotope[] { new Isotope (63, 150, null, Decay.ECap1, 36.9, 'y'), new Isotope (63, 151, 47.8, Decay.Alpha, 5E18, 'y'), new Isotope (63, 152, null, Decay.ECap1|Decay.BetaMinus, 13.54, 'y'), new Isotope (63, 153, 52.2), new Isotope (63, 154, null, Decay.BetaMinus, 8.59, 'y'), new Isotope (63, 155, null, Decay.BetaMinus, 4.76, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Europio" }, { "it","Europio" }, { "ru","Европий" } }
        );

        /// <summary>Chemical element 64.</summary>
        public static Nuclide Gadolinium { get; } = new Nuclide
        (
            64, "Gd", nameof (Gadolinium), Category.Lanthan, 6, 0,
            melt: 1585.0, boil: 3273.0,
            weight: 157.25,
            known: 1880, credit: "Jean Charles Galissard de Marignac",
            naming: "After the mineral gadolinite, itself named for the Finnish chemist Johan Gadolin",
            isotopes: new Isotope[] { new Isotope (64, 148, null, Decay.Alpha, 75, 'y'), new Isotope (64, 150, null, Decay.Alpha, 1.8E6, 'y'), new Isotope (64, 152, 0.20, Decay.Alpha, 1.08E14, 'y'), new Isotope (64, 154, 2.18), new Isotope (64, 155, 14.80), new Isotope (64, 156, 20.47), new Isotope (64, 157, 15.65), new Isotope (64, 158, 24.84), new Isotope (64, 160, 21.86) },
            nameMap: new Dictionary<string,string>() { { "es","Gadolinio" }, { "it","Gadolinio" }, { "ru","Гадолиний" } }
        );

        /// <summary>Chemical element 65.</summary>
        public static Nuclide Terbium { get; } = new Nuclide
        (
            65, "Tb", nameof (Terbium), Category.Lanthan, 6, 0,
            melt: 1629.0, boil: 3396.0,
            weight: 158.93,
            known: 1843, credit: "Carl Gustaf Mosander",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (65, 157, null, Decay.ECap1, 71, 'y'), new Isotope (65, 158, null, Decay.ECap1|Decay.BetaMinus, 180, 'y'), new Isotope (65, 159, 100.0) },
            nameMap: new Dictionary<string,string>() { { "es","Terbio" }, { "it","Terbio" }, { "ru","Тербий" } }
        );

        /// <summary>Chemical element 66.</summary>
        public static Nuclide Dysprosium { get; } = new Nuclide
        (
            66, "Dy", nameof (Dysprosium), Category.Lanthan, 6, 0,
            melt: 1680.0, boil: 2840.0,
            weight: 162.500,
            known: 1886, credit: "Lecoq de Boisbaudran",
            naming: "From the Greek δυσπρόσιτος (dysprositos), meaning 'hard to get'",
            isotopes: new Isotope[] { new Isotope (66, 154, null, Decay.Alpha, 3.0E6, 'y'), new Isotope (66, 156, 0.056), new Isotope (66, 158, 0.095), new Isotope (66, 160, 2.329), new Isotope (66, 161, 18.889), new Isotope (66, 162, 25.475), new Isotope (66, 163, 24.896), new Isotope (66, 164, 28.260) },
            nameMap: new Dictionary<string,string>() { { "es","Disprosio" }, { "it","Disprosio" }, { "ru","Диспрозий" } }
        );

        /// <summary>Chemical element 67.</summary>
        public static Nuclide Holmium { get; } = new Nuclide
        (
            67, "Ho", nameof (Holmium), Category.Lanthan, 6, 0,
            melt: 1734.0, boil: 2873.0,
            weight: 164.930,
            known: 1878, credit: "Jacques-Louis Soret, Marc Delafontaine",
            naming: "after the Latin name for Stockholm",
            isotopes: new Isotope[] { new Isotope (67, 163, null, Decay.ECap1, 4570, 'y'), new Isotope (67, 164, null, Decay.ECap1, 29.0, 'm'), new Isotope (67, 165, 100.0), new Isotope (67, 166, null, Decay.BetaMinus, 26.763, 'h'), new Isotope (67, 167, null, Decay.BetaMinus, 3.1, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Holmio" }, { "it","Olmio" }, { "ru","Гольмий" } }
        );

        /// <summary>Chemical element 68.</summary>
        public static Nuclide Erbium { get; } = new Nuclide
        (
            68, "Er", nameof (Erbium), Category.Lanthan, 6, 0,
            melt: 1802.0, boil: 3141.0,
            weight: 167.26,
            known: 1843, credit: "Carl Gustaf Mosander",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (68, 160, null, Decay.ECap1, 28.58, 'h'), new Isotope (68, 162, 0.139), new Isotope (68, 164, 1.601), new Isotope (68, 165, null, Decay.ECap1, 10.36, 'h'), new Isotope (68, 166, 33.503), new Isotope (68, 167, 22.869), new Isotope (68, 168, 26.978), new Isotope (68, 169, null, Decay.BetaMinus, 9.4, 'd'), new Isotope (68, 170, 14.910), new Isotope (68, 171, null, Decay.BetaMinus, 7.516, 'h'), new Isotope (68, 172, null, Decay.BetaMinus, 49.3, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Erbio" }, { "it","Erbio" }, { "ru","Эрбий" } }
        );

        /// <summary>Chemical element 69.</summary>
        public static Nuclide Thulium { get; } = new Nuclide
        (
            69, "Tm", nameof (Thulium), Category.Lanthan, 6, 0,
            melt: 1818.0, boil: 2223.0,
            weight: 168.93,
            known: 1879, credit: "Per Teodor Cleve",
            naming: "after Thule, a Greek name associated with Scandinavia",
            isotopes: new Isotope[] { new Isotope (69, 167, null, Decay.ECap1, 9.25, 'd'), new Isotope (69, 168, null, Decay.ECap1, 93.0, 'd'), new Isotope (69, 169, 100.0), new Isotope (69, 170, null, Decay.BetaMinus, 128.6, 'd'), new Isotope (69, 171, null, Decay.BetaMinus, 1.92, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Tulio" }, { "it","Tulio" }, { "ru","Тулий" } }
        );

        /// <summary>Chemical element 70.</summary>
        public static Nuclide Ytterbium { get; } = new Nuclide
        (
            70, "Yb", nameof (Ytterbium), Category.Lanthan, 6, 0,
            melt: 1097.0, boil: 1469.0,
            weight: 173.05,
            known: 1878, credit: "Jean Charles Galissard de Marignac",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (70, 166, null, Decay.ECap1, 56.7, 'h'), new Isotope (70, 168, 0.126), new Isotope (70, 169, null, Decay.ECap1, 32.026, 'd'), new Isotope (70, 170, 3.023), new Isotope (70, 171, 14.216), new Isotope (70, 172, 21.754), new Isotope (70, 173, 16.098), new Isotope (70, 174, 31.896), new Isotope (70, 175, null, Decay.BetaMinus, 4.185, 'd'), new Isotope (70, 176, 12.887), new Isotope (70, 177, null, Decay.BetaMinus, 1.911, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Iterbio" }, { "it","Itterbio" }, { "ru","Иттербий" } }
        );

        /// <summary>Chemical element 71.</summary>
        public static Nuclide Lutetium { get; } = new Nuclide
        (
            71, "Lu", nameof (Lutetium), Category.TMetal, 6, 3,
            melt: 1925.0, boil: 3675.0,
            weight: 174.97,
            known: 1906, credit: "Carl Auer von Welsbach, Georges Urbain",
            naming: "From the Latin Lutetia, meaning 'Paris'",
            isotopes: new Isotope[] { new Isotope (71, 173, null, Decay.ECap1, 1.37, 'y'), new Isotope (71, 174, null, Decay.ECap1, 3.31, 'y'), new Isotope (71, 175, 97.401), new Isotope (71, 176, 2.599, Decay.BetaMinus, 3.78, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Lutecio" }, { "fr","Lutétium" }, { "it","Lutezio" }, { "ru","Лютеций" } }
        );

        /// <summary>Chemical element 72.</summary>
        public static Nuclide Hafnium { get; } = new Nuclide
        (
            72, "Hf", nameof (Hafnium), Category.TMetal, 6, 4,
            melt: 2506.0, boil: 4876.0,
            weight: 178.49,
            known: 1922, credit: "Dirk Coster, George de Hevesy",
            naming: "From the Latin Hafnia, meaning 'Copenhagen'",
            isotopes: new Isotope[] { new Isotope (72, 172, null, Decay.ECap1, 1.87, 'y'), new Isotope (72, 174, 0.16, Decay.Alpha, 2E15, 'y'), new Isotope (72, 176, 5.26), new Isotope (72, 177, 18.60), new Isotope (72, 178, 27.28), new Isotope (72, 178, null, Decay.IT, 31, 'y'), new Isotope (72, 179, 13.62), new Isotope (72, 180, 35.08), new Isotope (72, 182, null, Decay.BetaMinus, 8.9E6, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Hafnio" }, { "it","Afnio" }, { "ru","Гафний" } }
        );

        /// <summary>Chemical element 73.</summary>
        public static Nuclide Tantalum { get; } = new Nuclide
        (
            73, "Ta", nameof (Tantalum), Category.TMetal, 6, 5,
            melt: 3290.0, boil: 5731.0,
            weight: 180.95,
            known: 1844, credit: "Heinrich Rose",
            naming: "From Tantalus, the father of Niobe in Greek mythology",
            isotopes: new Isotope[] { new Isotope (73, 177, null, Decay.ECap1, 56.56, 'h'), new Isotope (73, 178, null, Decay.ECap1, 2.36, 'h'), new Isotope (73, 179, null, Decay.ECap1, 1.82, 'y'), new Isotope (73, 180, 0.012), new Isotope (73, 180, null, Decay.ECap1|Decay.BetaMinus, 8.125, 'h'), new Isotope (73, 181, 99.988), new Isotope (73, 182, null, Decay.BetaMinus, 114.43, 'd'), new Isotope (73, 183, null, Decay.BetaMinus, 5.1, 'd') },
            nameMap: new Dictionary<string,string>() { { "de","Tantal" }, { "es","Tantalio" }, { "fr","Tantale" }, { "it","Tantalio" }, { "ru","Тантал" } }
        );

        /// <summary>Chemical element 74.</summary>
        public static Nuclide Tungsten { get; } = new Nuclide
        (
            74, "W", nameof (Tungsten), Category.TMetal, 6, 6,
            melt: 3695.0, boil: 6203.0,
            weight: 183.84,
            known: 1783, credit: "Juan José Elhuyar, Fausto Elhuyar",
            naming: "From the Swedish word for heavy stone",
            isotopes: new Isotope[] { new Isotope (74, 180, 0.12, Decay.Alpha, 1.8E18, 'y'), new Isotope (74, 181, null, Decay.ECap1, 121.2, 'd'), new Isotope (74, 182, 26.50), new Isotope (74, 183, 14.31), new Isotope (74, 184, 30.64), new Isotope (74, 185, null, Decay.BetaMinus, 75.1, 'd'), new Isotope (74, 186, 28.43) },
            nameMap: new Dictionary<string,string>() { { "de","Wolfram" }, { "es","Wolframio" }, { "fr","Tungstène" }, { "it","Tunsteno" }, { "ru","Вольфрам" } }
        );

        /// <summary>Chemical element 75.</summary>
        public static Nuclide Rhenium { get; } = new Nuclide
        (
            75, "Re", nameof (Rhenium), Category.TMetal, 6, 7,
            melt: 3459.0, boil: 5903.0,
            weight: 186.21,
            known: 1908, credit: "Masataka Ogawa",
            naming: "From Latin Rhenus, meaning 'Rhine'",
            isotopes: new Isotope[] { new Isotope (75, 185, 37.4), new Isotope (75, 187, 62.6, Decay.BetaMinus, 4.12E10, 'y') },
            nameMap: new Dictionary<string,string>() { { "de","Rhénium" }, { "es","Renio" }, { "it","Renio" }, { "ru","Рений" } }
        );

        /// <summary>Chemical element 76.</summary>
        public static Nuclide Osmium { get; } = new Nuclide
        (
            76, "Os", nameof (Osmium), Category.TMetal, 6, 8,
            melt: 3306.0, boil: 5285.0,
            weight: 190.23,
            known: 1803, credit: "Smithson Tennant",
            naming: "After Greek osme, meaning 'a smell'",
            isotopes: new Isotope[] { new Isotope (76, 184, 0.02), new Isotope (76, 185, null, Decay.ECap1, 93.6, 'd'), new Isotope (76, 186, 1.59, Decay.Alpha, 2.0E15, 'y'), new Isotope (76, 187, 1.96), new Isotope (76, 188, 13.24), new Isotope (76, 189, 16.15), new Isotope (76, 190, 26.26), new Isotope (76, 191, null, Decay.BetaMinus, 15.4, 'd'), new Isotope (76, 192, 40.78), new Isotope (76, 193, null, Decay.BetaMinus, 30.11, 'd'), new Isotope (76, 194, null, Decay.BetaMinus, 6, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Osmio" }, { "it","Osmio" }, { "ru","Осмий" } }
        );

        /// <summary>Chemical element 77.</summary>
        public static Nuclide Iridium { get; } = new Nuclide
        (
            77, "Ir", nameof (Iridium), Category.TMetal, 6, 9,
            melt: 2719.0, boil: 4403.0,
            weight: 192.22,
            known: 1803, credit: "Smithson Tennant",
            naming: "After the Greek goddess Iris, personification of the rainbow",
            isotopes: new Isotope[] { new Isotope (77, 188, null, Decay.ECap1, 1.73, 'd'), new Isotope (77, 189, null, Decay.ECap1, 13.2, 'd'), new Isotope (77, 190, null, Decay.ECap1, 11.8, 'd'), new Isotope (77, 191, 37.3), new Isotope (77, 192, null, Decay.ECap1, 73.827, 'd'), new Isotope (77, 192, null, Decay.IT, 241, 'y'), new Isotope (77, 193, 62.7), new Isotope (77, 193, null, Decay.IT, 10.5, 'd'), new Isotope (77, 194, null, Decay.BetaMinus, 19.3, 'h'), new Isotope (77, 194, null, Decay.IT, 171.0, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Iridio" }, { "it","Iridio" }, { "ru","Иридий" } }
        );

        /// <summary>Chemical element 78.</summary>
        public static Nuclide Platinum { get; } = new Nuclide
        (
            78, "Pt", nameof (Platinum), Category.TMetal, 6, 10,
            melt: 2041.4, boil: 4098.0,
            weight: 195.08,
            known: 1735, credit: "Antonio de Ulloa",
            naming: "From the Spanish platina, meaning 'silver'",
            isotopes: new Isotope[] { new Isotope (78, 190, 0.012, Decay.Alpha, 6.5E11, 'y'), new Isotope (78, 192, 0.782), new Isotope (78, 193, null, Decay.ECap1, 50, 'y'), new Isotope (78, 194, 32.864), new Isotope (78, 195, 33.775), new Isotope (78, 196, 25.211), new Isotope (78, 198, 7.356) },
            nameMap: new Dictionary<string,string>() { { "de","Platin" }, { "es","Platino" }, { "fr","Platine" }, { "it","Platino" }, { "ru","Платина" } }
        );

        /// <summary>Chemical element 79.</summary>
        public static Nuclide Gold { get; } = new Nuclide
        (
            79, "Au", nameof (Gold), Category.TMetal, 6, 11,
            melt: 1337.33, boil: 3243.0,
            weight: 196.97,
            known: 0,
            naming: "From Proto-Germanic gulþą",
            isotopes: new Isotope[] { new Isotope (79, 195, null, Decay.ECap1, 186.10, 'd'), new Isotope (79, 196, null, Decay.ECap1|Decay.BetaMinus, 6.183, 'd'), new Isotope (79, 197, 100.0), new Isotope (79, 198, null, Decay.BetaMinus, 2.69517, 'd'), new Isotope (79, 199, null, Decay.BetaMinus, 3.169, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Oro" }, { "fr","Or" }, { "it","Oro" }, { "ru","Золото" } }
        );

        /// <summary>Chemical element 80.</summary>
        public static Nuclide Mercury { get; } = new Nuclide
        (
            80, "Hg", nameof (Mercury), Category.TMetal, 6, 12,
            melt: 234.3210, boil: 629.88,
            weight: 200.59,
            known: 0,
            naming: "After the Roman god Mercury",
            isotopes: new Isotope[] { new Isotope (80, 194, null, Decay.ECap1, 444, 'y'), new Isotope (80, 195, null, Decay.ECap1, 9.9, 'h'), new Isotope (80, 196, 0.15), new Isotope (80, 197, null, Decay.ECap1, 64.14, 'h'), new Isotope (80, 198, 10.04), new Isotope (80, 199, 16.94), new Isotope (80, 200, 23.14), new Isotope (80, 201, 13.17), new Isotope (80, 202, 29.74), new Isotope (80, 203, null, Decay.BetaMinus, 46.612, 'd'), new Isotope (80, 204, 6.82) },
            nameMap: new Dictionary<string,string>() { { "de","Quecksilber" }, { "es","Mercurio" }, { "fr","Mercure" }, { "it","Mercurio" }, { "ru","Ртуть" } }
        );

        /// <summary>Chemical element 81.</summary>
        public static Nuclide Thallium { get; } = new Nuclide
        (
            81, "Tl", nameof (Thallium), Category.PtMetal, 6, 13,
            melt: 577.0, boil: 1746.0,
            weight: 204.38,
            known: 1861, credit: "William Crookes",
            naming: "From Greek θαλλός (thallos), meaning 'a green shoot or twig'",
            isotopes: new Isotope[] { new Isotope (81, 203, 29.5), new Isotope (81, 204, null, Decay.BetaMinus|Decay.ECap1, 3.78, 'y'), new Isotope (81, 205, 70.5), new Isotope (81, 206, 0.0, Decay.BetaMinus, 252.0, 's'), new Isotope (81, 210, 0.0, Decay.BetaMinus, 78.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Talio" }, { "it","Tallio" }, { "ru","Таллий" } }
        );

        /// <summary>Chemical element 82.</summary>
        public static Nuclide Lead { get; } = new Nuclide
        (
            82, "Pb", nameof (Lead), Category.PtMetal, 6, 14,
            melt: 600.61, boil: 2022.0,
            weight: 207.2,
            life: Nutrition.Absorbed,
            known: 0,
            naming: "From the Old English lēad",
            isotopes: new Isotope[] { new Isotope (82, 204, 1.4), new Isotope (82, 206, 24.1), new Isotope (82, 207, 22.1), new Isotope (82, 208, 52.4), new Isotope (82, 210, 0.0, Decay.BetaMinus, 22.3, 'y'), new Isotope (82, 214, 0.0, Decay.BetaMinus, 26.8, 'm') },
            nameMap: new Dictionary<string,string>() { { "de","Blei" }, { "es","Plomo" }, { "fr","Plomb" }, { "it","Piombo" }, { "ru","Свинец" } }
        );

        /// <summary>Chemical element 83.</summary>
        public static Nuclide Bismuth { get; } = new Nuclide
        (
            83, "Bi", nameof (Bismuth), Category.PtMetal, 6, 15,
            melt: 544.7, boil: 1837.0,
            weight: 208.98,
            known: 0,
            naming: "Perhaps related to Old High German hwiz, meaning 'white'",
            isotopes: new Isotope[] { new Isotope (83, 207, null, Decay.BetaPlus, 31.55, 'y'), new Isotope (83, 208, null, Decay.BetaPlus, 3.68E5, 'y'), new Isotope (83, 209, 100.0, Decay.Alpha, 2.01E19, 'y'), new Isotope (83, 210, 0.0, Decay.BetaMinus|Decay.Alpha, 5.012, 'd'), new Isotope (83, 210, 0.0, Decay.Alpha|Decay.BetaMinus, 5.012, 'd'), new Isotope (83, 210, null, Decay.IT|Decay.Alpha, 3.04E6, 'y'), new Isotope (83, 214, 0.0, Decay.Alpha|Decay.BetaMinus, 19.9, 'm') },
            nameMap: new Dictionary<string,string>() { { "de","Bismut" }, { "es","Bismuto" }, { "it","Bismuto" }, { "ru","Висмут" } }
        );

        /// <summary>Chemical element 84.</summary>
        public static Nuclide Polonium { get; } = new Nuclide
        (
            84, "Po", nameof (Polonium), Category.PtMetal, 6, 16,
            melt: 527.0, boil: 1235.0,
            weight: 209,
            known: 1898, credit: "Pierre Curie, Marie Curie",
            naming: "After Latin Polonia, meaning Poland",
            isotopes: new Isotope[] { new Isotope (84, 208, null, Decay.Alpha|Decay.BetaPlus, 2.898, 'y'), new Isotope (84, 209, null, Decay.Alpha|Decay.BetaPlus, 125.2, 'y'), new Isotope (84, 210, 0.0, Decay.Alpha, 138.376, 'd'), new Isotope (84, 214, 0.0, Decay.Alpha, 164.3, 'n'), new Isotope (84, 218, 0.0, Decay.Alpha|Decay.BetaMinus, 3.1, 'm') },
            nameMap: new Dictionary<string,string>() { { "es","Polono" }, { "it","Polonio" }, { "ru","Полоний" } }
        );

        /// <summary>Chemical element 85.</summary>
        public static Nuclide Astatine { get; } = new Nuclide
        (
            85, "At", nameof (Astatine), Category.Halogen, 6, 17,
            melt: 503.0, boil: 503.0,
            weight: 210,
            known: 1940, credit: "Dale R. Corson, Kenneth Ross MacKenzie, Emilio Segrè",
            naming: "From the Greek αστατος (astatos), meaning 'unstable'",
            isotopes: new Isotope[] { new Isotope (85, 209, null, Decay.BetaPlus|Decay.Alpha, 5.41, 'h'), new Isotope (85, 210, null, Decay.BetaPlus|Decay.Alpha, 8.1, 'h'), new Isotope (85, 211, null, Decay.ECap1|Decay.Alpha, 7.21, 'h'), new Isotope (85, 217, null, Decay.Alpha|Decay.BetaMinus, 32.3, 't'), new Isotope (85, 218, null, Decay.Alpha|Decay.BetaMinus, 1.5, 's'), new Isotope (85, 219, null, Decay.Alpha|Decay.BetaMinus, 56.0, 's') },
            nameMap: new Dictionary<string,string>() { { "de","Astat" }, { "es","Astato" }, { "fr","Astate" }, { "it","Astato" }, { "ru","Астат" } }
        );

        /// <summary>Chemical element 86.</summary>
        public static Nuclide Radon { get; } = new Nuclide
        (
            86, "Rn", nameof (Radon), Category.NobleGas, 6, 18,
            melt: 202.0, boil: 211.5,
            weight: 222,
            known: 1899, credit: "Ernest Rutherford, Robert B. Owens",
            naming: "After 'radium emanation'",
            isotopes: new Isotope[] { new Isotope (86, 210, null, Decay.Alpha, 2.4, 'h'), new Isotope (86, 211, null, Decay.ECap1|Decay.Alpha, 14.6, 'h'), new Isotope (86, 219, null, Decay.Alpha, 3.96, 's'), new Isotope (86, 220, null, Decay.Alpha, 55.6, 's'), new Isotope (86, 222, 0.0, Decay.Alpha, 3.8235, 'd'), new Isotope (86, 224, null, Decay.BetaMinus, 1.8, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Radón" }, { "ru","Радон" } }
        );

        /// <summary>Chemical element 87.</summary>
        public static Nuclide Francium { get; } = new Nuclide
        (
            87, "Fr", nameof (Francium), Category.AlMetal, 7, 1,
            melt: 281.0, boil: 890.0,
            weight: 223,
            known: 1939, credit: "Marguerite Perey",
            naming: "After France",
            isotopes: new Isotope[] { new Isotope (87, 212, null, Decay.BetaPlus|Decay.Alpha, 20.0, 'm'), new Isotope (87, 221, 0.0, Decay.Alpha, 4.8, 'm'), new Isotope (87, 222, null, Decay.BetaMinus, 14.2, 'm'), new Isotope (87, 223, 0.0, Decay.BetaMinus|Decay.Alpha, 22.0, 'm') },
            nameMap: new Dictionary<string,string>() { { "es","Francio" }, { "it","Francio" }, { "ru","Франций" } }
        );

        /// <summary>Chemical element 88.</summary>
        public static Nuclide Radium { get; } = new Nuclide
        (
            88, "Ra", nameof (Radium), Category.AlEMetal, 7, 2,
            melt: 973.0, boil: 2010.0,
            weight: 226,
            known: 1898, credit: "Pierre Curie, Marie Curie",
            naming: "From Latin radius, meaning 'ray'",
            isotopes: new Isotope[] { new Isotope (88, 223, 0.0, Decay.Alpha, 11.43, 'd'), new Isotope (88, 224, 0.0, Decay.Alpha, 3.6319, 'd'), new Isotope (88, 225, 0.0, Decay.BetaMinus, 14.9, 'd'), new Isotope (88, 226, 0.0, Decay.Alpha, 1600, 'y'), new Isotope (88, 228, 0.0, Decay.BetaMinus, 5.75, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Radio" }, { "it","Radio" }, { "ru","Радий" } }
        );

        /// <summary>Chemical element 89.</summary>
        public static Nuclide Actinium { get; } = new Nuclide
        (
            89, "Ac", nameof (Actinium), Category.Actin, 7, 0,
            melt: 1500.0, boil: 5800.0,
            weight: 227,
            known: 1899, credit: "André-Louis Debierne",
            naming: "From Greek ακτίνος (aktinos), meaning beam or ray",
            isotopes: new Isotope[] { new Isotope (89, 225, 0.0, Decay.Alpha, 10.0, 'd'), new Isotope (89, 226, null, Decay.BetaMinus|Decay.ECap1|Decay.Alpha, 29.37, 'h'), new Isotope (89, 227, 0.0, Decay.BetaMinus|Decay.Alpha, 21.772, 'y'), new Isotope (89, 228, null, Decay.BetaMinus, 6.13, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Actinio" }, { "it","Attinio" }, { "ru","Актиний" } }
        );

        /// <summary>Chemical element 90.</summary>
        public static Nuclide Thorium { get; } = new Nuclide
        (
            90, "Th", nameof (Thorium), Category.Actin, 7, 0,
            melt: 2023.0, boil: 5061.0,
            weight: 232.0377,
            known: 1829, credit: "Jöns Jakob Berzelius",
            naming: "After the Norse god of thunder 'Thor'",
            isotopes: new Isotope[] { new Isotope (90, 227, 0.0, Decay.Alpha, 18.68, 'd'), new Isotope (90, 228, 0.0, Decay.Alpha, 1.9116, 'y'), new Isotope (90, 229, 0.0, Decay.Alpha, 7917, 'y'), new Isotope (90, 230, 0.02, Decay.Alpha, 75400, 'y'), new Isotope (90, 231, 0.0, Decay.BetaMinus, 25.5, 'h'), new Isotope (90, 232, 99.98, Decay.Alpha, 1.405E10, 'y'), new Isotope (90, 234, 0.0, Decay.BetaMinus, 24.1, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Torio" }, { "it","Torio" }, { "ru","Торий" } }
        );

        /// <summary>Chemical element 91.</summary>
        public static Nuclide Protactinium { get; } = new Nuclide
        (
            91, "Pa", nameof (Protactinium), Category.Actin, 7, 0,
            melt: 1841.0, boil: 4300.0,
            weight: 231.03588,
            known: 1913, credit: "Kasimir Fajans, Oswald Helmuth Göhring",
            naming: "From 'proto-actinium'",
            isotopes: new Isotope[] { new Isotope (91, 229, null, Decay.ECap1, 1.5, 'd'), new Isotope (91, 230, null, Decay.ECap1, 17.4, 'd'), new Isotope (91, 231, 100.0, Decay.Alpha, 3.276E4, 'y'), new Isotope (91, 232, 0.0, Decay.BetaMinus, 1.31, 'd'), new Isotope (91, 233, 0.0, Decay.BetaMinus, 26.967, 'd'), new Isotope (91, 234, 0.0, Decay.BetaMinus, 6.75, 'h'), new Isotope (91, 234, 0.0, Decay.BetaMinus, 1.17, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Protaktinium" }, { "es","Protactinio" }, { "it","Protoattinio" }, { "ru","Протактиний" } }
        );

        /// <summary>Chemical element 92.</summary>
        public static Nuclide Uranium { get; } = new Nuclide
        (
            92, "U", nameof (Uranium), Category.Actin, 7, 0,
            melt: 1405.3, boil: 4404.0,
            weight: 238.02891,
            known: 1789, credit: "Martin Heinrich Klaproth",
            naming: "After the planet Uranus",
            isotopes: new Isotope[] { new Isotope (92, 232, null, Decay.SF|Decay.Alpha, 68.9, 'y'), new Isotope (92, 233, 0.0, Decay.SF|Decay.Alpha, 1.592E5, 'y'), new Isotope (92, 234, 0.005, Decay.SF|Decay.Alpha, 2.455E5, 'y'), new Isotope (92, 235, 0.720, Decay.SF|Decay.Alpha, 7.04E8, 'y'), new Isotope (92, 236, 0.0, Decay.SF|Decay.Alpha, 2.342E7, 'y'), new Isotope (92, 238, 99.274, Decay.SF|Decay.Alpha|Decay.Beta2, 4.468E9, 'y'), new Isotope (92, 240, null, Decay.BetaMinus, 14.1, 'h') },
            nameMap: new Dictionary<string,string>() { { "de","Uran" }, { "es","Uranio" }, { "it","Uranio" }, { "ru","Уран" } }
        );

        /// <summary>Chemical element 93.</summary>
        public static Nuclide Neptunium { get; } = new Nuclide
        (
            93, "Np", nameof (Neptunium), Category.Actin, 7, 0,
            melt: 912.0, boil: 4447.0,
            weight: 237,
            known: 1940, credit: "Edwin McMillan and Philip H. Abelson",
            naming: "After the planet Neptune",
            isotopes: new Isotope[] { new Isotope (93, 235, null, Decay.Alpha|Decay.ECap1, 396.1, 'd'), new Isotope (93, 236, null, Decay.ECap1|Decay.BetaMinus|Decay.Alpha, 1.54E5, 'y'), new Isotope (93, 237, 0.0, Decay.Alpha, 2.144E6, 'y'), new Isotope (93, 238, null, Decay.BetaMinus, 2.117, 'd'), new Isotope (93, 239, 0.0, Decay.BetaMinus, 2.356, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Neptunio" }, { "it","Nettunio" }, { "ru","Нептуний" } }
        );

        /// <summary>Chemical element 94.</summary>
        public static Nuclide Plutonium { get; } = new Nuclide
        (
            94, "Pu", nameof (Plutonium), Category.Actin, 7, 0,
            melt: 912.5, boil: 3505.0,
            weight: 244,
            known: 1940, credit: "Glenn T. Seaborg, Arthur Wahl, Joseph W. Kennedy, Edwin McMillan",
            naming: "After the dwarf planet Pluto",
            isotopes: new Isotope[] { new Isotope (94, 238, 0.0, Decay.SF|Decay.Alpha, 87.74, 'y'), new Isotope (94, 239, 0.0, Decay.SF|Decay.Alpha, 2.41E4, 'y'), new Isotope (94, 240, 0.0, Decay.SF|Decay.Alpha, 6500, 'y'), new Isotope (94, 241, null, Decay.BetaMinus|Decay.SF, 14, 'y'), new Isotope (94, 242, null, Decay.SF|Decay.Alpha, 3.73E5, 'y'), new Isotope (94, 244, 0.0, Decay.SF|Decay.Alpha, 8.08E7, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Plutonio" }, { "it","Plutonio" }, { "ru","Плутоний" } }
        );

        /// <summary>Chemical element 95.</summary>
        public static Nuclide Americium { get; } = new Nuclide
        (
            95, "Am", nameof (Americium), Category.Actin, 7, 0,
            melt: 1449.0, boil: 2880.0,
            weight: 243,
            known: 1944, credit: "Glenn T. Seaborg, Ralph A. James, Leon O. Morgan, Albert Ghiorso",
            naming: "After the Americas",
            isotopes: new Isotope[] { new Isotope (95, 241, null, Decay.SF|Decay.Alpha, 432.2, 'y'), new Isotope (95, 242, null, Decay.IT|Decay.Alpha|Decay.SF, 141, 'y'), new Isotope (95, 243, null, Decay.SF|Decay.Alpha, 7370, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Americio" }, { "fr","Américium" }, { "it","Americio" }, { "ru","Америций" } }
        );

        /// <summary>Chemical element 96.</summary>
        public static Nuclide Curium { get; } = new Nuclide
        (
            96, "Cm", nameof (Curium), Category.Actin, 7, 0,
            melt: 1613.0, boil: 3383.0,
            weight: 247,
            known: 1944, credit: "Glenn T. Seaborg, Ralph A. James, Albert Ghiorso",
            naming: "After Marie Skłodowska-Curie and Pierre Curie",
            isotopes: new Isotope[] { new Isotope (96, 242, null, Decay.SF|Decay.Alpha, 160.0, 'd'), new Isotope (96, 243, null, Decay.Alpha|Decay.ECap1|Decay.SF, 29.1, 'y'), new Isotope (96, 244, null, Decay.SF|Decay.Alpha, 18.1, 'y'), new Isotope (96, 245, null, Decay.SF|Decay.Alpha, 8500, 'y'), new Isotope (96, 246, null, Decay.Alpha|Decay.SF, 4730, 'y'), new Isotope (96, 247, null, Decay.Alpha, 1.56E7, 'y'), new Isotope (96, 248, null, Decay.Alpha|Decay.SF, 3.4E5, 'y'), new Isotope (96, 250, null, Decay.SF|Decay.Alpha|Decay.BetaMinus, 9000, 'y') },
            nameMap: new Dictionary<string,string>() { { "es","Curio" }, { "it","Curio" }, { "ru","Кюрий" } }
        );

        /// <summary>Chemical element 97.</summary>
        public static Nuclide Berkelium { get; } = new Nuclide
        (
            97, "Bk", nameof (Berkelium), Category.Actin, 7, 0,
            melt: 1259.0, boil: 2900.0,
            weight: 247,
            known: 1949, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Berkeley, California",
            isotopes: new Isotope[] { new Isotope (97, 245, null, Decay.ECap1|Decay.Alpha, 4.94, 'd'), new Isotope (97, 246, null, Decay.Alpha|Decay.ECap1, 1.8, 'd'), new Isotope (97, 247, null, Decay.Alpha, 1380, 'y'), new Isotope (97, 248, null, Decay.Alpha, 300, 'y'), new Isotope (97, 249, null, Decay.Alpha|Decay.SF|Decay.BetaMinus, 330.0, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Berkelio" }, { "fr","Berkélium" }, { "it","Berkelio" }, { "ru","Берклий" } }
        );

        /// <summary>Chemical element 98.</summary>
        public static Nuclide Californium { get; } = new Nuclide
        (
            98, "Cf", nameof (Californium), Category.Actin, 7, 0,
            melt: 1173.0, boil: 1743.0,
            weight: 251,
            known: 1950, credit: "Lawrence Berkeley National Laboratory",
            naming: "After California",
            isotopes: new Isotope[] { new Isotope (98, 248, null, Decay.Alpha|Decay.SF, 333.5, 'd'), new Isotope (98, 249, null, Decay.Alpha|Decay.SF, 351, 'y'), new Isotope (98, 250, null, Decay.Alpha|Decay.SF, 13.08, 'y'), new Isotope (98, 251, null, Decay.Alpha, 898, 'y'), new Isotope (98, 252, null, Decay.Alpha|Decay.SF, 2.645, 'y'), new Isotope (98, 253, null, Decay.BetaMinus|Decay.Alpha, 17.81, 'd'), new Isotope (98, 254, null, Decay.SF|Decay.Alpha, 60.5, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Californio" }, { "it","Californio" }, { "ru","Калифорний" } }
        );

        /// <summary>Chemical element 99.</summary>
        public static Nuclide Einsteinium { get; } = new Nuclide
        (
            99, "Es", nameof (Einsteinium), Category.Actin, 7, 0,
            melt: 1133.0, boil: 1269.0,
            weight: 252,
            known: 1952, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Albert Einstein",
            isotopes: new Isotope[] { new Isotope (99, 252, null, Decay.Alpha|Decay.ECap1|Decay.BetaMinus, 471.7, 'd'), new Isotope (99, 253, null, Decay.SF|Decay.Alpha, 20.47, 'd'), new Isotope (99, 254, null, Decay.ECap1|Decay.BetaMinus|Decay.Alpha, 275.7, 'd'), new Isotope (99, 255, null, Decay.BetaMinus|Decay.Alpha|Decay.SF, 39.8, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Einsteinio" }, { "it","Einsteinio" }, { "ru","Эйнштейний" } }
        );

        /// <summary>Chemical element 100.</summary>
        public static Nuclide Fermium { get; } = new Nuclide
        (
            100, "Fm", nameof (Fermium), Category.Actin, 7, 0,
            melt: 1800.0, boil: null,
            weight: 257,
            known: 1952, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Enrico Fermi",
            isotopes: new Isotope[] { new Isotope (100, 252, null, Decay.SF|Decay.Alpha, 25.39, 'h'), new Isotope (100, 253, null, Decay.ECap1|Decay.Alpha, 3.0, 'd'), new Isotope (100, 255, null, Decay.SF|Decay.Alpha, 20.07, 'h'), new Isotope (100, 257, null, Decay.Alpha|Decay.SF, 100.5, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Fermio" }, { "it","Fermio" }, { "ru","Фермий" } }
        );

        /// <summary>Chemical element 101.</summary>
        public static Nuclide Mendelevium { get; } = new Nuclide
        (
            101, "Md", nameof (Mendelevium), Category.Actin, 7, 0,
            melt: 1100.0, boil: null,
            weight: 258,
            known: 1955, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Dmitri Mendeleev",
            isotopes: new Isotope[] { new Isotope (101, 256, null, Decay.ECap1, 1.17, 'h'), new Isotope (101, 257, null, Decay.ECap1|Decay.Alpha|Decay.SF, 5.52, 'h'), new Isotope (101, 258, null, Decay.Alpha|Decay.ECap1|Decay.BetaMinus, 51.5, 'd'), new Isotope (101, 259, null, Decay.SF|Decay.Alpha, 1.6, 'h'), new Isotope (101, 260, null, Decay.SF|Decay.Alpha|Decay.ECap1|Decay.BetaMinus, 31.8, 'd') },
            nameMap: new Dictionary<string,string>() { { "es","Mendelevio" }, { "fr","Mendelévium" }, { "it","Mendelevio" }, { "ru","Менделевий" } }
        );

        /// <summary>Chemical element 102.</summary>
        public static Nuclide Nobelium { get; } = new Nuclide
        (
            102, "No", nameof (Nobelium), Category.Actin, 7, 0,
            melt: 1100.0, boil: null,
            weight: 259,
            known: 1966, credit: "Joint Institute for Nuclear Research",
            naming: "After Alfred Nobel",
            isotopes: new Isotope[] { new Isotope (102, 253, null, Decay.Alpha|Decay.BetaPlus, 1.6, 'm'), new Isotope (102, 254, null, Decay.Alpha|Decay.BetaPlus, 51.0, 's'), new Isotope (102, 255, null, Decay.Alpha|Decay.BetaPlus, 3.1, 'm'), new Isotope (102, 257, null, Decay.Alpha|Decay.BetaPlus, 25.0, 's'), new Isotope (102, 259, null, Decay.Alpha|Decay.ECap1|Decay.SF, 58.0, 'm') },
            nameMap: new Dictionary<string,string>() { { "es","Nobelio" }, { "fr","Nobélium" }, { "it","Nobelio" }, { "ru","Нобелий" } }
        );

        /// <summary>Chemical element 103.</summary>
        public static Nuclide Lawrencium { get; } = new Nuclide
        (
            103, "Lr", nameof (Lawrencium), Category.TMetal, 7, 3,
            melt: 1900.0, boil: null,
            weight: 266,
            known: 1961, credit: "Lawrence Berkeley National Laboratory, Joint Institute for Nuclear Research",
            naming: "After Ernest Lawrence",
            isotopes: new Isotope[] { new Isotope (103, 254, null, Decay.Alpha|Decay.ECap1, 13.0, 's'), new Isotope (103, 255, null, Decay.Alpha, 21.5, 's'), new Isotope (103, 256, null, Decay.Alpha, 27.0, 's'), new Isotope (103, 259, null, Decay.Alpha|Decay.SF, 6.2, 's'), new Isotope (103, 260, null, Decay.Alpha, 2.7, 'm'), new Isotope (103, 261, null, Decay.SF, 44.0, 'm'), new Isotope (103, 262, null, Decay.ECap1, 3.6, 'h'), new Isotope (103, 264, null, Decay.SF, 3.0, 'h'), new Isotope (103, 266, null, Decay.SF, 10.0, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Laurencio" }, { "it","Laurenzio" }, { "ru","Лоуренсий" } }
        );

        /// <summary>Chemical element 104.</summary>
        public static Nuclide Rutherfordium { get; } = new Nuclide
        (
            104, "Rf", nameof (Rutherfordium), Category.TMetal, 7, 4,
            melt: 2400.0, boil: null,
            weight: 267,
            known: 1964, credit: "Joint Institute for Nuclear Research, Lawrence Berkeley National Laboratory",
            naming: "After Ernest Rutherford",
            isotopes: new Isotope[] { new Isotope (104, 261, null, Decay.Alpha|Decay.ECap1|Decay.SF, 70.0, 's'), new Isotope (104, 263, null, Decay.Alpha|Decay.SF, 15.0, 'm'), new Isotope (104, 265, null, Decay.SF, 1.1, 'm'), new Isotope (104, 266, null, Decay.SF, 23.0, 's'), new Isotope (104, 267, null, Decay.SF, 1.3, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Rutherfordio" }, { "it","Rutherfordio" }, { "ru","Резерфордий" } }
        );

        /// <summary>Chemical element 105.</summary>
        public static Nuclide Dubnium { get; } = new Nuclide
        (
            105, "Db", nameof (Dubnium), Category.TMetal, 7, 5,
            melt: null,
            weight: 268,
            known: 1970, credit: "Lawrence Berkeley Laboratory, Joint Institute for Nuclear Research",
            naming: "After Dubna, Moscow Oblast, Russia",
            isotopes: new Isotope[] { new Isotope (105, 262, null, Decay.Alpha|Decay.SF, 34.0, 's'), new Isotope (105, 263, null, Decay.SF|Decay.Alpha|Decay.ECap1, 27.0, 's'), new Isotope (105, 266, null, Decay.SF, 20.0, 'm'), new Isotope (105, 267, null, Decay.SF, 1.2, 'h'), new Isotope (105, 268, null, Decay.SF, 28.0, 'h'), new Isotope (105, 270, null, Decay.SF|Decay.Alpha, 15.0, 'h') },
            nameMap: new Dictionary<string,string>() { { "es","Dubnio" }, { "it","Dubnio" }, { "ru","Дубний" } }
        );

        /// <summary>Chemical element 106.</summary>
        public static Nuclide Seaborgium { get; } = new Nuclide
        (
            106, "Sg", nameof (Seaborgium), Category.TMetal, 7, 6,
            melt: null,
            weight: 269,
            known: 1974, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Glenn Seaborg",
            isotopes: new Isotope[] { new Isotope (106, 265, null, Decay.Alpha, 8.9, 's'), new Isotope (106, 267, null, Decay.Alpha, 1.4, 'm'), new Isotope (106, 269, null, Decay.Alpha, 14.0, 'm'), new Isotope (106, 271, null, Decay.Alpha|Decay.SF, 1.6, 'm') },
            nameMap: new Dictionary<string,string>() { { "es","Seaborgio" }, { "it","Seaborgio" }, { "ru","Сиборгий" } }
        );

        /// <summary>Chemical element 107.</summary>
        public static Nuclide Bohrium { get; } = new Nuclide
        (
            107, "Bh", nameof (Bohrium), Category.TMetal, 7, 7,
            melt: null,
            weight: 270,
            known: 1981, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Niels Bohr",
            isotopes: new Isotope[] { new Isotope (107, 267, null, Decay.Alpha, 17.0, 's'), new Isotope (107, 270, null, Decay.Alpha, 60.0, 's'), new Isotope (107, 271, null, Decay.Alpha, 1.5, 's'), new Isotope (107, 272, null, Decay.Alpha, 11.0, 's'), new Isotope (107, 274, null, Decay.Alpha, 44.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Bohrio" }, { "it","Bohrio" }, { "ru","Борий" } }
        );

        /// <summary>Chemical element 108.</summary>
        public static Nuclide Hassium { get; } = new Nuclide
        (
            108, "Hs", nameof (Hassium), Category.TMetal, 7, 8,
            melt: null,
            weight: 269,
            known: 1984, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Latin Hassia, for Hesse, Germany",
            isotopes: new Isotope[] { new Isotope (108, 269, null, Decay.Alpha, 16.0, 's'), new Isotope (108, 271, null, Decay.Alpha, 9.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Hassio" }, { "it","Hassio" }, { "ru","Хассий" } }
        );

        /// <summary>Chemical element 109.</summary>
        public static Nuclide Meitnerium { get; } = new Nuclide
        (
            109, "Mt", nameof (Meitnerium), Category.TMetal, 7, 9,
            melt: null,
            weight: 278,
            known: 1982, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Lise Meitner",
            isotopes: new Isotope[] { new Isotope (109, 274, null, Decay.Alpha, 0.4, 's'), new Isotope (109, 276, null, Decay.Alpha, 0.6, 's'), new Isotope (109, 278, null, Decay.Alpha, 4.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Meitnerio" }, { "fr","Meitnérium" }, { "it","Meitnerio" }, { "ru","Мейтнерий" } }
        );

        /// <summary>Chemical element 110.</summary>
        public static Nuclide Darmstadtium { get; } = new Nuclide
        (
            110, "Ds", nameof (Darmstadtium), Category.TMetal, 7, 10,
            melt: null,
            weight: 281,
            known: 1994, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Darmstadt, Germany",
            isotopes: new Isotope[] { new Isotope (110, 279, null, Decay.Alpha|Decay.SF, 0.2, 's'), new Isotope (110, 281, null, Decay.SF|Decay.Alpha, 14.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Darmstadtio" }, { "it","Darmstadtio" }, { "ru","Дармштадтий" } }
        );

        /// <summary>Chemical element 111.</summary>
        public static Nuclide Roentgenium { get; } = new Nuclide
        (
            111, "Rg", nameof (Roentgenium), Category.TMetal, 7, 11,
            melt: null,
            weight: 282,
            known: 1994, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Wilhelm Röntgen",
            isotopes: new Isotope[] { new Isotope (111, 272, null, Decay.Alpha, 2.0, 't'), new Isotope (111, 274, null, Decay.Alpha, 12.0, 't'), new Isotope (111, 278, null, Decay.Alpha, 4.0, 't'), new Isotope (111, 279, null, Decay.Alpha, 0.09, 's'), new Isotope (111, 280, null, Decay.Alpha, 4.6, 's'), new Isotope (111, 281, null, Decay.SF|Decay.Alpha, 17.0, 's'), new Isotope (111, 282, null, Decay.Alpha, 100.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Roentgenio" }, { "it","Roentgenio" }, { "ru","Рентгений" } }
        );

        /// <summary>Chemical element 112.</summary>
        public static Nuclide Copernicium { get; } = new Nuclide
        (
            112, "Cn", nameof (Copernicium), Category.TMetal, 7, 12,
            melt: null,
            weight: 285,
            known: 1996, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Nicolaus Copernicus",
            isotopes: new Isotope[] { new Isotope (112, 277, null, Decay.Alpha, 0.69, 't'), new Isotope (112, 281, null, Decay.Alpha, 0.18, 's'), new Isotope (112, 282, null, Decay.SF, 0.91, 't'), new Isotope (112, 283, null, Decay.Alpha|Decay.SF, 4.2, 's'), new Isotope (112, 284, null, Decay.SF|Decay.Alpha, 98.0, 't'), new Isotope (112, 285, null, Decay.Alpha, 28.0, 's'), new Isotope (112, 286, null, Decay.SF, 8.45, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Copernicio" }, { "it","Copernicio" }, { "ru","Коперниций" } }
        );

        /// <summary>Chemical element 113.</summary>
        public static Nuclide Nihonium { get; } = new Nuclide
        (
            113, "Nh", nameof (Nihonium), Category.PtMetal, 7, 13,
            melt: 700.0, boil: 1430.0,
            weight: 286,
            known: 2003, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Nihon (Japan)",
            isotopes: new Isotope[] { new Isotope (113, 278, null, Decay.Alpha, 1.4, 't'), new Isotope (113, 282, null, Decay.Alpha, 73.0, 't'), new Isotope (113, 283, null, Decay.Alpha, 75.0, 't'), new Isotope (113, 284, null, Decay.Alpha|Decay.ECap1, 0.91, 's'), new Isotope (113, 285, null, Decay.Alpha, 4.2, 's'), new Isotope (113, 286, null, Decay.Alpha, 9.5, 's'), new Isotope (113, 287, null, Decay.Alpha, 5.5, 's'), new Isotope (113, 290, null, Decay.Alpha, 2.0, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Nihonio" }, { "it","Nihonio" }, { "ru","Нихоний" } }
        );

        /// <summary>Chemical element 114.</summary>
        public static Nuclide Flerovium { get; } = new Nuclide
        (
            114, "Fl", nameof (Flerovium), Category.PtMetal, 7, 14,
            melt: null, boil: 210.0,
            weight: 289,
            known: 1999, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Flerov Laboratory of Nuclear Reactions",
            isotopes: new Isotope[] { new Isotope (114, 284, null, Decay.SF, 2.5, 't'), new Isotope (114, 285, null, Decay.Alpha, 0.10, 's'), new Isotope (114, 286, null, Decay.Alpha|Decay.SF, 0.12, 's'), new Isotope (114, 287, null, Decay.Alpha, 0.48, 's'), new Isotope (114, 288, null, Decay.Alpha, 0.66, 's'), new Isotope (114, 289, null, Decay.Alpha, 1.9, 's') },
            nameMap: new Dictionary<string,string>() { { "es","Flerovio" }, { "fr","Flérovium" }, { "it","Flerovio" }, { "ru","Флеровий" } }
        );

        /// <summary>Chemical element 115.</summary>
        public static Nuclide Moscovium { get; } = new Nuclide
        (
            115, "Mc", nameof (Moscovium), Category.PtMetal, 7, 15,
            melt: 670.0, boil: 1400.0,
            weight: 290,
            known: 2003, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Moscow",
            isotopes: new Isotope[] { new Isotope (115, 287, null, Decay.Alpha, 37.0, 't'), new Isotope (115, 288, null, Decay.Alpha, 164.0, 't'), new Isotope (115, 289, null, Decay.Alpha, 330.0, 't'), new Isotope (115, 290, null, Decay.Alpha, 650.0, 't') },
            nameMap: new Dictionary<string,string>() { { "es","Moscovio" }, { "it","Moscovio" }, { "ru","Московий" } }
        );

        /// <summary>Chemical element 116.</summary>
        public static Nuclide Livermorium { get; } = new Nuclide
        (
            116, "Lv", nameof (Livermorium), Category.PtMetal, 7, 16,
            melt: 673.0, boil: 1035.0,
            weight: 293,
            known: 2000, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Lawrence Livermore National Laboratory",
            isotopes: new Isotope[] { new Isotope (116, 290, null, Decay.Alpha, 8.3, 't'), new Isotope (116, 291, null, Decay.Alpha, 19.0, 't'), new Isotope (116, 292, null, Decay.Alpha, 13.0, 't'), new Isotope (116, 293, null, Decay.Alpha, 57.0, 't'), new Isotope (116, 294, null, Decay.Alpha, 54.0, 't') },
            nameMap: new Dictionary<string,string>() { { "es","Livermorio" }, { "it","Livermorio" }, { "ru","Ливерморий" } }
        );

        /// <summary>Chemical element 117.</summary>
        public static Nuclide Tennessine { get; } = new Nuclide
        (
            117, "Ts", nameof (Tennessine), Category.Halogen, 7, 17,
            melt: 623.0, boil: 883.0,
            weight: 294,
            known: 2009, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory, Vanderbilt University, Oak Ridge National Laboratory",
            naming: "After Tennessee",
            isotopes: new Isotope[] { new Isotope (117, 293, null, Decay.Alpha, 22.0, 't'), new Isotope (117, 294, null, Decay.Alpha, 51.0, 't') },
            nameMap: new Dictionary<string,string>() { { "es","Teneso" }, { "fr","Tennesse" }, { "it","Tennesso" }, { "ru","Теннессин" } }
        );

        /// <summary>Chemical element 118.</summary>
        public static Nuclide Oganesson { get; } = new Nuclide
        (
            118, "Og", nameof (Oganesson), Category.NobleGas, 7, 18,
            melt: 325.0, boil: 350.0,
            weight: 294,
            known: 2002, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Yuri Oganessian",
            isotopes: new Isotope[] { new Isotope (118, 294, null, Decay.SF|Decay.Alpha, 700.0, 'i') },
            nameMap: new Dictionary<string,string>() { { "es","Oganesón" }, { "ru","Оганесон" } }
        );

        private static readonly Nuclide[] _nuclides = new Nuclide[]
        {
            Neutron,
            Hydrogen, Helium, Lithium, Beryllium,
            Boron, Carbon, Nitrogen, Oxygen, Fluorine, Neon, Sodium, Magnesium,
            Aluminium, Silicon, Phosphorus, Sulfur, Chlorine, Argon, Potassium, Calcium,
            Scandium, Titanium, Vanadium, Chromium, Manganese, Iron, Cobalt, Nickel, Copper, Zinc,
            Gallium, Germanium, Arsenic, Selenium, Bromine, Krypton, Rubidium, Strontium,
            Yttrium, Zirconium, Niobium, Molybdenum, Technetium, Ruthenium, Rhodium, Palladium, Silver, Cadmium,
            Indium, Tin, Antimony, Tellurium, Iodine, Xenon, Caesium, Barium,
            Lanthanum, Cerium, Praseodymium, Neodymium, Promethium, Samarium, Europium, Gadolinium, Terbium, Dysprosium, Holmium, Erbium, Thulium, Ytterbium,
            Lutetium, Hafnium, Tantalum, Tungsten, Rhenium, Osmium, Iridium, Platinum, Gold, Mercury,
            Thallium, Lead, Bismuth, Polonium, Astatine, Radon, Francium, Radium,
            Actinium, Thorium, Protactinium, Uranium, Neptunium, Plutonium, Americium, Curium, Berkelium, Californium, Einsteinium, Fermium, Mendelevium, Nobelium,
            Lawrencium, Rutherfordium, Dubnium, Seaborgium, Bohrium, Hassium, Meitnerium, Darmstadtium, Roentgenium, Copernicium,
            Nihonium, Flerovium, Moscovium, Livermorium, Tennessine, Oganesson
        };

        private static readonly string[] _okLanguages = new string[] { "de", "en", "en-GB", "en-US", "es", "fr", "it", "ru" };
        private static readonly Dictionary<string,int> _maxNameLengths = new Dictionary<string,int>();

        static Nuclide()
        {
            foreach (var lg in _okLanguages)
                _maxNameLengths.Add (lg, 0);

            foreach (var nuc in _nuclides)
            {
                foreach (var lg in _okLanguages)
                {
                    var name = nuc.GetName (lg);
                    if (_maxNameLengths[lg] < name.Length)
                        _maxNameLengths[lg] = name.Length;
                }

                nuc.Occurrence = Origin.Synthetic;
                foreach (var iso in nuc.Isotopes)
                {
                    var occr = iso.Occurrence;
                    if (nuc.Occurrence < occr)
                        nuc.Occurrence = occr;
                }
            }
        }

        /// <summary>Returns the longest length of any element name for each language.</summary>
        public static ReadOnlyDictionary<string,int> MaxNameLengths { get; }
        = new ReadOnlyDictionary<string,int> (_maxNameLengths);

        /// <summary>Provides category group names suitable for legends.</summary>
        public static ReadOnlyDictionary<string,string[]> CategoryGroupNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Metalle", "Halbmetall", "Nichtmetalle" } },
                { "en", new string[] { "Metal", "Metalloid", "Nonmetal" } },
                { "es", new string[] { "Metales", "Metaloide", "No metales" } },
                { "fr", new string[] { "Métaux", "Métalloïde", "Non-métaux" } },
                { "it", new string[] { "Metalli", "Metalloide", "Nonmetalli" } },
                { "ru", new string[] { "Металлы", "Металлоид", "Неметаллы" } }
            });

        /// <summary>Provides localized category terms suitable for legends.</summary>
        /// <remarks>These terms are in singular form, sentence casing, and favor IUPAC recommendations.</remarks>
        public static ReadOnlyDictionary<string,string[]> CategoryNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                {
                    "de", new string[]
                    {
                        "Alkalimetall", "Erdalkalimetall", "Lanthanoid", "Actinoid",
                        "Übergangsmetall", "Metall",
                        "Halbmetall", "Nichtmetall", "Halogen", "Edelgas"
                    }
                },
                {
                    "en", new string[]
                    {
                        "Alkali metal", "Alkaline earth metal", "Lanthanoid", "Actinoid",
                        "Transition metal", "Post-transition metal",
                        "Metalloid", "Other nonmetal", "Halogen", "Noble gas"
                    }
                },
                {
                    "es", new string[]
                    {
                        "Metal alcalino", "Metal alcalinotérreo", "Lantánido", "Actínido",
                        "Metal de transición", "Otro metal",
                        "Metaloide", "Otros no metal", "Halógeno", "Gas noble"
                    }
                },
                {
                    "fr", new string[]
                    {
                        "Métal alcalin", "Métal alcalino-terreux", "Lanthanide", "Actinide",
                        "Métal de transition", "Métal pauvre",
                        "Métalloïde", "Autres non-métal", "Halogène", "Gaz noble"
                    }
                },
                {
                    "it", new string[]
                    {
                        "Metallo alcalino", "Metallo alcalino terroso", "Lantanide", "Attinide",
                        "Metallo di transizione", "metallo post-transizione",
                        "Metalloide", "Poliatomici", "Alogena", "Gas nobile"
                    }
                },
                {
                    "ru", new string[]
                    {
                        "Щелочные металлы", "Щёлочноземельные металлы", "Лантаноид", "Актинид",
                        "Переходный металл", "Постпе-реходные",
                        "Металлоид", "Другие неметаллы", "Галогены", "Благородные газы"
                    }
                }
            });

        /// <summary>Provides language translations of column headings to an isotope chart.</summary>
        public static ReadOnlyDictionary<string,string[]> IsotopesHeadings { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Isotop", "NH", ",5t", "ZA", "Zerfallsprodukt" } },
                { "en", new string[] { "Isotope", "Natural Abundance", "Half-life", "Decay Mode", "Product" } },
                { "es", new string[] { "Isótopo", "Abundancia natural", "Periodo", "MD", "Producto" } },
                { "fr", new string[] { "Isotope", "Abondance naturelle", "Période", "MD", "Produit" } },
                { "it", new string[] { "Isotopo", "Abbondanza in natura", "TD", "DM", "Prodotto" } },
                { "ru", new string[] { "Изотоп", "Распространенность", "Период полураспада", "Режим распада", "Продукт" } }
            });

        /// <summary>Returns all the possible characters that may be returned by the <see cref="Isotope.DecayMode"/> bitflag property.</summary>
        public static string DecayModeCodes
         => "apbBeEngTCF";

        /// <summary>Provide language translations of decay modes.</summary>
        public static ReadOnlyDictionary<string,string[]> DecayModeNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Alpha-Zerfall", "Beta-Plus-Zerfall", "Beta-Minus-Zerfall", "Doppelter Beta-Minus-Zerfall", "Elektroneneinfang", "Doppelter Elektroneneinfang", "Neutronenemission", "Gamma-Zerfall", "Innere Konversion", "Isomerie-Übergang", "Spontane Spaltung" } },
                { "en", new string[] { "Alpha decay", "Beta plus decay", "Beta minus decay", "Double beta minus decay", "Electron capture", "Double electron capture", "Neutron emission", "Gamma decay", "Internal Conversion", "Isomeric transition", "Spontaneous fission" } },
                { "es", new string[] { "Desintegración alfa", "Emisión de positrones", "Desintegración beta", "Doble desintegración beta", "Captura electrónica", "Captura de doble electrón", "Emisión de neutrones", "Transición isomérica", "Conversión interna", "Transición isomérica", "Fisión espontánea" } },
                { "fr", new string[] { "Radioactivité α", "Émission de positron", "Rayonnement β-", "Double désintégration bêta", "Capture électronique", "Double capture électronique", "Émission de neutron", "Rayonnement γ", "Conversion interne", "Isomérie nucléaire", "Fission spontanée" } },
                { "it", new string[] { "Decadimento alfa", "Emissione di positroni", "Decadimento beta", "Doppio decadimento beta", "Cattura elettronica", "Doppia cattura elettronica", "Emissione di neutroni", "Transizione isomerica", "Conversione interna", "Transizione isomerica", "Fissione spontanea" } },
                { "ru", new string[] { "Альфа распад", "Бета плюс распад", "Бета минус распад", "Двойной бета минус распад", "Электронный захват", "Двойной электронный захват", "Нейтронная эмиссия", "Гамма-распад", "Внутренняя конверсия", "Изомерия атомных ядер", "Спонтанное деление" } }
            });

        /// <summary>Returns all the possible characters that may be returned by the <see cref="Isotope.DecayMode"/> bitflag property.</summary>
        public static ReadOnlyCollection<string> DecayModeSymbols { get; } = new ReadOnlyCollection<string>(new string[]
          { "α", "β+", "β−", "β−β−", "ε", "εε", "n", "γ", "IT", "IC", "SF" });

        /// <summary>Returns the number of available decay mode flags.</summary>
        public static int DecayModeCount { get; } = Enum.GetValues (typeof (Decay)).Cast<int>().Count() - 1;

        /// <summary>1- or 2-character codes for values of Nutrition.</summary>
        public static ReadOnlyCollection<string> LifeCodes { get; } = new ReadOnlyCollection<string> (new string[]
        { "", "EB", "ET", "BT", "A" });

        /// <summary>One-line descriptions for values of Nutrition.</summary>
        public static ReadOnlyDictionary<string,string[]> LifeDescriptions { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                {
                    "de", new string[]
                    {
                        "Wesentliches Element (> ,1 Massenprozent)",
                        "Wesentliches Element (< ,1 Massenprozent)",
                        "Nützliches Spurenelement",
                        "Element absorbiert, aber nicht verwendet",
                        "Nicht absorbiert"
                    }
                },
                {
                    "en", new string[]
                    {
                        "Essential element (> .1% by mass)",
                        "Essential element (< .1% by mass)",
                        "Beneficial trace element",
                        "Nonbeneficial trace element",
                        "Not absorbed"
                    }
                },
                {
                    "es", new string[]
                    {
                        "Elemento esencial (> ,1% en masa)",
                        "Elemento esencial (< ,1% en masa)",
                        "Oligoelemento beneficioso",
                        "Oligoelemento no beneficioso",
                        "No absorbido"
                    }
                },
                {
                    "fr", new string[]
                    {
                        "Élément essentiel (> ,1% en masse)",
                        "Élément essentiel (< ,1% en masse)",
                        "Oligo-élément bénéfique",
                        "Oligo-élément non bénéfique",
                        "Non absorbé"
                    }
                },
                {
                    "it", new string[]
                    {
                        "Elemento essenziale (> ,1% in massa)",
                        "Elemento essenziale (< ,1% in massa)",
                        "Benefico oligoelemento",
                        "Oligoelemento non benefico",
                        "Non assorbito"
                    }
                },
                {
                    "ru", new string[]
                    {
                        "Существенный элемент (> ,1% по массе)",
                        "Существенный элемент (< ,1% по массе)",
                        "Полезный микроэлемент",
                        "Неблагоприятный микроэлемент",
                        "Не впитывается"
                    }
                }
            });

        /// <summary>First characters of origin codes in most languages.</summary>
        public static string OccurrenceCodes => "SCDP";

        /// <summary>Short terms for all values of Origin.</summary>
        public static ReadOnlyDictionary<string,string[]> OccurrenceNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Künstlichen", "Kosmogen", "Zerfall", "Primordial" } },
                { "en", new string[] { "Synthetic", "Cosmogenic", "Decay", "Primordial" } },
                { "es", new string[] { "Sintético", "Cosmogénicos", "Decadencia", "Primordial" } },
                { "fr", new string[] { "Synthétique", "Cosmogénique", "Désintégration", "Primordial" } },
                { "it", new string[] { "Sintetico", "Cosmogenico", "Decadimento", "Primordiali" } },
                { "ru", new string[] { "Синтезированные", "Космогенный", "Распад", "Изначальный" } }
            });

        /// <summary>Terms for all values of StabilityIndex.</summary>
        public static ReadOnlyDictionary<string,string[]> StabilityDescriptions { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                {
                    "de", new string[] { "Stabil", "Leicht radioaktiv", "Etwas radioaktiv", "Deutlich radioaktiv", "Hochradioaktiv", "Extrem radioaktiv" }
                },
                {
                    "en", new string[] { "Stable", "Slightly radioactive", "Somewhat radioactive", "Significantly radioactive", "Highly radioactive", "Extremely radioactive" }
                },
                {
                    "es", new string[] { "Estable", "Ligeramente radiactivo", "Algo radiactivo", "Significativamente radiactivo", "Altamente radiactivo", "Extremadamente radiactivo" }
                },
                {
                    "fr", new string[] { "Stable", "Légèrement radioactif", "Un peu radioactif", "Significativement radioactif", "Très radioactif", "Extrêmement radioactif" }
                },
                {
                    "it", new string[] { "Stabile", "Leggermente radioattivo", "Un po 'radioattivo", "Significativamente radioattivo", "Altamente radioattivo", "Estremamente radioattivo" }
                },
                {
                    "ru", new string[] { "Стабильный", "Слабо радиоактивный", "Немного радиоактивный", "Значительно радиоактивный", "Очень радиоактивный", "Чрезвычайно радиоактивный" }
                }
            });

        /// <summary>Highest possible value for StabilityIndex.</summary>
        public static int StabilityIndexMax => 5;

        /// <summary>Character codes for values of State if known, a blank for unknown.</summary>
        public static string StateCodes => " SLG";

        /// <summary>Terms for all values of State.</summary>
        public static ReadOnlyDictionary<string,string[]> StateNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Unbekannt", "Solide", "Flüssigkeit", "Gas" } },
                { "en", new string[] { "Unknown", "Solid", "Liquid", "Gas" } },
                { "es", new string[] { "Desconocido", "Sólido", "Líquida", "Gas" } },
                { "fr", new string[] { "Inconnue", "Solide", "Liquide", "Gaz" } },
                { "it", new string[] { "Sconosciuto", "Solido", "Liquido", "Gas" } },
                { "ru", new string[] { "Неизвестно", "Твердый", "жидкость", "Газ" } }
            });

        /// <summary>Temperature suffixes.</summary>
        public static ReadOnlyDictionary<string,string[]> TemperatureSuffix { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "ns", "μs", "ms", "s", "min", "h", "d", "a" } },
                { "en", new string[] { "ns", "μs", "ms", "s", "m", "h", "d", "y" } },
                { "es", new string[] { "ns", "μs", "ms", "s", "min", "h", "días", "años" } },
                { "fr", new string[] { "ns", "μs", "ms", "ans", "min", "h", "j", "años" } },
                { "it", new string[] { "ns", "μs", "ms", "s", "minuti", "ore", "giorni", "anni" } },
                { "ru", new string[] { "нс", "μс", "мс", "с", "м", "час", "д", "год" } }
            });

        /// <summary>Terms that an application might use.</summary>
        public static ReadOnlyDictionary<string,string[]> ThemeNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Thema", "Elementkategorien", "Blöcke", "Geschichte", "Unterschiede", "Biologie", "Vorkommen", "Stabilität", "Aggregatzustände", "Einfarbig" } },
                { "en", new string[] { "Theme", "Categories", "Blocks", "History", "Differences", "Biology", "Occurrence", "Stability", "States", "Monochrome" } },
                { "es", new string[] { "Tema", "Categorías", "Bloques", "Historia", "Diferencias", "Biología", "Aparición", "Estabilidad", "Estados", "Monocromo" } },
                { "fr", new string[] { "Thème", "Familles", "Blocs", "Historique", "Différences", "La biologie", "Désintégration", "La stabilité", "États", "Monocromo" } },
                { "it", new string[] { "Teme", "Categorie", "Blocchi", "Storia", "Differenze", "Biologia", "Presenza", "Stabilità", "Stati", "Monocromo" } },
                { "ru", new string[] { "Тема", "Категории", "блоки", "История", "Отличия", "Биология", "Появление", "Стабильность", "Состояния", "Монохромный" } }
            });

        /// <summary>The immutable table of the elements (and n too).</summary>
        /// <remarks>Use <see cref="Z"/> as the <see cref="P:Kaos.Physics.Nuclide.Item(System.Int32)">indexer</see>.</remarks>
        public static ReadOnlyCollection<Nuclide> Table { get; } = new ReadOnlyCollection<Nuclide> (_nuclides);

        private Nuclide
        (
            int z,
            string symbol, string name,
            Category category,
            int period, int group,
            double weight,
            Isotope[] isotopes,
            IDictionary<string,string> nameMap,
            string naming,
            double? boil=null, double? melt=null,
            int known=0, string credit=null,
            Nutrition life=Nutrition.None
        )
        {
            Isotopes = new ReadOnlyCollection<Isotope> (isotopes);
            NameMap = new ReadOnlyDictionary<string,string> (nameMap);
            Z = z;
            Symbol = symbol;
            Name = name;
            Period = period;
            Group = group;
            Category = category;
            Melt = melt;
            Boil = boil;
            Weight = weight;
            Naming = naming;
            Known = known;
            Credit = credit;
            Life = life;
            Block = group == 0 ? (z == 0 ? ' ' : 'f') : group <= 2 || z == 2 ? 's' : group >= 13 ? 'p' : 'd';

            if (Known <= 0)
                KnownIndex = 0;
            else if (Known <= 1789)
                KnownIndex = 1;
            else if (Known <= 1869)
                KnownIndex = 2;
            else if (Known <= 1923)
                KnownIndex = 3;
            else if (Known <= 1945)
                KnownIndex = 4;
            else if (Known <= 2000)
                KnownIndex = 5;
            else if (Known <= 2012)
                KnownIndex = 6;

            StableCount = 0;
            StabilityIndex = Nuclide.StabilityIndexMax;
            foreach (var iso in isotopes)
            {
                if (iso.DecayMode == Decay.None)
                    ++StableCount;

                var trySix = iso.StabilityIndex;
                if (StabilityIndex > trySix)
                    StabilityIndex = trySix;
            }
        }

        /// <summary>Provides translations from world-English.</summary>
        /// <remarks>
        /// Using the method <see cref="GetName(string)"/> is the recommended way of looking up by language.
        /// </remarks>
        public ReadOnlyDictionary<string,string> NameMap { get; }

        /// <summary>The list of isotopes of the element.</summary>
        /// <remarks>
        /// Use the indexer here to access as a simple array
        /// or use the <see cref="P:Kaos.Physics.Nuclide.Item(System.Int32)">indexer</see> to seek by <em>N</em>.
        /// </remarks>
        public ReadOnlyCollection<Isotope> Isotopes { get; }

        /// <summary>Find the <see cref="Isotope"/> with the supplied nucleon count.</summary>
        /// <param name="a">Nucleon count (<em>N</em> + <em>Z</em>).</param>
        /// <returns>Collection item if exists, else <b>null</b>.</returns>
        /// <remarks>To access the nuclides as a vector, use the <see cref="Isotopes"/>.</remarks>
        public Isotope this[int a]
        {
            get
            {
                for (var ix = 0; ix < Isotopes.Count; ++ix)
                    if (Isotopes[ix].A == a)
                        return Isotopes[ix];
                return null;
            }
        }

        /// <summary>Proton count. Also known as atomic number.</summary>
        public int Z { get; }

        /// <summary>1- or 2-character universal representation of the nuclide.</summary>
        /// <remarks>Temporary symbols may be 3 characters.</remarks>
        public string Symbol { get; }

        /// <summary>Name of the nuclide in world-English (en).</summary>
        /// <remarks>Use <see cref="GetName(string)"/> for translations to other languages.</remarks>
        public string Name { get; }

        /// <summary>All elements with the same period have the same number of electron shells.</summary>
        /// <remarks>Value is the row number in a standard table or 0 for block f elements.</remarks>
        public int Period { get; }

        /// <summary>Column number in an 18-column standard table or 0 for block f elements.</summary>
        public int Group { get; }

        /// <summary>The temperature at which the nuclide changes state from solid to liquid at standard pressure (1 atm).</summary>
        /// <remarks>The returned value is in kelvins if known, else <b>null</b>.</remarks>
        public double? Melt { get; }

        /// <summary>The temperature at which the nuclide changes state from liquid to gas at standard pressure (1 atm).</summary>
        /// <remarks>The returned value is in kelvins if known, else <b>null</b>.</remarks>
        public double? Boil { get; }

        /// <summary>Year discovered or 0 for elements known to the ancients.</summary>
        public int Known { get; }

        /// <summary>Returns scaler value (0-6) of <see cref="Known"/>.</summary>
        public int KnownIndex { get; }

        /// <summary>Identity of discoverer(s).</summary>
        public string Credit { get; }

        /// <summary>Etymology notes.</summary>
        public string Naming { get; }

        /// <summary>Character code of a set of elements this element belongs to that are unified by the orbitals their valence electrons or vacancies lie in.</summary>
        public char Block { get; }

        /// <summary>Returns the region of the periodic table with similar traits.</summary>
        /// <remarks>For descriptions use <see cref="Nuclide.CategoryNames"/>.</remarks>
        public Category Category { get; }

        /// <summary>Returns scaler value (0-9) of <see cref="Category"/>.</summary>
        public int CategoryIndex => (int) Category;

        /// <summary>Returns an abbreviation of the category name.</summary>
        public string CategoryAbbr => Enum.GetName (typeof (Category), Category);

        /// <summary>Returns the biological significance of the element.</summary>
        /// <remarks>For descriptions use <see cref="Nuclide.LifeDescriptions"/></remarks>
        public Nutrition Life { get; }

        /// <summary>Returns scaler value (0-4) of <see cref="Life"/>.</summary>
        public int LifeIndex => (int) Life;

        /// <summary>Returns a 1- or 2-character code of <see cref="Life"/>.</summary>
        public string LifeCode => LifeCodes[(int) Life];

        /// <summary>Returns the column number of the element in a 32-column standard table.</summary>
        /// <remarks>Numbering starts at 1. For row number, use <see cref="Period"/>.</remarks>
        public int LongColumn
        {
            get
            {
                if (Group > 2) return Group + 14;
                if (Group > 0) return Group;
                if (Z >= 89) return Z - 86;
                return Z - 54;
            }
        }

        /// <summary>Returns the nuclide's origin in nature, or synthetic.</summary>
        /// <remarks>For descriptions use <see cref="Nuclide.OccurrenceNames"/>.</remarks>
        public Origin Occurrence { get; private set; }

        /// <summary>Returns scaler value (0-3) of <see cref="Occurrence"/>.</summary>
        public int OccurrenceIndex => (int) Occurrence;

        /// <summary>Returns character code of <see cref="Occurrence"/>.</summary>
        public char OccurrenceCode => OccurrenceCodes[(int) Occurrence];

        /// <summary>Returns scaler of radioactivity of the most stable isotope.</summary>
        /// <remarks>A value of 0 indicates stable and 5 the most radioactive.</remarks>
        public int StabilityIndex { get; }

        /// <summary>Returns total number of stable isotopes.</summary>
        public int StableCount { get; }

        /// <summary>Returns state of matter at 0 °C, standard atmosphere (1 atm).</summary>
        public State StateAt0C => GetState (273.15);

        /// <summary>Returns index value (0-3) of state at 0 °C, standard atmosphere (1 atm).</summary>
        public int StateAt0CIndex => (int) GetState (273.15);

        /// <summary>Returns character code of state at 0 °C, standard atmosphere (1 atm).</summary>
        public char StateAt0CCode => StateCodes[(int) GetState (273.15)];

        /// <summary>Returns conventional atomic weight (IUPAC 2009–2017).</summary>
        /// <remarks>For elements with no stable isotope, returns atomic mass of least unstable isotope.</remarks>
        public double Weight { get; }

        /// <summary>Iterate thru the chemical elements in <em>Z</em> order.</summary>
        /// <returns>An iterator for elements.</returns>
        /// <remarks>Results exclude the <see cref="Neutron"/>.</remarks>
        public static IEnumerable<Nuclide> GetElements()
        {
            for (var ix = 1; ix < _nuclides.Length; ++ix)
                yield return _nuclides[ix];
        }

        /// <summary>Iterate thru the elemental isotopes in <em>Z</em>, <em>A</em> order.</summary>
        /// <returns>An iterator for isotopes.</returns>
        /// <remarks>Results exclude the <see cref="Neutron"/>.</remarks>
        public static IEnumerable<Isotope> GetIsotopes()
        {
            for (var ix = 1; ix < _nuclides.Length; ++ix)
                foreach (var iso in _nuclides[ix].Isotopes)
                    yield return iso;
        }

        /// <summary>Iterate thru the ASCII lines of a 32-column, standard form table.</summary>
        /// <returns>An iterator for lines of text.</returns>
        public static IEnumerable<string> GetLongTable()
        {
            var sb = new StringBuilder();
            int row = 1;
            int column = 1;

            foreach (Nuclide nuc in Nuclide.GetElements())
            {
                for (; row < nuc.Period; ++row)
                {
                    column = 1;
                    yield return sb.ToString();
                    sb.Clear();
                }

                for (; column < nuc.LongColumn; ++column)
                    sb.Append (". ");

                ++column;
                sb.Append (nuc.Symbol);
                if (nuc.Symbol.Length == 1)
                    sb.Append (' ');
            }

            yield return sb.ToString();
        }

        /// <summary>Seek a nuclide by symbol.</summary>
        /// <param name="symbol">1- or 2-character code of nuclide to seek.</param>
        /// <returns>Nuclide if found, else <b>null</b>.</returns>
        public static Nuclide GetBySymbol (string symbol)
        {
            for (var ix = 0; ix < _nuclides.Length; ++ix)
                if (_nuclides[ix].Symbol == symbol)
                    return _nuclides[ix];
            return null;
        }

        /// <summary>Get the element name in the supplied language.</summary>
        /// <param name="lang">A 2- or 5-character language code.</param>
        /// <returns>Language-specific element name if available, else returns <see cref="Name"/>.</returns>
        /// <remarks>
        /// <para>
        /// The language code may be either a 2-character ISO 639-1 language code
        /// or a 639-1 code followed by a dash followed by a 2-character ISO 3166-1 country code.
        /// </para>
        /// <para>
        /// Use the <see cref="Name"/> property to get the element name for the default language.
        /// </para>
        /// </remarks>
        public string GetName (string lang)
        {
            lang = lang.ToUpper();
            foreach (var pair in NameMap)
                if (pair.Key.ToUpper() == lang)
                    return pair.Value;
            return Name;
        }

        /// <summary>Get the element name in the supplied culture.</summary>
        /// <param name="culture">A culture with a 2- or 5-character language code.</param>
        /// <returns>Language-specific element name if available, else returns <see cref="Name"/>.</returns>
        /// <remarks>
        /// Use the <see cref="Name"/> property to get the element name for the default language.
        /// </remarks>
        public string GetName (CultureInfo culture)
         => GetName (culture.Name);

        /// <summary>Get the nuclide's state of matter at the supplied temperature.</summary>
        /// <param name="kelvins">Temperature in K.</param>
        /// <remarks>Use the <see cref="Nuclide.StateNames"/> collection for names of states.</remarks>
        /// <returns>State at the supplied K and standard atmosphere (atm 1) if known, else returns <b>Unknown</b>.</returns>
        public State GetState (double kelvins)
        {
            if (Melt != null && kelvins < Melt.Value)
                return State.Solid;
            if (Boil != null)
                if (kelvins >= Boil.Value)
                    return State.Gas;
                else if (Melt != null && kelvins >= Melt.Value)
                    return State.Liquid;
            return State.Unknown;
        }

        /// <summary>Gets the default culture with language 'en' and with no country.</summary>
        public static CultureInfo EnCulture { get; } = new CultureInfo ("en");

        /// <summary>Provide nuclide contents in fixed-width form for the default culture.</summary>
        /// <returns>A string holding fixed-width columns.</returns>
        public string ToFixedWidthString() => ToFixedWidthString (EnCulture);

        /// <summary>Provide nuclide contents in fixed-width form for the supplied culture.</summary>
        /// <param name="culture">Culture with language code used for translations and numeric formatting.</param>
        /// <returns>A string holding fixed-width columns.</returns>
        public string ToFixedWidthString (CultureInfo culture)
        {
            var sb = new StringBuilder();

            var ts = Z.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.AppendFormat (ts);
            sb.Append (' ');
            sb.Append (Symbol);
            sb.Append (' ', 3 - Symbol.Length);
            ts = GetName (culture);
            sb.Append (ts);
            sb.Append (' ', Nuclide.MaxNameLengths[culture.TwoLetterISOLanguageName] - ts.Length + 1);
            sb.Append (Period);
            ts = Group.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            sb.Append (CategoryAbbr);
            sb.Append (' ', 9 - CategoryAbbr.Length);
            ts = Known.ToString();
            sb.Append (' ', 4 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            sb.Append (KnownIndex);
            ts = StableCount.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            sb.Append (StabilityIndex);
            sb.Append (' ');
            sb.Append (Block);
            sb.Append (' ');
            sb.Append (OccurrenceCode);
            sb.Append (' ');
            sb.Append (LifeCode);
            sb.Append (' ', 3 - LifeCode.Length);
            sb.Append (StateAt0CCode);
            ts = Melt == null ? string.Empty : Melt.Value.ToString ("F3", culture);
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            ts = Boil == null ? string.Empty : Boil.Value.ToString ("F3", culture);
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            ts = Weight.ToString ("F3", culture);
            sb.Append (' ', 8 - ts.Length);
            sb.Append (ts);
            return sb.ToString();
        }

        /// <summary>Provide nuclide contents in JSON form.</summary>
        /// <param name="quote">Name delimiter, or empty string for no delimiter.</param>
        /// <returns>A string defining a JSON object.</returns>
        /// <remarks>
        /// Pass an empty string for JavaScript results
        /// or pass a string with a quote character for JSON results.
        /// </remarks>
        public string ToJsonString (string quote)
        {
            var sb = new StringBuilder();
            sb.Append ($"{quote}z{quote}:");
            string ts = Z.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}symbol{quote}: \"");
            sb.Append (Symbol);
            sb.Append ("\",");
            sb.Append (' ', 3 - Symbol.Length);
            sb.Append ($"{quote}period{quote}: ");
            sb.Append (Period);
            sb.Append ($", {quote}group{quote}:");
            ts = Group.ToString();
            sb.Append (' ', 2 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}categoryIndex{quote}: ");
            sb.Append (CategoryIndex);
            sb.Append ($", {quote}block{quote}: \"");
            sb.Append (Block);
            sb.Append ($"\", {quote}occurrenceIndex{quote}: ");
            sb.Append (OccurrenceIndex);
            sb.Append ($", {quote}lifeIndex{quote}: ");
            sb.Append (LifeIndex);
            sb.Append ($", {quote}discoveryYear{quote}:");
            ts = Known.ToString();
            sb.Append (' ', 5 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}discoveryIndex{quote}: ");
            sb.Append (KnownIndex);
            sb.Append ($", {quote}stateIndex{quote}: ");
            sb.Append (StateAt0CIndex);
            sb.Append ($", {quote}melt{quote}:");
            ts = Melt == null ? "null" : Melt.Value.ToString ("F3");
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}boil{quote}:");
            ts = Boil == null ? "null" : Boil.Value.ToString ("F3");
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}weight{quote}:");
            ts = Weight.ToString ("F3");
            sb.Append (' ', 8 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}stableCount{quote}:");
            ts = StableCount.ToString();
            sb.Append (' ', 2 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}stabilityIndex{quote}: ");
            sb.Append (StabilityIndex);
            sb.Append ($", {quote}isotopes{quote}: [");
            for (var ix = 0; ix < Isotopes.Count; ++ix)
            {
                if (ix != 0)
                    sb.Append (", ");
                sb.Append (Isotopes[ix].ToJsonString (quote));
            }
            sb.Append (']');
            return sb.ToString();
        }

        /// <summary>Provide nuclide contents in short form.</summary>
        /// <returns>A string> containing the symbol of the nuclide.</returns>
        public override string ToString() => Symbol;
    }
}
