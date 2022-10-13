using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using ManagementScripts;

namespace TwitchIntegration
{
    public class TwitchChat : MonoBehaviour
    {
        private TcpClient twitchClient;
        internal StreamReader reader;
        internal StreamWriter writer;
        public static TwitchChat Instance;

        //time
        public DateTime lastMessageTime;


        public string username, password, channelName; //Get the password from https://twitchapps.com/tmi


        void Start()
        {
            Instance = this;
            password = Settings.Instance.TwitchOAuth;
            username = Settings.Instance.TwitchUsername;
            channelName = Settings.Instance.TwitchUsername;
            Connect();
        }

        

        void Update()
        {
            if (!twitchClient.Connected)
            {
                Connect();
            }

            ReadChat();
            //run this once per second
        }

        private void OnDestroy()
        {
            twitchClient.Close();
            reader = StreamReader.Null;
            writer = StreamWriter.Null;
        }

        public async void Connect()
        {
            Main.loggerInstance?.Msg("Connecting to twitch");
            twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
            reader = new StreamReader(twitchClient.GetStream());
            writer = new StreamWriter(twitchClient.GetStream());
            writer.AutoFlush = true;

            await writer.WriteLineAsync("PASS " + password);
            await writer.WriteLineAsync("NICK " + username.ToLower());
            await writer.WriteLineAsync("USER " + username.ToLower() + " 8 * :" + username.ToLower());
            await writer.WriteLineAsync("JOIN #" + channelName.ToLower());
            await writer.WriteLineAsync("CAP REQ :twitch.tv/commands twitch.tv/tags");
            await writer.FlushAsync();
            Update();
        }

        private void ReadChat()
        {
            if (twitchClient.Available > 0)
            {
                string? message = reader.ReadLine();

                if (message == null) return;
                lastMessageTime = DateTime.Now;
                ParsedMessage parsedMessage = new ParsedMessage(message);
                if (Settings.Instance.debugMode)
                    Main.loggerInstance?.Msg($"{parsedMessage}");

                HandleParsedMessage(parsedMessage);
            }
        }

        private async void HandleParsedMessage(ParsedMessage parsedMessage)
        {
            switch (parsedMessage.command?.command)
            {
                case "PING":
                {
                    string res = parsedMessage.command.command.Replace("PING", "PONG");
                    await writer.WriteLineAsync(res);
                    await writer.FlushAsync();
                    Main.loggerInstance?.Msg("Responded to ping from Twitch");
                    break;
                }
                case "PRIVMSG":
                {
                    string? message = parsedMessage.parameters;

                    if (message?.StartsWith(Settings.Instance.CommandPrefix) ?? false)
                    {
                        string[] splitMessage = message.Split(' ');
                        string command = splitMessage[0].ToLower().Replace(Settings.Instance.CommandPrefix, "");
                        List<string>? args = null;
                        if (splitMessage.Length > 1)
                        {
                            args = splitMessage.Skip(1).ToList();
                        }
                        
                        HandleCommand(command, args, parsedMessage);
                    }

                    break;
                }
                case "JOIN":
                {
                    Main.loggerInstance?.Msg($"Joined channel: {parsedMessage.source?.nick ?? "Unknown"}");
                    SendChat("Hello! I am connected to chat");
                    break;
                }
            }
        }

