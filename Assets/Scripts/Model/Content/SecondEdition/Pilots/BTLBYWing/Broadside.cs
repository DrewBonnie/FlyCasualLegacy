﻿using Arcs;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class Broadside : BTLBYWing
        {
            public Broadside() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Broadside\"",
                    3,
                    35,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Astromech },
                    abilityType: typeof(Abilities.SecondEdition.BroadsideAbility)
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BroadsideAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ArcForShot.ArcType == ArcType.SingleTurret
                && (Combat.ArcForShot.Facing == ArcFacing.Left || Combat.ArcForShot.Facing == ArcFacing.Right)
                && Combat.DiceRollAttack.Blanks > 0;
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
