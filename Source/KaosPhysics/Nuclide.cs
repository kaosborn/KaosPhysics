using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Kaos.Physics
{
    public enum Category { AlMetal, AlEMetal, Lanthanoid, Actinoid, TMetal, PtMetal, Metaloid, Nonmetal, Halogen, NobleGas }
    public enum Origin { Synthetic, Cosmogenic, Decay, Primordial }
    public enum Nutrition { None, BulkEssential, TraceEssential, Beneficial, Absorbed }
    public enum State { Unknown, Solid, Liquid, Gas }

    public class Nuclide
    {
        public const double PrimordialCutoff = 5.0E15;  // seconds
        public const double secondsPerYear = 31556952.0;

        public static Nuclide Neutron { get; } = new Nuclide
        (
            0, "n", nameof (Neutron), Category.Nonmetal, 0, 0,
            melt: null, boil: null,
            weight: 1.008,
            known: 1932, credit: "James Chadwick",
            naming: "After Latin neuter, meaning neutral",
            isotopes: new Isotope[] { new Isotope (0, 1, 0.0, 610.1, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "es","Neutrón" }, { "it","Neutrone" }, { "ru","Водород" } }
        );

        public static Nuclide Hydrogen { get; } = new Nuclide
        (
            z: 1, symbol: "H", name: nameof (Hydrogen), category: Category.Nonmetal, period: 1, group: 1,
            melt: 13.99, boil: 20.271,
            weight: 1.008,
            life: Nutrition.BulkEssential,
            known: 1766, credit: "Henry Cavindish",
            naming: "From Greek, meaning water-former",
            isotopes: new Isotope[] { new Isotope (1, 1, 99.98), new Isotope (1, 2, 0.02), new Isotope (1, 3, 0.0, 12.32*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "de","Wasserstoff" }, { "es","Hidrógeno" }, { "fr","Hydrogène" }, { "it","Hydrogène" }, { "ru","Нейтрон" } }
        );

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

        public static Nuclide Beryllium { get; } = new Nuclide
        (
            4, "Be", nameof (Beryllium), Category.AlEMetal, 2, 2,
            melt: 1560.0, boil: 2742.0,
            weight: 9.0122,
            known: 1798, credit: "Louis Nicolas Vauquelin",
            naming: "From the Greek beryllos, meaning beryl mineral",
            isotopes: new Isotope[] { new Isotope (4, 7, 0.0, 53.12*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (4, 9, 100.0), new Isotope (4, 10, 0.0, 1.39E6*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "es","Berilio" }, { "fr","Béryllium" }, { "it","Berillio" }, { "ru","Бериллий" } }
        );

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

        public static Nuclide Carbon { get; } = new Nuclide
        (
            6, "C", nameof (Carbon), Category.Nonmetal, period: 2, group: 14,
            melt: 3915.0, boil: 3915.0,
            weight: 12.011,
            life: Nutrition.BulkEssential,
            known: 1789, credit: "Antoine Lavoisier",
            naming: "From the Latin carbo, meaning coal",
            isotopes: new Isotope[] { new Isotope (6, 11, null, 20.0*60, Decay.BetaPlus), new Isotope (6, 12, 98.9), new Isotope (6, 13, 1.1), new Isotope (6, 14, 0.0, 5730*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "de","Kohlensto" }, { "es","Carbono" }, { "fr","Carbone" }, { "it","Carbonio" }, { "ru","Углерод" } }
        );

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

        public static Nuclide Fluorine { get; } = new Nuclide
        (
            9, "F", nameof (Fluorine), Category.Halogen, 2, 17,
            melt: 53.48, boil: 85.03,
            weight: 18.998,
            life: Nutrition.Beneficial,
            known: 1810, credit: "André-Marie Ampère",
            naming: "From fluoric acid",
            isotopes: new Isotope[] { new Isotope (9, 18, 0.0, 109.8, Decay.BetaPlus|Decay.ECap1), new Isotope (9, 19, 100.0) },
            nameMap: new Dictionary<string,string> { { "de","Fluor" }, { "es","Fluor" }, { "fr","Fluor" }, { "it","Fluoro" }, { "ru","Фтор" } }
         );

        public static Nuclide Neon { get; } = new Nuclide
        (
            10, "Ne", nameof (Neon), Category.NobleGas, 2, 18,
            melt: 24.56, boil: 27.104,
            weight: 20.1797,
            known: 1898, credit: "William Ramsay, Morris Travers",
            naming: "From Latin novum via Greek, meaning new",
            isotopes: new Isotope[] { new Isotope (10, 20, 90.48), new Isotope (10, 21, 0.27), new Isotope (10, 22, 9.25) },
            nameMap: new Dictionary<string,string> { { "es","Neón" }, { "fr","Néon" }, { "ru","Неон" } }
        );

        public static Nuclide Sodium { get; } = new Nuclide
        (
            11, "Na", nameof (Sodium), Category.AlMetal, 3, 1,
            melt: 370.944, boil: 1156.090,
            weight: 22.989,
            life: Nutrition.BulkEssential,
            known: 1807, credit: "Humphry Davy",
            naming: "Possibly from the Arabic suda, meaning headache",
            isotopes: new Isotope[] { new Isotope (11, 22, 0.0, 2.602*31556952.0, Decay.BetaPlus), new Isotope (11, 23, 100.0), new Isotope (11, 24, 0.0, 14.96*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "de","Natrium" }, { "es","Sodio" }, { "it","Sodio" }, { "ru","Натрий" } }
        );

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

        public static Nuclide Aluminium { get; } = new Nuclide
        (
            13, "Al", nameof (Aluminium), Category.PtMetal, 3, 13,
            melt: 933.47, boil: 2743.0,
            weight: 26.981,
            life: Nutrition.Absorbed,
            known: 1824, credit: "Hans Christian Ørsted",
            naming: "From the Latin alum, the mineral from which it was isolated",
            isotopes: new Isotope[] { new Isotope (13, 26, 0.0, 7.17E5*31556952.0, Decay.BetaPlus|Decay.ECap1|Decay.Gamma), new Isotope (13, 27, 100.0) },
            nameMap: new Dictionary<string,string> { { "en-US","Aluminum" }, { "es","Aluminio" }, { "it","Alluminio" }, { "ru","Алюминий" } }
        );

        public static Nuclide Silicon { get; } = new Nuclide
        (
            14, "Si", nameof (Silicon), Category.Metaloid, 3, 14,
            melt: 1687, boil: 3538,
            weight: 28.085,
            life: Nutrition.Beneficial,
            known: 1823, credit: "Jöns Jacob Berzelius",
            naming: "From the Latin silicis, meaning flint",
            isotopes: new Isotope[] { new Isotope (14, 28, 92.2), new Isotope (14, 29, 4.7), new Isotope (14, 30, 3.1), new Isotope (14, 31, 0.0, 2.62*3600, Decay.BetaMinus), new Isotope (14, 32, 0.0, 1.53*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "de","Silicium" }, { "es","Silicio" }, { "fr","Silicium" }, { "it","Silicio" }, { "ru","Кремний" } }
        );

        public static Nuclide Phosphorus { get; } = new Nuclide
        (
            15, "P", nameof (Phosphorus), Category.Nonmetal, 3, 15,
            melt: 317.3, boil: 553.7, // white phosphorus
            weight: 30.973,
            life: Nutrition.BulkEssential,
            known: 1669, credit: "Hennig Brand",
            naming: "From the Greek φῶς and -φέρω, meaning light-bringer",
            isotopes: new Isotope[] { new Isotope (15, 31, 100.0), new Isotope (15, 32, 0.0, 14.28*86400.0, Decay.BetaMinus), new Isotope (15, 33, 0.0, 25.3*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "de","Phosphor" }, { "es","Fósforo" }, { "fr","Phosphore" }, { "it","Fosforo" }, { "ru","Фосфор" } }
        );

        public static Nuclide Sulfur { get; } = new Nuclide
        (
            16, "S", nameof (Sulfur), Category.Nonmetal, 3, 16,
            melt: 388.36, boil: 717.8,
            weight: 32.06,
            life: Nutrition.BulkEssential,
            known: 0,
            naming: "From the Latin sulpur",
            isotopes: new Isotope[] { new Isotope (16, 32, 94.99), new Isotope (16, 33, 0.75), new Isotope (16, 34, 4.25), new Isotope (16, 35, 0.0, 87.37*86400.0, Decay.BetaMinus), new Isotope (16, 36, 0.01) },
            nameMap: new Dictionary<string,string> { { "de","Schwefel" }, { "en-GB","Sulphur" }, { "es","Azufre" }, { "fr","Soufre" }, { "it","Zolfo" }, { "ru","Сера" } }
        );

        public static Nuclide Chlorine { get; } = new Nuclide
        (
            17, "Cl", nameof (Chlorine), Category.Halogen, 3, 17,
            melt: 171.6, boil: 239.11,
            weight: 35.45,
            life: Nutrition.BulkEssential,
            known: 1774, credit: "Carl Wilhelm Scheele",
            naming: "From the Greek χλωρος (chlōros), meaning green-yellow",
            isotopes: new Isotope[] { new Isotope (17, 35, 76.0), new Isotope (17, 36, 0.0, 3.01E5*31556952.0, Decay.BetaMinus|Decay.ECap1), new Isotope (17, 37, 24.0) },
            nameMap: new Dictionary<string,string> { { "de","Chlor" }, { "es","Cloro" }, { "fr","Chlore" }, { "it","Cloro" }, { "ru","Хлор" } }
        );

        public static Nuclide Argon { get; } = new Nuclide
        (
            18, "Ar", nameof (Argon), Category.NobleGas, 3, 18,
            melt: 83.81, boil: 87.302,
            weight: 39.95,
            known: 1894, credit: "Lord Rayleigh, William Ramsay",
            naming: "From the Greek ἀργόν, meaning inactive",
            isotopes: new Isotope[] { new Isotope (18, 36, 0.334), new Isotope (18, 37, null, 35*86400.0, Decay.ECap1), new Isotope (18, 38, 0.063), new Isotope (18, 39, 0.0, 269*31556952.0, Decay.BetaMinus), new Isotope (18, 40, 99.604), new Isotope (18, 41, null, 109.34, Decay.BetaMinus), new Isotope (18, 42, null, 32.9*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string> { { "es","Argón" }, { "ru","Аргон" } }
        );

        public static Nuclide Potassium { get; } = new Nuclide
        (
            19, "K", nameof (Potassium), Category.AlMetal, 4, 1,
            melt: 336.7, boil: 1032.0,
            weight: 39.0983,
            life: Nutrition.BulkEssential,
            known: 1807, credit: "Humphry Davy",
            naming: "From placing in a pot the ash of burnt wood",
            isotopes: new Isotope[] { new Isotope (19, 39, 93.258), new Isotope (19, 40, 0.0117, 1.248E9*31556952.0, Decay.BetaMinus|Decay.ECap1|Decay.BetaPlus), new Isotope (19, 41, 6.730) },
            nameMap: new Dictionary<string,string> { { "de","Kalium" }, { "es","Potasio" }, { "it","Potassio" }, { "ru","Калий" } }
        );

        public static Nuclide Calcium { get; } = new Nuclide
        (
            20, "Ca", nameof (Calcium), Category.AlEMetal, 4, 2,
            melt: 1115.0, boil: 1757.0,
            weight: 40.078,
            life: Nutrition.BulkEssential,
            known: 1808, credit: "Humphry Davy",
            naming: "From the Latin calx, meaning lime",
            isotopes: new Isotope[] { new Isotope (20, 40, 96.941), new Isotope (20, 41, 0.0, 9.94E4*31556952.0, Decay.ECap1), new Isotope (20, 42, 0.647), new Isotope (20, 43, 0.135), new Isotope (20, 44, 2.086), new Isotope (20, 45, null, 162.6*86400.0, Decay.BetaMinus), new Isotope (20, 46, 0.004), new Isotope (20, 47, null, 4.5*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (20, 48, 0.187, 6.4E19*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "es","Calcio" }, { "it","Calcio" }, { "ru","Кальций" } }
        );
        public static Nuclide Scandium { get; } = new Nuclide
        (
            21, "Sc", nameof (Scandium), Category.TMetal, 4, 3,
            melt: 1814.0, boil: 3109.0,
            weight: 44.955,
            known: 1879, credit: "Lars Fredrik Nilson",
            naming: "From the Latin Scandia, meaning Scandinavia",
            isotopes: new Isotope[] { new Isotope (21, 44, null, 58.61*3600, Decay.IC|Decay.Gamma|Decay.ECap1), new Isotope (21, 45, 100.0), new Isotope (21, 46, null, 83.79*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (21, 47, null, 80.38*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (21, 48, null, 43.67*3600, Decay.BetaMinus|Decay.Gamma) },
            nameMap: new Dictionary<string,string>() { { "es","Escandio" }, { "it","Scandio" }, { "ru","Скандий" } }
        );

        public static Nuclide Titanium { get; } = new Nuclide
        (
            22, "Ti", nameof (Titanium), Category.TMetal, 4, 4,
            melt: 1941.0, boil: 3560.0,
            weight: 47.867,
            known: 1791, credit: "William Gregor",
            naming: "For the Titans of Greek mythology",
            isotopes: new Isotope[] { new Isotope (22, 44, null, 63*31556952.0, Decay.ECap1|Decay.Gamma), new Isotope (22, 46, 8.25), new Isotope (22, 47, 7.44), new Isotope (22, 48, 73.72), new Isotope (22, 49, 5.41), new Isotope (22, 50, 5.18) },
            nameMap: new Dictionary<string,string>() { { "de","Titan" }, { "es","Titanio" }, { "fr","Titane" }, { "it","Titanio" }, { "ru","Титан" } }
        );

        public static Nuclide Vanadium { get; } = new Nuclide
        (
            23, "V", nameof (Vanadium), Category.TMetal, 4, 5,
            melt: 2183.0, boil: 3680.0,
            weight: 50.9415,
            life: Nutrition.Beneficial,
            known: 1867, credit: "Henry Enfield Roscoe",
            naming: "For the Scandinavian goddess of beauty and fertility, Vanadís",
            isotopes: new Isotope[] { new Isotope (23, 48, null, 16.0*86400.0, Decay.BetaPlus), new Isotope (23, 49, null, 330.0*86400.0, Decay.ECap1), new Isotope (23, 50, 0.25, 1.5E17*31556952.0, Decay.ECap1|Decay.BetaMinus), new Isotope (23, 51, 99.75) },
            nameMap: new Dictionary<string,string>() { { "es","Vanadio" }, { "it","Vanadio" }, { "ru","Ванадий" } }
        );

        public static Nuclide Chromium { get; } = new Nuclide
        (
            24, "Cr", nameof (Chromium), Category.TMetal, 4, 6,
            melt: 2180.0, boil: 2944.0,
            weight: 51.9961,
            life: Nutrition.Absorbed,
            known: 1797, credit: "Louis Nicolas Vauquelin",
            naming: "From the Greek Chroma, meaing color",
            isotopes: new Isotope[] { new Isotope (24, 50, 4.345), new Isotope (24, 51, null, 27.7025*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (24, 52, 83.789), new Isotope (24, 53, 9.501), new Isotope (24, 54, 2.365) },
            nameMap: new Dictionary<string,string>() { { "de","Chrom" }, { "es","Cromo" }, { "fr","Chrome" }, { "it","Cromo" }, { "ru","Хром" } }
        );

        public static Nuclide Manganese { get; } = new Nuclide
        (
            25, "Mn", nameof (Manganese), Category.TMetal, 4, 7,
            melt: 1519.0, boil: 2334.0,
            weight: 54.938,
            life: Nutrition.TraceEssential,
            known: 1774, credit: "Johann Gottlieb Gahn",
            naming: "From the Greek word for Magnesia (now in Turkey)",
            isotopes: new Isotope[] { new Isotope (25, 52, null, 5.6*86400.0, Decay.ECap1|Decay.BetaPlus|Decay.Gamma), new Isotope (25, 53, 0.0, 3.74E6*31556952.0, Decay.ECap1), new Isotope (25, 54, null, 312.03*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (25, 55, 100.0) },
            nameMap: new Dictionary<string,string>() { { "de","Mangan" }, { "es","Manganeso" }, { "fr","Manganèse" }, { "ru","Марганец" } }
        );

        public static Nuclide Iron { get; } = new Nuclide
        (
            26, "Fe", nameof (Iron), Category.TMetal, 4, 8,
            melt: 1811.0, boil: 3134.0,
            weight: 55.845,
            life: Nutrition.TraceEssential,
            known: 0,
            naming: "From proto-Germanic isarnan",
            isotopes: new Isotope[] { new Isotope (26, 54, 5.85), new Isotope (26, 55, null, 2.73*31556952.0, Decay.ECap1), new Isotope (26, 56, 91.75), new Isotope (26, 57, 2.12), new Isotope (26, 58, 0.28), new Isotope (26, 59, null, 44.6*86400.0, Decay.BetaMinus), new Isotope (26, 60, 0.0, 2.6E6*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Eisen" }, { "es","Hierro" }, { "fr","Fer" }, { "it","Ferro" }, { "ru","Железо" } }
        );

        public static Nuclide Cobalt { get; } = new Nuclide
        (
            27, "Co", nameof (Cobalt), Category.TMetal, 4, 9,
            melt: 1768.0, boil: 2723.0,
            weight: 58.933,
            life: Nutrition.TraceEssential,
            known: 1735, credit: "Georg Brandt",
            naming: "From the German kobold, meaning goblin",
            isotopes: new Isotope[] { new Isotope (27, 56, null, 77.27*86400.0, Decay.ECap1), new Isotope (27, 57, null, 271.79*86400.0, Decay.ECap1), new Isotope (27, 58, null, 70.86*86400.0, Decay.ECap1), new Isotope (27, 59, 100.0), new Isotope (27, 60, null, 5.2714*31556952.0, Decay.BetaMinus|Decay.Gamma) },
            nameMap: new Dictionary<string,string>() { { "es","Cobalto" }, { "it","Cobalto" }, { "ru","Кобальт" } }
        );

        public static Nuclide Nickel { get; } = new Nuclide
        (
            28, "Ni", nameof (Nickel), Category.TMetal, 4, 10,
            melt: 1728.0, boil: 3003.0,
            weight: 58.6934,
            life: Nutrition.Absorbed,
            known: 1751, credit: "Axel Fredrik Cronstedt",
            naming: "After a mischievous sprite of German mythology, Nickel",
            isotopes: new Isotope[] { new Isotope (28, 58, 68.077), new Isotope (28, 59, 0.0, 7.6E4*31556952.0, Decay.ECap1), new Isotope (28, 60, 26.223), new Isotope (28, 61, 1.140), new Isotope (28, 62, 3.635), new Isotope (28, 63, null, 100*31556952.0, Decay.BetaMinus), new Isotope (28, 64, 0.926) },
            nameMap: new Dictionary<string,string>() { { "es","Niquel" }, { "it","Nichel" }, { "ru","Никель" } }
        );

        public static Nuclide Copper { get; } = new Nuclide
        (
            29, "Cu", nameof (Copper), Category.TMetal, 4, 11,
            melt: 1357.77, boil: 2835.0,
            weight: 63.546,
            life: Nutrition.TraceEssential,
            known: 0,
            naming: "From the Latin Cyprium, an alloy from Cyprus",
            isotopes: new Isotope[] { new Isotope (29, 63, 69.15), new Isotope (29, 64, null, 12.70*3600, Decay.ECap1|Decay.BetaMinus), new Isotope (29, 65, 30.85), new Isotope (29, 67, null, 61.83*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Kupfer" }, { "es","Cobre" }, { "fr","Cuivre" }, { "it","Rame" }, { "ru","Медь" } }
        );

        public static Nuclide Zinc { get; } = new Nuclide
        (
            30, "Zn", nameof (Zinc), Category.TMetal, 4, 12,
            melt: 692.68, boil: 1180.0,
            weight: 65.38,
            life: Nutrition.TraceEssential,
            known: 1746, credit: "Andreas Sigismund Marggraf",
            naming: "Probably from the German zinke, meaning pointed or jagged",
            isotopes: new Isotope[] { new Isotope (30, 64, 49.2), new Isotope (30, 65, null, 244.0*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (30, 66, 27.7), new Isotope (30, 67, 4.0), new Isotope (30, 68, 18.5), new Isotope (30, 69, null, 56.0*60, Decay.BetaMinus), new Isotope (30, 70, 0.6), new Isotope (30, 71, null, 2.4*60, Decay.BetaMinus), new Isotope (30, 72, null, 46.5*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Zink" }, { "it","Zinco" }, { "ru","Цинк" } }
        );

        public static Nuclide Gallium { get; } = new Nuclide
        (
            31, "Ga", nameof (Gallium), Category.PtMetal, 4, 13,
            melt: 302.9146, boil: 2673.0,
            weight: 69.723,
            known: 1875, credit: "Lecoq de Boisbaudran",
            naming: "From Latin Gallia, meaning Gaul",
            isotopes: new Isotope[] { new Isotope (31, 66, null, 95*3600, Decay.BetaPlus), new Isotope (31, 67, null, 3.3*86400.0, Decay.ECap1), new Isotope (31, 68, null, 1.2*3600, Decay.BetaPlus), new Isotope (31, 69, 60.11), new Isotope (31, 70, null, 21.0*60, Decay.BetaMinus|Decay.ECap1), new Isotope (31, 71, 39.89), new Isotope (31, 72, null, 14.1*3600, Decay.BetaMinus), new Isotope (31, 73, null, 4.9*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Galio" }, { "it","Gallio" }, { "ru","Галлий" } }
        );

        public static Nuclide Germanium { get; } = new Nuclide
        (
            32, "Ge", nameof (Germanium), Category.Metaloid, 4, 14,
            melt: 1211.40, boil: 3106.0,
            weight: 72.630,
            known: 1886, credit: "Clemens Winkler",
            naming: "From the Latin Germania, meaning Germany",
            isotopes: new Isotope[] { new Isotope (32, 68, null, 270.95*86400.0, Decay.ECap1), new Isotope (32, 70, 20.52), new Isotope (32, 71, null, 11.3*86400.0, Decay.ECap1), new Isotope (32, 72, 27.45), new Isotope (32, 73, 7.76), new Isotope (32, 74, 36.7), new Isotope (32, 76, 7.75, 1.78E21*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "es","Germanio" }, { "it","Germanio" }, { "ru","Германий" } }
        );

        public static Nuclide Arsenic { get; } = new Nuclide
        (
            33, "As", nameof (Arsenic), Category.Metaloid, 4, 15,
            melt: 887.0, boil: 887.0,
            weight: 74.921,
            life: Nutrition.Absorbed,
            known: 0,
            naming: "From Arabic al-zarnīḵ, meaning yellow orpiment",
            isotopes: new Isotope[] { new Isotope (33, 73, null, 80.3*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (33, 74, null, 17.8*86400.0, Decay.ECap1|Decay.BetaPlus|Decay.Gamma|Decay.BetaMinus), new Isotope (33, 75, 100.0) },
            nameMap: new Dictionary<string,string>() { { "de","Arsen" }, { "es","Arsénico" }, { "it","Arsenico" }, { "ru","Мышьяк" } }
        );

        public static Nuclide Selenium { get; } = new Nuclide
        (
            34, "Se", nameof (Selenium), Category.Nonmetal, 4, 16,
            melt: 494.0, boil: 958.0,
            weight: 78.971,
            life: Nutrition.TraceEssential,
            known: 1817, credit: "Jöns Jakob Berzelius, Johann Gottlieb Gahn",
            naming: "From Greek σελήνη (selḗnē), meaning Moon",
            isotopes: new Isotope[] { new Isotope (34, 72, null, 8.4*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (34, 74, 0.86), new Isotope (34, 75, null, 119.8*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (34, 76, 9.23), new Isotope (34, 77, 7.60), new Isotope (34, 78, 23.69), new Isotope (34, 79, 0.0, 3.27E5*31556952.0, Decay.BetaMinus), new Isotope (34, 80, 49.80), new Isotope (34, 82, 8.82, 1.08E20*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "de","Selen" }, { "es","Selenio" }, { "fr","Sélénium" }, { "it","Selenio" }, { "ru","Селен" } }
        );

        public static Nuclide Bromine { get; } = new Nuclide
        (
            35, "Br", nameof (Bromine), Category.Halogen, 4, 17,
            melt: 265.8, boil: 332.0,
            weight: 79.904,
            life: Nutrition.Absorbed,
            known: 1825, credit: "Antoine Jérôme Balard, Carl Jacob Löwig",
            naming: "From the Greek βρῶμος, meaning stench",
            isotopes: new Isotope[] { new Isotope (35, 79, 51.0), new Isotope (35, 81, 49.0) },
            nameMap: new Dictionary<string,string>() { { "de","Brom" }, { "es","Bromo" }, { "fr","Brome" }, { "it","Bromo" }, { "ru","Бром" } }
        );

        public static Nuclide Krypton { get; } = new Nuclide
        (
            36, "Kr", nameof (Krypton), Category.NobleGas, 4, 18,
            melt: 115.78, boil: 119.93,
            weight: 83.798,
            known: 1898, credit: "William Ramsay, Morris Travers",
            naming: "From the Greek kryptos, meaning hidden",
            isotopes: new Isotope[] { new Isotope (36, 78, 0.36, 9.2E21*31556952.0, Decay.ECap2), new Isotope (36, 79, null, 35*3600, Decay.ECap1|Decay.BetaPlus|Decay.Gamma), new Isotope (36, 80, 2.29), new Isotope (36, 81, 0.0, 2.3E5*31556952.0, Decay.ECap1|Decay.Gamma), new Isotope (36, 82, 11.59), new Isotope (36, 83, 11.50), new Isotope (36, 84, 56.99), new Isotope (36, 85, null, 11*31556952.0, Decay.BetaMinus), new Isotope (36, 86, 17.28) },
            nameMap: new Dictionary<string,string>() { { "es","Kriptón" }, { "it","Kripton" }, { "ru","Криптон" } }
        );

        public static Nuclide Rubidium { get; } = new Nuclide
        (
            37, "Rb", nameof (Rubidium), Category.AlMetal, 5, 1,
            melt: 312.45, boil: 961.0,
            weight: 85.4678,
            life: Nutrition.Absorbed,
            known: 1861, credit: "Robert Bunsen, Gustav Kirchhoff",
            naming: "From the Latin rubidus, meaning deep red",
            isotopes: new Isotope[] { new Isotope (37, 83, null, 86.2*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (37, 84, null, 32.9*86400.0, Decay.ECap1|Decay.BetaPlus|Decay.Gamma|Decay.BetaMinus), new Isotope (37, 85, 72.17), new Isotope (37, 86, null, 18.7*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (37, 87, 27.83, 4.9E10*31556952.0, Decay.BetaMinus), },
            nameMap: new Dictionary<string,string>() { { "es","Rubidoo" }, { "it","Rubidoo" }, { "ru","Рубидий" } }
        );

        public static Nuclide Strontium { get; } = new Nuclide
        (
            38, "Sr", nameof (Strontium), Category.AlEMetal, 5, 2,
            melt: 1050.0, boil: 1650.0,
            weight: 87.62,
            life: Nutrition.Absorbed,
            known: 1787, credit: "William Cruickshank",
            naming: "From the Scottish village of Strontian",
            isotopes: new Isotope[] { new Isotope (38, 82, null, 25.36*86400.0, Decay.ECap1), new Isotope (38, 83, null, 1.35*86400.0, Decay.ECap1|Decay.BetaPlus|Decay.Gamma), new Isotope (38, 84, 0.56), new Isotope (38, 85, null, 64.84*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (38, 86, 9.86), new Isotope (38, 87, 7.0), new Isotope (38, 88, 82.58), new Isotope (38, 89, null, 50.52*86400.0, Decay.ECap1|Decay.BetaMinus), new Isotope (38, 90, 0.0, 28.90*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Estronzio" }, { "it","Stronzio" }, { "ru","Стронций" } }
        );

        public static Nuclide Yttrium { get; } = new Nuclide
        (
            39, "Y", nameof (Yttrium), Category.TMetal, 5, 3,
            melt: 1799.0, boil: 3203.0,
            weight: 88.905,
            known: 1794, credit: "Johan Gadolin",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (39, 87, null, 3.4*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (39, 88, null, 106.6*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (39, 89, 100.0), new Isotope (39, 90, null, 2.7*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (39, 91, null, 58.5*86400.0, Decay.BetaMinus|Decay.Gamma) },
            nameMap: new Dictionary<string,string>() { { "es","Itrio" }, { "it","Ittrio" }, { "ru","Иттрий" } }
        );

        public static Nuclide Zirconium { get; } = new Nuclide
        (
            40, "Zr", nameof (Zirconium), Category.TMetal, 5, 4,
            melt: 2128.0, boil: 4650.0,
            weight: 91.224,
            known: 1789, credit: "Martin Heinrich Klaproth",
            naming: "From the mineral zircon",
            isotopes: new Isotope[] { new Isotope (40, 88, null, 83.4*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (40, 89, null, 78.4*3600, Decay.ECap1|Decay.BetaPlus|Decay.Gamma), new Isotope (40, 90, 51.45), new Isotope (40, 91, 11.22), new Isotope (40, 92, 17.15), new Isotope (40, 93, 0.0, 1.53E6*31556952.0, Decay.BetaMinus), new Isotope (40, 94, 17.38), new Isotope (40, 96, 2.8, 2E19*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "es","Zirconio" }, { "it","Zirconio" }, { "ru","Цирконий" } }
        );

        public static Nuclide Niobium { get; } = new Nuclide
        (
            41, "Nb", nameof (Niobium), Category.TMetal, 5, 5,
            melt: 2750.0, boil: 5017.0,
            weight: 92.906,
            known: 1801, credit: "Charles Hatchett",
            naming: "From Niobe, the daughter of Tantalus",
            isotopes: new Isotope[] { new Isotope (41, 90, null, 15*3600, Decay.BetaPlus), new Isotope (41, 91, null, 680*31556952.0, Decay.ECap1), new Isotope (41, 92, 0.0, 3.47E7*31556952.0, Decay.ECap1|Decay.Gamma), new Isotope (41, 93, 100.0), new Isotope (41, 94, 0.0, 20.3E3*31556952.0, Decay.BetaMinus|Decay.Gamma), new Isotope (41, 95, null, 35.0*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (41, 96, null, 86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Niob" }, { "es","Niobo" }, { "it","Niobio" }, { "ru","Ниобий" } }
        );

        public static Nuclide Molybdenum { get; } = new Nuclide
        (
            42, "Mo", nameof (Molybdenum), Category.TMetal, 5, 6,
            melt: 2896.0, boil: 4912.0,
            weight: 95.95,
            known: 1778, credit: "Carl Wilhelm Scheele",
            life: Nutrition.TraceEssential,
            naming: "From the ore molybdena",
            isotopes: new Isotope[] { new Isotope (42, 92, 14.65), new Isotope (42, 93, null, 4.0E3*31556952.0, Decay.ECap1), new Isotope (42, 94, 9.19), new Isotope (42, 95, 15.87), new Isotope (42, 96, 16.67), new Isotope (42, 97, 9.58), new Isotope (42, 98, 24.29), new Isotope (42, 99, null, 65.94*3600, Decay.BetaMinus|Decay.Gamma), new Isotope (42, 100, 9.74, 7.8E18*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "de","Molybdän" }, { "es","Molibdeno" }, { "fr","Molybdène" }, { "it","Molibdeno" }, { "ru","Молибден" } }
        );

        public static Nuclide Technetium { get; } = new Nuclide
        (
            43, "Tc", nameof (Technetium), Category.TMetal, 5, 7,
            melt: 2430.0, boil: 4538.0,
            weight: 97,
            known: 1937, credit: "Emilio Segrè, Carlo Perrier",
            naming: "From the Greek τεχνητός, meaning artificial",
            isotopes: new Isotope[] { new Isotope (43, 95, null, 61.0*86400.0, Decay.ECap1|Decay.Gamma|Decay.IC), new Isotope (43, 96, null, 4.3*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (43, 97, null, 4.21E6*31556952.0, Decay.ECap1), new Isotope (43, 98, null, 4.2E6*31556952.0, Decay.BetaMinus|Decay.Gamma), new Isotope (43, 99, 0.0, 2.111E5*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "fr","Technétium" }, { "es","Tecnecio" }, { "it","Tecnezio" }, { "ru","Технеций" } }
        );

        public static Nuclide Ruthenium { get; } = new Nuclide
        (
            44, "Ru", nameof (Ruthenium), Category.TMetal, 5, 8,
            melt: 2607.0, boil: 4423.0,
            weight: 101.07,
            known: 1844, credit: "Karl Ernst Claus",
            naming: "From the Latin Ruthenia, meaning Russia",
            isotopes: new Isotope[] { new Isotope (44, 96, 5.54), new Isotope (44, 97, null, 2.9*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (44, 98, 1.87), new Isotope (44, 99, 12.76), new Isotope (44, 100, 12.60), new Isotope (44, 101, 17.06), new Isotope (44, 102, 31.55), new Isotope (44, 103, null, 39.26*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (44, 104, 18.62), new Isotope (44, 106, null, 373.59*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "fr","Ruthénium" }, { "es","Rutenio" }, { "it","Rutenio" }, { "ru","Рутений" } }
        );

        public static Nuclide Rhodium { get; } = new Nuclide
        (
            45, "Rh", nameof (Rhodium), Category.TMetal, 5, 9,
            melt: 2237.0, boil: 3968.0,
            weight: 102.905,
            known: 1804, credit: "William Hyde Wollaston",
            naming: "From Greek ῥόδον (rhodon), meaning rose",
            isotopes: new Isotope[] { new Isotope (45, 99, null, 16.1*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (45, 101, null, 4.34*86400.0, Decay.ECap1|Decay.IC|Decay.Gamma), new Isotope (45, 101, null, 3.3*31556952.0, Decay.ECap1|Decay.Gamma), new Isotope (45, 102, null, 3.7*31556952.0, Decay.ECap1|Decay.Gamma), new Isotope (45, 102, null, 207.0*86400.0, Decay.ECap1|Decay.BetaPlus|Decay.BetaMinus|Decay.Gamma), new Isotope (45, 103, 100.0), new Isotope (45, 105, null, 35.36*3600, Decay.BetaMinus|Decay.Gamma) },
            nameMap: new Dictionary<string,string>() { { "es","Rodio" }, { "it","Rodio" }, { "ru","Родий" } }
        );

        public static Nuclide Palladium { get; } = new Nuclide
        (
            46, "Pd", nameof (Palladium), Category.TMetal, 5, 10,
            melt: 1828.05, boil: 3236.0,
            weight: 106.42,
            known: 1802, credit: "William Hyde Wollaston",
            naming: "After the asteroid 2 Pallas",
            isotopes: new Isotope[] { new Isotope (46, 100, null, 3.63*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (46, 102, 1.02), new Isotope (46, 103, null, 16.991*86400.0, Decay.ECap1), new Isotope (46, 104, 11.14), new Isotope (46, 105, 22.33), new Isotope (46, 106, 27.33), new Isotope (46, 107, 0.0, 6.5E6*31556952.0, Decay.BetaMinus), new Isotope (46, 108, 26.46), new Isotope (46, 110, 11.72) },
            nameMap: new Dictionary<string,string>() { { "es","Paladio" }, { "it","Palladio" }, { "ru","Палладий" } }
        );

        public static Nuclide Silver { get; } = new Nuclide
        (
            47, "Ag", nameof (Silver), Category.TMetal, 5, 11,
            melt: 1234.93, boil: 2435.0,
            weight: 107.8682,
            known: 0,
            naming: "From Proto-Germanic silubra",
            isotopes: new Isotope[] { new Isotope (47, 105, null, 41.2*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (47, 106, null, 8.28*86400.0, Decay.ECap1|Decay.Gamma), new Isotope (47, 107, 51.839), new Isotope (47, 108, null, 418*31556952.0, Decay.ECap1|Decay.IT|Decay.Gamma), new Isotope (47, 109, 48.161), new Isotope (47, 110, null, 249.95*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (47, 111, null, 7.45*86400.0, Decay.BetaMinus|Decay.Gamma) },
            nameMap: new Dictionary<string,string>() { { "de","Silber" }, { "es","Plata" }, { "fr","Argent" }, { "it","Argento" }, { "ru","Серебро" } }
        );

        public static Nuclide Cadmium { get; } = new Nuclide
        (
            48, "Cd", nameof (Cadmium), Category.TMetal, 5, 12,
            weight: 112.414,
            melt: 594.22, boil: 1040.0,
            known: 1817, credit: "Karl Samuel Leberecht Hermann, Friedrich Stromeyer",
            naming: "From the mineral calamine named after the Greek mythological character Κάδμος (Cadmus)",
            isotopes: new Isotope[] { new Isotope (48, 106, 1.25), new Isotope (48, 107, null, 6.5*3600, Decay.ECap1), new Isotope (48, 108, 0.89), new Isotope (48, 109, null, 462.6*86400.0, Decay.ECap1), new Isotope (48, 110, 12.47), new Isotope (48, 111, 12.80), new Isotope (48, 112, 24.11), new Isotope (48, 113, 12.23, 7.7E15*31556952.0, Decay.BetaMinus), new Isotope (48, 113, null, 14.1*31556952.0, Decay.BetaMinus|Decay.IT), new Isotope (48, 114, 28.75), new Isotope (48, 115, null, 53.46*3600, Decay.BetaMinus), new Isotope (48, 116, 7.51, 3.1E19*365.25*3600, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "es","Cadmio" }, { "it","Cadmio" }, { "ru","Кадмий" } }
        );

        public static Nuclide Indium { get; } = new Nuclide
        (
            49, "In", nameof (Indium), Category.PtMetal, 5, 13,
            melt: 429.7485, boil: 2345.0,
            weight: 114.818,
            known: 1863, credit: "Ferdinand Reich, Hieronymous Theodor Richter",
            naming: "From the indigo color seen in its spectrum, after the Latin indicum, meaning 'of India'",
            isotopes: new Isotope[] { new Isotope (49, 111, null, 2.8*86400.0, Decay.ECap1), new Isotope (49, 113, 4.28), new Isotope (49, 115, 95.72, 4.41E14*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Indio" }, { "it","Indio" }, { "ru","Индий" } }
        );

        public static Nuclide Tin { get; } = new Nuclide
        (
            50, "Sn", nameof (Tin), Category.PtMetal, 5, 14,
            melt: 505.08, boil: 2875.0,
            weight: 118.710,
            life: Nutrition.Absorbed,
            known: 0,
            naming: "From proto-Germanic 'tin-om'",
            isotopes: new Isotope[] { new Isotope (50, 112, 0.97), new Isotope (50, 114, 0.66), new Isotope (50, 115, 0.34), new Isotope (50, 116, 14.54), new Isotope (50, 117, 7.68), new Isotope (50, 118, 24.22), new Isotope (50, 119, 8.59), new Isotope (50, 120, 32.58), new Isotope (50, 122, 4.63), new Isotope (50, 124, 5.79), new Isotope (50, 126, 0.0, 2.3E5*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Zinn" }, { "es","Estaño" }, { "fr","Etain" }, { "it","Stagno" }, { "ru","Олово" } }
        );

        public static Nuclide Antimony { get; } = new Nuclide
        (
            51, "Sb", nameof (Antimony), Category.Metaloid, 5, 15,
            melt: 903.78, boil: 1908.0,
            weight: 121.760,
            known: 0,
            naming: "From Latin antimonium",
            isotopes: new Isotope[] { new Isotope (51, 121, 57.21), new Isotope (51, 123, 42.79), new Isotope (51, 125, null, 2.7582*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Antimon" }, { "es","Antimonio" }, { "fr","Antimoine" }, { "it","Antimonio" }, { "ru","Сурьма" } }
        );

        public static Nuclide Tellurium { get; } = new Nuclide
        (
            52, "Te", nameof (Tellurium), Category.Metaloid, 5, 16,
            melt: 722.66, boil: 1261.0,
            weight: 127.60,
            known: 1782, credit: "Franz-Joseph Müller von Reichenstein",
            naming: "From Latin tellus, meaning 'earth'",
            isotopes: new Isotope[] { new Isotope (52, 120, 0.09), new Isotope (52, 121, null, 16.78*86400.0, Decay.ECap1), new Isotope (52, 122, 2.55), new Isotope (52, 123, 0.89), new Isotope (52, 124, 4.74), new Isotope (52, 125, 7.07), new Isotope (52, 126, 18.84), new Isotope (52, 127, null, 9.35*3600, Decay.BetaMinus), new Isotope (52, 128, 31.74, 2.2E24*31556952.0, Decay.Beta2), new Isotope (52, 129, null, 69.6*60, Decay.BetaMinus), new Isotope (52, 130, 34.08, 7.9E20*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "de","Tellur" }, { "es","Teluro" }, { "fr","Tellure" }, { "it","Tellurio" }, { "ru","Теллур" } }
        );

        public static Nuclide Iodine { get; } = new Nuclide
        (
            53, "I", nameof (Iodine), Category.Halogen, 5, 17,
            melt: 386.85, boil: 457.4,
            weight: 126.904,
            known: 1811, credit: "Bernard Courtois",
            life: Nutrition.TraceEssential,
            naming: "From the Greek ἰοειδής (ioeidēs), meaning 'violet'",
            isotopes: new Isotope[] { new Isotope (53, 123, null, 13.0*3600, Decay.ECap1|Decay.Gamma), new Isotope (53, 124, null, 4.176*86400.0, Decay.ECap1), new Isotope (53, 125, null, 59.40*86400.0, Decay.ECap1), new Isotope (53, 127, 100.0), new Isotope (53, 129, 0.0, 1.57E7*31556952.0, Decay.BetaMinus), new Isotope (53, 131, null, 8.02070*86400.0, Decay.BetaMinus|Decay.Gamma), new Isotope (53, 135, null, 6.57*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Iod" }, { "es","Yodo" }, { "fr","Iode" }, { "it","Iodio" }, { "ru","Иод" } }
        );

        public static Nuclide Xenon { get; } = new Nuclide
        (
            54, "Xe", nameof (Xenon), Category.NobleGas, 5, 18,
            melt: 161.40, boil: 165.051,
            weight: 131.293,
            known: 1898, credit: "William Ramsay, Morris Travers",
            naming: "From the Greek ξένον (xénon), meaning 'foreigner'",
            isotopes: new Isotope[] { new Isotope (54, 124, 0.095, 1.8E22*31556952.0, Decay.ECap2), new Isotope (54, 125, null, 16.9*3600, Decay.ECap1), new Isotope (54, 126, 0.89), new Isotope (54, 127, null, 36.345*86400.0, Decay.ECap1), new Isotope (54, 128, 1.910), new Isotope (54, 129, 26.401), new Isotope (54, 130, 4.071), new Isotope (54, 131, 21.232), new Isotope (54, 132, 26.909), new Isotope (54, 133, null, 5.247*86400.0, Decay.BetaMinus), new Isotope (54, 134, 10.436), new Isotope (54, 135, null, 9.14*3600, Decay.BetaMinus), new Isotope (54, 136, 8.857, 2.165E21*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "es","Xénon" }, { "fr","Xénon" }, { "ru","Ксенон" } }
        );

        public static Nuclide Caesium { get; } = new Nuclide
        (
            55, "Cs", nameof (Caesium), Category.AlMetal, 6, 1,
            melt: 301.7, boil: 944.0,
            weight: 132.905,
            known: 1860, credit: "Robert Bunsen, Gustav Kirchhoff",
            naming: "From the Latin word caesius, meaning 'sky-blue'",
            isotopes: new Isotope[] { new Isotope (55, 133, 100.0), new Isotope (55, 134, null, 2.0648*31556952.0, Decay.ECap1|Decay.BetaMinus), new Isotope (55, 135, 0.0, 2.3E6*31556952.0, Decay.BetaMinus), new Isotope (55, 137, null, 30.17*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Cäsium" }, { "en-US","Cesium" }, { "es","Cesio" }, { "fr","Césium" }, { "it","Cesio" }, { "ru","Цезий" } }
        );

        public static Nuclide Barium { get; } = new Nuclide
        (
            56, "Ba", nameof (Barium), Category.AlEMetal, 6, 2,
            melt: 1000.0, boil: 2118.0,
            weight: 137.327,
            known: 1772, credit: "Carl Wilhelm Scheele",
            naming: "After the mineral 'baryta'",
            isotopes: new Isotope[] { new Isotope (56, 130, 0.11, 0.5E21*31556952.0, Decay.ECap2), new Isotope (56, 132, 0.10), new Isotope (56, 133, null, 10.51*31556952.0, Decay.ECap1), new Isotope (56, 134, 2.42), new Isotope (56, 135, 6.59), new Isotope (56, 136, 7.85), new Isotope (56, 137, 11.23), new Isotope (56, 138, 71.70) },
            nameMap: new Dictionary<string,string>() { { "es","Bario" }, { "fr","Baryum" }, { "it","Bario" }, { "ru","Барий" } }
        );

        public static Nuclide Lanthanum { get; } = new Nuclide
        (
            57, "La", nameof (Lanthanum), Category.Lanthanoid, 6, 0,
            melt: 1193.0, boil: 3737.0,
            weight: 138.905,
            known: 1838, credit: "Carl Gustaf Mosander",
            naming: "From Greek λανθάνειν (lanthanein), meaning to lie hidden",
            isotopes: new Isotope[] { new Isotope (57, 137, null, 6E4*31556952.0, Decay.ECap1), new Isotope (57, 138, 0.089, 1.05E11*31556952.0, Decay.ECap1|Decay.BetaMinus), new Isotope (57, 139, 99.911) },
            nameMap: new Dictionary<string,string>() { { "de","Lanthan" }, { "es","Lantano" }, { "fr","Lanthane" }, { "it","Lantanio" }, { "ru","Лантан" } }
        );

        public static Nuclide Cerium { get; } = new Nuclide
        (
            58, "Ce", nameof (Cerium), Category.Lanthanoid, 6, 0,
            melt: 1068.0, boil: 3716.0,
            weight: 140.116,
            known: 1803, credit: "Martin Heinrich Klaproth, Jöns Jakob Berzelius, Wilhelm Hisinger",
            naming: "After the dwarf planet Ceres",
            isotopes: new Isotope[] { new Isotope (58, 134, null, 3.16*86400.0, Decay.ECap1), new Isotope (58, 136, 0.186), new Isotope (58, 138, 0.251), new Isotope (58, 139, null, 137.640*86400.0, Decay.ECap1), new Isotope (58, 140, 88.449), new Isotope (58, 141, null, 32.501*86400.0, Decay.BetaMinus), new Isotope (58, 142, 11.114), new Isotope (58, 143, null, 33.039*3600, Decay.BetaMinus), new Isotope (58, 144, null, 284.893*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Cer" }, { "es","Cerio" }, { "fr","Cérium" }, { "it","Cerio" }, { "ru","Церий" } }
        );

        public static Nuclide Praseodymium { get; } = new Nuclide
        (
            59, "Pr", nameof (Praseodymium), Category.Lanthanoid, 6, 0,
            melt: 1208.0, boil: 3403.0,
            weight: 140.907,
            known: 1885, credit: "Carl Auer von Welsbach",
            naming: "From the Greek πρασιος, meaning 'leek green'",
            isotopes: new Isotope[] { new Isotope (59, 141, 100.0), new Isotope (59, 142, null, 19.12*3600, Decay.BetaMinus|Decay.ECap1), new Isotope (59, 143, null, 13.57*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Praseodym" }, { "es","Praseodimio" }, { "fr","Raséodyme" }, { "it","Praseodimio" }, { "ru","Празеодим" } }
        );

        public static Nuclide Neodymium { get; } = new Nuclide
        (
            60, "Nd", nameof (Neodymium), Category.Lanthanoid, 6, 0,
            melt: 1297.0, boil: 3347.0,
            weight: 144.242,
            known: 1885, credit: "Carl Auer von Welsbach",
            naming: "From the Greek νέος (neos) and διδύμος (didymos), meaning 'new twin'",
            isotopes: new Isotope[] { new Isotope (60, 142, 27.2), new Isotope (60, 143, 12.2), new Isotope (60, 144, 23.8, 2.29E15*31556952.0, Decay.Alpha), new Isotope (60, 145, 8.3), new Isotope (60, 146, 17.2), new Isotope (60, 148, 5.8, 2.7E18*31556952.0, Decay.Beta2), new Isotope (60, 150, 5.6, 21.0E18*31556952.0, Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "de","Neodym" }, { "es","Neodimio" }, { "fr","Néodyme" }, { "it","Neodimio" }, { "ru","Неодим" } }
        );

        public static Nuclide Promethium { get; } = new Nuclide
        (
            61, "Pm", nameof (Promethium), Category.Lanthanoid, 6, 0,
            melt: 1315.0, boil: 3273.0,
            weight: 145,
            known: 1942, credit: "Chien Shiung Wu, Emilio Segrè, Hans Bethe",
            naming: "After Prometheus, a Titan in Greek mythology",
            isotopes: new Isotope[] { new Isotope (61, 145, 0.0, 17.7*31556952.0, Decay.ECap1), new Isotope (61, 146, null, 5.53*31556952.0, Decay.ECap1|Decay.BetaMinus), new Isotope (61, 147, 0.0, 2.6234*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Prometio" }, { "fr","Prométhium" }, { "it","Promezio" }, { "ru","Прометий" } }
        );

        public static Nuclide Samarium { get; } = new Nuclide
        (
            62, "Sm", nameof (Samarium), Category.Lanthanoid, 6, 0,
            melt: 1345.0, boil: 2173.0,
            weight: 150.36,
            known: 1879, credit: "Lecoq de Boisbaudran",
            naming: "After the mineral samarskite",
            isotopes: new Isotope[] { new Isotope (62, 144, 3.08), new Isotope (62, 145, null, 340.0*86400.0, Decay.ECap1), new Isotope (62, 146, null, 6.8E7*31556952.0, Decay.Alpha), new Isotope (62, 147, 15.0, 1.06E11*31556952.0, Decay.Alpha), new Isotope (62, 148, 11.25, 7E15*31556952.0, Decay.Alpha), new Isotope (62, 149, 13.82), new Isotope (62, 150, 7.37), new Isotope (62, 151, null, 90*31556952.0, Decay.BetaMinus), new Isotope (62, 152, 26.74), new Isotope (62, 153, null, 46.284*3600, Decay.BetaMinus), new Isotope (62, 154, 22.74) },
            nameMap: new Dictionary<string,string>() { { "es","Samario" }, { "it","Samario" }, { "ru","Самарий" } }
        );

        public static Nuclide Europium { get; } = new Nuclide
        (
            63, "Eu", nameof (Europium), Category.Lanthanoid, 6, 0,
            melt: 1099.0, boil: 1802.0,
            weight: 151.964,
            known: 1896, credit: "Eugène-Anatole Demarçay",
            naming: "After the continent of Europe",
            isotopes: new Isotope[] { new Isotope (63, 150, null, 36.9*31556952.0, Decay.ECap1), new Isotope (63, 151, 47.8, 5E18*31556952.0, Decay.Alpha), new Isotope (63, 152, null, 13.54*31556952.0, Decay.ECap1|Decay.BetaMinus), new Isotope (63, 153, 52.2), new Isotope (63, 154, null, 8.59*31556952.0, Decay.BetaMinus), new Isotope (63, 155, null, 4.76*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Europio" }, { "it","Europio" }, { "ru","Европий" } }
        );

        public static Nuclide Gadolinium { get; } = new Nuclide
        (
            64, "Gd", nameof (Gadolinium), Category.Lanthanoid, 6, 0,
            melt: 1585.0, boil: 3273.0,
            weight: 157.25,
            known: 1880, credit: "Jean Charles Galissard de Marignac",
            naming: "After the mineral gadolinite, itself named for the Finnish chemist Johan Gadolin",
            isotopes: new Isotope[] { new Isotope (64, 148, null, 75*31556952.0, Decay.Alpha), new Isotope (64, 150, null, 1.8E6*31556952.0, Decay.Alpha), new Isotope (64, 152, 0.20, 1.08E14*31556952.0, Decay.Alpha), new Isotope (64, 154, 2.18), new Isotope (64, 155, 14.80), new Isotope (64, 156, 20.47), new Isotope (64, 157, 15.65), new Isotope (64, 158, 24.84), new Isotope (64, 160, 21.86) },
            nameMap: new Dictionary<string,string>() { { "es","Gadolinio" }, { "it","Gadolinio" }, { "ru","Гадолиний" } }
        );

        public static Nuclide Terbium { get; } = new Nuclide
        (
            65, "Tb", nameof (Terbium), Category.Lanthanoid, 6, 0,
            melt: 1629.0, boil: 3396.0,
            weight: 158.925,
            known: 1843, credit: "Carl Gustaf Mosander",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (65, 157, null, 71*31556952.0, Decay.ECap1), new Isotope (65, 158, null, 180*31556952.0, Decay.ECap1|Decay.BetaMinus), new Isotope (65, 159, 100.0) },
            nameMap: new Dictionary<string,string>() { { "es","Terbio" }, { "it","Terbio" }, { "ru","Тербий" } }
        );

        public static Nuclide Dysprosium { get; } = new Nuclide
        (
            66, "Dy", nameof (Dysprosium), Category.Lanthanoid, 6, 0,
            melt: 1680.0, boil: 2840.0,
            weight: 162.500,
            known: 1886, credit: "Lecoq de Boisbaudran",
            naming: "From the Greek δυσπρόσιτος (dysprositos), meaning 'hard to get'",
            isotopes: new Isotope[] { new Isotope (66, 154, null, 3.0E6*31556952.0, Decay.Alpha), new Isotope (66, 156, 0.056), new Isotope (66, 158, 0.095), new Isotope (66, 160, 2.329), new Isotope (66, 161, 18.889), new Isotope (66, 162, 25.475), new Isotope (66, 163, 24.896), new Isotope (66, 164, 28.260) },
            nameMap: new Dictionary<string,string>() { { "es","Disprosio" }, { "it","Disprosio" }, { "ru","Диспрозий" } }
        );

        public static Nuclide Holmium { get; } = new Nuclide
        (
            67, "Ho", nameof (Holmium), Category.Lanthanoid, 6, 0,
            melt: 1734.0, boil: 2873.0,
            weight: 164.930,
            known: 1878, credit: "Jacques-Louis Soret, Marc Delafontaine",
            naming: "after the Latin name for Stockholm",
            isotopes: new Isotope[] { new Isotope (67, 163, null, 4570*31556952.0, Decay.ECap1), new Isotope (67, 164, null, 29.0*60, Decay.ECap1), new Isotope (67, 165, 100.0), new Isotope (67, 166, null, 26.763*3600, Decay.BetaMinus), new Isotope (67, 167, null, 3.1*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Holmio" }, { "it","Olmio" }, { "ru","Гольмий" } }
        );

        public static Nuclide Erbium { get; } = new Nuclide
        (
            68, "Er", nameof (Erbium), Category.Lanthanoid, 6, 0,
            melt: 1802.0, boil: 3141.0,
            weight: 167.259,
            known: 1843, credit: "Carl Gustaf Mosander",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (68, 160, null, 28.58*3600, Decay.ECap1), new Isotope (68, 162, 0.139), new Isotope (68, 164, 1.601), new Isotope (68, 165, null, 10.36*3600, Decay.ECap1), new Isotope (68, 166, 33.503), new Isotope (68, 167, 22.869), new Isotope (68, 168, 26.978), new Isotope (68, 169, null, 9.4*86400.0, Decay.BetaMinus), new Isotope (68, 170, 14.910), new Isotope (68, 171, null, 7.516*3600, Decay.BetaMinus), new Isotope (68, 172, null, 49.3*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Erbio" }, { "it","Erbio" }, { "ru","Эрбий" } }
        );

        public static Nuclide Thulium { get; } = new Nuclide
        (
            69, "Tm", nameof (Thulium), Category.Lanthanoid, 6, 0,
            melt: 1818.0, boil: 2223.0,
            weight: 168.934,
            known: 1879, credit: "Per Teodor Cleve",
            naming: "after Thule, a Greek name associated with Scandinavia",
            isotopes: new Isotope[] { new Isotope (69, 167, null, 9.25*86400.0, Decay.ECap1), new Isotope (69, 168, null, 93.0*86400.0, Decay.ECap1), new Isotope (69, 169, 100.0), new Isotope (69, 170, null, 128.6*86400.0, Decay.BetaMinus), new Isotope (69, 171, null, 1.92*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Tulio" }, { "it","Tulio" }, { "ru","Тулий" } }
        );

        public static Nuclide Ytterbium { get; } = new Nuclide
        (
            70, "Yb", nameof (Ytterbium), Category.Lanthanoid, 6, 0,
            melt: 1097.0, boil: 1469.0,
            weight: 173.045,
            known: 1878, credit: "Jean Charles Galissard de Marignac",
            naming: "After the Swedish village of Ytterby",
            isotopes: new Isotope[] { new Isotope (70, 166, null, 56.7*3600, Decay.ECap1), new Isotope (70, 168, 0.126), new Isotope (70, 169, null, 32.026*86400.0, Decay.ECap1), new Isotope (70, 170, 3.023), new Isotope (70, 171, 14.216), new Isotope (70, 172, 21.754), new Isotope (70, 173, 16.098), new Isotope (70, 174, 31.896), new Isotope (70, 175, null, 4.185*86400.0, Decay.BetaMinus), new Isotope (70, 176, 12.887), new Isotope (70, 177, null, 1.911*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Iterbio" }, { "it","Itterbio" }, { "ru","Иттербий" } }
        );

        public static Nuclide Lutetium { get; } = new Nuclide
        (
            71, "Lu", nameof (Lutetium), Category.TMetal, 6, 3,
            melt: 1925.0, boil: 3675.0,
            weight: 174.9668,
            known: 1906, credit: "Carl Auer von Welsbach, Georges Urbain",
            naming: "From the Latin Lutetia, meaning 'Paris'",
            isotopes: new Isotope[] { new Isotope (71, 173, null, 1.37*31556952.0, Decay.ECap1), new Isotope (71, 174, null, 3.31*31556952.0, Decay.ECap1), new Isotope (71, 175, 97.401), new Isotope (71, 176, 2.599, 3.78*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Lutecio" }, { "fr","Lutétium" }, { "it","Lutezio" }, { "ru","Лютеций" } }
        );

        public static Nuclide Hafnium { get; } = new Nuclide
        (
            72, "Hf", nameof (Hafnium), Category.TMetal, 6, 4,
            melt: 2506.0, boil: 4876.0,
            weight: 178.486,
            known: 1922, credit: "Dirk Coster, George de Hevesy",
            naming: "From the Latin Hafnia, meaning 'Copenhagen'",
            isotopes: new Isotope[] { new Isotope (72, 172, null, 1.87*31556952.0, Decay.ECap1), new Isotope (72, 174, 0.16, 2E15*31556952.0, Decay.Alpha), new Isotope (72, 176, 5.26), new Isotope (72, 177, 18.60), new Isotope (72, 178, 27.28), new Isotope (72, 178, null, 31*31556952.0, Decay.IT), new Isotope (72, 179, 13.62), new Isotope (72, 180, 35.08), new Isotope (72, 182, null, 8.9E6*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Hafnio" }, { "it","Afnio" }, { "ru","Гафний" } }
        );

        public static Nuclide Tantalum { get; } = new Nuclide
        (
            73, "Ta", nameof (Tantalum), Category.TMetal, 6, 5,
            melt: 3290.0, boil: 5731.0,
            weight: 180.947,
            known: 1844, credit: "Heinrich Rose",
            naming: "From Tantalus, the father of Niobe in Greek mythology",
            isotopes: new Isotope[] { new Isotope (73, 177, null, 56.56*3600, Decay.ECap1), new Isotope (73, 178, null, 2.36*3600, Decay.ECap1), new Isotope (73, 179, null, 1.82*31556952.0, Decay.ECap1), new Isotope (73, 180, 0.012), new Isotope (73, 180, null, 8.125*3600, Decay.ECap1|Decay.BetaMinus), new Isotope (73, 181, 99.988), new Isotope (73, 182, null, 114.43*86400.0, Decay.BetaMinus), new Isotope (73, 183, null, 5.1*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Tantal" }, { "es","Tantalio" }, { "fr","Tantale" }, { "it","Tantalio" }, { "ru","Тантал" } }
        );

        public static Nuclide Tungsten { get; } = new Nuclide
        (
            74, "W", nameof (Tungsten), Category.TMetal, 6, 6,
            melt: 3695.0, boil: 6203.0,
            weight: 183.84,
            known: 1783, credit: "Juan José Elhuyar, Fausto Elhuyar",
            naming: "From the Swedish word for heavy stone",
            isotopes: new Isotope[] { new Isotope (74, 180, 0.12, 1.8E18*31556952.0, Decay.Alpha), new Isotope (74, 181, null, 121.2*86400.0, Decay.ECap1), new Isotope (74, 182, 26.50), new Isotope (74, 183, 14.31), new Isotope (74, 184, 30.64), new Isotope (74, 185, null, 75.1*86400.0, Decay.BetaMinus), new Isotope (74, 186, 28.43) },
            nameMap: new Dictionary<string,string>() { { "de","Wolfram" }, { "es","Wolframio" }, { "fr","Tungstène" }, { "it","Tunsteno" }, { "ru","Вольфрам" } }
        );

        public static Nuclide Rhenium { get; } = new Nuclide
        (
            75, "Re", nameof (Rhenium), Category.TMetal, 6, 7,
            melt: 3459.0, boil: 5903.0,
            weight: 186.207,
            known: 1908, credit: "Masataka Ogawa",
            naming: "From Latin Rhenus, meaning 'Rhine'",
            isotopes: new Isotope[] { new Isotope (75, 185, 37.4), new Isotope (75, 187, 62.6, 4.12E10*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Rhénium" }, { "es","Renio" }, { "it","Renio" }, { "ru","Рений" } }
        );

        public static Nuclide Osmium { get; } = new Nuclide
        (
            76, "Os", nameof (Osmium), Category.TMetal, 6, 8,
            melt: 3306.0, boil: 5285.0,
            weight: 190.23,
            known: 1803, credit: "Smithson Tennant",
            naming: "After Greek osme, meaning 'a smell'",
            isotopes: new Isotope[] { new Isotope (76, 184, 0.02), new Isotope (76, 185, null, 93.6*86400.0, Decay.ECap1), new Isotope (76, 186, 1.59, 2.0E15*31556952.0, Decay.Alpha), new Isotope (76, 187, 1.96), new Isotope (76, 188, 13.24), new Isotope (76, 189, 16.15), new Isotope (76, 190, 26.26), new Isotope (76, 191, null, 15.4*86400.0, Decay.BetaMinus), new Isotope (76, 192, 40.78), new Isotope (76, 193, null, 30.11*86400.0, Decay.BetaMinus), new Isotope (76, 194, null, 6*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Osmio" }, { "it","Osmio" }, { "ru","Осмий" } }
        );

        public static Nuclide Iridium { get; } = new Nuclide
        (
            77, "Ir", nameof (Iridium), Category.TMetal, 6, 9,
            melt: 2719.0, boil: 4403.0,
            weight: 192.217,
            known: 1803, credit: "Smithson Tennant",
            naming: "After the Greek goddess Iris, personification of the rainbow",
            isotopes: new Isotope[] { new Isotope (77, 188, null, 1.73*86400.0, Decay.ECap1), new Isotope (77, 189, null, 13.2*86400.0, Decay.ECap1), new Isotope (77, 190, null, 11.8*86400.0, Decay.ECap1), new Isotope (77, 191, 37.3), new Isotope (77, 192, null, 73.827*86400.0, Decay.ECap1), new Isotope (77, 192, null, 241*31556952.0, Decay.IT), new Isotope (77, 193, 62.7), new Isotope (77, 193, null, 10.5*86400.0, Decay.IT), new Isotope (77, 194, null, 19.3*3600, Decay.BetaMinus), new Isotope (77, 194, null, 171.0*86400.0, Decay.IT) },
            nameMap: new Dictionary<string,string>() { { "es","Iridio" }, { "it","Iridio" }, { "ru","Иридий" } }
        );

        public static Nuclide Platinum { get; } = new Nuclide
        (
            78, "Pt", nameof (Platinum), Category.TMetal, 6, 10,
            melt: 2041.4, boil: 4098.0,
            weight: 195.084,
            known: 1735, credit: "Antonio de Ulloa",
            naming: "From the Spanish platina, meaning 'silver'",
            isotopes: new Isotope[] { new Isotope (78, 190, 0.012, 6.5E11*31556952.0, Decay.Alpha), new Isotope (78, 192, 0.782), new Isotope (78, 193, null, 50*31556952.0, Decay.ECap1), new Isotope (78, 194, 32.864), new Isotope (78, 195, 33.775), new Isotope (78, 196, 25.211), new Isotope (78, 198, 7.356) },
            nameMap: new Dictionary<string,string>() { { "de","Platin" }, { "es","Platino" }, { "fr","Platine" }, { "it","Platino" }, { "ru","Платина" } }
        );

        public static Nuclide Gold { get; } = new Nuclide
        (
            79, "Au", nameof (Gold), Category.TMetal, 6, 11,
            melt: 1337.33, boil: 3243.0,
            weight: 196.966,
            known: 0,
            naming: "From Proto-Germanic gulþą",
            isotopes: new Isotope[] { new Isotope (79, 195, null, 186.10*86400.0, Decay.ECap1), new Isotope (79, 196, null, 6.183*86400.0, Decay.ECap1|Decay.BetaMinus), new Isotope (79, 197, 100.0), new Isotope (79, 198, null, 2.69517*86400.0, Decay.BetaMinus), new Isotope (79, 199, null, 3.169*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Oro" }, { "fr","Or" }, { "it","Oro" }, { "ru","Золото" } }
        );

        public static Nuclide Mercury { get; } = new Nuclide
        (
            80, "Hg", nameof (Mercury), Category.TMetal, 6, 12,
            melt: 234.3210, boil: 629.88,
            weight: 200.592,
            known: 0,
            naming: "After the Roman god Mercury",
            isotopes: new Isotope[] { new Isotope (80, 194, null, 444*31556952.0, Decay.ECap1), new Isotope (80, 195, null, 9.9*3600, Decay.ECap1), new Isotope (80, 196, 0.15), new Isotope (80, 197, null, 64.14*3600, Decay.ECap1), new Isotope (80, 198, 10.04), new Isotope (80, 199, 16.94), new Isotope (80, 200, 23.14), new Isotope (80, 201, 13.17), new Isotope (80, 202, 29.74), new Isotope (80, 203, null, 46.612*86400.0, Decay.BetaMinus), new Isotope (80, 204, 6.82) },
            nameMap: new Dictionary<string,string>() { { "de","Quecksilber" }, { "es","Mercurio" }, { "fr","Mercure" }, { "it","Mercurio" }, { "ru","Ртуть" } }
        );

        public static Nuclide Thallium { get; } = new Nuclide
        (
            81, "Tl", nameof (Thallium), Category.PtMetal, 6, 13,
            melt: 577.0, boil: 1746.0,
            weight: 204.38,
            known: 1861, credit: "William Crookes",
            naming: "From Greek θαλλός (thallos), meaning 'a green shoot or twig'",
            isotopes: new Isotope[] { new Isotope (81, 203, 29.5), new Isotope (81, 204, null, 3.78*31556952.0, Decay.BetaMinus|Decay.ECap1), new Isotope (81, 205, 70.5) },
            nameMap: new Dictionary<string,string>() { { "es","Talio" }, { "it","Tallio" }, { "ru","Таллий" } }
        );

        public static Nuclide Lead { get; } = new Nuclide
        (
            82, "Pb", nameof (Lead), Category.PtMetal, 6, 14,
            melt: 600.61, boil: 2022.0,
            weight: 207.2,
            life: Nutrition.Absorbed,
            known: 0,
            naming: "From the Old English lēad",
            isotopes: new Isotope[] { new Isotope (82, 204, 1.4), new Isotope (82, 206, 24.1), new Isotope (82, 207, 22.1), new Isotope (82, 208, 52.4) },
            nameMap: new Dictionary<string,string>() { { "de","Blei" }, { "es","Plomo" }, { "fr","Plomb" }, { "it","Piombo" }, { "ru","Свинец" } }
        );

        public static Nuclide Bismuth { get; } = new Nuclide
        (
            83, "Bi", nameof (Bismuth), Category.PtMetal, 6, 15,
            melt: 544.7, boil: 1837.0,
            weight: 208.980,
            known: 0,
            naming: "Perhaps related to Old High German hwiz, meaning 'white'",
            isotopes: new Isotope[] { new Isotope (83, 207, null, 31.55*31556952.0, Decay.BetaPlus), new Isotope (83, 208, null, 3.68E5*31556952.0, Decay.BetaPlus), new Isotope (83, 209, 100.0, 2.01E19*31556952.0, Decay.Alpha), new Isotope (83, 210, 0.0, 5.012*86400.0, Decay.BetaMinus|Decay.Alpha), new Isotope (83, 210, null, 3.04E6*31556952.0, Decay.IT|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "de","Bismut" }, { "es","Bismuto" }, { "it","Bismuto" }, { "ru","Висмут" } }
        );

        public static Nuclide Polonium { get; } = new Nuclide
        (
            84, "Po", nameof (Polonium), Category.PtMetal, 6, 16,
            melt: 527.0, boil: 1235.0,
            weight: 209,
            known: 1898, credit: "Pierre Curie, Marie Curie",
            naming: "After Latin Polonia, meaning Poland",
            isotopes: new Isotope[] { new Isotope (84, 208, null, 2.898*31556952.0, Decay.Alpha|Decay.BetaPlus), new Isotope (84, 209, null, 125.2*31556952.0, Decay.Alpha|Decay.BetaPlus), new Isotope (84, 210, 0.0, 138.376*86400.0, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Polono" }, { "it","Polonio" }, { "ru","Полоний" } }
        );

        public static Nuclide Astatine { get; } = new Nuclide
        (
            85, "At", nameof (Astatine), Category.Halogen, 6, 17,
            melt: 503.0, boil: 503.0,
            weight: 210,
            known: 1940, credit: "Dale R. Corson, Kenneth Ross MacKenzie, Emilio Segrè",
            naming: "From the Greek αστατος (astatos), meaning 'unstable'",
            isotopes: new Isotope[] { new Isotope (85, 209, null, 5.41*3600, Decay.BetaPlus|Decay.Alpha), new Isotope (85, 210, null, 8.1*3600, Decay.BetaPlus|Decay.Alpha), new Isotope (85, 211, null, 7.21*3600, Decay.ECap1|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "de","Astat" }, { "es","Astato" }, { "fr","Astate" }, { "it","Astato" }, { "ru","Астат" } }
        );

        public static Nuclide Radon { get; } = new Nuclide
        (
            86, "Rn", nameof (Radon), Category.NobleGas, 6, 18,
            melt: 202.0, boil: 211.5,
            weight: 222,
            known: 1899, credit: "Ernest Rutherford, Robert B. Owens",
            naming: "After 'radium emanation'",
            isotopes: new Isotope[] { new Isotope (86, 210, null, 2.4*3600, Decay.Alpha), new Isotope (86, 211, null, 14.6*3600, Decay.ECap1|Decay.Alpha), new Isotope (86, 222, 0.0, 3.8235*86400.0, Decay.Alpha), new Isotope (86, 224, null, 1.8*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Radón" }, { "ru","Радон" } }
        );

        public static Nuclide Francium { get; } = new Nuclide
        (
            87, "Fr", nameof (Francium), Category.AlMetal, 7, 1,
            melt: 281.0, boil: 890.0,
            weight: 223,
            known: 1939, credit: "Marguerite Perey",
            naming: "After France",
            isotopes: new Isotope[] { new Isotope (87, 212, null, 20.0*60, Decay.BetaPlus|Decay.Alpha), new Isotope (87, 221, 0.0, 4.8*60, Decay.Alpha),new Isotope (87, 222, null, 14.2*60, Decay.BetaMinus), new Isotope (87, 223, 0.0, 22.0*60, Decay.BetaMinus|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Francio" }, { "it","Francio" }, { "ru","Франций" } }
        );

        public static Nuclide Radium { get; } = new Nuclide
        (
            88, "Ra", nameof (Radium), Category.AlEMetal, 7, 2,
            melt: 973.0, boil: 2010.0,
            weight: 226,
            known: 1898, credit: "Pierre Curie, Marie Curie",
            naming: "From Latin radius, meaning 'ray'",
            isotopes: new Isotope[] { new Isotope (88, 223, 0.0, 11.43*86400.0, Decay.Alpha), new Isotope (88, 224, 0.0, 3.6319*86400.0, Decay.Alpha), new Isotope (88, 225, 0.0, 14.9*86400.0, Decay.BetaMinus), new Isotope (88, 226, 0.0, 1600*31556952.0, Decay.Alpha), new Isotope (88, 228, 0.0, 5.75*31556952.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Radio" }, { "it","Radio" }, { "ru","Радий" } }
        );

        public static Nuclide Actinium { get; } = new Nuclide
        (
            89, "Ac", nameof (Actinium), Category.Actinoid, 7, 0,
            melt: 1500.0, boil: 5800.0,
            weight: 227,
            known: 1899, credit: "André-Louis Debierne",
            naming: "From Greek ακτίνος (aktinos), meaning beam or ray",
            isotopes: new Isotope[] { new Isotope (89, 225, 0.0, 10.0*86400.0, Decay.Alpha), new Isotope (89, 226, null, 29.37*3600, Decay.BetaMinus|Decay.ECap1|Decay.Alpha), new Isotope (89, 227, 0.0, 21.772*31556952.0, Decay.BetaMinus|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Actinio" }, { "it","Attinio" }, { "ru","Актиний" } }
        );

        public static Nuclide Thorium { get; } = new Nuclide
        (
            90, "Th", nameof (Thorium), Category.Actinoid, 7, 0,
            melt: 2023.0, boil: 5061.0,
            weight: 232.0377,
            known: 1829, credit: "Jöns Jakob Berzelius",
            naming: "After the Norse god of thunder 'Thor'",
            isotopes: new Isotope[] { new Isotope (90, 227, 0.0, 18.68*86400.0, Decay.Alpha), new Isotope (90, 228, 0.0, 1.9116*31556952.0, Decay.Alpha), new Isotope (90, 229, 0.0, 7917*31556952.0, Decay.Alpha), new Isotope (90, 230, 0.02, 75400*31556952.0, Decay.Alpha), new Isotope (90, 231, 0.0, 25.5*3600, Decay.BetaMinus), new Isotope (90, 232, 99.98, 1.405E10*31556952.0, Decay.Alpha), new Isotope (90, 234, 0.0, 24.1*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Torio" }, { "it","Torio" }, { "ru","Торий" } }
        );

        public static Nuclide Protactinium { get; } = new Nuclide
        (
            91, "Pa", nameof (Protactinium), Category.Actinoid, 7, 0,
            melt: 1841.0, boil: 4300.0,
            weight: 231.03588,
            known: 1913, credit: "Kasimir Fajans, Oswald Helmuth Göhring",
            naming: "From 'proto-actinium'",
            isotopes: new Isotope[] { new Isotope (91, 229, null, 1.5*86400.0, Decay.ECap1), new Isotope (91, 230, null, 17.4*86400.0, Decay.ECap1), new Isotope (91, 231, 100.0, 3.276E4*31556952.0, Decay.Alpha), new Isotope (91, 232, 0.0, 1.31*86400.0, Decay.BetaMinus), new Isotope (91, 233, 0.0, 26.967*86400.0, Decay.BetaMinus), new Isotope (91, 234, 0.0, 6.75*3600, Decay.BetaMinus), new Isotope (91, 234, 0.0, 1.17*3600, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "de","Protaktinium" }, { "es","Protactinio" }, { "it","Protoattinio" }, { "ru","Протактиний" } }
        );

        public static Nuclide Uranium { get; } = new Nuclide
        (
            92, "U", nameof (Uranium), Category.Actinoid, 7, 0,
            melt: 1405.3, boil: 4404.0,
            weight: 238.02891,
            known: 1789, credit: "Martin Heinrich Klaproth",
            naming: "After the planet Uranus",
            isotopes: new Isotope[] { new Isotope (92, 232, null, 68.9*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (92, 233, 0.0, 1.592E5*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (92, 234, 0.005, 2.455E5*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (92, 235, 0.720, 7.04E8*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (92, 236, 0.0, 2.342E7*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (92, 238, 99.274, 4.468E9*31556952.0, Decay.Fission|Decay.Alpha|Decay.Beta2) },
            nameMap: new Dictionary<string,string>() { { "de","Uran" }, { "es","Uranio" }, { "it","Uranio" }, { "ru","Уран" } }
        );

        public static Nuclide Neptunium { get; } = new Nuclide
        (
            93, "Np", nameof (Neptunium), Category.Actinoid, 7, 0,
            melt: 912.0, boil: 4447.0,
            weight: 237,
            known: 1940, credit: "Edwin McMillan and Philip H. Abelson",
            naming: "After the planet Neptune",
            isotopes: new Isotope[] { new Isotope (93, 235, null, 396.1*86400.0, Decay.Alpha|Decay.ECap1), new Isotope (93, 236, null, 1.54E5, Decay.ECap1|Decay.BetaMinus|Decay.Alpha), new Isotope (93, 237, 0.0, 2.144E6*31556952.0, Decay.Alpha), new Isotope (93, 239, 0.0, 2.356*86400.0, Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Neptunio" }, { "it","Nettunio" }, { "ru","Нептуний" } }
        );

        public static Nuclide Plutonium { get; } = new Nuclide
        (
            94, "Pu", nameof (Plutonium), Category.Actinoid, 7, 0,
            melt: 912.5, boil: 3505.0,
            weight: 244,
            known: 1940, credit: "Glenn T. Seaborg, Arthur Wahl, Joseph W. Kennedy, Edwin McMillan",
            naming: "After the dwarf planet Pluto",
            isotopes: new Isotope[] { new Isotope (94, 238, 0.0, 87.74*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (94, 239, 0.0, 2.41E4*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (94, 240, 0.0, 6500*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (94, 241, null, 14*31556952.0, Decay.BetaMinus|Decay.Fission), new Isotope (94, 242, null, 3.73E5*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (94, 244, 0.0, 8.08E7*31556952.0, Decay.Fission|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Plutonio" }, { "it","Plutonio" }, { "ru","Плутоний" } }
        );

        public static Nuclide Americium { get; } = new Nuclide
        (
            95, "Am", nameof (Americium), Category.Actinoid, 7, 0,
            melt: 1449.0, boil: 2880.0,
            weight: 243,
            known: 1944, credit: "Glenn T. Seaborg, Ralph A. James, Leon O. Morgan, Albert Ghiorso",
            naming: "After the Americas",
            isotopes: new Isotope[] { new Isotope (95, 241, null, 432.2*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (95, 242, null, 141*31556952.0, Decay.IT|Decay.Alpha|Decay.Fission), new Isotope (95, 243, null, 7370*31556952.0, Decay.Fission|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Americio" }, { "fr","Américium" }, { "it","Americio" }, { "ru","Америций" } }
        );

        public static Nuclide Curium { get; } = new Nuclide
        (
            96, "Cm", nameof (Curium), Category.Actinoid, 7, 0,
            melt: 1613.0, boil: 3383.0,
            weight: 247,
            known: 1944, credit: "Glenn T. Seaborg, Ralph A. James, Albert Ghiorso",
            naming: "After Marie Skłodowska-Curie and Pierre Curie",
            isotopes: new Isotope[] { new Isotope (96, 242, null, 160.0*86400.0, Decay.Fission|Decay.Alpha), new Isotope (96, 243, null, 29.1*31556952.0, Decay.Alpha|Decay.ECap1|Decay.Fission), new Isotope (96, 244, null, 18.1*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (96, 245, null, 8500*31556952.0, Decay.Fission|Decay.Alpha), new Isotope (96, 246, null, 4730*31556952.0, Decay.Alpha|Decay.Fission), new Isotope (96, 247, null, 1.56E7*31556952.0, Decay.Alpha), new Isotope (96, 248, null, 3.4E5*31556952.0, Decay.Alpha|Decay.Fission), new Isotope (96, 250, null, 9000*31556952.0, Decay.Fission|Decay.Alpha|Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Curio" }, { "it","Curio" }, { "ru","Кюрий" } }
        );

        public static Nuclide Berkelium { get; } = new Nuclide
        (
            97, "Bk", nameof (Berkelium), Category.Actinoid, 7, 0,
            melt: 1259.0, boil: 2900.0,
            weight: 247,
            known: 1949, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Berkeley, California",
            isotopes: new Isotope[] { new Isotope (97, 245, null, 4.94*86400.0, Decay.ECap1|Decay.Alpha), new Isotope (97, 246, null, 1.8*86400.0, Decay.Alpha|Decay.ECap1), new Isotope (97, 247, null, 1380*31556952.0, Decay.Alpha), new Isotope (97, 248, null, 300*31556952.0, Decay.Alpha), new Isotope (97, 249, null, 330.0*86400.0, Decay.Alpha|Decay.Fission|Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Berkelio" }, { "fr","Berkélium" }, { "it","Berkelio" }, { "ru","Берклий" } }
        );

        public static Nuclide Californium { get; } = new Nuclide
        (
            98, "Cf", nameof (Californium), Category.Actinoid, 7, 0,
            melt: 1173.0, boil: 1743.0,
            weight: 251,
            known: 1950, credit: "Lawrence Berkeley National Laboratory",
            naming: "After California",
            isotopes: new Isotope[] { new Isotope (98, 248, null, 333.5*86400.0, Decay.Alpha|Decay.Fission), new Isotope (98, 249, null, 351*31556952.0, Decay.Alpha|Decay.Fission), new Isotope (98, 250, null, 13.08*31556952.0, Decay.Alpha|Decay.Fission), new Isotope (98, 251, null, 898*31556952.0, Decay.Alpha), new Isotope (98, 252, null, 2.645*31556952.0, Decay.Alpha|Decay.Fission), new Isotope (98, 253, null, 17.81*86400.0, Decay.BetaMinus|Decay.Alpha), new Isotope (98, 254, null, 60.5*86400.0, Decay.Fission|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Californio" }, { "it","Californio" }, { "ru","Калифорний" } }
        );

        public static Nuclide Einsteinium { get; } = new Nuclide
        (
            99, "Es", nameof (Einsteinium), Category.Actinoid, 7, 0,
            melt: 1133.0, boil: 1269.0,
            weight: 252,
            known: 1952, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Albert Einstein",
            isotopes: new Isotope[] { new Isotope (99, 252, null, 471.7*86400.0, Decay.Alpha|Decay.ECap1|Decay.BetaMinus), new Isotope (99, 253, null, 20.47*86400.0, Decay.Fission|Decay.Alpha), new Isotope (99, 254, null, 275.7*86400.0, Decay.ECap1|Decay.BetaMinus|Decay.Alpha), new Isotope (99, 255, null, 39.8*86400.0, Decay.BetaMinus|Decay.Alpha|Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Einsteinio" }, { "it","Einsteinio" }, { "ru","Эйнштейний" } }
        );

        public static Nuclide Fermium { get; } = new Nuclide
        (
            100, "Fm", nameof (Fermium), Category.Actinoid, 7, 0,
            melt: 1800.0, boil: null,
            weight: 257,
            known: 1952, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Enrico Fermi",
            isotopes: new Isotope[] { new Isotope (100, 252, null, 25.39*3600, Decay.Fission|Decay.Alpha), new Isotope (100, 253, null, 3.0*86400.0, Decay.ECap1|Decay.Alpha), new Isotope (100, 255, null, 20.07*3600, Decay.Fission|Decay.Alpha), new Isotope (100, 257, null, 100.5*86400.0, Decay.Alpha|Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Fermio" }, { "it","Fermio" }, { "ru","Фермий" } }
        );

        public static Nuclide Mendelevium { get; } = new Nuclide
        (
            101, "Md", nameof (Mendelevium), Category.Actinoid, 7, 0,
            melt: 1100.0, boil: null,
            weight: 257,
            known: 1955, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Dmitri Mendeleev",
            isotopes: new Isotope[] { new Isotope (101, 256, null, 1.17*3600, Decay.ECap1), new Isotope (101, 257, null, 5.52*3600, Decay.ECap1|Decay.Alpha|Decay.Fission), new Isotope (101, 258, null, 51.5*86400.0, Decay.Alpha|Decay.ECap1|Decay.BetaMinus), new Isotope (101, 259, null, 1.6*3600, Decay.Fission|Decay.Alpha), new Isotope (101, 260, null, 31.8*86400.0, Decay.Fission|Decay.Alpha|Decay.ECap1|Decay.BetaMinus) },
            nameMap: new Dictionary<string,string>() { { "es","Mendelevio" }, { "fr","Mendelévium" }, { "it","Mendelevio" }, { "ru","Менделевий" } }
        );

        public static Nuclide Nobelium { get; } = new Nuclide
        (
            102, "No", nameof (Nobelium), Category.Actinoid, 7, 0,
            melt: 1100.0, boil: null,
            weight: 259,
            known: 1966, credit: "Joint Institute for Nuclear Research",
            naming: "After Alfred Nobel",
            isotopes: new Isotope[] { new Isotope (102, 253, null, 1.6*60, Decay.Alpha|Decay.BetaPlus), new Isotope (102, 254, null, 51.0, Decay.Alpha|Decay.BetaPlus), new Isotope (102, 255, null, 3.1*60, Decay.Alpha|Decay.BetaPlus), new Isotope (102, 257, null, 25.0, Decay.Alpha|Decay.BetaPlus), new Isotope (102, 259, null, 58.0*60, Decay.Alpha|Decay.ECap1|Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Nobelio" }, { "fr","Nobélium" }, { "it","Nobelio" }, { "ru","Нобелий" } }
        );

        public static Nuclide Lawrencium { get; } = new Nuclide
        (
            103, "Lr", nameof (Lawrencium), Category.TMetal, 7, 3,
            melt: 1900.0, boil: null,
            weight: 266,
            known: 1961, credit: "Lawrence Berkeley National Laboratory, Joint Institute for Nuclear Research",
            naming: "After Ernest Lawrence",
            isotopes: new Isotope[] { new Isotope (103, 254, null, 13.0, Decay.Alpha|Decay.ECap1), new Isotope (103, 255, null, 21.5, Decay.Alpha), new Isotope (103, 256, null, 27.0, Decay.Alpha), new Isotope (103, 259, null, 6.2, Decay.Alpha|Decay.Fission), new Isotope (103, 260, null, 2.7*60, Decay.Alpha), new Isotope (103, 261, null, 44.0*60, Decay.Fission), new Isotope (103, 262, null, 3.6*3600, Decay.ECap1), new Isotope (103, 263, null, 10.0*3600, Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Laurencio" }, { "it","Laurenzio" }, { "ru","Лоуренсий" } }
        );

        public static Nuclide Rutherfordium { get; } = new Nuclide
        (
            104, "Rf", nameof (Rutherfordium), Category.TMetal, 7, 4,
            melt: 2400.0, boil: null,
            weight: 267,
            known: 1964, credit: "Joint Institute for Nuclear Research, Lawrence Berkeley National Laboratory",
            naming: "After Ernest Rutherford",
            isotopes: new Isotope[] { new Isotope (104, 261, null, 70.0, Decay.Alpha|Decay.ECap1|Decay.Fission), new Isotope (104, 263, null, 15.0*60, Decay.Alpha|Decay.Fission), new Isotope (104, 265, null, 1.1*60, Decay.Fission), new Isotope (104, 266, null, 23.0, Decay.Fission), new Isotope (104, 267, null, 1.3*3600, Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Rutherfordio" }, { "it","Rutherfordio" }, { "ru","Резерфордий" } }
        );

        public static Nuclide Dubnium { get; } = new Nuclide
        (
            105, "Db", nameof (Dubnium), Category.TMetal, 7, 5,
            melt: null,
            weight: 268,
            known: 1970, credit: "Lawrence Berkeley Laboratory, Joint Institute for Nuclear Research",
            naming: "After Dubna, Moscow Oblast, Russia",
            isotopes: new Isotope[] { new Isotope (105, 262, null, 34.0, Decay.Alpha|Decay.Fission), new Isotope (105, 263, null, 27.0, Decay.Fission|Decay.Alpha|Decay.ECap1), new Isotope (105, 266, null, 20.0*60, Decay.Fission), new Isotope (105, 267, null, 1.2*3600, Decay.Fission), new Isotope (105, 268, null, 28.0*3600, Decay.Fission), new Isotope (105, 270, null, 15.0*3600, Decay.Fission|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Dubnio" }, { "it","Dubnio" }, { "ru","Дубний" } }
        );

        public static Nuclide Seaborgium { get; } = new Nuclide
        (
            106, "Sg", nameof (Seaborgium), Category.TMetal, 7, 6,
            melt: null,
            weight: 269,
            known: 1974, credit: "Lawrence Berkeley National Laboratory",
            naming: "After Glenn Seaborg",
            isotopes: new Isotope[] { new Isotope (106, 265, null, 8.9, Decay.Alpha), new Isotope (106, 267, null, 1.4*60, Decay.Alpha), new Isotope (106, 269, null, 14.0*60, Decay.Alpha), new Isotope (106, 271, null, 1.6*60, Decay.Alpha|Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Seaborgio" }, { "it","Seaborgio" }, { "ru","Сиборгий" } }
        );

        public static Nuclide Bohrium { get; } = new Nuclide
        (
            107, "Bh", nameof (Bohrium), Category.TMetal, 7, 7,
            melt: null,
            weight: 270,
            known: 1981, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Niels Bohr",
            isotopes: new Isotope[] { new Isotope (107, 267, null, 17.0, Decay.Alpha), new Isotope (107, 270, null, 60.0, Decay.Alpha), new Isotope (107, 271, null, 1.5, Decay.Alpha), new Isotope (107, 272, null, 11.0, Decay.Alpha), new Isotope (107, 274, null, 44.0, Decay.Alpha), new Isotope (107, 278, null, 11.5*60, Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Bohrio" }, { "it","Bohrio" }, { "ru","Борий" } }
        );

        public static Nuclide Hassium { get; } = new Nuclide
        (
            108, "Hs", nameof (Hassium), Category.TMetal, 7, 8,
            melt: null,
            weight: 269,
            known: 1984, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Latin Hassia, for Hesse, Germany",
            isotopes: new Isotope[] { new Isotope (108, 269, null, 16.0, Decay.Alpha), new Isotope (108, 271, null, 9.0, Decay.Alpha), new Isotope (108, 277, null, 110.0, Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Hassio" }, { "it","Hassio" }, { "ru","Хассий" } }
        );

        public static Nuclide Meitnerium { get; } = new Nuclide
        (
            109, "Mt", nameof (Meitnerium), Category.TMetal, 7, 9,
            melt: null,
            weight: 278,
            known: 1982, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Lise Meitner",
            isotopes: new Isotope[] { new Isotope (109, 274, null, 0.4, Decay.Alpha), new Isotope (109, 276, null, 0.6, Decay.Alpha), new Isotope (109, 278, null, 4.0, Decay.Alpha), new Isotope (109, 282, null, 67.0, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Meitnerio" }, { "fr","Meitnérium" }, { "it","Meitnerio" }, { "ru","Мейтнерий" } }
        );

        public static Nuclide Darmstadtium { get; } = new Nuclide
        (
            110, "Ds", nameof (Darmstadtium), Category.TMetal, 7, 10,
            melt: null,
            weight: 281,
            known: 1994, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Darmstadt, Germany",
            isotopes: new Isotope[] { new Isotope (110, 279, null, 0.2, Decay.Alpha|Decay.Fission), new Isotope (110, 281, null, 14.0, Decay.Fission|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Darmstadtio" }, { "it","Darmstadtio" }, { "ru","Дармштадтий" } }
        );

        public static Nuclide Roentgenium { get; } = new Nuclide
        (
            111, "Rg", nameof (Roentgenium), Category.TMetal, 7, 11,
            melt: null,
            weight: 282,
            known: 1994, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Wilhelm Röntgen",
            isotopes: new Isotope[] { new Isotope (111, 272, null, 0.002, Decay.Alpha), new Isotope (111, 274, null, 0.012, Decay.Alpha), new Isotope (111, 278, null, 0.004, Decay.Alpha), new Isotope (111, 279, null, 0.09, Decay.Alpha), new Isotope (111, 280, null, 4.6, Decay.Alpha), new Isotope (111, 281, null, 17.0, Decay.Fission|Decay.Alpha), new Isotope (111, 282, null, 100.0, Decay.Alpha), new Isotope (111, 283, null, 5.1*60, Decay.Fission), new Isotope (111, 286, null, 10.7*60, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Roentgenio" }, { "it","Roentgenio" }, { "ru","Рентгений" } }
        );

        public static Nuclide Copernicium { get; } = new Nuclide
        (
            112, "Cn", nameof (Copernicium), Category.TMetal, 7, 12,
            melt: null,
            weight: 285,
            known: 1996, credit: "Gesellschaft für Schwerionenforschung",
            naming: "After Nicolaus Copernicus",
            isotopes: new Isotope[] { new Isotope (112, 277, null, 0.00069, Decay.Alpha), new Isotope (112, 281, null, 0.18, Decay.Alpha), new Isotope (112, 282, null, 0.00091, Decay.Fission), new Isotope (112, 283, null, 4.2, Decay.Alpha|Decay.Fission), new Isotope (112, 284, null, 0.098, Decay.Fission|Decay.Alpha), new Isotope (112, 285, null, 28.0, Decay.Alpha), new Isotope (112, 286, null, 8.45, Decay.Fission) },
            nameMap: new Dictionary<string,string>() { { "es","Copernicio" }, { "it","Copernicio" }, { "ru","Коперниций" } }
        );

        public static Nuclide Nihonium { get; } = new Nuclide
        (
            113, "Nh", nameof (Nihonium), Category.PtMetal, 7, 13,
            melt: 700.0, boil: 1430.0,
            weight: 286,
            known: 2003, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Nihon (Japan)",
            isotopes: new Isotope[] { new Isotope (113, 278, null, 0.0014, Decay.Alpha), new Isotope (113, 282, null, 0.073, Decay.Alpha), new Isotope (113, 283, null, 0.075, Decay.Alpha), new Isotope (113, 284, null, 0.91, Decay.Alpha|Decay.ECap1), new Isotope (113, 285, null, 4.2, Decay.Alpha), new Isotope (113, 286, null, 9.5, Decay.Alpha), new Isotope (113, 287, null, 5.5, Decay.Alpha), new Isotope (113, 290, null, 2.0, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Nihonio" }, { "it","Nihonio" }, { "ru","Нихоний" } }
        );

        public static Nuclide Flerovium { get; } = new Nuclide
        (
            114, "Fl", nameof (Flerovium), Category.PtMetal, 7, 14,
            melt: null, boil: 210.0,
            weight: 286,
            known: 1999, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Flerov Laboratory of Nuclear Reactions",
            isotopes: new Isotope[] { new Isotope (114, 284, null, 0.0025, Decay.Fission), new Isotope (114, 285, null, 0.10, Decay.Alpha), new Isotope (114, 286, null, 0.12, Decay.Alpha|Decay.Fission), new Isotope (114, 287, null, 0.48, Decay.Alpha), new Isotope (114, 288, null, 0.66, Decay.Alpha), new Isotope (114, 289, null, 1.9, Decay.Alpha), new Isotope (114, 290, null, 19.0, Decay.ECap1|Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Flerovio" }, { "fr","Flérovium" }, { "it","Flerovio" }, { "ru","Флеровий" } }
        );

        public static Nuclide Moscovium { get; } = new Nuclide
        (
            115, "Mc", nameof (Moscovium), Category.PtMetal, 7, 15,
            melt: 670.0, boil: 1400.0,
            weight: 290,
            known: 2003, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Moscow",
            isotopes: new Isotope[] { new Isotope (115, 287, null, 0.037, Decay.Alpha), new Isotope (115, 288, null, 0.164, Decay.Alpha), new Isotope (115, 289, null, 0.33, Decay.Alpha), new Isotope (115, 290, null, 0.65, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Moscovio" }, { "it","Moscovio" }, { "ru","Московий" } }
        );

        public static Nuclide Livermorium { get; } = new Nuclide
        (
            116, "Lv", nameof (Livermorium), Category.PtMetal, 7, 16,
            melt: 673.0, boil: 1035.0,
            weight: 293,
            known: 2000, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Lawrence Livermore National Laboratory",
            isotopes: new Isotope[] { new Isotope (116, 290, null, 0.0083, Decay.Alpha), new Isotope (116, 291, null, 0.019, Decay.Alpha), new Isotope (116, 292, null, 0.013, Decay.Alpha), new Isotope (116, 293, null, 0.057, Decay.Alpha), new Isotope (116, 294, null, 0.054, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Livermorio" }, { "it","Livermorio" }, { "ru","Ливерморий" } }
        );

        public static Nuclide Tennessine { get; } = new Nuclide
        (
            117, "Ts", nameof (Tennessine), Category.Halogen, 7, 17,
            melt: 623.0, boil: 883.0,
            weight: 294,
            known: 2009, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory, Vanderbilt University, Oak Ridge National Laboratory",
            naming: "After Tennessee",
            isotopes: new Isotope[] { new Isotope (117, 293, null, 0.022, Decay.Alpha), new Isotope (117, 293, null, 0.051, Decay.Alpha) },
            nameMap: new Dictionary<string,string>() { { "es","Teneso" }, { "fr","Tennesse" }, { "it","Tennesso" }, { "ru","Теннессин" } }
        );

        public static Nuclide Oganesson { get; } = new Nuclide
        (
            118, "Og", nameof (Oganesson), Category.NobleGas, 7, 18,
            melt: 325.0, boil: 350.0,
            weight: 294,
            known: 2002, credit: "Joint Institute for Nuclear Research, Lawrence Livermore National Laboratory",
            naming: "After Yuri Oganessian",
            isotopes: new Isotope[] { new Isotope (118, 294, null, 0.00069, Decay.Fission|Decay.Alpha), new Isotope (118, 295, null, 0.181, Decay.Alpha) },
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

        public static ReadOnlyDictionary<string,int> MaxNameLengths
        = new ReadOnlyDictionary<string,int> (_maxNameLengths);


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

        // * suitable for legends
        // * singular form, sentence casing
        // * favor IUPAC recommendations
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

        public static ReadOnlyDictionary<string,string[]> DecayModeHeadings { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string,string[]>()
            {
                { "de", new string[] { "Isotop", "NH", ",5t", "ZA", "Zerfallsprodukt" } },
                { "en", new string[] { "Isotope", "Natural Abundance", "Half-life", "Decay Mode", "Product" } },
                { "es", new string[] { "Isótopo", "Abundancia natural", "Periodo", "MD", "Producto" } },
                { "fr", new string[] { "Isotope", "Abondance naturelle", "Période", "MD", "Produit" } },
                { "it", new string[] { "Isotopo", "Abbondanza in natura", "TD", "DM", "Prodotto" } },
                { "ru", new string[] { "RRR", "RRR", "RRR", "RRR", "RRR" } }
            });

        public static ReadOnlyCollection<string> LifeCodes { get; } = new ReadOnlyCollection<string> (new string[]
        { "", "EB", "ET", "BT", "A" });

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

        public static string OccurrenceCodes => "SCDP";

        public static ReadOnlyDictionary<string,string[]> OccurrenceNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string, string[]>()
            {
                { "de", new string[] { "Künstlichen", "Kosmogen", "Zerfall", "Primordial" } },
                { "en", new string[] { "Synthetic", "Cosmogenic", "Decay", "Primordial" } },
                { "es", new string[] { "Sintético", "Cosmogénicos", "Decadencia", "Primordial" } },
                { "fr", new string[] { "Synthétique", "Cosmogénique", "Désintégration", "Primordial" } },
                { "it", new string[] { "Sintetico", "Cosmogenico", "Decadimento", "Primordiali" } },
                { "ru", new string[] { "Синтезированные", "Космогенный", "Распад", "Изначальный" } }
            });

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

        public static int StabilityIndexMax => 5;

        public static string StateCodes => " SLG";

        public static ReadOnlyDictionary<string,string[]> StateNames { get; } = new ReadOnlyDictionary<string,string[]>
            (new Dictionary<string, string[]>()
            {
                { "de", new string[] { "Unbekannt", "Solide", "Flüssigkeit", "Gas" } },
                { "en", new string[] { "Unknown", "Solid", "Liquid", "Gas" } },
                { "es", new string[] { "Desconocido", "Sólido", "Líquida", "Gas" } },
                { "fr", new string[] { "Inconnue", "Solide", "Liquide", "Gaz" } },
                { "it", new string[] { "Sconosciuto", "Solido", "Liquido", "Gas" } },
                { "ru", new string[] { "Неизвестно", "Твердый", "жидкость", "Газ" } }
            });

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
                if (iso.Halflife == null)
                    ++StableCount;

                var trySix = iso.StabilityIndex;
                if (StabilityIndex > trySix)
                    StabilityIndex = trySix;
            }
        }

        public ReadOnlyDictionary<string,string> NameMap { get; }
        public ReadOnlyCollection<Isotope> Isotopes { get; }

        public int Z { get; }
        public string Symbol { get; }
        public string Name { get; }
        public int Period { get; }
        public int Group { get; }
        public Category Category { get; }
        public char Block { get; }
        public double? Melt { get; }
        public double? Boil { get; }
        public double Weight { get; }
        public State Atm0State => GetState (273.15);
        public int Atm0StateIndex => (int) GetState (273.15);
        public char Atm0StateCode => StateCodes[(int) GetState (273.15)];
        public int Known { get; }
        public int KnownIndex { get; }
        public string Credit { get; }
        public string Naming { get; }
        public Origin Occurrence { get; private set; }
        public int OccurrenceIndex => (int) Occurrence;
        public char OccurrenceCode => OccurrenceCodes[(int) Occurrence];
        public Nutrition Life { get; }
        public int LifeIndex => (int) Life;
        public string LifeCode => LifeCodes[(int) Life];
        public int StabilityIndex { get; }
        public int StableCount { get; }
        public int CategoryIndex => (int) Category;
        public string CategoryAbbr => Enum.GetName (typeof (Category), Category);

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

        public string GetName (string lang)
        {
            lang = lang.ToUpper();
            foreach (var pair in NameMap)
                if (pair.Key.ToUpper() == lang)
                    return pair.Value;
            return Name;
        }

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

        public override string ToString() => Symbol;

        public string ToFixedColumns (string lang)
        {
            var sb = new StringBuilder();

            var ts = Z.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.AppendFormat (ts);
            sb.Append (' ');
            sb.Append (Symbol);
            sb.Append (' ', 3 - Symbol.Length);
            ts = lang == null ? Name : GetName (lang);
            sb.Append (ts);
            sb.Append (' ', Nuclide.MaxNameLengths[lang ?? "en"] - ts.Length + 1);
            sb.Append (Period);
            ts = Group.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append (' ');
            sb.Append (CategoryAbbr);
            sb.Append (' ', 11 - CategoryAbbr.Length);
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
            sb.Append (Atm0StateCode);
            ts = Melt == null ? string.Empty : Melt.Value.ToString ("F4");
            sb.Append (' ', 10 - ts.Length);
            sb.Append (ts);
            ts = Boil == null ? string.Empty : Boil.Value.ToString ("F4");
            sb.Append (' ', 10 - ts.Length);
            sb.Append (ts);
            ts = Weight.ToString ("F4");
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            return sb.ToString();
        }

        public StringBuilder ToJsonSB (string quote)
        {
            var sb = new StringBuilder();
            sb.Append ($"{quote}z{quote}:");
            string ts = Z.ToString();
            sb.Append (' ', 3 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}symbol{quote}: \"{Symbol}\",");
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
            sb.Append ($", {quote}atm0StateIndex{quote}: ");
            sb.Append (Atm0StateIndex);
            sb.Append ($", {quote}melt{quote}:");
            ts = Melt == null ? "null" : Melt.Value.ToString ("F4");
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}boil{quote}:");
            ts = Boil == null ? "null" : Boil.Value.ToString ("F4");
            sb.Append (' ', 9 - ts.Length);
            sb.Append (ts);
            sb.Append ($", {quote}weight{quote}:");
            ts = Weight.ToString ("F4");
            sb.Append (' ', 9 - ts.Length);
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
                    sb.Append (',');
                sb.Append (Isotopes[ix].ToJson());
            }
            sb.Append (']');
            return sb;
        }

        public static IEnumerable<Nuclide> GetElements()
        {
            for (int ix = 1; ix < _nuclides.Length; ++ix)
                yield return _nuclides[ix];
        }

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
    }
}
