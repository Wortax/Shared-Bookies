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
            "Book_Woodcutting"
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

            foreach (string stat in PowerBooks_list)
            {
                if (Game1.player.stats.Get(stat) > 0)
                {
                    data.PowerBooks.Add(stat);
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

            foreach (string stat in data.PowerBooks)
            {
                if (Game1.player.stats.Get(stat) == 0)
                {
                    Game1.player.stats.Set(stat, 1);
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
