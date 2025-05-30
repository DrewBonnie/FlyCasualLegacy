﻿using SubPhases;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameCommands
{
    public class BombPlacementCommand : GameCommand
    {
        public BombPlacementCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            Console.Write("Bomb is placed");

            PlaceBombTokenSubphase.FinishBombPlacement
            (
                new Vector3
                (
                    float.Parse(GetString("positionX"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("positionY"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("positionZ"), CultureInfo.InvariantCulture)
                ),
                new Vector3
                (
                    float.Parse(GetString("rotationX"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationY"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationZ"), CultureInfo.InvariantCulture)
                )
            );
        }
    }

}
