﻿using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class Scorch : TIESeBomber
        {
            public Scorch() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "\"Scorch\"",
                    4,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ScorchBomberPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "scorch-tiesebomber";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScorchBomberPilotAbility : GenericAbility
    {
        GenericShip SufferedShip;

        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Cancel,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Success },
                payAbilityCost: PlanToAssignStrainToDefender,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PlanToAssignStrainToDefender(Action<bool> callback)
        {
            SufferedShip = Combat.Defender;
            SufferedShip.OnAttackFinishAsDefender += RegisterGetStrainToken;
            callback(true);
        }

        private void RegisterGetStrainToken(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, GetStrainToken);
        }

        private void GetStrainToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {SufferedShip.PilotInfo.PilotName} gains Strain token");

            SufferedShip.OnAttackFinishAsDefender -= RegisterGetStrainToken;

            SufferedShip.Tokens.AssignToken(typeof(Tokens.StrainToken), FinishAblity);
        }

        private void FinishAblity()
        {
            SufferedShip = null;
            Triggers.FinishTrigger();
        }

        private bool IsAvailable()
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.DiceRollAttack.RegularSuccesses == 0) return false;

            if (!Tools.IsFriendly(Combat.Attacker, HostShip)) return false;

            DistanceInfo positionInfo = new DistanceInfo(HostShip, Combat.Attacker);
            if (positionInfo.Range > 1) return false;

            return true;
        }

        private int GetAiPriority()
        {
            throw new NotImplementedException();
        }
    }
}
