﻿using Ship;
using Upgrade;
using System;
using BoardTools;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class ChewbaccaResistance : GenericUpgrade
    {
        public static readonly int ChewbaccaFullChargeValue = 2;

        public ChewbaccaResistance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Chewbacca",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                charges: ChewbaccaFullChargeValue,
                regensCharges: false,
                cannotBeRecharged: false,
                abilityType: typeof(Abilities.SecondEdition.ChewbaccaResistanceCrewAbility)
            );

            NameCanonical = "chewbacca-crew-swz19";

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(302, 1)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaResistanceCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += UpdateChewbaccaChargesOnSetup;
            GenericShip.OnDamageCardIsDealtGlobal += UpdateChewbaccaChargesOnDamageDealt;
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                AbilityIsAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Change,
                1,
                new System.Collections.Generic.List<DieSide>() { DieSide.Focus },
                DieSide.Crit,
                payAbilityCost: SpendCharges
                );
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= UpdateChewbaccaChargesOnSetup;
            GenericShip.OnDamageCardIsDealtGlobal -= UpdateChewbaccaChargesOnDamageDealt;
            RemoveDiceModification();
        }

        private void UpdateChewbaccaChargesOnSetup()
        {
            this.HostUpgrade.State.LoseCharge();
        }

        private void UpdateChewbaccaChargesOnDamageDealt(GenericShip shipTakingDamage)
        {
            var distanceInfo = new DistanceInfo(HostShip, shipTakingDamage);
            if (Tools.IsFriendly(shipTakingDamage, HostShip) &&
                distanceInfo.Range <= 3)
            {
                if (this.HostUpgrade.State.Charges != UpgradesList.SecondEdition.ChewbaccaResistance.ChewbaccaFullChargeValue)
                {
                    Messages.ShowInfo(this.HostUpgrade.UpgradeInfo.Name + " recovers 1 charge");
                }
                this.HostUpgrade.State.RestoreCharge();
            }
        }

        public bool AbilityIsAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack &&
                Combat.Attacker == HostShip &&
                HostUpgrade.State.Charges == UpgradesList.SecondEdition.ChewbaccaResistance.ChewbaccaFullChargeValue
                );
        }

        public int GetDiceModificationPriority()
        {
            int result = 0;
            if (Combat.AttackStep == CombatStep.Attack &&
                Combat.Attacker == HostShip && Combat.DiceRollAttack.Focuses > 0)
            {
                result = 100;
                //If attacker has more than one focus result and a focus token
                //the focus token should take priority
                if (Combat.DiceRollAttack.Focuses > 1 &&
                    HostShip.Tokens.CountTokensByType(typeof(FocusToken)) > 0)
                {
                    result = 70;
                }
            }
            return result;
        }

        public void SpendCharges(Action<bool> callback)
        {
            if (this.HostUpgrade.State.Charges >= UpgradesList.SecondEdition.ChewbaccaResistance.ChewbaccaFullChargeValue)
            {
                this.HostUpgrade.State.SpendCharges(UpgradesList.SecondEdition.ChewbaccaResistance.ChewbaccaFullChargeValue);
                Messages.ShowInfo(this.HostUpgrade.UpgradeInfo.Name + " was activated");
                callback(true);
            }
            else
            {
                Messages.ShowError(this.HostUpgrade.UpgradeInfo.Name + " could not activate: Chewbacca does not have enough charges");
                callback(false);
            }
        }
    }
}