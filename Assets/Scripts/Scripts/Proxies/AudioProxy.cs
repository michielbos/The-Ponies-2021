using System.Linq;
using Controllers;
using MoonSharp.Interpreter;

namespace Scripts.Proxies {

[MoonSharpUserData]
public class AudioProxy {
    private static AudioProxy _instance;

    [MoonSharpHidden]
    public static AudioProxy Instance => _instance ?? (_instance = new AudioProxy());

    public static string[] getAudioNames(string prefix) => ContentController.Instance.GetAudioNames(prefix).ToArray();

    public static string[] getTopLevelAudioFolders(string prefix) => ContentController.Instance
        .GetTopLevelAudioFolders(prefix)
        .ToArray();
}

}