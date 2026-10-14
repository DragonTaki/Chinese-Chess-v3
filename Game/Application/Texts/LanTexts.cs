/* ----- ----- ----- ----- */
// LanTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>Localized sentences for account-free LAN play.</summary>
    public static class LanTexts
    {
        public static string ChooseConnection => Lang.Get("lan.choose_connection");
        public static string Server => Lang.Get("lan.server");
        public static string Title => Lang.Get("lan.title");
        public static string OnlineTitle => Lang.Get("lan.online_title");
        public static string RoomSettings => Lang.Get("lan.room_settings");
        public static string JoinRoom => Lang.Get("lan.join_room");
        public static string RoomAddress => Lang.Get("lan.room_address");
        public static string InvalidRoomAddress => Lang.Get("lan.invalid_room_address");
        public static string RoomNotFound => Lang.Get("lan.room_not_found");
        public static string AmbiguousRoom => Lang.Get("lan.ambiguous_room");
        public static string RoomNumber(string number, string host) => Lang.Get("lan.room_number", number, host);
        public static string FoundRoom(string number, string host, string address) => Lang.Get("lan.found_room", number, host, address);
        public static string Nickname => Lang.Get("lan.nickname");
        public static string Search => Lang.Get("lan.search");
        public static string Searching => Lang.Get("lan.searching");
        public static string NearbyRooms => Lang.Get("lan.nearby_rooms");
        public static string NoRooms => Lang.Get("lan.no_rooms");
        public static string InvalidNickname => Lang.Get("lan.invalid_nickname");
        public static string NetworkFailure => Lang.Get("lan.network_failure");
        public static string Disconnected => Lang.Get("lan.disconnected");
        public static string MoveRejected => Lang.Get("lan.move_rejected");
        public static string OutOfSync => Lang.Get("lan.out_of_sync");
        public static string Host(string name, string address) => Lang.Get("lan.host", name, address);
        public static string Room(string host) => Lang.Get("lan.room", host);
    }
}
