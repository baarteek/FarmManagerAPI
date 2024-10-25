using System.ComponentModel;

namespace FarmManagerAPI.Models.Enums
{
    public enum AgrotechnicalIntervention
    {
        [Description("Brak wybranej wartości")]
        None,

        [Description("Działanie rolno-środowiskowo-klimatyczne PROW 2014-2020")]
        PRSK1420,

        [Description("Rolnictwo ekologiczne PROW 2014-2020")]
        RE1420,

        [Description("Płatności rolno-środowiskowo-klimatyczne WPR PS")]
        ZRSK2327,

        [Description("Rolnictwo ekologiczne WPR PS")]
        RE2327,

        [Description("Międzyplony ozime lub wsiewki śródplonowe")]
        E_MPW,

        [Description("Opracowanie i przestrzeganie planu nawożenia: wariant podstawowy lub wariant z wapnowaniem")]
        E_OPN,

        [Description("Uproszczone systemy uprawy")]
        E_USU,

        [Description("Wymieszanie słomy z glebą")]
        E_WSG,

        [Description("Biologiczna ochrona upraw")]
        E_BOU
    }
}
