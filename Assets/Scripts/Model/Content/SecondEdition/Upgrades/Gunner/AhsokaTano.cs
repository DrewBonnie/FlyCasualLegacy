﻿using Upgrade;
using Ship;
using Arcs;
using System.Linq;
using BoardTools;
using System.Collections.Generic;
using System;
using SubPhases;
using ActionsList;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class AhsokaTanoGunner : GenericUpgrade
    {
        public AhsokaTanoGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ahsoka Tano",
                UpgradeType.Gunner,
                cost: 9,
                isLimited: true,
                addForce: 1,
                abilityType: typeof(Abilities.SecondEdition.AhsokaTanoGunnerAbility),
                restriction: new FactionRestriction(Faction.Republic)
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(194, 10)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/12/bc/12bc6f68-e805-4985-8d94-12bfa1a4b617/swz48_cards-ahsoka.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AhsokaTanoGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += TryRegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= TryRegisterTrigger;
        }

        private void TryRegisterTrigger(GenericShip ship)
        {
            if (HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            if (AnyTargetIsPresent())
            {
                SelectTargetForAbility(
                    TargetIsSelected,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may spend 1 Force to choose a ship in your firing arc - it may perform a red Focus acton, even while stressed",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void TargetIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Selection.ChangeActiveShip(TargetShip);

            HostShip.State.SpendForce(
                1,
                delegate
                {
                    TargetShip.AskPerformFreeAction(
                        new FocusAction() { Color = Actions.ActionColor.Red, CanBePerformedWhileStressed = true },
                        FinishAbility,
                        descriptionShort: HostUpgrade.UpgradeInfo.Name,
                        descriptionLong: "You may perfrom a red Focus action even while stressed",
                        imageHolder: HostUpgrade,
                        isForced: true
                    );
                }
            );
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(HostShip);
            Triggers.FinishTrigger();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly)
                && FilterTargetsByRangeInArc(ship, 1, 3);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private bool AnyTargetIsPresent()
        {
            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (FilterTargets(friendlyShip)) return true;
            }

            return false;
        }
    }
}