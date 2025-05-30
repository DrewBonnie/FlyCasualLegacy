﻿using Ship;
using Upgrade;
using UnityEngine;
using SquadBuilderNS;
using Tokens;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Maul : GenericUpgrade
    {
        public Maul() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Maul",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum, Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.MaulCrewAbility)
            );
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            if (squadList.SquadFaction == Faction.Scum) return true;

            if (squadList.SquadFaction == Faction.Rebel)
            {
                if (squadList.HasPilot("Ezra Bridger") || squadList.HasUpgrade("Ezra Bridger"))
                {
                    return true;
                }
                else
                {
                    Messages.ShowError("Maul cannot be in a Rebel squad that does not contain Ezra Bridger");
                    return false;
                }
            }

            return false;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MaulCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += MaulDiceModification;
            HostShip.OnAttackHitAsAttacker += RegisterRemoveStress;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= MaulDiceModification;
            HostShip.OnAttackHitAsAttacker -= RegisterRemoveStress;
        }

        private void MaulDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.MaulDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip
            };
            HostShip.AddAvailableDiceModificationOwn(newAction);
        }

        private void RegisterRemoveStress()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, RemoveStress);
        }

        private void RemoveStress(object sender, System.EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                Messages.ShowInfo("Maul removes a stress token from " + HostShip.PilotInfo.PilotName);
                HostShip.Tokens.RemoveToken(
                    typeof(StressToken),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace ActionsList
{

    public class MaulDiceModification : GenericAction
    {

        public MaulDiceModification()
        {
            Name = DiceModificationName = "Maul";

            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) && (!Combat.Attacker.Tokens.HasToken(typeof(StressToken)))) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if ((Combat.AttackStep == CombatStep.Attack) && (!Combat.Attacker.Tokens.HasToken(typeof(StressToken))))
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            HostShip.OnRerollIsConfirmed += AssignStressForEachRerolled;

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        private void AssignStressForEachRerolled(GenericShip ship)
        {
            int diceRerolledCount = DiceRerollManager.CurrentDiceRerollManager.GetDiceReadyForReroll().Count();

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Maul: Assign stress for each rerolled die",
                TriggerType = TriggerTypes.OnRerollIsConfirmed,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = delegate { StartAssignStess(diceRerolledCount); }
            });

            HostShip.OnRerollIsConfirmed -= AssignStressForEachRerolled;
        }

        private void StartAssignStess(int diceRerolledCount)
        {
            Messages.ShowInfo(string.Format("Maul's Ability: You gain {0} stress tokens", diceRerolledCount));
            AssignStressRecursive(diceRerolledCount);
        }

        private void AssignStressRecursive(int count)
        {
            if (count > 0)
            {
                count--;
                HostShip.Tokens.AssignToken(typeof(StressToken), delegate { AssignStressRecursive(count); });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }

}