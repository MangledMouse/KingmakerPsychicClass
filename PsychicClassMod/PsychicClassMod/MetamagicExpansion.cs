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
            void ruleSpellResistanceCheckTriggered(RuleSpellResistanceCheck evt);
        }

        [Harmony12.HarmonyPatch(typeof(RuleSpellResistanceCheck))]
        [Harmony12.HarmonyPatch("OnTrigger", Harmony12.MethodType.Normal)]
        static class ruleSpellResistanceCheck_OnTrigger_Patch
        {
            internal static void Postfix(RuleSpellResistanceCheck __instance, RulebookEventContext context)
            {
                //Main.logger.Log("Made it to spell resistance check");
                EventBus.RaiseEvent<IRuleSpellResistanceTriggered>((Action<IRuleSpellResistanceTriggered>)(h => h.ruleSpellResistanceCheckTriggered(__instance)));

                if (__instance.Initiator.Descriptor.State.IsDead)
                    return;

                var contextRelevant = __instance.Context;

                if (contextRelevant == null)
                    return;

                if (!contextRelevant.HasMetamagic((Metamagic)FurtherMetamagicExtension.RelentlessCasting))
                {
                    return;
                }

                if (__instance.IsSpellResisted)
                {
                    int old_value = __instance.Roll;
                    Harmony12.Traverse.Create(__instance).Property("Roll").SetValue(RulebookEvent.Dice.D20);
                    int new_value = __instance.Roll;
                    Common.AddBattleLogMessage(__instance.Initiator.CharacterName + " rerolls saving throw due to relentless casting: " + $"{old_value}  >>  {new_value}");
                }
            }
        }
    }
}