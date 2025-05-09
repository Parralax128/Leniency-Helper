using System;
using System.IO;
using System.Reflection;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;


namespace Celeste.Mod.LeniencyHelper.Module;

public static class SessionSerialization
{
    public static void SaveSession(int saveFileIndex, LeniencyHelperModuleSession session)
    {
        string path = UserIO.GetSaveFilePath(SaveData.GetFilename(saveFileIndex) + "-modsession-LeniencyHelper");

        if (File.Exists(path))
        {
            File.Delete(path);
        }
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        StreamWriter writer = new StreamWriter(path);

        string triggerTweaks = "[dict]TriggerTweaks:", controllerTweaks = "\n\n[dict]ControllerTweaks:", useController = "\n\n[dict]UseController:";
        foreach (string tweak in TweakList)
        {
            triggerTweaks += $"\n{tweak}: {session.TriggerTweaks[tweak]}";
            controllerTweaks += $"\n{tweak}: {session.ControllerTweaks[tweak]}";
            useController += $"\n{tweak}: {session.UseController[tweak]}";
        }
        writer.Write(triggerTweaks + controllerTweaks + useController);


        string triggerSettings = "\n\n[list]TriggerSettings:", controllerSettings = "\n\n[list]ControllerSettings:";
        foreach (FieldInfo field in SettingMaster.SettingListFields.Values)
        {
            triggerSettings += $"\n{field.FieldType} {field.Name} {session.TriggerSettings.Get(field.Name)}";
            controllerSettings += $"\n{field.FieldType} {field.Name} {session.ControllerSettings.Get(field.Name)}";
        }
        writer.Write(triggerSettings + controllerSettings);

        writer.Close();
    }

    public static LeniencyHelperModuleSession LoadSession(int saveFileIndex)
    {
        string path = UserIO.GetSaveFilePath(SaveData.GetFilename(saveFileIndex) + "-modsession-LeniencyHelper");

        LeniencyHelperModuleSession result = new LeniencyHelperModuleSession();
        if (!File.Exists(path))
        {
            return result;
        }

        StreamReader reader = new StreamReader(path);

        string line = reader.ReadLine();

        for (int c = 0; c < 3; c++)
        {
            while (line == "" || line.StartsWith("[dict]"))
                line = reader.ReadLine();

            foreach (string tweak in TweakList)
            {
                switch (c)
                {
                    case 0: result.TriggerTweaks[tweak] = TweakLineIsTrue(line); break;
                    case 1: result.ControllerTweaks[tweak] = TweakLineIsTrue(line); break;
                    case 2: result.UseController[tweak] = TweakLineIsTrue(line); break;
                }
                line = reader.ReadLine();
            }
        }


        for (int c = 0; c < 2; c++)
        {
            while (line == "" || line.StartsWith("[list]"))
                line = reader.ReadLine();
            foreach (string fieldName in SettingMaster.SettingListFields.Keys)
            {
                if (c == 0) result.TriggerSettings.Set(fieldName, GetSettingFromLine(line));
                else result.ControllerSettings.Set(fieldName, GetSettingFromLine(line));

                line = reader.ReadLine();
            }
        }

        reader.Close();


        return result;
    }

    private static bool TweakLineIsTrue(string line)
    {
        if (!line.Contains(':'))
        {
            return false;
        }

        int index = 0;
        while (line[index] != ':') index++;
        index += 2;

        string valueWord = "";
        while (index < line.Length)
        {
            valueWord += line[index];
            index++;
        }

        return valueWord == "True";
    }
    private static object GetSettingFromLine(string line)
    {
        int index = 0;

        string typeName = "";
        while (line[index] != ' ')
        {
            typeName += line[index];
            index++;
        }

        index = line.Length - 1;
        string value = "";
        while (line[index] != ' ')
        {
            value = line[index] + value;
            index--;
        }

        Type type = Type.GetType(typeName);

        if (type.IsEnum)
        {
            return Enum.Parse(type, value);
        }

        return Convert.ChangeType(value, type);
    }
}
