using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MMessageManager
    {
        private List<MMessage> emptyMessagesPool = new List<MMessage>();

        private List<MMessage> messages = new List<MMessage>();

        public void ShowMessage(string text, Vector2 position)
        {
            MMessage message;

            if(emptyMessagesPool.Count > 0)
            {
                message = emptyMessagesPool.ElementAt(emptyMessagesPool.Count - 1);
                emptyMessagesPool.RemoveAt(emptyMessagesPool.Count - 1);
                
            }
            else
            {
                message = new MMessage();
            }

            message.Text = text;
            message.Alpha = 1.5f;
            message.Position = position;

            messages.Add(message);
        }

        public void Update()
        {
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                MMessage message = messages[i];
                message.Position -= new Vector2(0, 3) * Engine.GameDeltaTime;
                message.Alpha -= Engine.GameDeltaTime;

                if (message.Alpha <= 0)
                {
                    messages.RemoveAt(i);
                    emptyMessagesPool.Add(message);
                }
            }
        }

        public void Render()
        {
            for (int i = 0; i < messages.Count; i++)
            {
                MMessage message = messages[i];

                RenderManager.StashGameFont.DrawText(RenderManager.SpriteBatch, message.Text, message.Position, Color.White * message.Alpha);
            }
        }

    }
}
