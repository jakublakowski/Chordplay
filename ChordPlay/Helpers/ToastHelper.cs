using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using NotificationsExtensions.TileContent;

namespace ChordPlay.Helpers
{
    public static class ToastHelper
    {
        public static void Tile()
        {
            string from = "ChordPlay";
            string subject = $"You havent practiced since {DateTime.Now}";

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = from
                    },

                    new AdaptiveText()
                    {
                        Text = subject,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                        HintWrap = true
                    },

                }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = from,
                        HintStyle = AdaptiveTextStyle.Subtitle
                    },

                    new AdaptiveText()
                    {
                        Text = subject,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                        HintWrap = true
                    },

                }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
        public static void Notify(string title, string msg)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                        {
                             new AdaptiveText()
                             {
                               Text = title
                             },
                             new AdaptiveText()
                             {
                              Text = msg
                             }
                        }
                }
            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual
            };


            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddSeconds(5);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
