﻿using ActionsList;
using Arcs;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ST70AssaultShip
    {
        public class GuildBountyHunter : ST70AssaultShip
        {
            public GuildBountyHunter() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "Guild Bounty Hunter",
                    2,
                    46,
                    limited: 2,
                    pilotTitle: "Blaster for Hire",
                    abilityType: typeof(Abilities.SecondEdition.GuildBountyHunterAbility)
                );

                ModelInfo.SkinName = "Red Stripes";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GuildBountyHunterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddGuildBountyHunterAbilityActionEffect;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddGuildBountyHunterAbilityActionEffect;
        }
        private void AddGuildBountyHunterAbilityActionEffect(GenericShip host)
        {
            GenericAction actionPilot = new ActionsList.SecondEdition.GuildBountyHunterActionEffect()
            {
                HostShip = host,
                ImageUrl = host.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(actionPilot);
        }
    }
}
namespace ActionsList.SecondEdition
{
    public class GuildBountyHunterActionEffect : GenericAction
    {
        public GuildBountyHunterActionEffect()
        {
            Name = DiceModificationName = "Guild Bounty Hunter";
        }
        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack)
                && Combat.ShotInfo.Range >= 1
                && Combat.ShotInfo.Range <= 2
                && Combat.DiceRollAttack.Focuses > 0
            )
            {
                if (AnyIllicitHasUsableCharges()) result = true;
            }
            return result;
        }
        private bool AnyIllicitHasUsableCharges()
        {
            foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Illicit))
            {
                if (upgrade.State.Charges > 0 && upgrade.UpgradeInfo.RegensChargesCount == 0) return true;
            }
            return false;
        }
        public override int GetDiceModificationPriority()
        {
            return 45;
        }
        public override void ActionEffect(Action callBack)
        {
            StartDecisionSubphase(callBack);
        }
        private void StartDecisionSubphase(Action callBack)
        {
            SpendIllicitChargeSubphase subphase = Phases.StartTemporarySubPhaseNew<SpendIllicitChargeSubphase>(
                "Choose one device to reload",
                callBack
            );
            subphase.DescriptionShort = "Choose one illicit to spend charge";
            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;
            foreach (GenericUpgrade upgrade in GetUsableIllicitUpgrades())
            {
                subphase.AddDecision(
                    upgrade.UpgradeInfo.Name,
                    delegate { SpendChargeAndFinish(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.State.Charges
                );
            }
            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.Start();
        }
        private void SpendChargeAndFinish(GenericUpgrade upgrade)
        {
            upgrade.State.SpendCharge();
            Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit);
            DecisionSubPhase.ConfirmDecision();
        }
        private List<GenericUpgrade> GetUsableIllicitUpgrades()
        {
            return HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Illicit)
                .Where(n => (n.State.Charges > 0 && n.UpgradeInfo.RegensChargesCount == 0))
                .ToList();
        }
        public class SpendIllicitChargeSubphase : DecisionSubPhase { }
    }
}