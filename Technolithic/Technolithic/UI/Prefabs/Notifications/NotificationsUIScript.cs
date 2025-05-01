using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{

    public enum NotificationLevel
    {
        INFO,
        DANGER,
        WARNING,
        ACHIEVEMENT
    }

    public class NotificationsUIScript : MScript
    {

        private Dictionary<string, MNode> notificationsNodes;
        private Dictionary<MNode, string> nodesNotifications;

        private Dictionary<string, int> notificationsCount;
        private Dictionary<string, float> notificationsTime;
        private List<string> notificationsToRemove;

        public NotificationsUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            notificationsNodes = new Dictionary<string, MNode>();
            nodesNotifications = new Dictionary<MNode, string>();
            notificationsCount = new Dictionary<string, int>();
            notificationsTime = new Dictionary<string, float>();
            notificationsToRemove = new List<string>();

            ListViewUIScript listView = ParentNode.GetComponent<ListViewUIScript>();
            listView.GrabMouse = false;
        }

        public override void Update(int mouseX, int mouseY)
        {
            foreach(var key in notificationsNodes.Keys)
            {
                notificationsTime[key] -= Engine.GameDeltaTime;
                if(notificationsTime[key] <= 0)
                {
                    notificationsToRemove.Add(key);
                }
            }

            foreach(var notifToRemove in notificationsToRemove)
            {
                MNode notifNode = notificationsNodes[notifToRemove];
                CloseNotification(notifNode);
            }

            if (notificationsToRemove.Count > 0)
                notificationsToRemove.Clear();
        }

        public void AddNotification(string notification, NotificationLevel notificationLevel = NotificationLevel.INFO, Entity triggeredEntity = null)
        {
            MNode node;

            if (notificationsNodes.TryGetValue(notification, out node))
            {
                notificationsCount[notification]++;
                notificationsTime[notification] = 60;
                string notificationString = $"{notification} ({notificationsCount[notification]})";
                ((MyText)notificationsNodes[notification].GetChildByName("Text")).Text = notificationString;
            } 
            else
            {
                node = new MNode(ParentNode.Scene);

                node.Width = 200;

                MImageUI icon = new MImageUI(ParentNode.Scene);
                icon.Width = 32;
                icon.Height = 32;
                icon.X = 8;
                icon.Y = 8;
                node.AddChildNode(icon);

                switch (notificationLevel)
                {
                    case NotificationLevel.DANGER:
                        {
                            icon.GetComponent<MImageCmp>().Texture = ResourceManager.NotificationDangerIcon;
                        }
                        break;
                    case NotificationLevel.INFO:
                        {
                            icon.GetComponent<MImageCmp>().Texture = ResourceManager.NotificationInfoIcon;
                        }
                        break;
                    case NotificationLevel.WARNING:
                        {
                            icon.GetComponent<MImageCmp>().Texture = ResourceManager.NotificationWarningIcon;
                        }
                        break;
                    case NotificationLevel.ACHIEVEMENT:
                        {
                            icon.GetComponent<MImageCmp>().Texture = ResourceManager.NotificationAchievementIcon;
                        }
                        break;
                }

                MyText text = new MyText(ParentNode.Scene);
                text.Text = notification;
                text.Name = "Text";
                if (icon == null)
                {
                    text.X = 8;
                }
                else
                {
                    text.X = icon.LocalX + icon.Width + 5;
                }
                text.Y = 8;
                text.Width = text.TextWidth;
                text.Outlined = true;
                node.AddChildNode(text);

                SmallButton eyeButton = null;

                if (triggeredEntity != null)
                {
                    eyeButton = new SmallButton(ParentNode.Scene, ResourceManager.EyeIcon);
                    eyeButton.Name = "eye_button";
                    eyeButton.X = text.LocalX + text.Width + 48;
                    eyeButton.Y = 0;
                    eyeButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnNotificationEyeButtonPressed);
                    node.AddChildNode(eyeButton);
                }

                SmallButton closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
                closeButton.Name = "close_button";
                if (eyeButton != null)
                {
                    closeButton.X = eyeButton.LocalX + eyeButton.Width + 5;
                }
                else
                {
                    closeButton.X = text.LocalX + text.Width + 48;
                }
                closeButton.Y = 0;
                closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnNotificationCloseButtonPressed);
                node.AddChildNode(closeButton);

                ListViewUIScript listView = ParentNode.GetComponent<ListViewUIScript>();
                listView.AddItem(node);

                notificationsNodes.Add(notification, node);
                nodesNotifications.Add(node, notification);
                notificationsCount.Add(notification, 1);
                notificationsTime.Add(notification, 60);

                switch(notificationLevel)
                {
                    case NotificationLevel.INFO:
                        ResourceManager.NotificationInfoSoundEffect.Play();
                        break;
                    case NotificationLevel.DANGER:
                        ResourceManager.NotificationDangerSoundEffect.Play();
                        break;
                    case NotificationLevel.WARNING:
                        ResourceManager.NotificationWarningSoundEffect.Play();
                        break;
                    case NotificationLevel.ACHIEVEMENT:
                        ResourceManager.NotificationAchievementSoundEffect.Play();
                        break;
                }
                
            }

            if (triggeredEntity != null)
            {
                node.SetMetadata("triggered_entity", triggeredEntity);
            }
        }

        private void OnNotificationEyeButtonPressed(bool value, ButtonScript buttonScript)
        {
            MNode notificationNode = buttonScript.ParentNode.ParentNode;
            Entity triggeredEntity = notificationNode.GetMetadata<Entity>("triggered_entity");
            CameraMovementScript cameraMovementScript = GameplayScene.Instance.GameplayCamera.Get<CameraMovementScript>();
            cameraMovementScript.SetEntityToFollow(triggeredEntity);
        }

        private void OnNotificationCloseButtonPressed(bool value, ButtonScript script)
        {
            CloseNotification(script.ParentNode.ParentNode);
        }

        private void CloseNotification(MNode notificationNode)
        {
            ListViewUIScript listView = ParentNode.GetComponent<ListViewUIScript>();
            listView.RemoveItem(notificationNode);

            string notification = nodesNotifications[notificationNode];

            MNode eyeButton = notificationNode.GetChildByName("eye_button");
            if(eyeButton != null)
            {
                eyeButton.GetComponent<ButtonScript>().RemoveOnClickedCallback(OnNotificationEyeButtonPressed);
            }

            MNode closeButton = notificationNode.GetChildByName("close_button");
            if (closeButton != null)
            {
                closeButton.GetComponent<ButtonScript>().RemoveOnClickedCallback(OnNotificationCloseButtonPressed);
            }

            notificationsCount.Remove(notification);
            notificationsNodes.Remove(notification);
            nodesNotifications.Remove(notificationNode);
            notificationsTime.Remove(notification);
        }
    }
}
