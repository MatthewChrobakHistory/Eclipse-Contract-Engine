using System;
using SFML.Graphics;
using Client.Networking;

namespace Client.Graphics.Sfml.Scenes.Objects
{
    public class Button : SceneObject
    {
        public string Caption;
        public Color TextColor = Color.Black;
        public uint FontSize = 12;

        public override void Draw() {
            // Draw the surface if we have one.
            base.Draw();

            // Draw the button's caption.
            base.RenderCaption(this.Caption, this.FontSize, this.TextColor);
        }

        public sealed override string GetObjectType() {
            return "button";
        }

        internal void cmdLogin_MouseDown(string button, int x, int y) {
            if (button == "left") {
                GetUIObject("txtUsername").Visible = true;
                GetUIObject("txtPassword").Visible = true;
                GetUIObject("cmdRegister").Visible = false;
                this.Visible = false;
                GetUIObject("cmdReturn").Visible = true;
                GetUIObject("cmdLoginUser").Visible = true;
            }
        }

        internal void cmdRegister_MouseDown(string button, int x, int y) {
            if (button == "left") {
                GetUIObject("txtUsername").Visible = true;
                GetUIObject("txtPassword").Visible = true;
                GetUIObject("cmdLogin").Visible = false;
                this.Visible = false;
                GetUIObject("cmdReturn").Visible = true;
                GetUIObject("cmdRegisterUser").Visible = true;
            }

        }

        internal void cmdReturn_MouseDown(string button, int x, int y) {
            if (button == "left") {
                GetUIObject("txtUsername").Visible = false;
                GetUIObject("txtPassword").Visible = false;
                GetUIObject("cmdLogin").Visible = true;
                GetUIObject("cmdRegister").Visible = true;
                this.Visible = false;
                GetUIObject("cmdRegisterUser").Visible = false;
                GetUIObject("cmdLoginUser").Visible = false;
            }

        }

        internal void cmdRegisterUser_MouseDown(string button, int x, int y) {
            if (button == "left") {
                string username = GetUIObject("txtUsername").GetStringValue("text");
                string password = GetUIObject("txtPassword").GetStringValue("text");

                NetworkManager.PacketManager.SendRegisterUser(username, password);
            }
        }

        internal void msgButton_MouseDown(string button, int x, int y) {
            if (button == "left") {
                GetUIObject("msgBackground").Visible = false;
                GetUIObject("msgMessage").Visible = false;
                this.Visible = false;
            }
        }

        internal void cmdLoginUser_MouseDown(string button, int x, int y) {
            if (button == "left") {
                string username = GetUIObject("txtUsername").GetStringValue("text");
                string password = GetUIObject("txtPassword").GetStringValue("text");

                NetworkManager.PacketManager.SendLoginUser(username, password);
            }
        }
    }
}
