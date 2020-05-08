﻿using Kingmaker.Blueprints;
using CallOfTheWild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;

namespace PsychicClassMod
{
    static class ClassUpdates
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

            PhrenicAmplificationsContinuation engine = new PhrenicAmplificationsContinuation(bar, bs, bcc, "PsychicDetective");

            BlueprintFeature relentlessCasting = engine.createRelentlessCasting();

            BlueprintFeatureSelection phrenicDabbler = library.Get<BlueprintFeatureSelection>("f5ab5bf71394419a87072445c46d3e79");
            phrenicDabbler.AllFeatures.AddToArray<BlueprintFeature>(relentlessCasting);
            //BlueprintFeature focused_force = engine.createFocusedForce();
            //BlueprintFeature biokinetic_healing = engine.createBiokineticHealing();
            //BlueprintFeature conjured_armor = engine.createConjuredArmor();
            //BlueprintFeature defensive_prognostication = engine.createDefensivePrognostication();
            //BlueprintFeature minds_eye = engine.createMindsEye();
            //BlueprintFeature overpowering_mind = engine.createOverpoweringMind();
            //BlueprintFeature will_of_the_dead = engine.createWillOfTheDead();
            //BlueprintFeature ongoing_defense = engine.createOngoingDefense();

            //phrenicDabbler.AllFeatures = new BlueprintFeature[]
            //{
            //    biokinetic_healing,
            //    conjured_armor,
            //    defensive_prognostication,
            //    focused_force,
            //    minds_eye,
            //    overpowering_mind,
            //    will_of_the_dead,
            //    ongoing_defense,
            //    relentlessCasting
            //};
        }
    }
}