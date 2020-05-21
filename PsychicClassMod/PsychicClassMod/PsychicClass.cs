using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfTheWild;
using CowWithHatsCustomSpellsMod;
using Kingmaker.RuleSystem;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Designers.Mechanics.Facts;

namespace PsychicClassMod
{
    public class PsychicClass
    {
        static LibraryScriptableObject library => Main.library;
        static public BlueprintCharacterClass psychic_class;
        static public BlueprintProgression psychic_progression;
        static public BlueprintFeature psychic_proficiencies;
        static public BlueprintFeature psychic_knacks;
        static public BlueprintSpellbook psychic_spellbook;
        static public BlueprintFeatureSelection psychic_bonus_spells_selector;
        static public BlueprintParametrizedFeature psychic_extra_spells_feature;
        
        static public BlueprintFeatureSelection phrenic_amplifications_feature;
        static public BlueprintAbilityResource phrenic_pool_resource;
        static public BlueprintFeature phrenic_pool_display_feature;

        //theAmplifications
        static public BlueprintFeature focused_force;
        static public BlueprintFeature ongoing_defense;
        static public BlueprintFeature biokinetic_healing;
        static public BlueprintFeature conjured_armor;
        static public BlueprintFeature defensive_prognostication;
        static public BlueprintFeature minds_eye;
        static public BlueprintFeature overpowering_mind;
        static public BlueprintFeature will_of_the_dead;
        static public BlueprintFeature relentless_casting;

        public static bool test_mode = false;
        
        internal static void createPsychicClass()
        {
            Main.logger.Log($"Psychic class test mod: {test_mode.ToString()}");
            var animal_class = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>("4cd1757a0eea7694ba5c933729a53920");
            var sorcerer_class = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>("b3a505fb61437dc4097f43c3f8f9a4cf"); //sorcerer class guid
            var psychic_detective_archetype = Investigator.psychic_detective;

            psychic_class = Helpers.Create<BlueprintCharacterClass>();
            psychic_class.name = "PsychicClass";
            library.AddAsset(psychic_class, "74d341651d4c423e884aae7c742dbef8"); //fresh guid 

            psychic_class.LocalizedName = Helpers.CreateString("Psychic.Name", "Psychic");
            psychic_class.LocalizedDescription = Helpers.CreateString("Psychic.Description",
                "Within the mind of any sentient being lies power to rival that of the greatest magical artifact or holy site. By accessing these staggering vaults of mental energy, the psychic can shape the world around her, the minds of others, and pathways across the planes. No place or idea is too secret or remote for a psychic to access, and she can pull from every type of psychic magic. Many methods allow psychics to tap into their mental abilities, and the disciplines they follow affect their abilities.\n"
                + "Role: With a large suite of spells, psychics can handle many situations, but they excel at moving and manipulating objects, as well as reading and influencing thoughts."
                );
            psychic_class.m_Icon = psychic_class.Icon;
            psychic_class.SkillPoints = sorcerer_class.SkillPoints;
            psychic_class.HitDie = DiceType.D6;
            psychic_class.BaseAttackBonus = sorcerer_class.BaseAttackBonus;
            psychic_class.FortitudeSave = sorcerer_class.FortitudeSave;
            psychic_class.ReflexSave = sorcerer_class.ReflexSave;
            psychic_class.WillSave = sorcerer_class.WillSave;
            psychic_class.Spellbook = createPsychicSpellbook(sorcerer_class, psychic_detective_archetype);
            psychic_class.ClassSkills = new StatType[] { StatType.SkillKnowledgeArcana, StatType.SkillKnowledgeWorld, StatType.SkillLoreNature, StatType.SkillLoreReligion, StatType.SkillPerception, StatType.SkillPersuasion};
            psychic_class.IsArcaneCaster = false;
            psychic_class.IsDivineCaster = false;
            psychic_class.PrimaryColor = sorcerer_class.PrimaryColor;
            psychic_class.SecondaryColor = sorcerer_class.SecondaryColor;
            psychic_class.RecommendedAttributes = new StatType[] { StatType.Intelligence };
            psychic_class.NotRecommendedAttributes = new StatType[0];
            psychic_class.EquipmentEntities = sorcerer_class.EquipmentEntities;
            psychic_class.MaleEquipmentEntities = sorcerer_class.MaleEquipmentEntities;
            psychic_class.FemaleEquipmentEntities = sorcerer_class.FemaleEquipmentEntities;
            psychic_class.StartingGold = sorcerer_class.StartingGold;
            psychic_class.StartingItems = sorcerer_class.StartingItems;

            psychic_class.AddComponent(Helpers.Create<PrerequisiteNoClassLevel>(p => p.CharacterClass = animal_class));

            createPsychicExtraSpellsFeatures();
            createPsychicProgression();
            
            psychic_class.Progression = psychic_progression;
            Helpers.RegisterClass(psychic_class);
        }

