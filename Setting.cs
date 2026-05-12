using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace FertilityControl
{
    [FileLocation(nameof(FertilityControl))]
    [SettingsUIGroupOrder(kFertilityGroup, kNonRenewableGroup, kActionsGroup, kActionsGroup)]
    [SettingsUIShowGroupName(kFertilityGroup, kNonRenewableGroup, kActionsGroup, kActionsGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kFertilityGroup = "Fertility";
        public const string kNonRenewableGroup = "NonRenewable";
        public const string kActionsGroup = "Actions";

        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUISection(kSection, kFertilityGroup)]
        [SettingsUISlider(min = 0f, max = 100f, step = 1f, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public float FertilityPercentPerDay { get; set; } = 20f;

        [SettingsUISection(kSection, kNonRenewableGroup)]
        public bool EnableOreOilRegen { get; set; } = false;

        [SettingsUISection(kSection, kNonRenewableGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsOreOilDisabled))]
        [SettingsUISlider(min = 0f, max = 100f, step = 1f, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public float OrePercentPerDay { get; set; } = 0f;

        [SettingsUISection(kSection, kNonRenewableGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsOreOilDisabled))]
        [SettingsUISlider(min = 0f, max = 100f, step = 1f, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public float OilPercentPerDay { get; set; } = 0f;

        [SettingsUIButton]
        [SettingsUISection(kSection, kActionsGroup)]
        [SettingsUIButtonGroup("FertilityButtons")]
        public bool RestoreFertilityButton
        {
            set { Mod.System?.RestoreResource(FertilityFixSystem.ResourceKind.Fertility); }
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kActionsGroup)]
        [SettingsUIButtonGroup("FertilityButtons")]
        public bool DepleteFertilityButton
        {
            set { Mod.System?.DepleteResource(FertilityFixSystem.ResourceKind.Fertility); }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kActionsGroup)]
        [SettingsUIButtonGroup("OreButtons")]
        public bool RestoreOreButton
        {
            set { Mod.System?.RestoreResource(FertilityFixSystem.ResourceKind.Ore); }
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kActionsGroup)]
        [SettingsUIButtonGroup("OreButtons")]
        public bool DepleteOreButton
        {
            set { Mod.System?.DepleteResource(FertilityFixSystem.ResourceKind.Ore); }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kActionsGroup)]
        [SettingsUIButtonGroup("OilButtons")]
        public bool RestoreOilButton
        {
            set { Mod.System?.RestoreResource(FertilityFixSystem.ResourceKind.Oil); }
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kActionsGroup)]
        [SettingsUIButtonGroup("OilButtons")]
        public bool DepleteOilButton
        {
            set { Mod.System?.DepleteResource(FertilityFixSystem.ResourceKind.Oil); }
        }

        public bool IsOreOilDisabled() => !EnableOreOilRegen;

        public override void SetDefaults()
        {
            FertilityPercentPerDay = 20f;
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
                { m_Setting.GetOptionGroupLocaleID(Setting.kActionsGroup), "Actions" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FertilityPercentPerDay)), "Fertility regeneration bonus (% per day)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.FertilityPercentPerDay)),
                    "Extra fertility regen per in-game day, on top of the vanilla baseline (~8% per day on a max-fertility cell). Default: 20%. At 20% bonus a fully drained cell recovers in roughly 3–4 days. Set to 0 to disable. Pollution still slowly degrades fields."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableOreOilRegen)), "Regenerate ore & oil" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableOreOilRegen)),
                    "If enabled, ore and oil deposits slowly refill as in Easy mode, regardless of the current game mode. Default: off."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OrePercentPerDay)), "Ore regeneration (% per day)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OrePercentPerDay)),
                    "Percent of a cell's maximum ore deposit that refills each in-game day. Default: 0% (no refill). Default for Easy Mode: 6%. At 1% per day a fully drained deposit recovers in 100 days."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OilPercentPerDay)), "Oil regeneration (% per day)" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OilPercentPerDay)),
                    "Percent of a cell's maximum oil deposit that refills each in-game day. Default: 0% (no refill). Default for Easy Mode: 6%. At 1% per day a fully drained deposit recovers in 100 days."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreFertilityButton)), "Restore full fertility" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreFertilityButton)),
                    "Instantly refills every cell's fertility to its maximum."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepleteFertilityButton)), "Deplete all fertility" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DepleteFertilityButton)),
                    "Instantly drains every cell's fertility to zero."
                },
                {
                    m_Setting.GetOptionWarningLocaleID(nameof(Setting.DepleteFertilityButton)),
                    "This will drain all farmland fertility across the entire map. Active farms will starve. Continue?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreOreButton)), "Restore full ore" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreOreButton)),
                    "Instantly refills every cell's ore deposit to its maximum."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepleteOreButton)), "Deplete all ore" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DepleteOreButton)),
                    "Instantly drains every cell's ore deposit to zero."
                },
                {
                    m_Setting.GetOptionWarningLocaleID(nameof(Setting.DepleteOreButton)),
                    "This will drain all ore deposits across the entire map. Ore extractors will run dry. Continue?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreOilButton)), "Restore full oil" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreOilButton)),
                    "Instantly refills every cell's oil deposit to its maximum."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DepleteOilButton)), "Deplete all oil" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DepleteOilButton)),
                    "Instantly drains every cell's oil deposit to zero."
                },
                {
                    m_Setting.GetOptionWarningLocaleID(nameof(Setting.DepleteOilButton)),
                    "This will drain all oil deposits across the entire map. Oil extractors will run dry. Continue?"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
