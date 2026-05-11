using Game;
using Game.Simulation;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace FertilityControl
{
    [UpdateAfter(typeof(NaturalResourceSystem))]
    public partial class FertilityFixSystem : GameSystemBase
    {
        // Match the vanilla NaturalResourceSystem cadence so "per tick" math lines up.
        private const int kUpdateInterval = 8192;

        // One in-game day spans 262144 sim frames. At our cadence that's 262144 / 8192 = 32 ticks/day.
        private const float kPerTickDayFraction = 8192f / 262144f;

        private NaturalResourceSystem m_NaturalResourceSystem;
        private Setting m_Setting;

        public void SetSetting(Setting setting) => m_Setting = setting;

        public override int GetUpdateInterval(SystemUpdatePhase phase) => kUpdateInterval;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_NaturalResourceSystem = World.GetOrCreateSystemManaged<NaturalResourceSystem>();
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (m_Setting == null)
            {
                return;
            }

            int fertilityBonus = math.max(0, m_Setting.FertilityBonusRate);
            bool oreOilEnabled = m_Setting.EnableOreOilRegen;

            if (fertilityBonus == 0 && !oreOilEnabled)
            {
                return;
            }

            JobHandle inDeps;
            CellMapData<NaturalResourceCell> data = m_NaturalResourceSystem.GetData(false, out inDeps);

            var job = new BoostJob
            {
                m_CellData = data,
                m_FertilityBonus = fertilityBonus,
                m_OreOilEnabled = oreOilEnabled ? (byte)1 : (byte)0,
                m_OrePerTick = m_Setting.OrePercentPerDay / 100f * kPerTickDayFraction,
                m_OilPerTick = m_Setting.OilPercentPerDay / 100f * kPerTickDayFraction,
            };

            JobHandle handle = job.Schedule(
                data.m_TextureSize.x * data.m_TextureSize.y,
                64,
                JobHandle.CombineDependencies(inDeps, Dependency));

            m_NaturalResourceSystem.AddWriter(handle);
            Dependency = handle;
        }

        [BurstCompile]
        private struct BoostJob : IJobParallelFor
        {
            public CellMapData<NaturalResourceCell> m_CellData;
            public int m_FertilityBonus;
            public byte m_OreOilEnabled;
            public float m_OrePerTick;
            public float m_OilPerTick;

            public void Execute(int index)
            {
                NaturalResourceCell cell = m_CellData.m_Buffer[index];

                if (m_FertilityBonus > 0)
                {
                    int used = math.max(0, (int)cell.m_Fertility.m_Used - m_FertilityBonus);
                    cell.m_Fertility.m_Used = (ushort)math.min(used, (int)cell.m_Fertility.m_Base);
                }

                if (m_OreOilEnabled != 0)
                {
                    float oreDrop = (float)cell.m_Ore.m_Base * m_OrePerTick;
                    cell.m_Ore.m_Used = (ushort)math.max(0f, (float)cell.m_Ore.m_Used - oreDrop);

                    float oilDrop = (float)cell.m_Oil.m_Base * m_OilPerTick;
                    cell.m_Oil.m_Used = (ushort)math.max(0f, (float)cell.m_Oil.m_Used - oilDrop);
                }

                m_CellData.m_Buffer[index] = cell;
            }
        }
    }
}