        private static void createPsychicExtraSpellsFeatures()
        {
            //BloodlineArcaneNewArcanaSelection   20a2435574bdd7f4e947f405df2b25ce Kingmaker.Blueprints.Classes.Selection.BlueprintFeatureSelection
            psychic_extra_spells_feature = library.CopyAndAdd<BlueprintParametrizedFeature>("4a2e8388c2f0dd3478811d9c947bebfb", "PsychicBonusSpellsFeature", "");//NewArcana feature
            LearnSpellParametrized learn_spell = psychic_extra_spells_feature.GetComponent<LearnSpellParametrized>().CreateCopy<LearnSpellParametrized>();
            psychic_extra_spells_feature.SetComponents(new BlueprintComponent[] { learn_spell});
            psychic_extra_spells_feature.SetDescription("At 2nd level, a psychic adds any one spell from the psychic spell list to her list of spells known. This spell must be of a level that she is capable of casting. She can also add one additional spell at 9th level and 17th level.");
            psychic_extra_spells_feature.SetName("Psychic Bonus Spell");
            psychic_extra_spells_feature.SpellcasterClass = psychic_class;
            psychic_extra_spells_feature.SpellList = psychic_spellbook.SpellList;
            psychic_extra_spells_feature.Groups = new FeatureGroup[] { FeatureGroup.None };
            learn_spell.SpellcasterClass = psychic_class;
            learn_spell.SpellList = psychic_spellbook.SpellList;

            psychic_bonus_spells_selector = Helpers.CreateFeatureSelection("PsychicBonusSpellsFeatureSelection",
                "Psychic Bonus Spell",
                "At 2nd level, a psychic adds any one spell from the psychic spell list to her list of spells known. This spell must be of a level that she is capable of casting. She can also add one additional spell at 9th level and 17th level.",
                "",
                psychic_extra_spells_feature.Icon,
                FeatureGroup.None
                );
            psychic_bonus_spells_selector.AllFeatures = new BlueprintFeature[] { psychic_extra_spells_feature };
        }

        private static void createPsychicProgression()
        {

            //createPsychicDisciplines
            createKnacks();
            
            psychic_progression = Helpers.CreateProgression("PsychicProgression",
                                       psychic_class.Name,
                                       psychic_class.Description,
                                       "785003e8aa014a8f9901b38700d8e6f2", //fresh guid
                                       psychic_class.Icon,
                                       FeatureGroup.None);
            psychic_progression.Classes = getPsychicArray();


            var psychic_proficiencies = library.CopyAndAdd<BlueprintFeature>("25c97697236ccf2479d0c6a4185eae7f",
                "PsychicProficiencies",
                "6fb9fbf71f4b42398b28a2f6d0a0521e" //fresh guid
                );
            psychic_proficiencies.SetName("Psychic Proficiencies");
            psychic_proficiencies.SetDescription("Psychics are proficient with all simple weapons. They are not proficient with any type of armor or shield. Unlike Arcane casters, Psychics trained in their use suffer no spell failure chance from using armor");

            var detect_magic = library.Get<BlueprintFeature>("ee0b69e90bac14446a4cf9a050f87f2e");
            createPhrenicAmplificationsFeatures();

            //level entries of features like phrenic discipline and such
            var entries = new List<LevelEntry>();
            entries.Add(Helpers.LevelEntry(1, psychic_proficiencies, 
                psychic_knacks, 
                detect_magic, 
                phrenic_pool_display_feature, 
                phrenic_amplifications_feature,
                library.Get<BlueprintFeature>("d3e6275cfa6e7a04b9213b7b292a011c"), // ray calculate feature
                library.Get<BlueprintFeature>("62ef1cdb90f1d654d996556669caf7fa"),  // touch calculate feature
                library.Get<BlueprintFeature>("9fc9813f569e2e5448ddc435abf774b3") //full caster feature
                ));

            psychic_progression.UIGroups = new UIGroup[1] { Helpers.CreateUIGroup(phrenic_amplifications_feature) };
            for(int i=3; i<=20; i++)
            {
                if ((i - 3) % 4 == 0)
                {
                    entries.Add(Helpers.LevelEntry(i, phrenic_amplifications_feature));
                    psychic_progression.UIGroups[0].Features.Add(phrenic_amplifications_feature);
                }
                else
                    entries.Add(Helpers.LevelEntry(i));
            }
            psychic_progression.UIGroups = psychic_progression.UIGroups.AddToArray<UIGroup>(Helpers.CreateUIGroup(psychic_extra_spells_feature));
            entries.Add(Helpers.LevelEntry(2, psychic_bonus_spells_selector));
            psychic_progression.UIGroups[0].Features.Add(psychic_bonus_spells_selector);
            entries.Add(Helpers.LevelEntry(9, psychic_bonus_spells_selector));
            psychic_progression.UIGroups[0].Features.Add(psychic_bonus_spells_selector);
            entries.Add(Helpers.LevelEntry(17, psychic_bonus_spells_selector));
            psychic_progression.UIGroups[0].Features.Add(psychic_bonus_spells_selector);
            //see the createWitchProgression function to see what to do next for phrenic amps
            //this is also probably where phrenic disciplines should live and that should use oracle mysteries as a base
            psychic_progression.UIDeterminatorsGroup = new BlueprintFeatureBase[] { psychic_proficiencies, psychic_knacks, detect_magic };
            psychic_progression.LevelEntries = entries.ToArray();
            BlueprintParametrizedFeature newArcana = library.Get<BlueprintParametrizedFeature>("4a2e8388c2f0dd3478811d9c947bebfb");
        }

