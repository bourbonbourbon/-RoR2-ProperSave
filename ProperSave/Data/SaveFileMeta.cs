﻿using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using TinyJson;

namespace ProperSave.Data
{
    public class SaveFileMeta
    {
        [DataMember(Name = "fn")]
        public string FileName { get; set; }
        [DataMember(Name = "upi")]
        public string UserProfileId { get; set; }
        [DataMember(Name = "si")]
        public ulong[] SteamIds { get; set; }
        [DataMember(Name = "gm")]
        public GameModeIndex GameMode { get; set; } = 0;

        [IgnoreDataMember]
        public string FilePath
        {
            get
            {
                return string.IsNullOrEmpty(FileName) ? null : $"{ProperSave.SavesDirectory}\\{FileName}.json";
            }
        }

        private static List<SaveFileMeta> SavesMetadata { get; } = new List<SaveFileMeta>();
        
        public static SaveFileMeta CreateMetadataForCurrentLobby()
        {
            return new SaveFileMeta
            {
                SteamIds = NetworkUser.readOnlyInstancesList.ToArray().Select(el => el.Network_id.steamId.value).ToArray(),
                UserProfileId = LocalUserManager.readOnlyLocalUsersList[0].userProfile.fileName,
                GameMode = Run.instance.gameModeIndex
            };
        }

        internal static SaveFileMeta GetCurrentLobbySaveMetadata(NetworkUser exceptUser = null)
        {
            var users = NetworkUser.readOnlyInstancesList.Select(el => el.Network_id.steamId.value).ToList();
            if (exceptUser != null)
            {
                users.Remove(exceptUser.Network_id.steamId.value);
            }
            var usersCount = users.Count();
            if (usersCount == 0)
            {
                return null;
            }
            var gameMode = PreGameController.instance ? PreGameController.instance.gameModeIndex : Run.instance ? Run.instance.gameModeIndex : GameModeIndex.Invalid;
            if (gameMode == GameModeIndex.Invalid)
            {
                return null;
            }
            if (usersCount == 1)
            {
                var profile = LocalUserManager.readOnlyLocalUsersList[0].userProfile.fileName.Replace(".xml", "");
                return SavesMetadata.FirstOrDefault(el => el.UserProfileId == profile && el.SteamIds.Length == 1 && el.GameMode == gameMode);
            }
            return SavesMetadata.FirstOrDefault(el => el.SteamIds.DifferenceCount(users) == 0 && el.GameMode == gameMode);
        }

        internal static void PopulateSavesMetadata()
        {
            if (!Directory.Exists(ProperSave.SavesDirectory))
            {
                Directory.CreateDirectory(ProperSave.SavesDirectory);
                return;
            }
            var path = $"{ProperSave.SavesDirectory}\\SavesMetadata.json";
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(path);
                var metadata = JSONParser.FromJson<SaveFileMeta[]>(json);

                SavesMetadata.AddRange(metadata);
            }
            catch (Exception e)
            {
                ProperSave.InstanceLogger.LogWarning("SavesMetadata file corrupted.");
                ProperSave.InstanceLogger.LogError(e);
            }
        }


        public static void AddIfNotExists(SaveFileMeta metadata)
        {
            if (SavesMetadata.Contains(metadata))
            {
                return;
            }
            SavesMetadata.Add(metadata);
            UpdateSaveMetadata();
        }

        public static void Remove(SaveFileMeta metadata)
        {
            if (SavesMetadata.Remove(metadata))
            {
                UpdateSaveMetadata();
            }
        }

        private static void UpdateSaveMetadata()
        {
            var path = $"{ProperSave.SavesDirectory}\\SavesMetadata.json";
            if (!Directory.Exists(ProperSave.SavesDirectory))
            {
                Directory.CreateDirectory(ProperSave.SavesDirectory);
            }

            try
            {
                File.WriteAllText(path, JSONWriter.ToJson(SavesMetadata));
            }
            catch (Exception e)
            {
                ProperSave.InstanceLogger.LogWarning("Can't update SavesMetadata file");
                ProperSave.InstanceLogger.LogError(e);
            }
        }
    }
}
