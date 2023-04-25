using Mod.ModHelper.Menu;
using MusicPlayer;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using NAudio;
using NAudio.Wave;

public class MainExt : MonoBehaviour
{
    public new const string name = "MusicPlayer";

    public const string description = "MusicPlayer by ElectroHeavenVN";

    public const string version = "v1.0.0";

    internal static MainExt instance = new MainExt();


    public static void OpenMenu()
    {
        //new MenuBuilder()
        //    .addItem("Mở danh sách nhạc", new MenuAction(() => MusicPlayerMain.OpenMusicPanel()))
        //    .setChatPopup(description + " " + version);
        MusicPlayerMain.OpenMusicPanel();
    }


    void Awake() { }

    void Start() { }

    void Update() { }

    void FixedUpdate() { }

    void OnGUI() { }

}
