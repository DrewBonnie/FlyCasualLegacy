﻿using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEWiWhisperModifiedInterceptor
    {
        public class P709thLegionAce : TIEWiWhisperModifiedInterceptor
        {
            public P709thLegionAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "709th Legion Ace",
                    4,
                    44,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent }
                );
            }
        }
    }
}
