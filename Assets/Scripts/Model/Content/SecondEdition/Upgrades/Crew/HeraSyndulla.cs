﻿using Upgrade;
using Ship;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class HeraSyndulla : GenericUpgrade
    {
        public HeraSyndulla() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hera Syndulla",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.HeraSyndullaCrewAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(408, 1)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HeraSyndullaCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed += AllowRedManeuversWhileStressed;
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= AllowRedManeuversWhileStressed;
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void AllowRedManeuversWhileStressed(ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void CheckAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckStressRemoval);
        }

        private void CheckStressRemoval(object sender, System.EventArgs e)
        {
            if (HostShip.Tokens.CountTokensByType<StressToken>() >= 3)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " removes 1 stress from " + HostShip.PilotInfo.PilotName + " at the cost of 1 damage");
                HostShip.Tokens.RemoveToken(typeof(StressToken), SufferDamage);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SufferDamage()
        {
            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostUpgrade
            };

            HostShip.Damage.TryResolveDamage(1, damageArgs, Triggers.FinishTrigger);
        }
    }
}