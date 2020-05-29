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
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Blueprints.Root;

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
        //static public BlueprintFeatureSelection phrenic_amplifications_feature_wisdom;
        static public BlueprintAbilityResource phrenic_pool_resource_charisma;
        static public BlueprintAbilityResource phrenic_pool_resource_wisdom;
        static public BlueprintFeature phrenic_pool_display_feature_charisma;
        static public BlueprintFeature phrenic_pool_display_feature_wisdom;

        //discipline elemnts things
        //static Dictionary<BlueprintProgression, BlueprintAbility[]> discipline_spells_map = new Dictionary<BlueprintProgression, BlueprintAbility[]>();
        static public BlueprintFeatureSelection psychic_disciplines;
        static DisciplineEngine discipline_engine;

        //the individual disciplines
        static public BlueprintProgression enlightenment_discipline;
        static public BlueprintProgression dream_discipline;
        static public BlueprintProgression abomination_discipline;

        //theAmplifications, since they are fueled by different thing there need to be 2 each
        static public BlueprintFeature[] charisma_amps;
        static public BlueprintFeature focused_force_cha;
        static public BlueprintFeature ongoing_defense_cha;
        static public BlueprintFeature biokinetic_healing_cha;
        static public BlueprintFeature conjured_armor_cha;
        static public BlueprintFeature defensive_prognostication_cha;
        static public BlueprintFeature minds_eye_cha;
        static public BlueprintFeature overpowering_mind_cha;
        static public BlueprintFeature will_of_the_dead_cha;
        static public BlueprintFeature relentless_casting_cha;

        static public BlueprintFeature[] wisdom_amps;
        static public BlueprintFeature focused_force_wis;
        static public BlueprintFeature ongoing_defense_wis;
        static public BlueprintFeature biokinetic_healing_wis;
        static public BlueprintFeature conjured_armor_wis;
        static public BlueprintFeature defensive_prognostication_wis;
        static public BlueprintFeature minds_eye_wis;
        static public BlueprintFeature overpowering_mind_wis;
        static public BlueprintFeature will_of_the_dead_wis;
        static public BlueprintFeature relentless_casting_wis;

        //Feats
        static public BlueprintFeature extra_amplification_feat;
        static public BlueprintFeature extra_phrenic_pool_feat_cha;
        //static public BlueprintFeature extra_amplification_feat_wis;
        static public BlueprintFeature extra_phrenic_pool_feat_wis;

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
            psychic_class.ClassSkills = new StatType[] { StatType.SkillKnowledgeArcana, StatType.SkillKnowledgeWorld, StatType.SkillLoreNature, StatType.SkillLoreReligion, StatType.SkillPerception, StatType.SkillPersuasion };
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
            //createDisciplines();

            psychic_class.Progression = psychic_progression;
            Helpers.RegisterClass(psychic_class);
            createPsychicFeats();
        }

        private static void createAllDisciplines()
        {
            psychic_disciplines = Helpers.CreateFeatureSelection("PsychicDisiplinesSelection",
                                                              "Psychic Disciplines",
                                                              "Each psychic has a particular method through which they access and improve their mental prowess. This is called her psychic discipline. " +
                                                              "Psychic disciplines grant bonus spells known and a selection of discipline powers. Each discipline grants a bonus spell known at level 1, level 4" +
                                                              "and every two levels thereafter up to level 18. Disciplines also grant powers at level 1, 5 and 13. The DC to save against these powers is equal to" +
                                                              "10 + 1/2 the psychic's level + the psychic's Intelligence modifier. \n" +
                                                              "Psychics also gain access to set a of techniques to empower their spell casting called phrenic amplifications. These techniques are used to empower " +
                                                              "psychic spellcasting and they are fueled by a pool of mental energy called the phrenic pool. " +
                                                              "The total number of points available in the psychic's phrenic pool is based on either her Wisdom or Charisma score. Which of these stats is used is " +
                                                              "determined by her discipline.",
                                                              "",
                                                              null,
                                                              FeatureGroup.BloodLine);

            discipline_engine = new DisciplineEngine();
            createEnlightenmentDiscipline();
            createDreamDiscipline();
            createAbominationDiscipline();

            psychic_disciplines.AllFeatures = new BlueprintFeature[] { enlightenment_discipline, dream_discipline };
            //psychic_disciplines.AllFeatures = new BlueprintFeature[] { enlightenment_discipline, dream_discipline};

        }

        static void createEnlightenmentDiscipline()
        {
            var spells = new BlueprintAbility[]
            {
                CowWithHatsCustomSpellsMod.NewSpells.heightened_awareness,
                CowWithHatsCustomSpellsMod.NewSpells.acute_senses,
                CallOfTheWild.NewSpells.countless_eyes,
                library.Get<BlueprintAbility>("c927a8b0cd3f5174f8c0b67cdbfde539"),//remove blindness
                library.Get<BlueprintAbility>("b3da3fbee6a751d4197e446c7e852bcb"),//true seeing
                library.Get<BlueprintAbility>("9f5ada581af3db4419b54db77f44e430"),// owl's wisdom mass
                library.Get<BlueprintAbility>("df2a0ba6b6dcecf429cbb80a56fee5cf"),//mind blank
                library.Get<BlueprintAbility>("42aa71adc7343714fa92e471baa98d42"),//protection from spells
                library.Get<BlueprintAbility>("41cf93453b027b94886901dbfc680cb9")//overwhelmining presence
            };

            enlightenment_discipline = createDisciplineWithoutPowers("EnlightenmentDiscipline",
                "Enlightenment",
                "Phrenic Pool Ability: Wisdom \n" +
                "Your quest for enlightenment has opened your eyes to new concepts and heights of spiritual awareness. You seek learning that allows you to evolve in mind and spirit, improving your next incarnation.",
                null,
                StatType.Wisdom,
                spells);

        }

        static void createDreamDiscipline()
        {
            var spells = new BlueprintAbility[]
                {
                    library.Get<BlueprintAbility>("bb7ecad2d3d2c8247a38f44855c99061"),//sleep
                    library.Get<BlueprintAbility>("fd4d9fd7f87575d47aafe2a64a6e2d8d"),//hideous laughter
                    library.Get<BlueprintAbility>("7658b74f626c56a49939d9c20580885e"),//deep slumber
                    library.Get<BlueprintAbility>("cf6c901fb7acc904e85c63b342e9c949"),//confusion
                    library.Get<BlueprintAbility>("12fb4a4c22549c74d949e2916a2f0b6a"),//phantasmal web
                    library.Get<BlueprintAbility>("7f71a70d822af94458dc1a235507e972"),//cloak of dreams
                    library.Get<BlueprintAbility>("98310a099009bbd4dbdf66bcef58b4cd"),//invisibility mass
                    CallOfTheWild.NewSpells.temporal_stasis,//temporal stasis
                    library.Get<BlueprintAbility>("3c17035ec4717674cae2e841a190e757")//dominate monster
                };
            dream_discipline = createDisciplineWithoutPowers("DreamDiscipline",
                "Dream",
                "Phrenic Pool Ability: Charisma \n" +
                "You discover deeper and more powerful corners of your mind through journeys you make in your dreams. Your consciousness expands outward into other dreaming minds, allowing you to explore the psychic landscapes of unconsciousness or regions of nightmare and horror.",
                null,
                StatType.Charisma,
                spells);
        }

        static void createAbominationDiscipline()
        {
            //useful info for making abomination discipline
            //RingOfCircumstancesSpellDCAbility	0810beaa61d766148bad72f83af0cec3	Kingmaker.UnitLogic.ActivatableAbilities.BlueprintActivatableAbility
            //that buff uses a IncreaseSpellSchoolDC component with school none

            //info for bleed effect
            //DuelistCripplingCriticalBleedActiveBuff d35102fe10a4171439c1da9be38ddb99    Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff
            //The bleed from that is handled with a buff that has the following componenets
            //a AddContextAction that hurts them on NewRound, 
            //a SpellDescriptorComponent with bleed, 
            //a RemovedByHeal component
            //and a CombatStateTrigger (this one is unclear)
            //DuelistCripplingCriticalBleedActiveBuff d35102fe10a4171439c1da9be38ddb99    Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff
            //BlueprintBuff duelistBleedBuff = library.Get<BlueprintBuff>("d35102fe10a4171439c1da9be38ddb99");
            //foreach (BlueprintComponent bc in duelistBleedBuff.GetComponents<BlueprintComponent>())
            //{
            //    Main.logger.Log($"bleed buff component {bc.name} and type {bc.GetType().ToString()}");
            //    AddInitiatorAttackWithWeaponTrigger aiawwt = bc as AddInitiatorAttackWithWeaponTrigger;
            //    if (aiawwt != null)
            //    {
            //        Main.logger.Log($"AddInitiatorAttackWithWeaponTrigger components {aiawwt}");
            //        foreach (GameAction ga in aiawwt.Action.Actions)
            //        {
            //            Main.logger.Log($"The actions on contact {ga.name} with type {ga.GetType().ToString()}");
            //            ContextActionApplyBuff applyAction = ga as ContextActionApplyBuff;
            //            if(applyAction !=null)
            //            {
            //                Main.logger.Log($"The buff that bleeds {applyAction.Buff.name} with type {applyAction.Buff.GetType().ToString()}");
            //                foreach(BlueprintComponent innerBC in applyAction.Buff.GetComponents<BlueprintComponent>())
            //                {
            //                    Main.logger.Log($"The true buff of note has component {innerBC.name} with type {innerBC.GetType().ToString()}");
            //                    AddFactContextActions afca = innerBC as AddFactContextActions;
            //                    if (afca != null)
            //                    {
            //                        foreach (ContextAction ca in afca.Activated.Actions)
            //                            Main.logger.Log($"An action when the bleed buff is activated: {ca.name} with type {ca.GetType().ToString()}");
            //                        foreach (ContextAction ca in afca.Deactivated.Actions)
            //                            Main.logger.Log($"An action when the bleed buff is Deactivated: {ca.name} with type {ca.GetType().ToString()}");
            //                        foreach (ContextAction ca in afca.NewRound.Actions)
            //                            Main.logger.Log($"An action when the bleed buff is NewRound: {ca.name} with type {ca.GetType().ToString()}");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //var ringOfCirumDCAbility = library.Get<BlueprintActivatableAbility>("0810beaa61d766148bad72f83af0cec3");
            //foreach(BlueprintComponent bc in ringOfCirumDCAbility.Buff.GetComponents<BlueprintComponent>())
            //{
            //    Main.logger.Log($"Ring of Circumstances spell dc ability buff component {bc.name} and type {bc.GetType().ToString()}");
            //    IncreaseSpellSchoolDC increase = bc as IncreaseSpellSchoolDC;
            //    if (increase != null)
            //        Main.logger.Log($"Ring's Increase spell school dc component school type {increase.School}");
            //}
        }

        static BlueprintProgression createDisciplineWithoutPowers(string name, string display_name, string description, UnityEngine.Sprite icon, StatType disciplineStat, BlueprintAbility[] spells)
        {
            var discipline = Helpers.CreateProgression(name + "Progression",
                display_name,
                description,
                "",
                icon,
                FeatureGroup.BloodLine);
            discipline.Classes = getPsychicArray();
            List<BlueprintFeature> level1Features = new List<BlueprintFeature>();
            //spells, discipline powers and the appropriate number of elements for phrenic amps
            var entries = new List<LevelEntry>();
            //discipline.UIGroups = Helpers.CreateUIGroups();//
            discipline.UIGroups = new UIGroup[2] { Helpers.CreateUIGroup(), Helpers.CreateUIGroup() };
            //add discipline spells
            for (int i = 0; i < spells.Length; i++)
            {
                var feat = Helpers.CreateFeature(name + spells[i].name,
                                                 spells[i].Name,
                                                 "At 1st level, 4th level and every two levels thereafter, a psychic learns an additional " +
                                                 "spell derived from her discipline.\n"
                                                 + spells[i].Name + ": " + spells[i].Description,
                                                 "",
                                                 spells[i].Icon,
                                                 FeatureGroup.None,
                                                 spells[i].CreateAddKnownSpell(psychic_class, i + 1)
                                                 );
                if (i == 0)
                    level1Features.Add(feat);
                    //entries.Add(Helpers.LevelEntry(1, feat));
                else
                    entries.Add(Helpers.LevelEntry(2 * (i + 1), feat));
                discipline.UIGroups[0].Features.Add(feat);
            }

            //add phrenic amp selections
            if (disciplineStat == StatType.Charisma)
            {
                discipline.UIGroups = discipline.UIGroups.AddToArray<UIGroup>(Helpers.CreateUIGroup(phrenic_pool_display_feature_charisma));
                level1Features.Add(phrenic_pool_display_feature_charisma);
                //entries.Add();
                discipline.UIGroups[1].Features.Add(phrenic_pool_display_feature_charisma);
                phrenic_amplifications_feature.AddComponent(Helpers.CreateAddAbilityResource(phrenic_pool_resource_charisma));
                phrenic_amplifications_feature.AllFeatures = phrenic_amplifications_feature.AllFeatures.AddToArray(charisma_amps);
            }
            if(disciplineStat == StatType.Wisdom)
            {
                discipline.UIGroups = discipline.UIGroups.AddToArray<UIGroup>(Helpers.CreateUIGroup(phrenic_pool_display_feature_wisdom));
                discipline.UIGroups[1].Features.Add(phrenic_pool_display_feature_wisdom);
                level1Features.Add(phrenic_pool_display_feature_wisdom);
                //entries.Add(Helpers.LevelEntry(1, phrenic_pool_display_feature_wisdom));
                phrenic_amplifications_feature.AddComponent(Helpers.CreateAddAbilityResource(phrenic_pool_resource_wisdom));
                phrenic_amplifications_feature.AllFeatures = phrenic_amplifications_feature.AllFeatures.AddToArray(wisdom_amps);
            }
            //if (disciplineStat == StatType.Wisdom)
            //{
            //    discipline.UIGroups = discipline.UIGroups.AddToArray<UIGroup>(Helpers.CreateUIGroup(phrenic_pool_display_feature_wisdom));
            //    discipline.UIGroups[1].Features.Add(phrenic_amplifications_feature_wisdom);
            //    discipline.UIGroups[0].Features.Add(phrenic_pool_display_feature_wisdom);
            //    entries.Add(Helpers.LevelEntry(1, phrenic_pool_display_feature_wisdom, phrenic_amplifications_feature_wisdom));
            //    for (int i = 3; i <= 20; i++)
            //    {
            //        if ((i - 3) % 4 == 0)
            //        {
            //            entries.Add(Helpers.LevelEntry(i, phrenic_amplifications_feature_wisdom));
            //            discipline.UIGroups[1].Features.Add(phrenic_amplifications_feature_wisdom);
            //        }
            //        //else
            //        //    entries.Add(Helpers.LevelEntry(i));
            //    }
            //}
            entries.Add(Helpers.LevelEntry(1, level1Features));
            discipline.LevelEntries = entries.ToArray();
            return discipline;
        }


        private static void createPsychicFeats()
        {
            var extra_amplification_selection = Helpers.CreateFeatureSelection("ExtraAmplificationFeatCharismaPsychic",
                "Extra Phrenic Amplification",
                "You gain one additional phrenic amplification.\n" +
                "Special: you can take this feat multiple times. Each time you do gain another amplfication.",
                "",
                null,
                FeatureGroup.Feat,
                //this should be a prerequisite features from list with number 1 and it should be either phrenic amps feature cha or phrenic amps feature wis
                phrenic_amplifications_feature.PrerequisiteFeature()
                ) ;
            //var extra_amplification_selection_wis = Helpers.CreateFeatureSelection("ExtraAmplificationFeatWisdomPsychic",
            //    "Extra Phrenic Amplification (Wisdom Psychic)",
            //    "You gain one additional phrenic amplification.\n" +
            //    "Special: you can take this feat multiple times. Each time you do gain another amplfication.",
            //    "",
            //    null,
            //    FeatureGroup.Feat,
            //    //this should be a prerequisite features from list with number 1 and it should be either phrenic amps feature cha or phrenic amps feature wis
            //    phrenic_pool_display_feature_wisdom.PrerequisiteFeature()
            //    );
            extra_amplification_selection.AllFeatures = phrenic_amplifications_feature.AllFeatures;
            extra_amplification_feat = extra_amplification_selection;
            extra_amplification_feat.Ranks = 10;
            extra_amplification_feat.Groups = new FeatureGroup[] { FeatureGroup.Feat };
            //extra_amplification_selection_wis.AllFeatures = phrenic_amplifications_feature_wisdom.AllFeatures;
            //extra_amplification_feat_wis = extra_amplification_selection_cha;
            //extra_amplification_feat_wis.Ranks = 10;
            //extra_amplification_feat_wis.Groups = new FeatureGroup[] { FeatureGroup.Feat };

            var extra_phrenic_pool_feat_cha = Helpers.CreateFeature("ExpandedPhrenicPoolFeatCharisma",
                "Expanded Phrenic Pool (Charisma)",
                "Your phrenic pool total increases by 2 points.",
                "",
                Helpers.GetIcon("42f96fc8d6c80784194262e51b0a1d25"), //extra arcana as used by psychic detective's feature version of this feat
                FeatureGroup.Feat,
                
                phrenic_pool_display_feature_charisma.PrerequisiteFeature(),
                Helpers.Create<IncreaseResourceAmount>(i => { i.Resource = phrenic_pool_resource_charisma; i.Value = 2; })
                );
            var extra_phrenic_pool_feat_wis = Helpers.CreateFeature("ExpandedPhrenicPoolFeatWisdom",
                "Expanded Phrenic Pool (Wisdom)",
                "Your phrenic pool total increases by 2 points.",
                "",
                Helpers.GetIcon("42f96fc8d6c80784194262e51b0a1d25"), //extra arcana as used by psychic detective's feature version of this feat
                FeatureGroup.Feat,

                phrenic_pool_display_feature_wisdom.PrerequisiteFeature(),
                Helpers.Create<IncreaseResourceAmount>(i => { i.Resource = phrenic_pool_resource_wisdom; i.Value = 2; })
                );

            library.AddFeats(extra_amplification_feat, extra_phrenic_pool_feat_cha, extra_phrenic_pool_feat_wis);
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
            createAllDisciplines();

            //level entries of features like bonus spells and proficiencies
            var entries = new List<LevelEntry>();
            entries.Add(Helpers.LevelEntry(1, psychic_proficiencies, 
                psychic_knacks, 
                detect_magic,
                psychic_disciplines,
                phrenic_amplifications_feature,
                //phrenic_pool_display_feature, 
                //phrenic_amplifications_feature,
                library.Get<BlueprintFeature>("d3e6275cfa6e7a04b9213b7b292a011c"), // ray calculate feature
                library.Get<BlueprintFeature>("62ef1cdb90f1d654d996556669caf7fa"),  // touch calculate feature
                library.Get<BlueprintFeature>("9fc9813f569e2e5448ddc435abf774b3") //full caster feature
                ));

            psychic_progression.UIGroups = psychic_progression.UIGroups.AddToArray<UIGroup>(Helpers.CreateUIGroup(psychic_extra_spells_feature));
            entries.Add(Helpers.LevelEntry(2, psychic_bonus_spells_selector));
            psychic_progression.UIGroups[0].Features.Add(psychic_bonus_spells_selector);
            entries.Add(Helpers.LevelEntry(9, psychic_bonus_spells_selector));
            psychic_progression.UIGroups[0].Features.Add(psychic_bonus_spells_selector);
            entries.Add(Helpers.LevelEntry(17, psychic_bonus_spells_selector));
            psychic_progression.UIGroups[0].Features.Add(psychic_bonus_spells_selector);

            psychic_progression.UIGroups = psychic_progression.UIGroups.AddToArray<UIGroup>(Helpers.CreateUIGroup(phrenic_amplifications_feature));
            psychic_progression.UIGroups[1].Features.Add(phrenic_amplifications_feature);
            //entries.Add(Helpers.LevelEntry(1, phrenic_amplifications_feature));
            for (int i = 3; i <= 20; i++)
            {
                if ((i - 3) % 4 == 0)
                {
                    entries.Add(Helpers.LevelEntry(i, phrenic_amplifications_feature));
                    psychic_progression.UIGroups[1].Features.Add(phrenic_amplifications_feature);
                }
                //else
                //    entries.Add(Helpers.LevelEntry(i));
            }

            psychic_progression.UIDeterminatorsGroup = new BlueprintFeatureBase[] { psychic_disciplines, psychic_proficiencies, psychic_knacks, detect_magic };
            psychic_progression.LevelEntries = entries.ToArray();
            //BlueprintParametrizedFeature newArcana = library.Get<BlueprintParametrizedFeature>("4a2e8388c2f0dd3478811d9c947bebfb");
        }

        private static void createPhrenicAmplificationsFeatures()
        {
            //to adjust the StatType to appropriately use the one that matches the Psychics phrenic discipline
            //will have to call these lines
            //var amount = getMaxAmount(phrenic_pool_resource);
            //Helpers.SetField(amount, "ResourceBonusStat", StatType.Wisdom);//it starts as charisma so we won't have to do anything for those
            //setMaxAmount(phrenic_pool_resource, amount);
            //phrenic_pool_resource.SetIncreasedByLevelStartPlusDivStep(0, 2, 1, 2, 1, 0, 0.0f, getPsychicArray());
            phrenic_pool_resource_charisma = Helpers.CreateAbilityResource("PsychicPhrenicPoolResourceCharisma", "Phrenic Pool","", "", 
                null);
            phrenic_pool_display_feature_charisma = Helpers.CreateFeature("PsychicPhrenicPoolResourceDisplayFeatureCharisma",
                "Phrenic Pool (Charisma)",
                "A psychic has a pool of supernatural mental energy that she can draw upon to manipulate psychic spells as she casts them. The maximum number of points in a " +
                "psychic’s phrenic pool is equal to 1/2 her psychic level + her Charisma modifier, as determined by her psychic discipline. " +
                "The phrenic pool is replenished each morning after 8 hours of rest or meditation. The psychic might be able to recharge points in her phrenic pool " +
                "in additional circumstances dictated by her psychic discipline. Points gained in excess of the pool’s maximum are lost.",
                "",
                null,
                FeatureGroup.None);
            phrenic_pool_resource_wisdom = Helpers.CreateAbilityResource("PsychicPhrenicPoolResourceWisdom", "Phrenic Pool", "", "",
                null);
            phrenic_pool_display_feature_wisdom = Helpers.CreateFeature("PsychicPhrenicPoolResourceDisplayFeatureWisdom",
                "Phrenic Pool (Wisdom)",
                "A psychic has a pool of supernatural mental energy that she can draw upon to manipulate psychic spells as she casts them. The maximum number of points in a " +
                "psychic’s phrenic pool is equal to 1/2 her psychic level + her Wisdom modifier, as determined by her psychic discipline. " +
                "The phrenic pool is replenished each morning after 8 hours of rest or meditation. The psychic might be able to recharge points in her phrenic pool " +
                "in additional circumstances dictated by her psychic discipline. Points gained in excess of the pool’s maximum are lost.",
                "",
                null,
                FeatureGroup.None);

            phrenic_pool_resource_wisdom.SetIncreasedByLevelStartPlusDivStepAndStatBonus(0, 2, 1, 2, 1, 0, 0.0f, getPsychicArray(), StatType.Wisdom);
            phrenic_pool_resource_charisma.SetIncreasedByLevelStartPlusDivStepAndStatBonus(0, 2, 1, 2, 1, 0, 0.0f, getPsychicArray(), StatType.Charisma);

            phrenic_amplifications_feature = Helpers.CreateFeatureSelection("PsychicCharismaPhrenicAmplificationsFeature",
               "Phrenic Amplifications",
               "A psychic develops particular techniques to empower her spellcasting, called phrenic amplifications. The psychic can activate a phrenic amplification only while casting a spell using psychic magic, and the amplification modifies either the spell’s effects or the process of casting it. The spell being cast is called the linked spell. The psychic can activate only one amplification each time she casts a spell, and doing so is part of the action used to cast the spell. She can use any amplification she knows with any psychic spell, unless the amplification’s description states that it can be linked only to certain types of spells. A psychic learns one phrenic amplification at 1st level. At 3rd level and every 4 levels thereafter, the psychic learns a new phrenic amplification. A phrenic amplification can’t be selected more than once. Once a phrenic amplification has been selected, it can’t be changed. Phrenic amplifications require the psychic to expend 1 or more points from her phrenic pool to function.",
               "",
               null,
               FeatureGroup.None
               );

            phrenic_amplifications_feature.AllFeatures = new BlueprintFeature[0];
            var phrenic_amplifications_engine_cha = new PhrenicAmplificationContinuations(phrenic_pool_resource_charisma, psychic_spellbook, psychic_class, "CharismaPsychic");

            biokinetic_healing_cha = phrenic_amplifications_engine_cha.createBiokineticHealing();
            conjured_armor_cha = phrenic_amplifications_engine_cha.createConjuredArmor();
            defensive_prognostication_cha = phrenic_amplifications_engine_cha.createDefensivePrognostication();
            focused_force_cha = phrenic_amplifications_engine_cha.createFocusedForce();
            minds_eye_cha = phrenic_amplifications_engine_cha.createMindsEye();
            ongoing_defense_cha = phrenic_amplifications_engine_cha.createOngoingDefense();
            overpowering_mind_cha = phrenic_amplifications_engine_cha.createOverpoweringMind();
            relentless_casting_cha = phrenic_amplifications_engine_cha.createRelentlessCasting();
            will_of_the_dead_cha = phrenic_amplifications_engine_cha.createWillOfTheDead();
            charisma_amps = new BlueprintFeature[] { biokinetic_healing_cha, conjured_armor_cha, defensive_prognostication_cha, focused_force_cha, minds_eye_cha, ongoing_defense_cha, overpowering_mind_cha, relentless_casting_cha, will_of_the_dead_cha };
            AdditionalHelpers.AscribeRequiredFeature(charisma_amps, phrenic_pool_display_feature_charisma);

            var phrenic_amplifications_engine_wis = new PhrenicAmplificationContinuations(phrenic_pool_resource_wisdom, psychic_spellbook, psychic_class, "WisdomPsychic");

            biokinetic_healing_wis = phrenic_amplifications_engine_wis.createBiokineticHealing();
            conjured_armor_wis = phrenic_amplifications_engine_wis.createConjuredArmor();
            defensive_prognostication_wis = phrenic_amplifications_engine_wis.createDefensivePrognostication();
            focused_force_wis = phrenic_amplifications_engine_wis.createFocusedForce();
            minds_eye_wis = phrenic_amplifications_engine_wis.createMindsEye();
            ongoing_defense_wis = phrenic_amplifications_engine_wis.createOngoingDefense();
            overpowering_mind_wis = phrenic_amplifications_engine_wis.createOverpoweringMind();
            relentless_casting_wis = phrenic_amplifications_engine_wis.createRelentlessCasting();
            will_of_the_dead_wis = phrenic_amplifications_engine_wis.createWillOfTheDead();
            wisdom_amps = new BlueprintFeature[] { biokinetic_healing_wis, conjured_armor_wis, defensive_prognostication_wis, focused_force_wis, minds_eye_wis, ongoing_defense_wis, overpowering_mind_wis, relentless_casting_wis, will_of_the_dead_wis};
            AdditionalHelpers.AscribeRequiredFeature(wisdom_amps, phrenic_pool_display_feature_wisdom);

            //Investigator.phrenic_dabbler.AllFeatures;
            //condiser the implementation of potent magic in CallOfTheWild to see how you probably want to implement the conditional stat
            //for the phrenic_pool_resource
            //This is how the replacement works for save editor in hex channeler, its probably not this i need
            //replace improved_channel_hex_positive for negative energy channeler 
            //Action<UnitDescriptor> save_game_fix = delegate (UnitDescriptor u)
            //{
            //    if (u.HasFact(witch_channel_negative))
            //    {
            //        while (u.Progression.Features.HasFact(improved_channel_hex_positive))
            //        {
            //            u.Progression.ReplaceFeature(improved_channel_hex_positive, improved_channel_hex_negative);
            //        }
            //    }
            //};
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
