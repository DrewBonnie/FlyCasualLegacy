﻿using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;
using Conditions;
using System.Linq;
using Tokens;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class DirectorKrennic : GenericUpgrade
    {
        public DirectorKrennic() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Director Krennic",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.FirstEdition.DirectorKrennicAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DirectorKrennicAbility : GenericAbility
    {
        protected virtual string Prompt
        {
            get
            {
                return "Choose Galactic Empire ship with 3 or fewer Shields:\nIt gets Optimized Prototype condition";
            }
        }

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterDirectorKrennicAbility;
            SecondAbility();
        }

        protected virtual void SecondAbility()
        {
            GenericShip.OnAttackFinishGlobal += OptimizedPrototypeKrennicTargetLockEffect;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterDirectorKrennicAbility;
            GenericShip.OnAttackFinishGlobal -= OptimizedPrototypeKrennicTargetLockEffect;
        }

        private void RegisterDirectorKrennicAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Director Krennic decision",
                TriggerType = TriggerTypes.OnSetupStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectDirectorKrennicTarget,
            });
        }

        private void SelectDirectorKrennicTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignOptimizedPrototype,
                  CheckRequirements,
                  GetAiOptimizedPrototypePriority,
                  HostShip.Owner.PlayerNo,
                  HostUpgrade.UpgradeInfo.Name,
                  Prompt,
                  HostUpgrade
              );
        }

        protected virtual void AssignOptimizedPrototype()
        {
            TargetShip.Tokens.AssignCondition(new OptimizedPrototype(TargetShip) { SourceUpgrade = HostUpgrade });
            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            var match = Tools.IsFriendly(ship,HostShip)
                && ship.State.ShieldsMax <= 3
                && ship.SubFaction == Faction.Imperial;
            return match;
        }

        private int GetAiOptimizedPrototypePriority(GenericShip ship)
        {
            int result = 0;

            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private void OptimizedPrototypeKrennicTargetLockEffect(GenericShip ship)
        {
            if (ship.ShipId == Combat.Attacker.ShipId)
            {
                var onRangePrototypeAttacked = Tools.IsFriendly(Combat.Attacker, HostShip)
                    && Combat.Attacker.Tokens.HasToken<OptimizedPrototype>()
                    && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                    && BoardTools.Board.GetRangeOfShips(Combat.Attacker, HostShip) <= 2;
                if (onRangePrototypeAttacked)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireTargetLock);
                }
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                AcquireTargetLock,
                descriptionLong: string.Format("Does {0} want to acquire a Target Lock on {1}?", HostShip.PilotInfo.PilotName, Combat.Defender.PilotInfo.PilotName),
                imageHolder: HostUpgrade,
                showAlwaysUseOption: true
            );
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(string.Format("Optimized Prototype: {0} gets a Target Lock on {1}.", HostShip.PilotInfo.PilotName, Combat.Defender.PilotInfo.PilotName));
            ActionsHolder.AcquireTargetLock(HostShip, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}

namespace Conditions
{
    public class OptimizedPrototype : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        public OptimizedPrototype(GenericShip host) : base(host)
        {
            Name = ImageName = "Optimized Prototype Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/optimized-prototype.png";
        }

        public override void WhenAssigned()
        {
            SubscribeToOptimizedPrototypeConditionEffects();
        }

        public override void WhenRemoved()
        {
            UnsubscribeFromOptimizedPrototypeConditionEffects();
        }

        private void SubscribeToOptimizedPrototypeConditionEffects()
        {
            Host.State.ShieldsMax++;
            Host.TryRegenShields();

            Roster.UpdateDamageIndicators(Host, Host.InfoPanel);

            Host.OnGenerateDiceModifications += AddOptimizedPrototypeCancelResultModification;
        }

        private void UnsubscribeFromOptimizedPrototypeConditionEffects()
        {
            Host.State.ShieldsMax--;
            Host.OnGenerateDiceModifications -= AddOptimizedPrototypeCancelResultModification;
        }

        private void AddOptimizedPrototypeCancelResultModification(GenericShip ship)
        {
            OptimizedPrototypeAction action = new OptimizedPrototypeAction()
            {
                HostShip = Host,
                SourceUpgrade = SourceUpgrade
            };

            Host.AddAvailableDiceModificationOwn(action);
        }
    }
}

namespace ActionsList
{
    public class OptimizedPrototypeAction : GenericAction
    {
        public GenericUpgrade SourceUpgrade;

        public OptimizedPrototypeAction()
        {
            Name = DiceModificationName = "Optimized Prototype";
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) result = false;
            if (!Combat.DiceRollAttack.ResultsArray.Any()) result = false;
            if (Combat.Defender.State.ShieldsCurrent == 0) result = false;

            return result;
        }

        private class OptimizedPrototypeDecisionSubPhase : DecisionSubPhase { }

        public override void ActionEffect(System.Action callBack)
        {
            var newSubPhase = Phases.StartTemporarySubPhaseNew<OptimizedPrototypeDecisionSubPhase>(Name, callBack);

            newSubPhase.DescriptionShort = "Director Krennic's Optimized Prototype";
            newSubPhase.DescriptionShort = "Do you want ot spend a die result to make the defender lose a shield?";
            newSubPhase.ImageSource = SourceUpgrade;

            newSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            newSubPhase.ShowSkipButton = true;
            newSubPhase.OnSkipButtonIsPressed = DontUseOptimizedPrototype;

            if (Combat.DiceRollAttack.Blanks > 0)
            {
                newSubPhase.AddDecision("Spend Blank", (s, o) => SpendBlankForEffect(DieSide.Blank));
            }
            if (Combat.DiceRollAttack.Focuses > 0)
            {
                newSubPhase.AddDecision("Spend Eye", (s, o) => SpendBlankForEffect(DieSide.Focus));
            }
            if (Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                newSubPhase.AddDecision("Spend Hit", (s, o) => SpendBlankForEffect(DieSide.Success));
            }
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                newSubPhase.AddDecision("Spend Critical Hit", (s, o) => SpendBlankForEffect(DieSide.Crit));
            }

            newSubPhase.DefaultDecisionName = newSubPhase.GetDecisions().Select(d => d.Name).FirstOrDefault();
            newSubPhase.Start();
        }

        private void SpendBlankForEffect(DieSide side)
        {
            Combat.DiceRollAttack.RemoveType(side);
            DefenderSuffersDamage();
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Damage from Optimized Prototype",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Combat.Defender.Owner.PlayerNo,
                    EventHandler = Combat.Defender.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = Combat.Attacker,
                        DamageType = DamageTypes.CardAbility
                    },
                    Skippable = true
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
        }

        private void DontUseOptimizedPrototype()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (IsDiceModificationAvailable())
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }
    }
}