﻿using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonCannon : GenericSpecialWeapon
    {
        public IonCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Cannon",
                UpgradeType.Cannon,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.IonDamageAbility)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IonDamageAbility : Abilities.FirstEdition.IonDamageAbility
    {

        protected override void IonWeaponEffect(object sender, System.EventArgs e)
        {
            int ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            if (ionTokens > 0)
            {
                DefenderSuffersDamage(delegate {
                    Combat.Defender.Tokens.AssignTokens(
                        () => new IonToken(Combat.Defender),
                        ionTokens,
                        delegate {
                            GameManagerScript.Wait(2, Triggers.FinishTrigger);
                        }
                    );
                });
            }
            else
            {
                DefenderSuffersDamage(Triggers.FinishTrigger);
            }
        }
    }
}