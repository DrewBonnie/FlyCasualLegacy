﻿using ActionsList;
using Ship;
using System;
using BoardTools;
using Upgrade;
using Tokens;
using SubPhases;
using System.Linq;
using Actions;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class LinGaava : TIEFoFighter
        {
            public LinGaava() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lin Gaava",
                    3,
                    32,
                    pilotTitle: "Impetuous Mechanic",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LinGaavaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LinGaavaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterLinGaavaAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterLinGaavaAbility;
        }
        private void RegisterLinGaavaAbility()
        {
            HostShip.Tokens.AssignCondition
            (
                new Conditions.PrimedForSpeed(HostShip) { SourceUpgrade = HostUpgrade }
            );
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": Assign Primed for Speed condition",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectLinGaavaTargets,
            });
        }

        private void SelectLinGaavaTargets(object Sender, System.EventArgs e)
        {
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Lin Gaava", Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = FilterSelection;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = 2;
            subphase.WhenDone = AssignConditions;

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Choose up to 2 other friendly TIE/fo or TIE/fo Fighters that have no equipped modification upgrades to assign Primed for Speed condition";
            subphase.ImageSource = HostShip;

            subphase.Start();
            
        }

        private void AssignConditions(Action callback)
        {
            foreach (GenericShip ship in Selection.MultiSelectedShips)
            {
                Messages.ShowInfo($"Primed for Speed condition is assigned to {ship.PilotInfo.PilotName}");
                ship.Tokens.AssignCondition
                (
                    new Conditions.PrimedForSpeed(ship) { SourceUpgrade = HostUpgrade }
                );
            }
            callback();
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private bool FilterSelection(GenericShip ship)
        {
            return Tools.IsFriendly(ship, HostShip) && 
                ship.ShipId != HostShip.ShipId &&
                (ship.GetType().IsSubclassOf(typeof(Ship.SecondEdition.TIEFoFighter.TIEFoFighter)) || ship.GetType().IsSubclassOf(typeof(Ship.SecondEdition.TIESfFighter.TIESfFighter))) && 
                !ship.UpgradeBar.GetUpgradesAll().Any(n => n.HasType(UpgradeType.Modification));
        }
    }
}

namespace Conditions
{
    public class PrimedForSpeed : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        private GenericShip CachedAttacker;
        private GenericShip CachedDefender;

        public PrimedForSpeed(GenericShip host) : base(host)
        {
            Name = ImageName = "Primed For Speed Condition";
            Temporary = false;

            Tooltip = "https://infinitearenas.com/xw2/images/conditions/primedforspeed.png";
        }

        public override void WhenAssigned()
        {
            Host.OnGenerateActions += AddSlam;
            Host.OnSlam += CheckAbility;
        }

        public override void WhenRemoved()
        {
            Host.OnGenerateActions -= AddSlam;
            Host.OnSlam -= CheckAbility;
        }

        private void AddSlam(GenericShip ship)
        {
            ship.AddAvailableAction(new SlamAction());
        }

        private void CheckAbility()
        {
                Triggers.RegisterTrigger
                (
                    new Trigger()
                    {
                        Name = "Primed For Speed",
                        TriggerType = TriggerTypes.OnActionIsPerformed,
                        TriggerOwner = Host.Owner.PlayerNo,
                        EventHandler = SufferDamage
                    }
                );
        }

        private void SufferDamage(object sender, EventArgs e)
        {
            DamageSourceEventArgs LinGaavaDamage = new DamageSourceEventArgs
            {
                Source = Host,
                DamageType = DamageTypes.CardAbility
            };

            Host.Damage.TryResolveDamage(1, LinGaavaDamage, delegate { });
            Host.Tokens.RemoveToken(typeof(WeaponsDisabledToken), Triggers.FinishTrigger);
        }
    }
}