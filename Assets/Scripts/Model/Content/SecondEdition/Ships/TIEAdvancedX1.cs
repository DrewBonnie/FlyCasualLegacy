﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using System;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class TIEAdvancedX1 : FirstEdition.TIEAdvanced.TIEAdvanced
        {
            public TIEAdvancedX1() : base()
            {
                ShipInfo.ShipName = "TIE Advanced x1";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Sensor);

                ShipInfo.ActionIcons.RemoveActions(typeof(EvadeAction));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(BarrelRollAction)));

                ShipAbilities.Add(new Abilities.SecondEdition.AdvancedTargetingComputer());

                IconicPilots[Faction.Imperial] = typeof(DarthVader);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll), MovementComplexity.Complex);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/0/08/Maneuver_tie_advanced_x1.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack against a defender you have locked, roll 1 additional attack die and change 1 hit result to a critical hit result.
    public class AdvancedTargetingComputer : GenericAbility
    {
        public override string Name { get { return "Advanced Targeting Computer"; } }

        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
            HostShip.Ai.OnGetActionPriority += IncreaseAILockPriority;

            AddDiceModification(
                "Advanced Targeting Computer",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide>() { DieSide.Success },
                DieSide.Crit
            );
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
            HostShip.Ai.OnGetActionPriority -= IncreaseAILockPriority;
            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            return 100;
        }

        private void IncreaseAILockPriority(GenericAction action, ref int priority)
        {
            if (action is TargetLockAction && TargetLockAction.HasValidLockTargetsAndNoLockOnShipInRange(HostShip)) priority = 55;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender)
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon;
        }

        private void CheckAbility(ref int value)
        {
            if (IsAvailable())
            {
                Messages.ShowInfo(Combat.Attacker.PilotInfo.PilotName + "'s target lock and Advanced Targeting Computer grants them +1 attack die");
                value++;
            }
        }

        private void ModifyDice(DiceRoll diceroll)
        {
            HostShip.OnImmediatelyAfterRolling -= ModifyDice;
            if (diceroll.Change(DieSide.Success, DieSide.Crit, 1) > 0)
            {
                Messages.ShowInfo("Advanced Targeting Computer converts one Hit to a Critical Hit");
            }
        }
    }
}
