using System.Collections.Generic;

namespace Spyro_Editor.Constants
{
    public enum Game
    {
        Spyro1,
        Spyro2,
        Spyro3
        // Spyro4 (maybe some day...)
    }

    public enum GameVariation
    {
        Spyro1_NTSC,
        Spyro2_NTSC,
        Spyro3_NTSC_1_0,
        Spyro3_NTSC_1_1,
        Spyro1_PAL,
        Spyro2_PAL,
        Spyro3_PAL
    }

    public enum WADSignature
    {
        Spyro1_NTSC = 39424000,
        Spyro2_NTSC = 32681984,
        Spyro3_NTSC_1_0 = 102907904,
        Spyro3_NTSC_1_1 = 102957056,
        Spyro1_PAL = 67569664,
        Spyro2_PAL = 51075072,
        Spyro3_PAL = 34547712
        // maybe add demos?
    }

    public enum SubfileType
    {
        Level,
        Cutscene,
        Flyover,
        Overlay,
        Other
    }

    public static class SubfileNames
    {
        // TODO: different maps for nstc vs pal and 3 1.0 vs 1.1

        public static Dictionary<short, string> Spyro1_NSTC = new Dictionary<short, string>
        {
            [4] = "Title Screen",
            [5] = "Introduction",
            [6] = "Ending",
            [7] = "Ending (Full Completion)",
            [11] = "Artisans",
            [13] = "Stone Hill",
            [15] = "Dark Hollow",
            [17] = "Town Square",
            [19] = "Toasty",
            [21] = "Sunny Flight",
            [23] = "Peace Keepers",
            [25] = "Dry Canyon",
            [27] = "Cliff Town",
            [29] = "Ice Cavern",
            [31] = "Doctor Shemp",
            [33] = "Night Flight",
            [35] = "Magic Crafters",
            [37] = "Alpine Ridge",
            [39] = "High Caves",
            [41] = "Wizard Peak",
            [43] = "Blowhard", // yeah man?!
            [45] = "Crystal Flight",
            [47] = "Beast Makers",
            [49] = "Terrace Village",
            [51] = "Misty Bog",
            [53] = "Tree Tops",
            [55] = "Metalhead",
            [57] = "Wild Flight",
            [59] = "Dream Weavers",
            [61] = "Dark Passage",
            [63] = "Lofty Castle",
            [65] = "Haunted Towers",
            [67] = "Jacques",
            [69] = "Icy Flight",
            [71] = "Gnorc Gnexus",
            [73] = "Gnorc Cove",
            [75] = "Twilight Harbor", // reignited did this one dirty
            [77] = "Gnasty Gnorc", // gah-nasty gah-nork
            [79] = "Gnasty's Loot",
            [83] = "Artisans",
            [84] = "Stone Hill",
            [85] = "Town Square",
            [86] = "Peace Keepers",
            [87] = "Cliff Town",
            [88] = "Doctor Shemp",
            [89] = "Magic Crafters",
            [90] = "High Caves",
            [91] = "Wizard Peak",
            [92] = "Toasty",
            [93] = "Terrace Village",
            [94] = "Metalhead",
            [95] = "Dark Passage",
            [96] = "Haunted Towers",
            [97] = "Wild Flight",
            [98] = "Gnorc Cove",
            [99] = "Icy Flight",
            [100] = "Gnasty Gnorc",
            [101] = "Twilight Harbor",
            [102] = "Jacques"
        };

