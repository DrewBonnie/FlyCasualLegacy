﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class TIEFoFighter : FirstEdition.TIEFoFighter.TIEFoFighter
        {
            public TIEFoFighter() : base()
            {
                ShipInfo.ShipName = "TIE/fo Fighter";

                ShipInfo.DefaultShipFaction = Faction.FirstOrder;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.FirstOrder };

                IconicPilots[Faction.FirstOrder] = typeof(Midnight);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/e6/Maneuver_tie_fo.png";
            }
        }
    }
}