        private static void createPhrenicAmplificationsFeatures()
        {
            phrenic_pool_resource = Helpers.CreateAbilityResource("PsychicPhrenicPoolResource", "Phrenic Pool","", "", 
                null);
            phrenic_pool_display_feature = Helpers.CreateFeature("PsychicPhrenicPoolResourceDisplayFeature",
                "Phrenic Pool",
                "A psychic has a pool of supernatural mental energy that she can draw upon to manipulate psychic spells as she casts them. The maximum number of points in a psychic’s phrenic pool is equal to 1/2 her psychic level + her Wisdom or Charisma modifier, as determined by her psychic discipline. The phrenic pool is replenished each morning after 8 hours of rest or meditation. The psychic might be able to recharge points in her phrenic pool in additional circumstances dictated by her psychic discipline. Points gained in excess of the pool’s maximum are lost.",
                "",
                null,
                FeatureGroup.None);
            //to adjust the StatType to appropriately use the one that matches the Psychics phrenic discipline
            //will have to call these lines
            //var amount = getMaxAmount(phrenic_pool_resource);
            //Helpers.SetField(amount, "ResourceBonusStat", StatType.Wisdom);//it starts as charisma so we won't have to do anything for those
            //setMaxAmount(phrenic_pool_resource, amount);
            //phrenic_pool_resource.SetIncreasedByLevelStartPlusDivStep(0, 2, 1, 2, 1, 0, 0.0f, getPsychicArray());
            phrenic_pool_resource.SetIncreasedByLevelStartPlusDivStepAndStatBonus(0, 2, 1, 2, 1, 0, 0.0f, getPsychicArray(), StatType.Charisma);

            phrenic_amplifications_feature = Helpers.CreateFeatureSelection("PsychicPhrenicAmplificationsFeature",
                "Phrenic Amplifications",
                "A psychic develops particular techniques to empower her spellcasting, called phrenic amplifications. The psychic can activate a phrenic amplification only while casting a spell using psychic magic, and the amplification modifies either the spell’s effects or the process of casting it. The spell being cast is called the linked spell. The psychic can activate only one amplification each time she casts a spell, and doing so is part of the action used to cast the spell. She can use any amplification she knows with any psychic spell, unless the amplification’s description states that it can be linked only to certain types of spells. A psychic learns one phrenic amplification at 1st level. At 3rd level and every 4 levels thereafter, the psychic learns a new phrenic amplification. A phrenic amplification can’t be selected more than once. Once a phrenic amplification has been selected, it can’t be changed. Phrenic amplifications require the psychic to expend 1 or more points from her phrenic pool to function.",
                "",
                null,
                FeatureGroup.None,
                Helpers.CreateAddAbilityResource(phrenic_pool_resource)
                );

            var phrenic_amplifications_engine = new PhrenicAmplificationContinuations(phrenic_pool_resource, psychic_spellbook, psychic_class, "Psychic");
            
            biokinetic_healing = phrenic_amplifications_engine.createBiokineticHealing();
            conjured_armor = phrenic_amplifications_engine.createConjuredArmor();
            defensive_prognostication = phrenic_amplifications_engine.createDefensivePrognostication();
            focused_force = phrenic_amplifications_engine.createFocusedForce();
            minds_eye = phrenic_amplifications_engine.createMindsEye();
            ongoing_defense = phrenic_amplifications_engine.createOngoingDefense();
            overpowering_mind = phrenic_amplifications_engine.createOverpoweringMind();
            relentless_casting = phrenic_amplifications_engine.createRelentlessCasting();
            will_of_the_dead = phrenic_amplifications_engine.createWillOfTheDead();

            phrenic_amplifications_feature.AllFeatures = new BlueprintFeature[]
                {
                     biokinetic_healing,
                conjured_armor,
                defensive_prognostication,
                focused_force,
                minds_eye,
                overpowering_mind,
                will_of_the_dead,
                ongoing_defense,
                relentless_casting
                };


            //Investigator.phrenic_dabbler.AllFeatures;


        }