        public static Dictionary<short, string> Spyro2_NSTC = new Dictionary<short, string>
        {
            [16] = "Summer Forest",
            [18] = "Glimmer",
            [20] = "Idol Springs",
            [22] = "Colossus",
            [24] = "Hurricos",
            [26] = "Aquaria Towers",
            [28] = "Sunny Beach",
            [30] = "Ocean Speedway",
            [32] = "Crush's Dungeon",
            [34] = "Autumn Plains",
            [36] = "Skelos Badlands",
            [38] = "Crystal Glacier",
            [40] = "Breeze Harbor", // trouble with the trolley, eh?
            [42] = "Zephyr",
            [44] = "Metro Speedway",
            [46] = "Scorch", // should have kept the baby in reignited...
            [48] = "Shady Oasis",
            [50] = "Magma Cone",
            [52] = "Fracture Hills",
            [54] = "Icy Speedway",
            [56] = "Gulp's Overlook",
            [58] = "Winter Tundra", // second-best hub world
            [60] = "Mystic Marsh",
            [62] = "Cloud Temples",
            [64] = "Canyon Speedway",
            [66] = "Robotica Farms",
            [68] = "Metropolis",
            [70] = "Dragon Shores",
            [72] = "Ripto's Arena",
            [74] = "We Need a Vacation!",
            [76] = "I've got a Dragon",
            [78] = "I'm a Faun you Dork!",
            [80] = "No Dragons? Wonderful!",
            [82] = "Bring it on Shorty!",
            [84] = "Boo!",
            [86] = "Gulp, Lunchtime!",
            [88] = "Spyro you did it!",
            [90] = "You Little Fools!",
            [92] = "What?! YOU AGAIN!",
            [94] = "Come on Sparx!",
            [96] = "Title Screen"
        };

        public static Dictionary<short, string> Spyro3_NSTC_1_1 = new Dictionary<short, string>
        {
            [7] = "Title Screen",
            [10] = "An Evil Plot Unfolds...",
            [13] = "The Second Warning",
            [16] = "A Monster To End All Monsters",
            [19] = "Hunter's Tussle",
            [22] = "Bianca Strikes Back",
            [25] = "An Apology, And Lunch",
            [28] = "Spike Is Born",
            [31] = "The Escape!",
            [34] = "Deja Vu?",
            [37] = "A Familiar Face",
            [40] = "Billy In The Wall",
            // [43] = "???", deleted cutscene?
            [46] = "One Less Noble Warrior",
            [49] = "THE END",
            [52] = "A Powerful Villain Emerges...",
            [55] = "A Desperate Rescue Begins...",
            [58] = "No Hard Feelings",
            [61] = "Byrd, James Byrd",
            [64] = "A Duplicitous, Larcenous Ursine",
            [67] = "The Dancing Bear",
            [98] = "Sunrise Spring",
            [100] = "Sunny Villa",
            [102] = "Cloud Spires",
            [104] = "Molten Crater",
            [106] = "Seashell Shore",
            [108] = "Mushroom Speedway",
            [110] = "Sheila's Alp",
            [112] = "Buzz's Dungeon",
            [114] = "Crawdad Farm",
            [116] = "Midday Gardens",
            [118] = "Icy Peak",
            [120] = "Enchanted Towers",
            [122] = "Spooky Swamp",
            [124] = "Bamboo Terrace",
            [126] = "Country Speedway",
            [128] = "Sgt. Byrd's Base",
            [130] = "Spike's Arena",
            [132] = "Spider Town",
            [134] = "Evening Lake",
            [136] = "Frozen Altars",
            [138] = "Lost Fleet",
            [140] = "Fireworks Factory", // overrated but still pretty good
            [142] = "Charmed Ridge",
            [144] = "Honey Speedway", // it's hip!
            [146] = "Bentley's Outpost",
            [148] = "Scorch's Pit",
            [150] = "Starfish Reef",
            [152] = "Midnight Mountain", // best hub world
            [154] = "Crystal Islands",
            [156] = "Desert Ruins",
            [158] = "Haunted Tomb",
            [160] = "Dino Mines",
            [162] = "Harbor Speedway",
            [164] = "Agent 9's Lab",
            [166] = "Sorceress's Lair",
            [168] = "Bugbot Factory",
            [170] = "Super Bonus Round",
        };
    }
}
