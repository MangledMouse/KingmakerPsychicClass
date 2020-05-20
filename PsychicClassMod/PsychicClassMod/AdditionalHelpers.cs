using CallOfTheWild;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychicClassMod
{
    public static class FurtherExtensionMethods
    {
        internal static readonly FastSetter setMaxAmount = Helpers.CreateFieldSetter<BlueprintAbilityResource>("m_MaxAmount");
        internal static readonly FastGetter getMaxAmount = Helpers.CreateFieldGetter<BlueprintAbilityResource>("m_MaxAmount");
        public static void SetIncreasedByLevelStartPlusDivStepAndStatBonus(this BlueprintAbilityResource resource, int baseValue,
            int startingLevel, int startingIncrease, int levelStep, int perStepIncrease, int minClassLevelIncrease, float otherClassesModifier, 
            BlueprintCharacterClass[] classes, StatType statType, BlueprintArchetype[] archetypes = null)
        {
            var amount = getMaxAmount(resource);
            Helpers.SetField(amount, "BaseValue", baseValue);
            Helpers.SetField(amount, "IncreasedByLevelStartPlusDivStep", true);
            Helpers.SetField(amount, "StartingLevel", startingLevel);
            Helpers.SetField(amount, "StartingIncrease", startingIncrease);
            Helpers.SetField(amount, "LevelStep", levelStep);
            Helpers.SetField(amount, "PerStepIncrease", perStepIncrease);
            Helpers.SetField(amount, "MinClassLevelIncrease", minClassLevelIncrease);
            Helpers.SetField(amount, "OtherClassesModifier", otherClassesModifier);
            Helpers.SetField(amount, "IncreasedByStat", true);
            Helpers.SetField(amount, "ResourceBonusStat", statType);

            Helpers.SetField(amount, "ClassDiv", classes);
            var emptyArchetypes = Array.Empty<BlueprintArchetype>();
            Helpers.SetField(amount, "ArchetypesDiv", archetypes ?? emptyArchetypes);

            // Enusre arrays are at least initialized to empty.
            var fieldName = "Class";
            if (Helpers.GetField(amount, fieldName) == null) Helpers.SetField(amount, fieldName, Array.Empty<BlueprintCharacterClass>());
            fieldName = "Archetypes";
            if (Helpers.GetField(amount, fieldName) == null) Helpers.SetField(amount, fieldName, emptyArchetypes);

            setMaxAmount(resource, amount);
        }
    }
    class AdditionalHelpers
    {
    }
}