        private void HandleCommand(string command, List<string>? args, ParsedMessage parsedMessage)
        {
            string? response = null;
            if (parsedMessage.tags?.broadcaster ?? false)
            {
                if (command == "reload")
                {
                    SendMessage("Reloading Config");
                    response = Commands.ReloadSettings();
                }
            }

            if (parsedMessage.tags?.mod ?? false)
            {
                if (command is "lay")
                {
                    if (args?.ElementAtOrDefault(0) == "all")
                    {
                        response = Commands.layAll();
                    }
                    else
                    {
                        GameObject? t = Tools.GetClosestEntity();
                        if (t != null)
                            response = Commands.lay(t);
                    }
                }
                else if (command is "push" or "move" or "launch")
                {
                    if (args?.ElementAtOrDefault(0) == "all")
                    {
                        response = Commands.pushAll(Tools.MinMaxDefault<int>(int.Parse(args.ElementAtOrDefault(1) ?? "10"), 1,
                            1000));
                    }
                    else
                    {
                        GameObject? a = Tools.GetClosestEntity();
                        if (a == null) return;
                        response = Commands.push(a, Tools.MinMaxDefault<int>(int.Parse(args?.ElementAtOrDefault(1) ?? "10"), 1,
                            1000));
                    }
                }


                else if (command is "setspeed" or "speed")
                {
                    if (args?[0] != null)
                        response = Commands.SetSpeed(Tools.MinMaxDefault<float>(float.Parse(args[0]), 0.01f, 50f));
                }

                else if (command is "setcap")
                {
                    if (args?[0] != null)
                        response = Commands.UpdateBibiteCap(Tools.MinMaxDefault<int>(int.Parse(args[0]), 0, 10000));
                }
                else if (command is "setlimit")
                {
                    if (args?[0] != null)
                        response = Commands.UpdateBibiteLimit(Tools.MinMaxDefault<int>(int.Parse(args[0]), 0, 10000));
                }
                
                else if (command is "setinterval" or "si")
                {
                    if (args?[0] != null)
                    {
                        if (args[0] == "off")
                            args[0] = "0";
                        Main.cinematicInterval = Tools.MinMaxDefault<int>(int.Parse(args[0]), 0, 86400);
                    }
                        
                }


                else if (command is "select")
                {
                    //if command is select closest
                    if (args?[0] == "closest")
                    {
                        UserControl.Instance.SelectClosestEntity();
                    }
                    //if command is select random
                    else if (args?[0] == "random")
                    {
                        response = Commands.SelectRandomEntity();
                    }
                    else if (args?[0] == "none")
                    {
                        Main.isCinematic = false;
                        UserControl.Instance.ClearTarget();
                    }
                    else if (args?[0] == "oldest")
                    {
                        GameObject? a = Tools.GetOldestBitbite();
                        if (a != null)
                            UserControl.Instance.SelectTarget(a);
                    }
                    else if (args?[0] == "highestgen" || args?[0] == "highest")
                    {
                        GameObject? a = Tools.GetHighestGenerationBitbite();
                        if (a != null)
                            UserControl.Instance.SelectTarget(a);
                    }
                }


                else if (command is "cinematic" or "cin" or "cinema" or "cinema")
                {
                    if (args == null)
                    {
                        Main.isCinematic = !Main.isCinematic;
                    }
                    else
                    {
                        Main.isCinematic = (args[0] == "true" || args[0] == "yes" || args[0] == "y");
                    }
                    response = $"Cinematic mode is now {(Main.isCinematic ? "on" : "off")}";
                }
                else if (command is "center")
                {
                    UserControl.Instance.ClearTarget();
                    Main.isCinematic = false;
                    if (Camera.main != null)
                    {
                        Transform? main = Camera.main.transform;
                        if (main != null) main.position = new Vector3(0, 0, main.position.z);
                    }
                }

                else if (command is "zoom")
                {
                    if (args == null)
                    {
                        response = Commands.ZoomIn();
                    }
                    else
                    {
                        switch (args[0])
                        {
                            case "in":
                                response = Commands.ZoomIn();
                                break;
                            case "out":
                                response = Commands.ZoomOut();
                                break;
                        }

                        if (float.TryParse(args[0], out float zoom))
                        {
                            response = Commands.Zoom(Tools.MinMaxDefault(zoom, -8f, 8f));
                        }
                    }
                }
            }


            
            
            if (command is "ping")
            {
                response = "Hi CoolCat";
            }

            if (command is "stats")
            {
                response = ($"Highest current Generation: {Tools.GetHighestGeneration()}");
            }
            
            if (response != null)
            {
                SendChat(response);
            }
        }


