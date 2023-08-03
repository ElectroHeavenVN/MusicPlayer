using Mod;
using Mod.Graphics;
using MusicPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

internal class GameEvents : Extension
{
    static GameEvents _instance = new GameEvents();

   // static float angle;

    public override void onPaintGameScr(mGraphics g)
    {
        //Test(g);
    }

    public override void onUpdateKeyPanel(Panel instance)
    {
        MusicPlayerMain.onUpdateKeyPanel(instance);
    }

    public override void onPanelHide(Panel instance)
    {
        MusicPlayerMain.onPanelHide(instance);
    }

    public override void onUpdateTouchPanel()
    {
        MusicPlayerMain.updateTouchPanel();
    }

    public override void onGameStarted()
    {
        Data.Load();
    }

    public override bool onGameClosing()
    {
        Data.Save();
        PlayerCore.StopBeforeExit();
        return false;
    }

    private void Test(mGraphics g)
    {
        //if (MusicPlayerMain.image != null)
        //{
        //    CustomGraphics.DrawImage(MusicPlayerMain.image, 50, 50, angle);
        //    //if (GameCanvas.gameTick % 3 == 0)
        //    angle += 2 / 3f;
        //    if (angle >= 360f)
        //        angle = 0f;
        //}
    }
}
