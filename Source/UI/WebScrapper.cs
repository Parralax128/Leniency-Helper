using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;
using System.Collections.Concurrent;

namespace Celeste.Mod.LeniencyHelper.UI;
class WebScrapper
{
    static HttpClient sharedClient = new HttpClient();
    public struct TweakInfo
    {
        public string tweakDescription = "";
        public Dictionary<string, string> settingDescs = new();

        public TweakInfo() { }
    }
    public static ConcurrentDictionary<Tweak, TweakInfo> TweaksInfo = new();

    public static void LoadInfo()
    {
        foreach(Tweak tweak in Module.LeniencyHelperModule.TweakList)
        {
            Thread temp = new Thread(() => ExtractTweakInfo(DialogUtils.TweakToUrl(tweak), tweak));
            temp.Start();
        }
    }
    static async void ExtractTweakInfo(string url, Tweak tweak)
    {
        TweakInfo info = new();
        string contents = null;

        try
        {
            HttpResponseMessage response = await sharedClient.GetAsync(url);

            response.EnsureSuccessStatusCode();
            string htmlContent = await response.Content.ReadAsStringAsync();

            contents = htmlContent.ToString();
        }
        catch (Exception e)
        {
            Logger.Error(Module.LeniencyHelperModule.Instance.Metadata.Name, $"Error fetching {url}:\n{e.Message}\n{e.InnerException}");
            TweaksInfo.TryAdd(tweak, default);
            return;
        }


        int startIndex, endIndex;

        //tweak desc
        {
            int headerIndex = contents.IndexOf("General info");
            if (headerIndex == -1) headerIndex = contents.IndexOf("General Info");

            if (headerIndex == -1) return;

            contents = contents[headerIndex..];
                
            startIndex = contents.IndexOf("<p>") + 3;
            endIndex = contents.IndexOf("</p>");

            info.tweakDescription = contents[startIndex..endIndex];
        }

            
        int settingIndex = contents.IndexOf("<div class=\"markdown-heading\"><h3 class=\"heading-element\">Settings");
        if(settingIndex <= 0)
        {
            info.settingDescs = null;
            TweaksInfo.TryAdd(tweak, info);
            return;
        }
        contents = contents[settingIndex..];

        //settings
        {
            do
            {
                startIndex = contents.IndexOf("<ins>") + 5;
                endIndex = contents.IndexOf("</ins>");
                string settingName = contents[startIndex..endIndex];

                startIndex = endIndex + 9;
                endIndex = Math.Min(contents.IndexOf("</p>"), contents.IndexOf("</li>"));

                if (startIndex >= endIndex)
                {
                    if (info.settingDescs == null) info.settingDescs = new();
                    info.settingDescs.Add(settingName, "");

                    contents = contents[(endIndex + 4)..];
                    continue;
                }

                string desc = contents[startIndex..endIndex];

                contents = contents[(endIndex+4)..];

                if (info.settingDescs == null) info.settingDescs = new();
                info.settingDescs.Add(settingName, desc);
            }
            while (contents.Contains("<ins>") && contents.IndexOf("<ins>") < 100);
        }

        TweaksInfo.TryAdd(tweak, info);
    }
}