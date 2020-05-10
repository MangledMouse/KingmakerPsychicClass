using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using CallOfTheWild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.Designers.Mechanics.Facts;

namespace PsychicClassMod
{
    public class PhrenicAmplificationContinuations : PhrenicAmplificationsEngine
    {
        BlueprintAbilityResource resource;
        BlueprintSpellbook spellbook;
        BlueprintCharacterClass character_class;
        string name_prefix;
        public PhrenicAmplificationContinuations(BlueprintAbilityResource pool_resource, BlueprintSpellbook linked_spellbook, BlueprintCharacterClass linked_class, string asset_prefix) : base(pool_resource, linked_spellbook, linked_class, asset_prefix)
        {
            resource = pool_resource;
            spellbook = linked_spellbook;
            character_class = linked_class;
            name_prefix = asset_prefix;
        }

        static LibraryScriptableObject library => Main.library;

        //public static 
        public BlueprintFeature createRelentlessCasting()
        {
            var buff = Helpers.CreateBuff(name_prefix + "RelentlessCastingBuff",
                                          "Relentless Casting",
                                          "When casting a spell the psychic may spend 1 point from her phrenic pool to roll twice on any checks to overcome spell penetration that would be required for the linked spell, taking the better result.",
                                          "",
                                          Helpers.GetIcon("ee7dc126939e4d9438357fbd5980d459"), //guid for spell penetration feat/feature
                                          null,
                                          Helpers.Create<CallOfTheWild.NewMechanics.SpendResourceOnSpellCast>(s => { s.spellbook = spellbook; s.resource = resource; }),
                                          Helpers.Create<AutoMetamagic>(a => { a.Metamagic = (Metamagic)MetamagicExpansion.FurtherMetamagicExtension.RelentlessCasting;})
                                          //Helpers.Create<CallOfTheWild.NewMechanics.MetamagicMechanics.MetamagicOnSpellDescriptor>(m =>
                                          //{
                                          //    m.amount = 1;
                                          //    m.resource = resource;
                                          //    m.Metamagic = (Metamagic)MetamagicExpansion.FurtherMetamagicExtension.RelentlessCasting;
                                          //    m.spellbook = spellbook;
                                          //})
                                          );

            var toggle = Common.buffToToggle(buff, UnitCommand.CommandType.Free, true,
                                             resource.CreateActivatableResourceLogic(spendType: ActivatableAbilityResourceLogic.ResourceSpendType.Never)
                                             );
            toggle.Group = ActivatableAbilityGroupExtension.PhrenicAmplification.ToActivatableAbilityGroup();
            var feature = Common.ActivatableAbilityToFeature(toggle, false);
            //feature.AddComponent(Helpers.Create<CallOfTheWild.OnCastMechanics.ForceFocusSpellDamageDiceIncrease>(s => { s.spellbook = spellbook; }));
            return feature;
        }
    }
}