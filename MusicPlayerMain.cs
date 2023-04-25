using Mod;
using Mod.CustomPanel;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using TagLib.Mpeg;
using UnityEngine;

namespace MusicPlayer
{
    internal class MusicPlayerMain
    {
        public static Image image;
        static int offsetTitle;
        static int offsetDesc;
        static long lastTimeResetTitle;
        static long lastTimeResetDesc;
        static int offsetTitlePlaying;
        static int offsetDescPlaying;
        static long lastTimeResetTitlePlaying;
        static long lastTimeResetDescPlaying;
        static int lastSelectedIndex;
        static bool isShowCurrentlyPlayingPanel;

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
            isShowCurrentlyPlayingPanel = false;
        }

        public static void setTabMusicPlayerPanel(Panel panel)
        {
            SetTabPanelTemplates.setTabListTemplate(panel, Music.listSongs.Count);
        }

        public static void doFireMusicPlayerPanel(Panel panel)
        {
            if (!isShowCurrentlyPlayingPanel)
            {
                if (panel.currentTabIndex == 0)
                {
                    //2DO: create menu
                    if (Music.indexPlaying != panel.selected)
                    {
                        Music.indexPlaying = panel.selected;
                        ResetOffsetCurrentlyPlaying();
                        Music.listSongs[Music.indexPlaying].SetVolume(.3f);
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
            if (!isShowCurrentlyPlayingPanel)
            {
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
            }
            else
            {
                paintCurrentlyPlayingSongLarge(panel, g);
            }
            GameCanvas.resetTrans(g);
        }

        public static void updateTouchPanel()
        {
            if (!isShowCurrentlyPlayingPanel)
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
                                Thread.Sleep(300);
                                isShowCurrentlyPlayingPanel = true;
                            });
                        GameCanvas.clearAllPointerEvent();
                        return;
                    }
                }
            }
            else
            {
                Panel panel = GameCanvas.panel;
                if (GameCanvas.isPointer(panel.startTabPos - 2, 52, panel.W, 25) || GameCanvas.isPointerHoldIn(panel.xScroll, panel.yScroll, panel.wScroll, panel.hScroll))
                {
                    //chặn chuyển tab
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
                Image albumArtwork = Music.listSongs[Music.indexPlaying].getAlbumArtwork(panel.ITEM_HEIGHT - 6, false);
                g.drawImage(albumArtwork, x + 3, y + 2);

                string caption = Music.listSongs[Music.indexPlaying].tag.Title;
                string description = string.Join(", ", Music.listSongs[Music.indexPlaying].tag.Artists);
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
       
        private static void paintCurrentlyPlayingSongLarge(Panel panel, mGraphics g)
        {
            g.setClip(panel.X, panel.Y, panel.W, panel.H);
            g.setColor(new Color(0.85f, 0.78f, 0.7f));
            g.fillRect(panel.startTabPos - 2, 52, panel.W - 2, 30);
            g.setColor(new Color(0.8f, 0.37f, 0.05f));
            g.fillRect(panel.startTabPos - 2, 51, panel.W - 2, 1);
            //2DO: draw album artwork
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
                string caption = Music.listSongs[i].tag.Title;
                string description = string.Join(", ", Music.listSongs[i].tag.Artists);
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
                {
                    if (captionWidth > width)
                    {
                        while (Utilities.getWidth(style, caption + "...") > width)
                            caption = caption.Remove(caption.Length - 1, 1);
                        caption += "...";
                    }
                }
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
                {
                    if (descriptionWidth > width)
                    {
                        while (Utilities.getWidth(style, description + "...") > width)
                            description = description.Remove(description.Length - 1, 1);
                        description += "...";
                    }
                }
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
                isShowCurrentlyPlayingPanel = false;
        }

        internal static void onUpdateKeyPanel(Panel instance)
        {
            if (instance == GameCanvas.panel && isShowCurrentlyPlayingPanel)
            {
                //GameCanvas.keyAsciiPress = 0;
                //GameCanvas.clearKeyHold();
                //GameCanvas.clearKeyPressed();
                GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false;
                GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = false;
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

        static void Test()
        {
            AudioFile audioFile1 = new AudioFile(@"");
            image = new Image();
            image.texture.LoadImage(audioFile1.Tag.Pictures[0].Data.Data);
            image.texture = Utils.CropFromSquareToCircle(Utils.CropToSquare(image.texture, 300));
            image.texture.anisoLevel = 0;
            image.texture.filterMode = FilterMode.Point;
            image.texture.mipMapBias = 0f;
            image.texture.wrapMode = TextureWrapMode.Clamp;
            image.texture.Apply();
            image.w = image.texture.width;
            image.h = image.texture.height;
            if (true)
            {
                new Thread(delegate ()
                {
                    using (var audioFile = new AudioFileReader(@""))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Volume = .5f;
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                            Thread.Sleep(1000);
                    }
                })
                { IsBackground = true }.Start();
            }
        }
    }
}
