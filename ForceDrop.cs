using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Logger = Rocket.Core.Logging.Logger;
namespace ForceDrop
{
    public class ForceDrop : RocketPlugin<ForceDropConfiguration>
    {
        private static ForceDrop Instance;

        protected override void Load()
        {
            if (Configuration.Instance.Enabled)
            {
                Instance = this;
                Logger.Log("[Plugin4U] ForceDrop loaded!");
                Logger.Log("Make sure to visit our site at www.Plugin4U.cf", ConsoleColor.Magenta);
            }
            else {
                Logger.Log("Configuration.Instance.Enabled == false");
                this.Unload();
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"drop_message","Successfully dropped {0} inventory!" },
                    {"drop_message_id","Successfully dropped {0} from {1} inventory!"},
                    {"drop_message_public","Successfully dropped all players inventory!" },
                    {"drop_message_id_public","Successfully dropped {0} from all players inventory!"},
                };
            }
        }

        [RocketCommand("ForceDrop", "", "", AllowedCaller.Both)]
        [RocketCommandAlias("Fd")]
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            switch (command.Length)
            {
                case (0):
                    if (caller is ConsolePlayer)
                        Logger.LogException(new InvalidOperationException($"Console does not have an iventory to drop! You must be a player to execute /fd or /ForceDrop"));
                    else
                        DropItems((UnturnedPlayer)caller);// DROP FOR CALLER
                    break;
                case (1):
                    CheckCommand();
                    break;
                case (2):
                    CheckCommand();
                    break;
                default:
                    Logger.LogException(new InvalidOperationException($"Incorrect command data!"));
                    break;
            }
            void CheckCommand()
            {
                ushort id = 0;
                if (command.Length > 1)
                {
                    if (!(ushort.TryParse(command[1], out id)))
                    {
                        Logger.LogError("Command must be a positive number");
                        return;
                    }                     
                }
                
                if (command[0].ToLower() == "all")// DROP FOR ALL
                {
                    if (Provider.clients.Count != 0 && id == 0)// NO ID
                    {
                        foreach (var steamplayer in Provider.clients)
                        {
                            DropItems(UnturnedPlayer.FromSteamPlayer(steamplayer));                          
                        }
                        UnturnedChat.Say(caller, Translate("drop_message_public"));
                    }
                    else if (Provider.clients.Count != 0)// WITH valid ID
                    {
                        foreach (var steamplayer in Provider.clients)
                        {
                            DropItems(UnturnedPlayer.FromSteamPlayer(steamplayer), id);
                        }
                        UnturnedChat.Say(caller, Translate("drop_message_id_public", id.ToString()));
                    }
                    else
                        Logger.LogError("players not found!");
                }
                else// DROP FOR PLAYER
                {                  
                    UnturnedPlayer player = UnturnedPlayer.FromName(command[0]);
                    if (player != null && id == 0)// NO ID
                    {
                        DropItems(player);
                        UnturnedChat.Say(caller, Translate("drop_message", caller.DisplayName));
                    }
                        
                    else if (player != null)// WITH ID
                    {
                        DropItems(player, id);
                        UnturnedChat.Say(caller, Translate("drop_message_id", id.ToString(), caller.DisplayName));
                    }
                        
                    else
                        Logger.LogError("player not found!");
                }
            }
        }
        void DropItems(UnturnedPlayer player)
        {
            byte itemsCount;
            for (byte page = 0; page < 7; page++)
            {
                itemsCount = player.Inventory.getItemCount(page);
                if (itemsCount == 0)
                    continue;

                for (byte index = 0; index < itemsCount; index++)
                {
                    player.Inventory.askDropItem(player.CSteamID, page, player.Inventory.getItem(page, index).x, player.Inventory.getItem(page, index).y);
                }

            }
        }
        void DropItems(UnturnedPlayer player, ushort id)
        {
            byte itemsCount;
            for (byte page = 0; page < 7; page++)
            {
                itemsCount = player.Inventory.getItemCount(page);
                if (itemsCount == 0)
                    continue;
                for (byte index = 0; index < itemsCount; index++)
                {
                    if (Configuration.Instance.WhiteListedFromDrop.Contains(player.Inventory.getItem(page, index).item.id))
                        continue;
                    else
                    {
                        //player.Inventory.sendDropItem(page, player.Inventory.getItem(page, index).x, player.Inventory.getItem(page, index).y);

                        Console.WriteLine($"Item #{index}  x: {player.Inventory.getItem(page, index).x}, y: {player.Inventory.getItem(page, index).y}");
                    }
                        //player.Inventory.askDropItem(player.CSteamID, page, player.Inventory.getItem(page, index).x, player.Inventory.getItem(page, index).y);
                }
            }
        }
    }
}