        private static BlueprintCharacterClass[] getPsychicArray()
        {
            return new BlueprintCharacterClass[] { psychic_class };
        }

        private static void createKnacks()
        {
            var daze = library.Get<BlueprintAbility>("55f14bc84d7c85446b07a1b5dd6b2b4c");
            psychic_knacks = Common.createCantrips("PsychicKnacksFeature",
                "Knacks",
                "Psychics learn a number of knacks, or 0-level spells. These spells are cast like any other spell, but they don’t consume any slots and can be used again. Knacks cast using other spell slots.",
                daze.Icon,
                "2eb3fdc283384dfab7ab19d39062ca36",//fresh guid
                psychic_class,
                StatType.Intelligence,
                psychic_class.Spellbook.SpellList.SpellsByLevel[0].Spells.ToArray());
        }

        private static BlueprintSpellbook createPsychicSpellbook(BlueprintCharacterClass sorcererForReference, BlueprintArchetype psychicDetectiveForReference)
        {
            psychic_spellbook = Helpers.Create<BlueprintSpellbook>();
            psychic_spellbook.name = "PsychicSpellbook";
            library.AddAsset(psychic_spellbook, "0dad06ad889d49c297056834980d6ee4");//fresh guid
            psychic_spellbook.Name = psychic_class.LocalizedName;
            psychic_spellbook.SpellsPerDay = sorcererForReference.Spellbook.SpellsPerDay;
            psychic_spellbook.SpellsKnown = sorcererForReference.Spellbook.SpellsKnown;
            psychic_spellbook.Spontaneous = true;
            psychic_spellbook.IsArcane = false;
            psychic_spellbook.AllSpellsKnown = false;
            psychic_spellbook.CanCopyScrolls = false;
            psychic_spellbook.CastingAttribute = StatType.Intelligence;
            psychic_spellbook.CharacterClass = psychic_class;
            psychic_spellbook.CasterLevelModifier = 0;
            psychic_spellbook.CantripsType = CantripsType.Cantrips;
            psychic_spellbook.SpellsPerLevel = sorcererForReference.Spellbook.SpellsPerLevel;

            psychic_spellbook.SpellList = Helpers.Create<BlueprintSpellList>();
            psychic_spellbook.SpellList.name = "PsychicSpellList";
            library.AddAsset(psychic_spellbook.SpellList, "fec41181285d4a089acbc07df01fc5a5");//fresh guid 
            psychic_spellbook.SpellList.SpellsByLevel = new SpellLevelList[10];
            for(int i = 0; i < psychic_spellbook.SpellList.SpellsByLevel.Length; i++)
            {
                psychic_spellbook.SpellList.SpellsByLevel[i] = new SpellLevelList(i);
            }

            Common.SpellId[] spells = createNonDetectiveSpellIds();

            foreach(var spell_id in spells)
            {
                var spell = library.Get<BlueprintAbility>(spell_id.guid);
                spell.AddToSpellList(psychic_spellbook.SpellList, spell_id.level);
                
            }

            foreach(SpellLevelList spellLevelList in Investigator.psychic_detective_spellbook.SpellList.SpellsByLevel)
            {
                foreach (BlueprintAbility spell in spellLevelList.Spells)
                {
                    //this pulls every spell from the Psychic Detective list and adds them 
                    //except for the two early access spells namely find traps and banishment

                    if (spell.AssetGuid != "4709274b2080b6444a3c11c6ebbe2404" && spell.AssetGuid != "d361391f645db984bbf58907711a146a")
                    {
                        //Main.logger.Log($"Spell to add {spell.Name} with type {spell.GetType().ToString()}");
                        spell.AddToSpellList(psychic_spellbook.SpellList, spellLevelList.SpellLevel);
                    }
                }
            }

            return psychic_spellbook;
        }