        public async void SendChat(string message)
        {
            if (!twitchClient.Connected)
                Connect();
            await writer.FlushAsync();
            await writer.WriteLineAsync($"PRIVMSG #{channelName} :{message}");
            await writer.FlushAsync();
        }
        
        
    }

    public class Tags
    {
        public Tags(Dictionary<string, int>? badges, bool broadcaster, bool mod, bool vip, bool subscriber, int bits)
        {
            this.badges = badges;
            this.broadcaster = broadcaster;
            this.mod = mod;
            this.vip = vip;
            this.subscriber = subscriber;
            this.bits = bits;
        }

        public Dictionary<string, int>? badges { get; set; }
        public bool broadcaster { get; set; }
        public bool mod { get; set; }
        public bool vip { get; set; }
        public bool subscriber { get; set; }

        public int bits { get; set; }
    }

    public class Source
    {
        public string nick { get; set; }
        public string? host { get; set; }
    }

    public class Command
    {
        public string? command { get; set; }
        public string? channel { get; set; }

        public Command(string command, string channel)
        {
            this.command = command;
            this.channel = channel;
        }
    }

    public class ParsedMessage
    {
        public Command? command { get; set; }
        public Tags? tags { get; set; }
        public Source? source { get; set; }
        public string? parameters { get; set; }
        public string raw { get; set; }

        public ParsedMessage(string raw)
        {
            int idx = 0;
            this.raw = raw;
            string rawTagsComponent = string.Empty;
            string rawSourceComponent = string.Empty;
            string? rawParametersComponent = string.Empty;


            int endIdx;
            if (raw[idx] == '@')
            {
                endIdx = raw.IndexOf(' ');
                rawTagsComponent = raw.Slice(1, endIdx);
                idx = endIdx + 1;
            }

            if (raw[idx] == ':')
            {
                idx += 1;
                endIdx = raw.IndexOf(' ', idx);
                rawSourceComponent = raw.Slice(idx, endIdx);
                idx = endIdx + 1;
            }

            endIdx = raw.IndexOf(':', idx);
            if (endIdx == -1)
            {
                endIdx = raw.Length;
            }

            string rawCommandComponent = raw.Slice(idx, endIdx).Trim();
            if (endIdx < raw.Length)
            {
                idx = endIdx + 1;
                rawParametersComponent = raw.Slice(idx);
            }

            command = parseCommand(rawCommandComponent);

            if (command == null) return;

            if (rawTagsComponent != string.Empty)
            {
                this.tags = parseTags(rawTagsComponent);
            }

            if (rawSourceComponent != string.Empty)
            {
                this.source = parseSource(rawSourceComponent);
            }

            this.parameters = rawParametersComponent;
        }

        public override string ToString()
        {
            ParsedMessage a = this;

            return
                $"command: {a.command?.command}, channel: {a.command?.channel}, nick: {a.source?.nick}, host: {a.source?.host}, broadcaster: {a.tags?.broadcaster} mod: {a.tags?.mod}, vip: {a.tags?.vip}, subscriber: {a.tags?.subscriber}, bits: {a.tags?.bits}, parameters: {a.parameters}";
        }

        public string ToString(bool includeBadges)
        {
            ParsedMessage a = this;

            string badges = String.Empty;

            foreach (var badge in a.tags?.badges ?? new Dictionary<string, int>())
            {
                badges += $"{badge.Key}({badge.Value}), ";
            }

            return
                $"command: {a.command?.command}, channel: {a.command?.channel}, nick: {a.source?.nick}, host: {a.source?.host}, broadcaster: {a.tags?.broadcaster}, mod: {a.tags?.mod}, vip: {a.tags?.vip}, subscriber: {a.tags?.subscriber}, bits: {a.tags?.bits}, parameters: {a.parameters}, badges: {badges}";
        }

