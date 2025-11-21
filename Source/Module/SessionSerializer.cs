using System;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.RepresentationModel;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;

namespace Celeste.Mod.LeniencyHelper.Module;

public static class SessionSerializer
{

    public static void ClearSessionFile(int saveFileIndex)
    {
        string path = UserIO.GetSaveFilePath(SaveData.GetFilename(saveFileIndex) + "-modsession-LeniencyHelper");

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void SaveSession(int saveFileIndex, LeniencyHelperSession session)
    {
        if (session == null)
        {
            Logger.Warn("LeniencyHelper", "no session to save!");
            return;
        }
        
        ClearSessionFile(saveFileIndex);

        string path = UserIO.GetSaveFilePath(SaveData.GetFilename(saveFileIndex) + "-modsession-LeniencyHelper");
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        StreamWriter writer = new StreamWriter(path);

        writer.Write("v1.2.2");
        string controllerTweaks = "\n[break]\nControllerTweaks", useController = "\n[break]\nUseController";
        foreach (string tweak in TweakList)
        {
            controllerTweaks += $"\n{tweak}: {session.ControllerTweaks[tweak]}";
            useController += $"\n{tweak}: {session.UseController[tweak]}";
        }
        writer.Write(controllerTweaks + useController);


        string controllerSettings = "\n[break]\nControllerSettings";
        foreach (FieldInfo field in SettingMaster.SettingListFields.Values)
        {
            controllerSettings += $"\n{field.FieldType} {field.Name} {session.ControllerSettings[field.Name]}";
        }

        writer.Write(controllerSettings + "\n[break]");

        writer.Close();
    }

    public static LeniencyHelperSession LoadSession(int saveFileIndex)
    {
        string path = UserIO.GetSaveFilePath(SaveData.GetFilename(saveFileIndex) + "-modsession-LeniencyHelper");

        LeniencyHelperSession result = new LeniencyHelperSession();
        if (!File.Exists(path))
        {
            return result;
        }

        StreamReader reader = new StreamReader(path);

        string version = reader.ReadLine();

        string line;
        string mode = "";
        while(!reader.EndOfStream)
        {
            line = reader.ReadLine();

            if (string.IsNullOrEmpty(line)) continue;

            if (line == "[break]")
            {
                mode = reader.ReadLine();
                if (string.IsNullOrEmpty(mode))
                {
                    break;
                }

                continue;
            }

            if (mode == "ControllerTweaks" || mode == "UseController")
            {
                ParseTweak(line, out string tweak, out bool enabled);

                if (TweakList.Contains(tweak))
                {
                    if (mode == "ControllerTweaks") result.ControllerTweaks[tweak] = enabled;
                    else result.UseController[tweak] = enabled;
                }                
            }
            else
            {
                ParseSetting(line, out string setting, out object value);

                if (SettingMaster.SettingListFields.ContainsKey(setting))
                {
                    result.ControllerSettings[setting] = value;
                }
            }
        }
        reader.Close();
        return result;
    }

    private static void ParseTweak(string line, out string tweak, out bool value)
    {
        int index = 0;

        string currentWord = "";
        while (line[index] != ':')
        {
            currentWord += line[index];
            index++;
        }

        tweak = currentWord;
        currentWord = "";
        index += 2;

        while(index < line.Length)
        {
            currentWord += line[index];
            index++;
        }

        value = currentWord == "True";
    }
    private static void ParseSetting(string line, out string setting, out object value)
    {
        string currentWord = "";
        int index = 0;

        while (line[index] != ' ')
        {
            currentWord += line[index];
            index++;
        }
        Type settingType = Type.GetType(currentWord);

        index++;
        currentWord = "";

        while (line[index] != ' ')
        {
            currentWord += line[index];
            index++;
        }
        setting = currentWord;
        currentWord = "";
        index++;

        while(index < line.Length)
        {
            currentWord += line[index];
            index++;
        }

        if (settingType.IsEnum) value = Enum.Parse(settingType, currentWord);
        else value = Convert.ChangeType(currentWord, settingType);
    }
}
