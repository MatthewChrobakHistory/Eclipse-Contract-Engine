using Client.Graphics.Sfml.Scenes.Objects;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Client.Graphics.Sfml.Scenes
{
    public class SceneSystem : IScenes
    {
        // The collection of all the scene related surfaces, and objects.
        private List<GraphicalSurface> _surfaces;
        public List<SceneObject>[] _UIObject { private set; get; }
        private GuiEditor SceneEditor = new GuiEditor();

        // The scene object that has the current focus.
        private SceneObject _curFocus;

        // General bool variable stating whether or not the mouse buttons are down.
        public bool MouseLeftDown { private set; get; }
        public bool MouseMiddleDown { private set; get; }
        public bool MouseRightDown { private set; get; }

        #region Core Scene System Logic
        // All these methods pertain to event handling.
        // You shouldn't have to change this.

        public SceneSystem() {
            // Create an array of collections containing scene objects for 
            // every client state.
            this._UIObject = new List<SceneObject>[(int)ClientState.Length];
            for (int i = 0; i < this._UIObject.Length; i++) {
                this._UIObject[i] = new List<SceneObject>();
            }

            // Load all the graphical surfaces.
            this.LoadSurfaces();

            // Load all the hard-coded scene objects.
            this.LoadSceneObjects();

            // Load messageboxes for each scene.
            foreach (var scene in _UIObject) {
                var msgBackground = new Objects.Image() {
                    Name = "msgBackground",
                    Width = 960,
                    Height = 640,
                    Surface = GetSurface("whitebox"),
                    Visible = false
                };
                var msgMessage = new Objects.Label() {
                    Name = "msgMessage",
                    Width = 960,
                    Height = 50,
                    Top = 200,
                    TextColor = new SFML.Graphics.Color(0, 0, 0, 255),
                    FontSize = 35,
                    Visible = false
                };
                var msgButton = new Objects.Button() {
                    Name = "msgButton",
                    Width = 200,
                    Height = 50,
                    Top = 350,
                    Left = 350,
                    Surface = GetSurface("orangefade"),
                    Caption = "Okay",
                    TextColor = new SFML.Graphics.Color(0, 0, 0, 255),
                    Visible = false
                };
                msgButton.MouseDown += msgButton.msgButton_MouseDown;

                scene.Add(msgBackground);
                scene.Add(msgMessage);
                scene.Add(msgButton);
            }
        }

        public void Reload() {

        }

        public void Destroy() {

        }

        private void LoadSurfaces() {
            // Initialize the collection.
            this._surfaces = new List<GraphicalSurface>();

            // Load every png file we find in the directory specified.
            foreach (string file in Directory.GetFiles(GraphicsManager.GuiPath).Where((x) => {
                return x.EndsWith(".png") || x.EndsWith(".jpg");
            })) {
                this._surfaces.Add(new GraphicalSurface(file));
            }
        }

        public void MouseMove(int x, int y) {

            // If our left mouse button is down, we can apply dragging
            // processing on our focused scene object.
            if ((this.MouseLeftDown || this.MouseRightDown) && this._curFocus != null) {
                this._curFocus.Drag(x, y);
            }

            // Make sure that we actually initialized the scene system.
            if (this._UIObject != null) {
                // Make sure that we actually have scene objects in our current state.
                if (this._UIObject[(int)Program.State] != null) {
                    // Loop through all possible values for the ZOrder.
                    for (int z = ZOrder.GetHighZ(); z >= 0; z--) {
                        // Loop through every scene object we have in our current state.
                        foreach (var obj in this._UIObject[(int)Program.State]) {
                            // Does the object's ZIndex match the ZOrder?
                            if (obj.Z == z) {
                                // Is the object visible?
                                if (obj.Visible) {
                                    // Did we move our mouse within the area of the scene object?
                                    if (x >= obj.Left && x <= obj.Left + obj.Width) {
                                        if (y >= obj.Top && y <= obj.Top + obj.Height) {

                                            // We did. Invoke appropriate event-handling methods.
                                            obj.ObjectMouseMove(x - obj.Left, y - obj.Top);

                                            // We assume we have the object we moused over.
                                            // Return so that we don't apply similar logic on scene objects that 
                                            // should not receive this processing.
                                            return;
                                        }
                                    }
                                }
                                // This break will break out of the current loop through all the scene objects for a respective Z value.
                                // It ensures we don't waste time looking for another object that can't exist.
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void MouseUp(string button, int x, int y) {
            // Set the states of the appropriate mouse button to not pressed.
            switch (button) {
                case "left":
                    this.MouseLeftDown = false;
                    break;
                case "middle":
                    this.MouseMiddleDown = false;
                    break;
                case "right":
                    this.MouseRightDown = false;
                    break;
            }

            // Since we're lifting a mouse button, do we have a 
            // currently focused scene object?
            if (this._curFocus != null) {
                // Invoke the EndDrag method for that object.
                this._curFocus.EndObjectDrag();
            }
        }

        public void MouseDown(string button, int x, int y) {
            // Set the states of the appropriate mouse button to pressed.
            switch (button) {
                case "left":
                    this.MouseLeftDown = true;
                    break;
                case "middle":
                    this.MouseMiddleDown = true;
                    break;
                case "right":
                    this.MouseRightDown = true;
                    break;
            }

            // Make sure that the scene system has actually been initialized.
            if (this._UIObject != null) {
                // Make sure that we actually have scene objects in our current state.
                if (this._UIObject[(int)Program.State] != null) {
                    // Loop through every possible ZOrder value.
                    for (int z = ZOrder.GetHighZ(); z >= 0; z--) {
                        // Loop through all the scene objects in our current state.
                        foreach (var obj in this._UIObject[(int)Program.State]) {
                            // Does the ZIndex match the ZOrder?
                            if (obj.Z == z) {
                                // Make sure that the object is visible.
                                if (obj.Visible) {
                                    // Did we click within the area of the scene object?
                                    if (x >= obj.Left && x <= obj.Left + obj.Width) {
                                        if (y >= obj.Top && y <= obj.Top + obj.Height) {

                                            //If we had a previous scene object, let that object
                                            // know that it no longer has the focus.
                                            if (this._curFocus != null) {
                                                this._curFocus.HasFocus = false;
                                            }

                                            // Assign this scene object as our currently focused scene object and
                                            // let it know that it has our focus.
                                            this._curFocus = obj;
                                            this._curFocus.HasFocus = true;

                                            if (!this.SceneEditor.Visible) {
                                                this.SceneEditor.Show();
                                            }
                                            this.SceneEditor.LoadObject(ref _curFocus);

                                            // Invoke the appropriate event handling methods.
                                            this._curFocus.ObjectMouseDown(button, x - obj.Left, y - obj.Top);
                                            this._curFocus.BeginDrag(x - obj.Left, y - obj.Top);

                                            // We assume that we have the object we clicked on.
                                            // Return so that we don't apply similar logic on scene objects that
                                            // should not receive this processing.
                                            return;
                                        }
                                    }
                                }
                                // This break will break out of the current loop through all the scene objects for a respective Z value.
                                // It ensures we don't waste time looking for another object that can't exist.
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void KeyDown(string key) {
            // Keypress event handling regarding the scene system requires an
            // object being focused.
            if (this._curFocus != null) {
                this._curFocus.ObjectKeyDown(key);
            }
        }

        public void KeyUp(string key) {
            // Keyup event handling regarding the scene system requires an
            // object being focused.
            if (this._curFocus != null) {
                this._curFocus.ObjectKeyUp(key);
            }
        }

        public void Draw() {
            // Make sure that we've actually loaded the scene system.
            if (this._UIObject != null) {
                // Make sure we actually have scene objects in our current state.
                if (this._UIObject[(int)Program.State] != null) {
                    // Draw every object in this scene if it's visible.
                    foreach (var obj in this._UIObject[(int)Program.State]) {
                        if (obj.Visible) {
                            obj.Draw();
                        }
                    }
                }
            }
        }

        public GraphicalSurface GetSurface(string tagName) {
            // Loop through our collection of graphical surfaces.
            foreach (var surface in this._surfaces) {
                // If the surface's tag matches our specific tag, return the surface.
                if (surface.Tag == tagName.ToLower()) {
                    return surface;
                }
            }
            // If the surface does not exist, return null.
            return null;
        }

        public SceneObject GetUIObject(string name) {
            // Make sure that we actually initialized the scene system.
            if (this._UIObject != null) {
                // Make sure we actually have scene objects in our current state.
                if (this._UIObject[(int)Program.State] != null) {
                    // Loop through all the scene objects in our current state.
                    foreach (var obj in this._UIObject[(int)Program.State]) {
                        // If the object has the same name as the one specified, return it.
                        if (obj.Name?.ToLower() == name?.ToLower()) {
                            return obj;
                        }
                    }
                }
            }
            // If the scene object could not be found, return null.
            return null;
        }

        public void ShowMessage(string message) {
            GetUIObject("msgBackground").Visible = true;
            ((Label)GetUIObject("msgMessage")).Caption = message;
            GetUIObject("msgMessage").Visible = true;
            GetUIObject("msgButton").Visible = true;
        }

        #endregion

        private void LoadSceneObjects() {
            LoadMainMenu();
        }

        private void LoadMainMenu() {
            var col = _UIObject[(int)ClientState.MainMenu];

            var imgBackground = new Objects.Image() {
                Name = "imgBackground",
                Width = GraphicsManager.WindowWidth,
                Height = GraphicsManager.WindowHeight,
                Surface = GetSurface("background")
            };
            col.Add(imgBackground);

            var imgLogo = new Objects.Image() {
                Name = "imgLogo",
                Width = 500,
                Height = 250,
                Top = 50,
                Left = 250,
                Surface = GetSurface("logo"),
                Dragable = true
            };
            col.Add(imgLogo);

            var cmdLogin = new Button() {
                Name = "cmdLogin",
                Caption = "Login",
                FontSize = 12,
                TextColor = Color.Black,
                Left = 350,
                Surface = GetSurface("orangefade"),
                Width = 100,
                Height = 30
            };
            cmdLogin.MouseDown += cmdLogin.cmdLogin_MouseDown;
            col.Add(cmdLogin);

            var cmdRegister = new Button() {
                Name = "cmdRegister",
                Caption = "Register",
                FontSize = 12,
                TextColor = Color.Black,
                Left = 475,
                Surface = GetSurface("orangefade"),
                Width = 100,
                Height = 30
            };
            cmdRegister.MouseDown += cmdRegister.cmdRegister_MouseDown;
            col.Add(cmdRegister);

            var txtUsername = new Textbox() {
                Name = "txtUsername",
                Width = 300,
                Top = 100,
                MaxLength = 12,
                TextColor = Color.Black,
                FontSize = 12,
                Height = 25,
                Left = 300,
                Surface = GetSurface("whitebox"),
                Visible = false
            };
            col.Add(txtUsername);

            var txtPassword = new Textbox() {
                Name = "txtPassword",
                Width = 300,
                Top = 150,
                MaxLength = 12,
                TextColor = Color.Black,
                FontSize = 12,
                Height = 25,
                Left = 300,
                Surface = GetSurface("whitebox"),
                Visible = false
            };
            col.Add(txtPassword);


            var cmdReturn = new Button() {
                Name = "cmdReturn",
                Width = 300,
                Height = 50,
                Caption = "Return",
                TextColor = Color.White,
                Surface = GetSurface("orangefade"),
                Visible = false,
                Left = 300,
                FontSize = 12
            };
            cmdReturn.MouseDown += cmdReturn.cmdReturn_MouseDown;
            col.Add(cmdReturn);

            var cmdRegisterUser = new Button() {
                Name = "cmdRegisterUser",
                Top = 200,
                Left = 400,
                Width = 100,
                Height = 25,


                Caption = "Register User",
                FontSize = 12,
                TextColor = Color.Black,

                Surface = GetSurface("orangefade"),
                Visible = false
            };
            cmdRegisterUser.MouseDown += cmdRegisterUser.cmdRegisterUser_MouseDown;
            col.Add(cmdRegisterUser);

            var cmdLoginUser = new Button() {
                Name = "cmdLoginUser",
                Top = 200,
                Left = 400,
                Width = 100,
                Height = 25,


                Caption = "Login User",
                FontSize = 12,
                TextColor = Color.Black,

                Surface = GetSurface("orangefade"),
                Visible = false
            };
            cmdLoginUser.MouseDown += cmdLoginUser.cmdLoginUser_MouseDown;
            col.Add(cmdLoginUser);
        }
    }
}
