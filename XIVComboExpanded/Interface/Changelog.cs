﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboExpanded.Interface
{
    public class Changelog
    {
        public static Dictionary<string, string[]> GetChangelog()
        {
            return new Dictionary<string, string[]>()
                {
                    {
                        "v2.0.0.11",
                        [
                            "Changed the Combo tabs order.",
                            "Fixed VPR's Serpent's Ire on Reawaken.",
                        ]
                    },
                    {
                        "v2.0.0.10",
                        [
                            "Fixed an issue with classes not being properly recognized as jobs and being auto-selected by the auto-selection setting.\nGrab your job stones!",
                            "Updated the order in which actions are used outside Fight or Flight when using the Paladin FoF Optimize feature. \nSee issue #418 for reasoning by @kaedys.",
                            "Fixed PLD's One-Stop Stun Button locking Shield Bash into an unusable Low Blow when abilities are disabled (like in Deep Dungeons)\nFurther checks will be added for Deep Dungeons compatibility.",
                            "Rewrote RPR features and logic, combining many prior ones and updating interactions by @kaedys.",
                            "Implemented RPR automatic Soul Slice feature by @kaedys.",
                            "Removed Sacrificium Priority and Soulsow on Shadow of Death features by @kaedys.",
                            "Added VPR's Auto Fang/Bite feature by @kaedys.",
                            "Added VPR's Serpent's Ire on Reawaken by @aldros-ffxi.",
                            "Added VPR's Uncoiled Fury to Writhing Snap by @aldros-ffxi.",
                            "Restored VPR's PvP Style Main Combo by @aldros-ffxi.",
                            "Restored VPR's PvP Combo Start Flanksbane Fang by @aldros-ffxi.",
                            "Restored VPR's PvP Combo Start Flanksting Strike by @aldros-ffxi.",
                            "Restored VPR's PvP Combo Start Hindsbane Fang by @aldros-ffxi.",
                            "Restored VPR's PvP AoE Combo Start Bloodied Maw by @aldros-ffxi.",
                            "Added GNB's Burst Strike into Gnashing Fang feature by @aldros-ffxi.",
                            "Added GNB's Burst Strike into Danger Zone feature by @aldros-ffxi.",
                        ]
                    },
                    {
                        "v2.0.0.9",
                        [
                            "Fixed SGE's Dosis Psyche Feature being a child combo of the Auto Eukrasian Dosis feature.",
                            "Fixed SGE's Toxikon into Psyche being a child combo of the Toxikon into Phlegma.",
                            "Split GNB's Double Down Feature into Double Down Burst Feature & Double Down Fated Feature.",
                            "Added NIN's Kazematoi Overcap Feature.",
                        ]
                    },
                    {
                        "v2.0.0.8",
                        [
                            "Added DNC's Single Target Feather Overcap Feature.",
                            "Added DNC's Auto-Fan Dance 3.",
                            "Added DNC's AoE Feather Overcap Feature.",
                            "Added DNC's AoE Auto-Fan Dance 3.",
                            "Added SGE's Phlegma into Psyche.",
                            "Added GNB's Expanded Continuation Disable X Features.",
                            "Added the currently installed version in the titlebar.",
                        ]
                    },
                    {
                        "v2.0.0.7",
                        [
                            "MCH's Gauss Round / Double Check & Ricochet / Checkmate Feature fix if Checkmate/Double Check were assigned in the hotbar instead of Ricochet/Gauss Round.",
                        ]
                    },
                    {
                        "v2.0.0.6",
                        [
                            "RPR's Perfect Harvest Feature fix by @kaedys.",
                            "VPR's Generation Legacy Feature fix by @kaedys.",
                            "Added SAM's Iaijutsu to Tsubame-gaeshi Single Sen by @mikel-gh."
                        ]
                    },
                    {
                        "v2.0.0.5",
                        [
                            "Fixed a white background which was leaking when an invalid job was automatically picked with the auto-pick setting.",
                            "BRD's Bloodletter Features fix when Heartbreak Shot was unlocked by @aldros-ffxi."
                        ]
                    },
                    {
                        "v2.0.0.4",
                        [
                            "Reworded the Accessiiblity Combos description.",
                            "Fixed SGE's Dyskrasia Psyche Feature's logic.",
                        ]
                    },
                    {
                        "v2.0.0.3",
                        [
                            "WAR's Storm's Path Combo logic fix (electric boogaloo) by @kaedys.",
                            "PLD's Divine Might Feature logic fix (also electric boogaloo) by @kaedys.",
                            "Added WHM's Glare 4 AoE Feature.",
                            "Added SGE's Dyskrasia Psyche Feature.",
                        ]
                    },
                    {
                        "v2.0.0.2",
                        [
                            "NIN's Auto-Refill Kazematoi additional logic fix.",
                            "WAR's Storm's Path Combo logic fix.",
                            "PLD's Divine Might Feature fix."
                        ]
                    },
                    {
                        "v2.0.0.1", ["NIN's Auto-Refill Kazematoi fix."]
                    },
                    {
                        "v2.0.0.0",
                        ["Initial re-release!",
                        "Every job is Dawntrail updated at least up to the level 90.",
                        "Please request reasonable new Features on GitHub (link in the About tab) if you'd like to see new combos.",
                        "A one-time pop-up allows for an easier first installation. It can be re-enabled in the Settings tab.",
                        "Please note that some jobs do not have any combos available at all if you don't enable Expanded combos.",]
                    },
                };
        }
    }
}
