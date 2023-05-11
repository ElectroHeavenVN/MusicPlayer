using Mod;
using Mod.CustomPanel;
using Mod.Graphics;
using Mod.ModHelper.CommandMod.Chat;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TagLib.Mpeg;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MusicPlayer
{
    internal class MusicPlayerMain
    {
        static readonly int OFFSET_CURRENTLY_PLAYING = GameCanvas.panel.H + 20;
        static int offsetTitle;
        static int offsetDesc;
        static long lastTimeResetTitle;
        static long lastTimeResetDesc;
        static int offsetTitlePlaying;
        static int offsetDescPlaying;
        static long lastTimeResetTitlePlaying;
        static long lastTimeResetDescPlaying;
        static int lastSelectedIndex;

        static int offsetCurrentlyPlayingPanel = OFFSET_CURRENTLY_PLAYING;
        //static float angle;
        public static Image downArrow = new Image();
        public static Image whiteDot = new Image();
        public static Image previous = new Image();
        public static Image play = new Image();
        public static Image pause = new Image();
        public static Image next = new Image();

        static MusicPlayerMain()
        {
            Utils.InitializeImage(downArrow, "Down_arrow.png", 8 * mGraphics.zoomLevel, 8 * mGraphics.zoomLevel);
            Utils.InitializeImage(whiteDot, "White_dot.png", 5 * mGraphics.zoomLevel, 5 * mGraphics.zoomLevel);

            Utils.InitializeImage(previous, "Previous.png", 12 * mGraphics.zoomLevel, 12 * mGraphics.zoomLevel);
            Utils.InitializeImage(play, "Play.png", 25 * mGraphics.zoomLevel, 25 * mGraphics.zoomLevel);
            Utils.InitializeImage(pause, "Pause.png", 25 * mGraphics.zoomLevel, 25 * mGraphics.zoomLevel);
            Utils.InitializeImage(next, "Next.png", 12 * mGraphics.zoomLevel, 12 * mGraphics.zoomLevel);
        }

        public static void OpenMusicPanel()
        {
            GameCanvas.panel.hideNow();
            GameCanvas.panel.tabName[CustomPanelMenu.TYPE_CUSTOM_PANEL_MENU] = new string[][]
            {
                new string[]
                {
                    "Bài",
                    "hát"
                },
                new string[]
                {
                    "Nghệ",
                    "sĩ"
                },
                new string[]
                {
                    "Album",
                    ""
                },
                new string[]
                {
                    "D.sách",
                    "phát"
                },
                new string[]
                {
                    "Cài",
                    "đặt"
                },
            };
            //panel.currentTabName = panel.tabName[CustomPanelMenu.TYPE_CUSTOM_PANEL_MENU];
            CustomPanelMenu.show(setTabMusicPlayerPanel, doFireMusicPlayerPanel, null, paintMusicPlayerPanel);
            GameCanvas.panel.currentTabIndex = 0;
            offsetCurrentlyPlayingPanel = OFFSET_CURRENTLY_PLAYING;  
        }

        public static void setTabMusicPlayerPanel(Panel panel)
        {
            SetTabPanelTemplates.setTabListTemplate(panel, Music.listSongs.Count);
        }

        public static void doFireMusicPlayerPanel(Panel panel)
        {
            if (offsetCurrentlyPlayingPanel == OFFSET_CURRENTLY_PLAYING)
            {
                if (panel.currentTabIndex == 0)
                {
                    //2DO: create menu
                    if (Music.indexPlaying != panel.selected)
                    {
                        Music.indexPlaying = panel.selected;
                        ResetOffsetCurrentlyPlaying();
                        Music.currentlyPlaying.SetVolume(.3f);
                        Music.Play(Music.indexPlaying);
                    }
                }
                else if (panel.currentTabIndex == 1)
                    ;
                else if (panel.currentTabIndex == 2)
                    ;
                else if (panel.currentTabIndex == 3)
                    ;
                else if (panel.currentTabIndex == 4)
                    ;
            }
        }

        public static void paintMusicPlayerPanel(Panel panel, mGraphics g)
        {
            g.setClip(panel.xScroll, panel.yScroll, panel.wScroll, panel.hScroll);
            g.setColor(new Color(0.85f, 0.78f, 0.7f));
            g.fillRect(panel.xScroll, panel.yScroll, panel.wScroll, panel.hScroll);
            g.translate(0, -panel.cmy);
            if (panel.currentTabIndex == 0)
                paintMusicList(panel, g);
            else if (panel.currentTabIndex == 1)
                ;
            else if (panel.currentTabIndex == 2)
                ;
            else if (panel.currentTabIndex == 3)
                ;
            else if (panel.currentTabIndex == 4)
                ;
            if (Music.indexPlaying != -1)
                paintCurrentlyPlayingSongSmall(panel, g);
            g.setClip(panel.xScroll, panel.yScroll, panel.wScroll, panel.hScroll);
            panel.paintScrollArrow(g);
            paintCurrentlyPlayingSongLarge(panel, g);
            GameCanvas.resetTrans(g);
        }

        public static void updateTouchPanel()
        {
            if (offsetCurrentlyPlayingPanel == OFFSET_CURRENTLY_PLAYING)    //Menu
            {
                if (Music.indexPlaying != -1)
                {
                    GetCurrentlyPlayingRect(GameCanvas.panel, out int x, out int y, out int w, out int h);
                    y -= GameCanvas.panel.cmy;
                    if (GameCanvas.isPointerHoldIn(x, y, w, h) && !GameCanvas.isPointerMove)
                    {
                        GameCanvas.isPointerJustDown = false;
                        GameScr.gI().isPointerDowning = false;
                        if (GameCanvas.isPointerClick)
                            ThreadPool.QueueUserWorkItem(o =>
                            {
                                while (offsetCurrentlyPlayingPanel > 0)
                                {
                                    offsetCurrentlyPlayingPanel -= 10 * mGraphics.zoomLevel;
                                    if (offsetCurrentlyPlayingPanel < 0)
                                        offsetCurrentlyPlayingPanel = 0;
                                    Thread.Sleep(10);
                                }
                                //Thread.Sleep(300);
                                //isShowCurrentlyPlayingPanel = true;
                            });
                        GameCanvas.clearAllPointerEvent();
                        return;
                    }
                }
            }
            else   //Hiện đang phát
            {
                Panel panel = GameCanvas.panel;
                getDownArrow(out int downArrowX, out int downArrowY);
                getControlButtons(out int playPauseX, out int playPauseY, out int prevX, out int prevY, out int nextX, out int nextY);
                getProgBar(out int progBarX, out int progBarY, out int progBarW, out int progBarH);
                if (GameCanvas.isPointerHoldIn(downArrowX, downArrowY, downArrow.w / mGraphics.zoomLevel, downArrow.h / mGraphics.zoomLevel))   //Ẩn hiện đang phát
                {
                    GameCanvas.isPointerJustDown = false;
                    GameScr.gI().isPointerDowning = false;
                    if (GameCanvas.isPointerClick)
                    {
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            while (offsetCurrentlyPlayingPanel < OFFSET_CURRENTLY_PLAYING)
                            {
                                offsetCurrentlyPlayingPanel += 10 * mGraphics.zoomLevel;
                                if (offsetCurrentlyPlayingPanel > OFFSET_CURRENTLY_PLAYING)
                                    offsetCurrentlyPlayingPanel = OFFSET_CURRENTLY_PLAYING;
                                Thread.Sleep(10);
                            }
                        });
                    }
                    GameCanvas.clearAllPointerEvent();
                    return;
                }
                else if (GameCanvas.isPointerHoldIn(prevX, prevY, previous.w / mGraphics.zoomLevel, previous.h / mGraphics.zoomLevel))  //prev
                {
                    GameCanvas.isPointerJustDown = false;
                    GameScr.gI().isPointerDowning = false;
                    if (GameCanvas.isPointerClick)
                    {
                        Music.Previous();
                    }
                    GameCanvas.clearAllPointerEvent();
                    return;
                }
                else if (GameCanvas.isPointerHoldIn(playPauseX, playPauseY, play.w / mGraphics.zoomLevel, play.h / mGraphics.zoomLevel))    //play/pause
                {
                    GameCanvas.isPointerJustDown = false;
                    GameScr.gI().isPointerDowning = false;
                    if (GameCanvas.isPointerClick)
                    {
                        Music.PauseOrPlay();
                    }
                    GameCanvas.clearAllPointerEvent();
                    return;
                }
                else if (GameCanvas.isPointerHoldIn(nextX, nextY, next.w / mGraphics.zoomLevel, next.h / mGraphics.zoomLevel))  //next
                {
                    GameCanvas.isPointerJustDown = false;
                    GameScr.gI().isPointerDowning = false;
                    if (GameCanvas.isPointerClick)
                    {
                        Music.Next();
                    }
                    GameCanvas.clearAllPointerEvent();
                    return;
                }
                else if (GameCanvas.isMouseFocus(0, progBarY - 2, panel.W, progBarH + 4))    //Tua   
                {
                    if (GameCanvas.isPointerJustRelease)
                    {
                        GameCanvas.isPointerJustDown = false;
                        GameScr.gI().isPointerDowning = false;
                        //if (GameCanvas.isPointerClick)
                        //{
                        Music.currentlyPlaying.Position = TimeSpan.FromTicks(Music.currentlyPlaying.Length.Ticks * (GameCanvas.pxMouse - progBarX) / progBarW);
                        if (Music.currentlyPlaying.Position == Music.currentlyPlaying.Length)
                            Music.Next();
                        //}
                        GameCanvas.clearAllPointerEvent();
                        return;
                    }

                }
                else if (GameCanvas.isPointer(panel.startTabPos - 2, 52, panel.W, 25) || GameCanvas.isPointerHoldIn(panel.xScroll, panel.yScroll, panel.wScroll, panel.hScroll))    //chặn chuyển tab
                {
                    GameCanvas.isPointerJustDown = false;
                    GameScr.gI().isPointerDowning = false;
                    if (GameCanvas.isPointerClick)
                    {

                    }
                    GameCanvas.clearAllPointerEvent();
                    return;
                }

            }
        }

        static void paintCurrentlyPlayingSongSmall(Panel panel, mGraphics g)
        {
            GetCurrentlyPlayingRect(panel, out int x, out int y, out int w, out int h);
            int OFFSET_ = 20;
            g.setColor(new Color(.2f, .2f, .2f));
            g.fillRect(x, y, w, h);
            g.setColor(Color.gray);
            g.drawRect(x, y, w - 1, h - 1);

            //album placeholder
            //g.setColor(Color.yellow);
            //g.drawRect(x + 3, y + 2, panel.ITEM_HEIGHT - 7, panel.ITEM_HEIGHT - 7);

            if (Music.indexPlaying != -1)
            {
                Image albumArtwork = Music.currentlyPlaying.getAlbumArtwork(panel.ITEM_HEIGHT - 6, false);
                g.drawImage(albumArtwork, x + 3, y + 2);

                string caption = Music.currentlyPlaying.file.Tag.Title;
                string description = string.Join(", ", Music.currentlyPlaying.file.Tag.Performers);
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontSize = 8 * mGraphics.zoomLevel,
                    fontStyle = FontStyle.Bold,
                    //richText = true
                };
                int captionWidth = (int)(Utilities.getWidth(style, caption) / 1.025f);
                int descriptionWidth = (int)(Utilities.getWidth(style, description) / 1.025f);
                style.normal.textColor = Color.white;
                g.setClip(x + 4 + OFFSET_, y, w - OFFSET_ - 7, h);

                if (captionWidth > w - OFFSET_ / 10 * 4)
                {
                    g.drawString(caption, x + 4 + OFFSET_ - offsetTitlePlaying, y, style);
                    if (Math.max(captionWidth, descriptionWidth) - offsetTitlePlaying < x)
                        offsetTitlePlaying = -w + OFFSET_;
                }
                else
                {
                    ResetOffsetCurrentlyPlaying(true, false);
                    g.drawString(caption, x + 4 + OFFSET_, y, style);
                }
                style.normal.textColor = new Color(.5f, .5f, .5f);
                style.fontSize = 6 * mGraphics.zoomLevel;
                if (descriptionWidth > w - OFFSET_ / 10 * 4)
                {
                    g.drawString(description, x + 4 + OFFSET_ - offsetDescPlaying, y + 11, style);
                    if (Math.max(captionWidth, descriptionWidth) - offsetDescPlaying < x)
                        offsetDescPlaying = -w + OFFSET_;
                }
                else
                {
                    ResetOffsetCurrentlyPlaying(false);
                    g.drawString(description, x + 4 + OFFSET_, y + 11, style);
                }
                if (GameCanvas.gameTick % 3 <= 1)
                {
                    if (offsetTitlePlaying != 0)
                    {
                        offsetTitlePlaying++;
                        lastTimeResetTitlePlaying = mSystem.currentTimeMillis();
                    }
                    else if (mSystem.currentTimeMillis() - lastTimeResetTitlePlaying > 2000)
                        offsetTitlePlaying++;
                    if (offsetDescPlaying != 0)
                    {
                        offsetDescPlaying++;
                        lastTimeResetDescPlaying = mSystem.currentTimeMillis();
                    }
                    else if (mSystem.currentTimeMillis() - lastTimeResetDescPlaying > 2000)
                        offsetDescPlaying++;
                }
            }
            else
                ResetOffsetCurrentlyPlaying();
        }
       
        static void paintCurrentlyPlayingSongLarge(Panel panel, mGraphics g)
        {
            if (offsetCurrentlyPlayingPanel == OFFSET_CURRENTLY_PLAYING)
                return;
            g.setClip(panel.X, panel.Y, panel.W - 1, panel.H);
            g.translate(0, offsetCurrentlyPlayingPanel);
            //background
            Image imageBg = Music.currentlyPlaying.getAlbumArtwork(panel.H - 50, false);
            g.drawImage(imageBg, panel.W / 2, panel.Y + 50 + 1, mGraphics.TOP | mGraphics.HCENTER);
            g.setColor(new Color(0, 0, 0, .8f));
            g.fillRect(panel.X, panel.Y + 50, panel.W, panel.H - 50);
            //album artwork
            Image image = Music.currentlyPlaying.getAlbumArtwork(panel.W - 20 * 2);
            CustomGraphics.DrawImage(image, 20 + g.getTranslateX(), 60 + g.getTranslateY());
            //arrow
            getDownArrow(out int downArrowX, out int downArrowY);
            g.drawImage(downArrow, downArrowX, downArrowY);
            //progress bar
            PaintProgressBar(g, panel);
            //title, artists
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 10 * mGraphics.zoomLevel,
                fontStyle = FontStyle.Bold,
                //richText = true
            };
            style.normal.textColor = Color.white;
            string caption = Music.currentlyPlaying.file.Tag.Title;
            string description = string.Join(", ", Music.currentlyPlaying.file.Tag.Performers);
            g.setClip(10, 215, panel.W - 10 * 2, 50);
            caption = Utilities.TrimUntilFit(caption, style, panel.W - 10 * 2);
            g.drawString(caption, panel.W / 2 - GameCanvas.w / 2, 215, style);
            style.fontSize -= mGraphics.zoomLevel * 4;
            style.normal.textColor = new Color(.6f, .6f, .6f);
            g.setClip(15, 230, panel.W - 15 * 2, 50);
            description = Utilities.TrimUntilFit(description, style, panel.W - 15 * 2);
            g.drawString(description, panel.W / 2 - GameCanvas.w / 2, 230, style);
            //controls (prev, pause, next)
            getControlButtons(out int playPauseX, out int playPauseY, out int prevX, out int prevY, out int nextX, out int nextY);
            if (Music.state == PlaybackState.Playing)
                g.drawImage(pause, playPauseX, playPauseY);
            else 
                g.drawImage(play, playPauseX, playPauseY);
            g.drawImage(previous, prevX, prevY);
            g.drawImage(next, nextX, nextY);
            //if (Music.state == PlaybackState.Playing)
            //    g.drawImage(pause, playPauseX, playPauseY, mGraphics.TOP | mGraphics.HCENTER);
            //else 
            //    g.drawImage(play, playPauseX, playPauseY, mGraphics.TOP | mGraphics.HCENTER);
            //g.drawImage(previous, prevX, prevY, mGraphics.TOP | mGraphics.HCENTER);
            //g.drawImage(next, nextX, nextY, mGraphics.TOP | mGraphics.HCENTER);

            g.setClip(panel.X, panel.Y, panel.W - 1, panel.H);
        }

        static void PaintProgressBar(mGraphics g, Panel panel)
        {
            getProgBar(out int progBarX, out int progBarY, out int progBarW, out int progBarH);
            g.setColor(new Color(.6f, .6f, .6f, .5f));
            g.fillRect(progBarX, progBarY, progBarW, progBarH);
            g.setColor(Color.white);
            TimeSpan currentPosition = Music.currentlyPlaying.Position;
            TimeSpan duration = Music.currentlyPlaying.Length;
            int width = (int)(progBarW * currentPosition.Ticks / duration.Ticks);
            if (GameCanvas.isPointerHoldIn(0, progBarY - 2, panel.W, progBarH + 4))
            {
                width = Mathf.Clamp(GameCanvas.pxMouse - progBarX, 0, progBarW);
            }
            g.fillRect(progBarX, progBarY, width, 1);
            CustomGraphics.DrawImage(whiteDot, (progBarX + width) * mGraphics.zoomLevel - whiteDot.w / 2 + g.getTranslateX(), (progBarY + .5f) * mGraphics.zoomLevel - whiteDot.h / 2 + g.getTranslateY(), 0, false);
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 6 * mGraphics.zoomLevel,
                fontStyle = FontStyle.Bold,
                //richText = true
            };
            style.normal.textColor = Color.white;
            g.drawString(currentPosition.toString(), progBarX, progBarY + 2, style);
            style.alignment = TextAnchor.UpperRight;
            g.drawString(duration.toString(), progBarX + progBarW - GameCanvas.w, progBarY + 2, style);

        }

        static void paintMusicList(Panel panel, mGraphics g)
        {
            int offset = panel.cmy / panel.ITEM_HEIGHT;
            if (offset < 0)
                offset = 0;
            //Debug.Log("offset: " + offset);
            if (Music.listSongs == null || Music.listSongs.Count != panel.currentListLength)
                return;
            bool isReset = true;
            for (int i = offset; i < Mathf.Clamp(offset + panel.hScroll / panel.ITEM_HEIGHT + 2, 0, Music.listSongs.Count); i++)
            {
                string caption = Music.listSongs[i].file.Tag.Title;
                string description = string.Join(", ", Music.listSongs[i].file.Tag.Performers);
                int x = panel.xScroll;
                int y = panel.yScroll + i * panel.ITEM_HEIGHT;
                int width = panel.wScroll;
                int height = panel.ITEM_HEIGHT - 1;
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontSize = 8 * mGraphics.zoomLevel,
                    fontStyle = FontStyle.Bold,
                    //richText = true
                };
                int captionWidth = (int)(Utilities.getWidth(style, caption) / 1.025f);
                int descriptionWidth = (int)(Utilities.getWidth(style, description) / 1.025f);
                if (i != panel.selected)
                    g.setColor(new Color(0.9f, 0.87f, 0.82f));
                //else
                if (i == panel.selected)
                {
                    if (captionWidth > width || descriptionWidth > width)
                        isReset = false;
                    if (panel.selected != lastSelectedIndex)
                        isReset = true;
                    g.setColor(new Color(0.97f, 1f, 0.29f));
                }
                g.fillRect(x, y, width, height);
                style.normal.textColor = new Color(0f, 0.33f, 0.15f);
                if (i != panel.selected)
                    caption = Utilities.TrimUntilFit(caption, style, width);
                if (i == panel.selected && captionWidth > width)
                {
                    g.drawString(caption, x + 4 - offsetTitle, y, style);
                    if (Math.max(captionWidth, descriptionWidth) - offsetTitle < x)
                        offsetTitle = -width;
                }
                else
                    g.drawString(caption, x + 4, y, style);
                style.normal.textColor = new Color(0f, 0.5f, 1f);
                style.fontSize = 7 * mGraphics.zoomLevel;
                if (i != panel.selected)
                    description = Utilities.TrimUntilFit(description, style, width);
                if (i == panel.selected && descriptionWidth > width)
                {
                    g.drawString(description, x + 4 - offsetDesc, y + 12, style);
                    if (Math.max(captionWidth, descriptionWidth) - offsetDesc < x)
                        offsetDesc = -width;
                }
                else 
                    g.drawString(description, x + 4, y + 12, style);
            }
            if (isReset)
            {
                offsetDesc = 0;
                offsetTitle = 0;
                lastTimeResetTitle = mSystem.currentTimeMillis();
                lastTimeResetDesc = mSystem.currentTimeMillis();
            }
            else if (GameCanvas.gameTick % 3 <= 1)
            {
                if (offsetTitle != 0)
                {
                    offsetTitle++;
                    lastTimeResetTitle = mSystem.currentTimeMillis();
                }
                else if (mSystem.currentTimeMillis() - lastTimeResetTitle > 2000)
                    offsetTitle++;
                if (offsetDesc != 0)
                {
                    offsetDesc++;
                    lastTimeResetDesc = mSystem.currentTimeMillis();
                }
                else if (mSystem.currentTimeMillis() - lastTimeResetDesc > 2000)
                    offsetDesc++;
            }
            lastSelectedIndex = panel.selected;
        }

        internal static void onPanelHide(Panel instance)
        {
            if (instance == GameCanvas.panel)
                offsetCurrentlyPlayingPanel = OFFSET_CURRENTLY_PLAYING;
        }

        internal static void onUpdateKeyPanel(Panel instance)
        {
            if (instance == GameCanvas.panel && instance.type == CustomPanelMenu.TYPE_CUSTOM_PANEL_MENU)
            {
                if (offsetCurrentlyPlayingPanel == 0)
                {
                    //GameCanvas.keyAsciiPress = 0;
                    //GameCanvas.clearKeyHold();
                    //GameCanvas.clearKeyPressed();
                    GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false;
                    GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = false;
                    if (GameCanvas.keyAsciiPress == ' ')  //space
                    {
                        Music.PauseOrPlay();
                        GameCanvas.clearKeyHold();
                        GameCanvas.clearKeyPressed();
                        GameCanvas.keyAsciiPress = 0;
                    }
                }
            }
        }

        private static void ResetOffsetCurrentlyPlaying(bool resetTitle = true, bool resetDesc = true)
        {
            if (resetTitle)
            {
                offsetTitlePlaying = 0;
                lastTimeResetTitlePlaying = mSystem.currentTimeMillis();
            }
            if (resetDesc)
            {
                offsetDescPlaying = 0;
                lastTimeResetDescPlaying = mSystem.currentTimeMillis();
            }
        }

        private static void GetCurrentlyPlayingRect(Panel panel, out int x, out int y, out int w, out int h)
        {
            x = panel.xScroll + 5;
            y = panel.yScroll + panel.hScroll / 20 * 17 + panel.cmy;
            w = panel.wScroll - 10;
            h = panel.ITEM_HEIGHT - 2;
        }

        static void getDownArrow(out int downArrowX, out int downArrowY)
        {
            downArrowX = GameCanvas.panel.X + GameCanvas.panel.W / 2 - downArrow.w / 4;
            downArrowY = GameCanvas.panel.Y + 51;
        }

        static void getProgBar(out int progBarX, out int progBarY, out int progBarW, out int progBarH)
        {
            progBarX = 20;
            progBarY = 205;
            progBarW = GameCanvas.panel.W - progBarX * 2;
            progBarH = 1;
        }

        static void getControlButtons(out int playPauseX, out int playPauseY, out int prevX, out int prevY, out int nextX, out int nextY) 
        { 
            Panel panel = GameCanvas.panel;
            playPauseX = panel.W / 2 - play.w / 4;
            prevX = playPauseX - play.w * 2 / 3 + previous.w / 4;
            nextX = playPauseX + play.w * 2 / 3 + next.w / 4;
            playPauseY = panel.Y + 245;
            prevY = nextY = playPauseY + play.h / 8;
        }

        //static void Test()
        //{
            //AudioFile audioFile1 = new AudioFile(@"");
            //image = new Image();
            //image.texture.LoadImage(audioFile1.Tag.Pictures[0].Data.Data);
            //image.texture = Utils.CropFromSquareToCircle(Utils.CropToSquare(image.texture, 300));
            //image.texture.anisoLevel = 0;
            //image.texture.filterMode = FilterMode.Point;
            //image.texture.mipMapBias = 0f;
            //image.texture.wrapMode = TextureWrapMode.Clamp;
            //image.texture.Apply();
            //image.w = image.texture.width;
            //image.h = image.texture.height;
            //if (true)
            //{
            //    new Thread(delegate ()
            //    {
            //        using (var audioFile = new AudioFileReader(@""))
            //        using (var outputDevice = new WaveOutEvent())
            //        {
            //            outputDevice.Init(audioFile);
            //            outputDevice.Volume = .5f;
            //            outputDevice.Play();
            //            while (outputDevice.PlaybackState == PlaybackState.Playing)
            //                Thread.Sleep(1000);
            //        }
            //    })
            //    { IsBackground = true }.Start();
            //}
        //}
    }
}
