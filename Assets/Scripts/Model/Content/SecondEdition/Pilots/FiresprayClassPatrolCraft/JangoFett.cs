﻿using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class JangoFett : FiresprayClassPatrolCraft
        {
            public JangoFett() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jango Fett",
                    6,
                    78,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JangoFettAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Crew },
                    factionOverride: Faction.Separatists
                );

                ModelInfo.SkinName = "Jango Fett";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JangoFettAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Jango Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Blank,
                timing: DiceModificationTimingType.Opposite
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if (Combat.Defender.ShipId == HostShip.ShipId)
                    {
                        result = IsMyRevealedManeuverIsLess(Combat.Attacker);
                    }
                    break;
                case CombatStep.Defence:
                    if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon)
                    {
                        result = IsMyRevealedManeuverIsLess(Combat.Defender);
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool IsMyRevealedManeuverIsLess(GenericShip anotherShip)
        {
            bool result = false;

            if (HostShip.RevealedManeuver != null && anotherShip.RevealedManeuver != null)
            {
                if (HostShip.RevealedManeuver.ColorComplexity < anotherShip.RevealedManeuver.ColorComplexity) result = true;
            }

            return result;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}