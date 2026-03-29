using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SharedBookies
{
    internal sealed class ModEntry : Mod
    {
        private static readonly List<string> PowerBooks_list = new()
        {
            //Vanilla game Books
            "Book_AnimalCatalogue",
            "Book_Mystery",
            "Book_Bombs",
            "Book_Friendship",
            "Book_Horse",
            "Book_Defense",
            "Book_Roe",
            "Book_Marlon",
            "Book_Void",
            "Book_Grass",
            "Book_PriceCatalogue",
            "Book_QueenOfSauce",
            "Book_WildSeeds",
            "Book_Trash",
            "Book_Crabbing",
            "Book_Diamonds",
            "Book_Artifact",
            "Book_Speed",
            "Book_Speed2",
            "Book_Woodcutting",

            //Spiderbuttons ExtraBooks
            "Spiderbuttons.ButtonsExtraBooks_Book_Carols",
            "Spiderbuttons.ButtonsExtraBooks_Book_Luck",
            "Spiderbuttons.ButtonsExtraBooks_Book_ExtraGifts",
            "Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason",
            "Spiderbuttons.ButtonsExtraBooks_Book_ArtisanMachines",
            "Spiderbuttons.ButtonsExtraBooks_Book_GiantCrops",
            "Spiderbuttons.ButtonsExtraBooks_Book_Popularity",
            "Spiderbuttons.ButtonsExtraBooks_Book_Optimization",
            "Spiderbuttons.ButtonsExtraBooks_Book_BusDriving",
            "Spiderbuttons.ButtonsExtraBooks_Book_QiNotebook",
            "Spiderbuttons.ButtonsExtraBooks_Book_PetGifts",
            "Spiderbuttons.ButtonsExtraBooks_Book_CheatCodes",
            "Spiderbuttons.ButtonsExtraBooks_Book_JunimoScrap",
            "Spiderbuttons.ButtonsExtraBooks_Book_Carols",
            "Spiderbuttons.ButtonsExtraBooks_Book_Coffee",
            "Spiderbuttons.ButtonsExtraBooks_Book_Sketchbook",


            // jeWel More Books
            "jeWel.MoreBooks_Book_Bomb",
            "jeWel.MoreBooks_Book_God",
            "jeWel.MoreBooks_Book_Attack",
            "jeWel.MoreBooks_Book_Green_Tea",
            "jeWel.MoreBooks_Book_Milk",
            "jeWel.MoreBooks_Book_Garbage_Can",
            "jeWel.MoreBooks_Book_Dagger",
            "jeWel.MoreBooks_Book_Ore_Catalogue",
            "jeWel.MoreBooks_Book_Rice",
            "jeWel.MoreBooks_Book_Speed",
            "jeWel.MoreBooks_Book_Carpenter_Catalogue",
            "jeWel.MoreBooks_Book_Lewis_shorts",
            "jeWel.MoreBooks_Book_Luck",
            "jeWel.MoreBooks_Book_Immunity",
            "jeWel.MoreBooks_Book_Magnet",
            "jeWel.MoreBooks_Book_Fishing"
        };

        public override void Entry(IModHelper helper)
        {
            helper.Events.Multiplayer.PeerConnected += OnSync;
            helper.Events.GameLoop.DayEnding += OnSync;

            helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;

            Monitor.Log("Shared Bookies mod loaded!", LogLevel.Info);
        }

        private void OnSync(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            var data = new SyncData();

            foreach (string book in PowerBooks_list)
            {
                if (Game1.player.stats.Get(book) > 0)
                {
                    data.PowerBooks.Add(book);
                }
            }

            Helper.Multiplayer.SendMessage(
                data,
                "SyncPowerBooks",
                new[] { ModManifest.UniqueID }
            );
        }

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID || e.Type != "SyncPowerBooks")
                return;

            var data = e.ReadAs<SyncData>();

            int powersAdded = 0;

            foreach (string book in data.PowerBooks)
            {
                if (Game1.player.stats.Get(book) == 0)
                {
                    Game1.player.stats.Set(book, 1);
                    powersAdded++;
                }
            }

            if (powersAdded > 0)
            {
                Monitor.Log($"[Apply] Added Power Book(s) = {powersAdded}", LogLevel.Info);
            }
        }

        private class SyncData
        {
            public List<string> PowerBooks { get; set; } = new();
        }
    }
}
