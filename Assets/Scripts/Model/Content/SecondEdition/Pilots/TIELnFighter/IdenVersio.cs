﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;
using Editions;
using SubPhases;
using Abilities.SecondEdition;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class IdenVersio : TIELnFighter
        {
            public IdenVersio() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Iden Versio",
                    4,
                    42,
                    isLimited: true,
                    abilityType: typeof(IdenVersioAbility),
                    charges: 1,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Inferno";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IdenVersioAbility : GenericAbility
    {
        private GenericShip curToDamage;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckIdenVersioAbilitySE;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckIdenVersioAbilitySE;
        }

        private void CheckIdenVersioAbilitySE(GenericShip toDamage, DamageSourceEventArgs e)
        {
            curToDamage = toDamage;

            // Is the defender on our team? If not return.
            if (!Tools.IsFriendly(curToDamage, HostShip))
                return;

            if (!(curToDamage is Ship.SecondEdition.TIELnFighter.TIELnFighter))
                return;

            // If the defender is at range one of us we register our trigger to prevent damage.
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(curToDamage, HostShip);
            if (distanceInfo.Range <= 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseIdenVersioAbilitySE);
            }
        }

        private void UseIdenVersioAbilitySE(object sender, System.EventArgs e)
        {
            // Are there any non-crit damage results in the damage queue?
            if (HostShip.State.Charges > 0)
            {
                // If there are we prompt to see if they want to use the ability.
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    delegate { HostShip.RemoveCharge(BlankDamage); },
                    descriptionLong: "Do you want to spend 1 Charge to prevent damage?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void BlankDamage()
        {
            curToDamage.AssignedDamageDiceroll.RemoveAll();
            DecisionSubPhase.ConfirmDecision();
        }


    }
}