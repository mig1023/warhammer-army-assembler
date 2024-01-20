﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace WarhammerArmyAssembler.Settings
{
    class ConfigFile
    {
        const string CONFIG_NAME = "WarhammerArmyAssembler.config";
        const string BACKUP_NAME = CONFIG_NAME + "_backup";

        public static void Save()
        {
            FileInfo backupConfig = new FileInfo(CONFIG_NAME);

            if (File.Exists(CONFIG_NAME))
            {
                if (File.Exists(BACKUP_NAME))
                    File.Delete(BACKUP_NAME);

                backupConfig.MoveTo(BACKUP_NAME);
            }

            using (TextWriter config = new StreamWriter(CONFIG_NAME))
            {
                foreach (KeyValuePair<string, string> setting in Values.All())
                    config.WriteLine($"{setting.Key} = {setting.Value}");
            }
        }

        async public static void Load()
        {
            Values.Clean();

            if (!File.Exists(CONFIG_NAME))
                return;

            using (FileStream config = File.OpenRead(CONFIG_NAME))
            {
                byte[] buffer = new byte[config.Length];
                await config.ReadAsync(buffer, 0, buffer.Length);
                string allSettings = Encoding.Default.GetString(buffer);

                List<string> settings = allSettings
                    .Split('\n')
                    .ToList();

                foreach (string setting in settings)
                {
                    if (String.IsNullOrWhiteSpace(setting))
                        continue;

                    List<string> parts = setting
                        .Split('=')
                        .Select(x => x.Trim())
                        .ToList();

                    Values.Set(parts[0], parts[1]);
                }
            }
        }
    }
}
