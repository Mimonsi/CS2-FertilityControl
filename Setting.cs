using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using System.Collections.Generic;

namespace FertilityControl
{
    [FileLocation(nameof(FertilityControl))]
    [SettingsUIGroupOrder(kFertilityGroup, kNonRenewableGroup)]
    [SettingsUIShowGroupName(kFertilityGroup, kNonRenewableGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kFertilityGroup = "Fertility";
        public const string kNonRenewableGroup = "NonRenewable";

        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUISection(kSection, kFertilityGroup)]
        [SettingsUISlider(min = 0, max = 1000, step = 25, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int FertilityBonusRate { get; set; } = 200;

        [SettingsUISection(kSection, kNonRenewableGroup)]
        public bool EnableOreOilRegen { get; set; } = false;

        [SettingsUISection(kSection, kNonRenewableGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsOreOilDisabled))]
        [SettingsUISlider(min = 0f, max = 10f, step = 1f, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public float OrePercentPerDay { get; set; } = 0f;

        [SettingsUISection(kSection, kNonRenewableGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsOreOilDisabled))]
        [SettingsUISlider(min = 0f, max = 10f, step = 1f, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public float OilPercentPerDay { get; set; } = 0f;

        public bool IsOreOilDisabled() => !EnableOreOilRegen;

        public override void SetDefaults()
        {
            FertilityBonusRate = 200;
            EnableOreOilRegen = false;
            OrePercentPerDay = 0f;
            OilPercentPerDay = 0f;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Fertility Control" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kFertilityGroup), "Farmland Fertility" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kNonRenewableGroup), "Non-Renewable Resources" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FertilityBonusRate)), "Fertility regeneration bonus" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.FertilityBonusRate)),
                    "Extra fertility regen per tick on top of the vanilla 25. Default 200 keeps active farms healthy long-term. Pollution still slowly degrades fields."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableOreOilRegen)), "Regenerate ore & oil" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableOreOilRegen)),
                    "If enabled, ore and oil deposits slowly refill as in Easy mode, regardless of the current game mode."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OrePercentPerDay)), "Ore regeneration (% per day)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OrePercentPerDay)),
                    "Percent of a cell's maximum ore deposit that refills each in-game day."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OilPercentPerDay)), "Oil regeneration (% per day)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OilPercentPerDay)),
                    "Percent of a cell's maximum oil deposit that refills each in-game day."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
