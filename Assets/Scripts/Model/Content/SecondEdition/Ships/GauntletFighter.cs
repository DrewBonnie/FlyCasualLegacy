﻿using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Arcs;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class GauntletFighter : GenericShip, IMovableWings
        {
            public WingsPositions CurrentWingsPosition { get; set; }
            public GauntletFighter() : base()
            {
                ShipInfo = new ShipCardInfo
                (
                    "Gauntlet Fighter",
                    BaseSize.Large,
                    Faction.Republic,
                    new ShipArcsInfo
                    (
                        new ShipArcInfo(ArcType.Front, 3),
                        new ShipArcInfo(ArcType.Rear, 2)
                    ),
                    2, 9, 2,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(ReinforceAction), ActionColor.Red),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(CoordinateAction), ActionColor.Red)
                    ),
                    new ShipUpgradesInfo
                    (
                        UpgradeType.Modification,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Title,
                        UpgradeType.Configuration
                    ),
                    factionsAll: new List<Faction>() { Faction.Republic, Faction.Separatists, Faction.Scum, Faction.Rebel, Faction.Imperial }
                );

                DefaultUpgrades.Add(typeof(UpgradesList.SecondEdition.SwivelWingDown));

                IconicPilots = new Dictionary<Faction, System.Type> 
                {
                    { Faction.Rebel, typeof(EzraBridger) },
                    { Faction.Imperial, typeof(ImperialSuperCommando) },
                    { Faction.Scum, typeof(RookKast) },
                    { Faction.Republic, typeof(NiteOwlLiberator) },
                    { Faction.Separatists, typeof(BoKatanKryzeSeparatists) }
                };

                ModelInfo = new ShipModelInfo
                (
                    "Gauntlet Fighter",
                    "Blue",
                    new Vector3(-3.7f, 7.84f, 5.55f),
                    6f,
                    isMetallic: true
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "Slave1-Fly1",
                        "Slave1-Fly2"
                    },
                    "Slave1-Fire", 3
                );

                ShipIconLetter = '6';
            }
        }
    }
}