using CallOfTheWild;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallOfTheWild.MetamagicFeats;

namespace PsychicClassMod
{
    static class MetamagicExpansion
    {
        //Need to reset the values of MetamagicExtender so I think I have to remake it here
        public enum FurtherMetamagicExtension
        {
            RelentlessCasting = 0x00000080
        }

        //public static void load()
        //{
        //}

        public interface IRuleSpellResistanceTriggered : IGlobalSubscriber
        {
            void ruleSavingThrowTriggered(RuleSpellResistanceCheck evt);
        }

        [Harmony12.HarmonyPatch(typeof(RuleSpellResistanceCheck))]
        [Harmony12.HarmonyPatch("OnTrigger", Harmony12.MethodType.Normal)]
        static class RuleSavingThrow_OnTrigger_Patch
        {
            internal static void Postfix(RuleSpellResistanceCheck __instance, RulebookEventContext context)
            {
                EventBus.RaiseEvent<IRuleSpellResistanceTriggered>((Action<IRuleSpellResistanceTriggered>)(h => h.ruleSavingThrowTriggered(__instance)));

                if (__instance.Initiator.Descriptor.State.IsDead)
                    return;


                var context2 = __instance.Reason?.Context;
                //var ability = __instance.Reason?.;

                if (context2 == null)
                {
                    return;
                }

                if (!context2.HasMetamagic((Metamagic)FurtherMetamagicExtension.RelentlessCasting))
                {
                    return;
                }

                if (__instance.IsSpellResisted)
                {
                    int old_value = __instance.Roll;
                    Harmony12.Traverse.Create(__instance).Property("Roll").SetValue(RulebookEvent.Dice.D20);
                    int new_value = __instance.Roll;
                    Common.AddBattleLogMessage(__instance.Initiator.CharacterName + " rerolls saving throw due to persistent spell: " + $"{old_value}  >>  {new_value}");
                }
            }
        }
    }
}