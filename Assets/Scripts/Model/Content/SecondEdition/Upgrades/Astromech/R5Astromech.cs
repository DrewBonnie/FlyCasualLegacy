﻿using Upgrade;
using System.Linq;
using System.Collections.Generic;
using ActionsList.SecondEdition;
using System;
using Ship;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class R5Astromech : GenericUpgrade, IVariableCost
    {
        public R5Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5 Astromech",
                UpgradeType.Astromech,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.R5AstromechAbility),
                charges: 2
            );
        }
        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 1},
                {1, 2},
                {2, 3},
                {3, 4}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}

namespace Abilities.SecondEdition
{
    //Action: Spend 1 charge to repair 1 facedown damage card.
    //Action: Repair 1 faceup Ship damage card.
    public class R5AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddAction;
        }

        private void AddAction(Ship.GenericShip ship)
        {
            if (ship.Damage.GetFacedownCards().Any() && HostUpgrade.State.Charges > 0)
            {

                ship.AddAvailableAction(new RepairAction(RepairAction.CardFace.FaceDown)
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    PayRepairCost = () =>
                    {
                        var result = false;
                        if (HostUpgrade.State.Charges > 0)
                        {
                            HostUpgrade.State.SpendCharge();
                            result = true;
                        }
                        return result;
                    }
                });
            }
            if (ship.Damage.GetFaceupCrits(CriticalCardType.Ship).Any())
            {
                ship.AddAvailableAction(new RepairAction(RepairAction.CardFace.FaceUp, CriticalCardType.Ship)
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    Source = this.HostUpgrade
                });
            }
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class RepairAction : GenericAction
    {
        public enum CardFace
        {
            FaceDown,
            FaceUp
        }

        public Func<bool> PayRepairCost = () => true;

        private readonly CardFace damageCardFace;
        private readonly CriticalCardType? criticalCardType;

        public RepairAction(CardFace face, CriticalCardType? type = null)
        {
            damageCardFace = face;
            criticalCardType = type;

            DiceModificationName = Name = "Repair 1 " + face.ToString().ToLower() + (type != null ? " " + type.ToString() : "") + " damage";
        }

        public override void ActionTake()
        {
            if (PayRepairCost())
            {
                if (damageCardFace == CardFace.FaceDown)
                {
                    if (HostShip.Damage.DiscardRandomFacedownCard())
                    {
                        Sounds.PlayShipSound("R2D2-Proud");
                        Messages.ShowInfoToHuman("Facedown Damage card is discarded");
                    }
                    Phases.CurrentSubPhase.CallBack();
                }
                else if (damageCardFace == CardFace.FaceUp)
                {
                    List<GenericDamageCard> shipCritsList = HostShip.Damage.GetFaceupCrits(criticalCardType);

                    if (shipCritsList.Count == 1)
                    {
                        HostShip.Damage.FlipFaceupCritFacedown(shipCritsList.First(), Phases.CurrentSubPhase.CallBack);
                        Sounds.PlayShipSound("R2D2-Proud");
                    }
                    else if (shipCritsList.Count > 1)
                    {
                        R5AstromechDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<R5AstromechDecisionSubPhase>(
                            "R5 Astromech: Select faceup ship Crit",
                            DecisionSubPhase.ConfirmDecision
                        );
                        subphase.DescriptionShort = "R5 Astromech";
                        subphase.DescriptionLong = "Select a faceup ship Crit damage card to flip it facedown";
                        subphase.ImageSource = Source;
                        subphase.Start();
                    }
                }
            }
        }
    }
}

namespace SubPhases
{
    public class R5AstromechDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(System.Action callBack)
        {
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var shipCrit in Selection.ActiveShip.Damage.GetFaceupCrits(CriticalCardType.Ship).ToList())
            {
                AddDecision(shipCrit.Name, delegate { DiscardCrit(shipCrit); }, shipCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void DiscardCrit(GenericDamageCard critCard)
        {
            Selection.ActiveShip.Damage.FlipFaceupCritFacedown(critCard, Phases.CurrentSubPhase.CallBack);
            Sounds.PlayShipSound("R2D2-Proud");
        }
    }
}