using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;

namespace FertilityControl
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(FertilityControl)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public static FertilityFixSystem System { get; private set; }

        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));


            AssetDatabase.global.LoadSettings(nameof(FertilityControl), m_Setting, new Setting(this));

            updateSystem.UpdateAt<FertilityFixSystem>(SystemUpdatePhase.GameSimulation);
            System = updateSystem.World.GetOrCreateSystemManaged<FertilityFixSystem>();
            System.SetSetting(m_Setting);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
            System = null;
        }
    }
}