        //create spell ids for all the spells that the psychic detective does not get, this is for spells above spell level 6
        private static Common.SpellId[] createNonDetectiveSpellIds()
        {
            return new Common.SpellId[]
            {
            new Common.SpellId("4709274b2080b6444a3c11c6ebbe2404", 2),//FindTraps  

            new Common.SpellId("d361391f645db984bbf58907711a146a", 7),//Banishment  
            new Common.SpellId("6f1dcf6cfa92d1948a740195707c0dbe", 7),//FingerOfDeath   
            new Common.SpellId(CallOfTheWild.NewSpells.fly_mass.AssetGuid, 7),//FlyMassAbility 
            new Common.SpellId("603e6473ae8f4aeaa93c0e0469ff3125", 7),//HoldPersonMassAbility
            new Common.SpellId("2b044152b3620c841badb090e01ed9de", 7),//Insanity
            new Common.SpellId("98310a099009bbd4dbdf66bcef58b4cd", 7),//InvisibilityMass
            new Common.SpellId("5c8cde7f0dcec4e49bfa2632dfe2ecc0", 7),//KiShout
            new Common.SpellId("df2a0ba6b6dcecf429cbb80a56fee5cf", 7),//MindBlank
            new Common.SpellId(CallOfTheWild.NewSpells.particulate_form.AssetGuid, 7),//ParticulateFormAbility
            new Common.SpellId("261e1788bfc5ac1419eec68b1d485dbc", 7),//PowerWordBlind
            new Common.SpellId("ab167fd8203c1314bac6568932f1752f", 7),//SummonMonsterVIIBase
            new Common.SpellId("1e2d1489781b10a45a3b70192bba9be3", 7),//WavesOfEctasy
            new Common.SpellId("3e4d3b9a5bd03734d9b053b9067c2f38", 7), //WavesOfExhaustion  

            new Common.SpellId("a5c56f0f699daec44b7aedd8b273b08a", 8), //BrilliantInspiration  
            new Common.SpellId("c3d2294a6740bc147870fff652f3ced5", 8), //DeathClutch
            new Common.SpellId("cbf3bafa8375340498b86a3313a11e2f", 8), //EuphoricTranquility
            new Common.SpellId("e788b02f8d21014488067bdd3ba7b325", 8), //FrightfulAspect
            new Common.SpellId(CallOfTheWild.NewSpells.irresistible_dance.AssetGuid, 8), //IrresistibleDanceAbility
            new Common.SpellId("87a29febd010993419f2a4a9bee11cfc", 8), //MindBlankCommunal
            new Common.SpellId("f958ef62eea5050418fb92dfa944c631", 8), //PowerWordStun
            new Common.SpellId("0e67fa8f011662c43934d486acc50253", 8), //PredictionOfFailure
            new Common.SpellId("42aa71adc7343714fa92e471baa98d42", 8), //ProtectionFromSpells
            new Common.SpellId("fd0d3840c48cafb44bb29e8eb74df204", 8), //ShoutGreater
            new Common.SpellId("d3ac756a229830243a72e84f3ab050d0", 8), //SummonMonsterVIIIBase
            new Common.SpellId(CallOfTheWild.NewSpells.temporal_stasis.AssetGuid, 8), //TemporalStasisAbility

            new Common.SpellId("3c17035ec4717674cae2e841a190e757", 9), //DominateMonster
            new Common.SpellId("1f01a098d737ec6419aedc4e7ad61fdd", 9), //Foresight
            new Common.SpellId("43740dab07286fe4aa00a6ee104ce7c1", 9), //HeroicInvocation
            new Common.SpellId("8edcdef6e9a24053a8654bfb53ff5c05", 9), //HoldMonsterMassAbility
            new Common.SpellId(CowWithHatsCustomSpellsMod.NewSpells.mages_disjunction.AssetGuid, 9), //MagesDisjunctionAbility
            new Common.SpellId("41cf93453b027b94886901dbfc680cb9", 9), //OverwhelmingPresence
            new Common.SpellId("2f8a67c483dfa0f439b293e094ca9e3c", 9), //PowerWordKill
            new Common.SpellId(CallOfTheWild.NewSpells.mass_suffocation.AssetGuid, 9), //SuffocationMassAbility
            new Common.SpellId("52b5df2a97df18242aec67610616ded0", 9), //SummonMonsterIXBase
            new Common.SpellId(CallOfTheWild.NewSpells.time_stop.AssetGuid, 9), //TimeStopAbility
            new Common.SpellId("b24583190f36a8442b212e45226c54fc", 9), //WailOfBanshee
            new Common.SpellId("870af83be6572594d84d276d7fc583e0", 9), //Weird

            };
        }
    }
}