        private Source? parseSource(string rawSourceComponent)
        {
            string[] sourceParts = rawSourceComponent.Split('!');


            if (sourceParts.Length == 2)
            {
                return new Source
                {
                    nick = sourceParts[0],
                    host = sourceParts[1]
                };
            }
            else
            {
                return new Source
                {
                    nick = sourceParts[0],
                    host = null
                };
            }
        }

        private Tags? parseTags(string rawTagsComponent)
        {
            Dictionary<string, int>? badges = new Dictionary<string, int>();
            string color = null;
            string display_name = null;
            string id = null;
            bool mod = false;
            bool vip = false;
            bool subscriber = false;
            bool broadcaster = false;
            string room_id = null;
            string tmi_sent_ts = null;
            string user_id = null;
            string user_type = null;
            int bits = 0;

            string[] delimitedTags = rawTagsComponent.Split(';');
            foreach (string rawTag in delimitedTags)
            {
                string[] kvp = rawTag.Split('=');
                string key = kvp[0];
                string value = kvp[1];
                switch (key)
                {
                    case "badges":
                    case "badge-info":
                    {
                        badges = null;
                        if (value.Length > 0)
                        {
                            var td = new Dictionary<string, int>();
                            string[] bb = value.Split(',');
                            foreach (string ap in bb)
                            {
                                string[] badge = ap.Split('/');
                                if (badge[0] == "broadcaster")
                                {
                                    broadcaster = true;
                                }

                                td.Add(badge[0], int.Parse(badge[1]));
                            }

                            badges = td;
                        }

                        break;
                    }
                    case "bits":
                    {
                        bits = int.Parse(value);
                        break;
                    }
                    case "mod":
                    {
                        mod = value == "1";
                        if (!mod)
                        {
                            mod = badges.ContainsKey("broadcaster");
                        }

                        break;
                    }
                    case "subscriber":
                    {
                        subscriber = value == "1";
                        break;
                    }
                    case "vip":
                    {
                        vip = value == "1";
                        break;
                    }
                }
            }

            return new Tags(badges, broadcaster, mod, vip, subscriber, bits);
        }

        private Command? parseCommand(string rawCommandComponent)
        {
            Command? parsedCommand = null;
            string[] commandParts = rawCommandComponent.Split(' ');

            switch (commandParts[0])
            {
                case "JOIN":
                case "PART":
                case "NOTICE":
                case "CLEARCHAT":
                case "HOSTTARGET":
                case "PRIVMSG":
                    parsedCommand = new Command(commandParts[0], commandParts[1]);
                    break;
                case "PING":
                    parsedCommand = new Command(commandParts[0], null);
                    break;
                case "CAP":
                    parsedCommand = new Command(commandParts[0], null);
                    break;
                case "GLOBALUSERSTATE":
                    parsedCommand = new Command(commandParts[0], null);
                    break;
                case "USERSTATE":
                case "ROOMSTATE":
                    parsedCommand = new Command(commandParts[0], commandParts[1]);
                    break;
                case "RECONNECT":
                    parsedCommand = new Command(commandParts[0], null);
                    break;
                case "421":
                    if (Settings.Instance.debugMode)
                    {
                        Main.loggerInstance?.Msg($"Unsupported IRC command: {commandParts[2]}");
                        Main.loggerInstance?.Msg($"Full message: {rawCommandComponent}");
                        // return parseCommand(rawCommandComponent.Replace(commandParts[2],
                        //     commandParts[2].Replace("PONG","")));

                    }

                    return null;
                    break;
                case "001":
                case "376":
                    parsedCommand = new Command(commandParts[0], commandParts[1]);
                    break;
                case "002": // Ignoring all other numeric messages.
                case "003":
                case "004":
                case "353": // Tells you who else is in the chat room you"re joining.
                case "366":
                case "372":
                case "375":
                    if (Settings.Instance.debugMode)
                        Main.loggerInstance?.Msg($"Ignoring IRC: {commandParts[0]}");
                    return null;
                default:
                    Main.loggerInstance?.Warning($"Unexpected command: {commandParts[0]}");
                    break;
            }

            return parsedCommand;
        }

        //
    }
}