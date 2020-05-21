using CallOfTheWild;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace PsychicClassMod
{
    class ClassUpdates
    {
            static LibraryScriptableObject library => Main.library;

            public static void load()
            {
                updatePhrenicDabbler();
            }

            public static void updatePhrenicDabbler()
            {
            Main.logger.Log("Made it to update function");
            //guid for PhrenicDabblerFeature f5ab5bf71394419a87072445c46d3e79
            //guid for PsychicDetectivePhrenicPoolResource c144ac3c84e34ff7a8d6c683516b67f8
            //guid for PsychicDetectiveSpellbook ad5f7dbc7c3c44e3abcf59358a09f6ab
            //guid for InvestigatorClass 2217b38cb7c6460e8a98bcfb8a1c022c
            BlueprintAbilityResource bar = library.Get<BlueprintAbilityResource>("c144ac3c84e34ff7a8d6c683516b67f8");
            BlueprintSpellbook bs = library.Get<BlueprintSpellbook>("ad5f7dbc7c3c44e3abcf59358a09f6ab");
            BlueprintCharacterClass bcc = library.Get<BlueprintCharacterClass>("2217b38cb7c6460e8a98bcfb8a1c022c");

            PhrenicAmplificationContinuations engine = new PhrenicAmplificationContinuations(bar, bs, bcc, "PsychicDetective");

            BlueprintFeature relentlessCasting = engine.createRelentlessCasting();

            BlueprintFeatureSelection phrenicDabbler = library.Get<BlueprintFeatureSelection>("f5ab5bf71394419a87072445c46d3e79");
            phrenicDabbler.AllFeatures = phrenicDabbler.AllFeatures.AddToArray<BlueprintFeature>(relentlessCasting);
        }
     }
}
