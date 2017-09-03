using System.Threading;
using System.Windows.Forms;

namespace Client.Graphics.Sfml.Scenes
{
    public partial class GuiEditor : Form
    {
        public GuiEditor() {
            InitializeComponent();
        }

        private SceneObject _obj;

        public void LoadObject(ref SceneObject obj) {
            this._obj = obj;

            string surface = string.Empty;
            string surface2 = string.Empty;
            string caption = string.Empty;
            string length = string.Empty;
            string color = string.Empty;
            string fontsize = string.Empty;
            string passwordchar = string.Empty;
            
            switch (obj.GetObjectType()) {
                case "image":
                    var img = (Objects.Image)obj;
                    if (img.Surface != null) {
                        surface = img.Surface.Tag;
                    }
                    break;
                case "label":
                    var lbl = (Objects.Label)obj;
                    caption = lbl.Caption;
                    color= ColorToString(lbl.TextColor);
                    fontsize = lbl.FontSize.ToString();
                    length = "NA";
                    if (lbl.Surface != null) {
                        surface = lbl.Surface.Tag;
                    }
                    break;
                case "textbox":
                    var txt = (Objects.Textbox)obj;
                    caption = txt.Text;
                    color = ColorToString(txt.TextColor);
                    fontsize = txt.FontSize.ToString();
                    passwordchar = txt.PasswordChar.ToString();
                    surface = txt.Surface.Tag;
                    length = txt.MaxLength.ToString();
                    break;
                case "checkbox":
                    var chk = (Objects.CheckBox)obj;
                    caption = chk.Caption;
                    color = ColorToString(chk.TextColor);
                    fontsize = chk.FontSize.ToString();
                    surface = chk.Surface.Tag;
                    surface2 = chk.SurfaceUnchecked.Tag;
                    break;
                case "button":
                    var btn = (Objects.Button)obj;
                    caption = btn.Caption;
                    color = ColorToString(btn.TextColor);
                    fontsize = btn.FontSize.ToString();
                    if (btn.Surface != null) {
                        surface = btn.Surface.Tag;
                    }
                    break;
            }

            this.txtSurface.Text = surface;
            this.txtSurface2.Text = surface2;
            this.txtCaption.Text = caption;
            this.txtLength.Text = length;
            this.txtColor.Text = color;
            this.txtFontSize.Text = fontsize;
            this.txtPasswordChar.Text = passwordchar;

            this.txtName.Text = obj.Name;
            this.txtWidth.Text = obj.Width.ToString();
            this.txtHeight.Text = obj.Height.ToString();
            this.txtTop.Text = obj.Top.ToString();
            this.txtLeft.Text = obj.Left.ToString();
            this.txtZAxis.Text = obj.Z.ToString();
            this.chkDragable.Checked = obj.Dragable;
        }

        private void GuiEditor_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
        }

        private void txtWidth_TextChanged(object sender, System.EventArgs e) {
            int.TryParse(txtWidth.Text, out _obj.Width);
        }

        private void txtHeight_TextChanged(object sender, System.EventArgs e) {
            int.TryParse(txtHeight.Text, out _obj.Height);
        }

        private void chkDragable_CheckedChanged(object sender, System.EventArgs e) {
            _obj.Dragable = chkDragable.Checked;
        }

        private void txtName_TextChanged(object sender, System.EventArgs e) {
            _obj.Name = txtName.Text;
        }

        private void txtTop_TextChanged(object sender, System.EventArgs e) {
            int.TryParse(txtTop.Text, out _obj.Top);
        }

        private void txtLeft_TextChanged(object sender, System.EventArgs e) {
            int.TryParse(txtLeft.Text, out _obj.Left);
        }

        private void txtCaption_TextChanged(object sender, System.EventArgs e) {
            switch (_obj?.GetObjectType()) {
                case "label":
                    var lbl = (Objects.Label)_obj;
                    lbl.Caption = txtCaption.Text;
                    break;
                case "textbox":
                    var txt = (Objects.Textbox)_obj;
                    txt.Text = txtCaption.Text;
                    break;
                case "button":
                    var btn = (Objects.Button)_obj;
                    btn.Caption = txtCaption.Text;
                    break;
            }
        }

        private void txtLength_TextChanged(object sender, System.EventArgs e) {
            switch (_obj?.GetObjectType()) {
                case "textbox":
                    var txt = (Objects.Textbox)_obj;
                    int.TryParse(txtLength.Text, out txt.MaxLength);
                    break;
            }
        }

        private void txtFontSize_TextChanged(object sender, System.EventArgs e) {
            switch (_obj?.GetObjectType()) {
                case "label":
                    var lbl = (Objects.Label)_obj;
                    uint.TryParse(txtFontSize.Text, out lbl.FontSize);
                    break;
                case "textbox":
                    var txt = (Objects.Textbox)_obj;
                    uint.TryParse(txtFontSize.Text, out txt.FontSize);
                    break;
                case "button":
                    var btn = (Objects.Button)_obj;
                    uint.TryParse(txtFontSize.Text, out btn.FontSize);
                    break;
            }
        }

        private void txtPasswordChar_TextChanged(object sender, System.EventArgs e) {
            switch (_obj?.GetObjectType()) {
                case "textbox":
                    var txt = (Objects.Textbox)_obj;
                    if (txtPasswordChar.Text.Length > 0) {
                        txt.PasswordChar = txtPasswordChar.Text[0];
                    } else {
                        txt.PasswordChar = '\0';
                    }
                    break;
            }
        }

        private void cmdGfxReload_Click(object sender, System.EventArgs e) {
            _obj.Surface = ((Sfml)GraphicsManager.Graphics).SceneSystem.GetSurface(txtSurface.Text);

            switch (_obj.GetObjectType()) {
                case "checkbox":
                    var chk = (Objects.CheckBox)_obj;
                    chk.SurfaceUnchecked = ((Sfml)GraphicsManager.Graphics).SceneSystem.GetSurface(txtSurface2.Text);
                    break;
            }
        }


        private void txtColor_TextChanged(object sender, System.EventArgs e) {
            switch (_obj?.GetObjectType()) {
                case "label":
                    var lbl = (Objects.Label)_obj;
                    lbl.TextColor = StringToColor(txtColor.Text);
                    break;
                case "textbox":
                    var txt = (Objects.Textbox)_obj;
                    txt.TextColor = StringToColor(txtColor.Text);
                    break;
                case "button":
                    var btn = (Objects.Button)_obj;
                    btn.TextColor = StringToColor(txtColor.Text);
                    break;
            }
        }

        private void lstObjects_SelectedIndexChanged(object sender, System.EventArgs e) {
            string name = lstObjects.Items[lstObjects.SelectedIndex].ToString();

            var collection = ((Sfml)GraphicsManager.Graphics).SceneSystem._UIObject[(int)Program.State];
            for (int i = 0; i < collection.Count; i++) {
                if (collection[i].Name == name) {
                    var obj = collection[i];
                    this.LoadObject(ref obj);
                    return;
                }
            }
        }

        private void GuiEditor_Load(object sender, System.EventArgs e) {
            var scene = ((Sfml)GraphicsManager.Graphics).SceneSystem;

            lstObjects.Items.Clear();
            foreach (var obj in scene._UIObject[(int)Program.State]) {
                lstObjects.Items.Add(obj.Name);
            }
        }

        private string GetObjectFormat(SceneObject obj) {
            string type = string.Empty;
            string dragable = obj.Dragable ? "Dragable = true,\n" : string.Empty;
            string extra = string.Empty;

            switch (obj.GetObjectType()) {
                case "label":
                    var lbl = (Objects.Label)obj;
                    type = "Objects.Label";
                    extra += "Caption = \"" + lbl.Caption + "\",\n";
                    extra += "\tTextColor = new SFML.Graphics.Color(" + ColorToString(lbl.TextColor) + "),\n";
                    extra += "\tFontSize = " + lbl.FontSize;
                    break;
                case "button":
                    var btn = (Objects.Button)obj;
                    type = "Objects.Button";
                    extra += "Caption = \"" + btn.Caption + "\",\n";
                    extra += "\tTextColor = new SFML.Graphics.Color(" + ColorToString(btn.TextColor) + "),\n";
                    extra += "\tFontSize = " + btn.FontSize;
                    break;
                case "textbox":
                    var txt = (Objects.Textbox)obj;
                    type = "Objects.Textbox";
                    extra += "Text = \"" + txt.Text + "\",\n";
                    extra += "\tTextColor = new SFML.Graphics.Color(" + ColorToString(txt.TextColor) + "),\n";
                    extra += "\tMaxLength = " + txt.MaxLength + ",\n";
                    extra += (txt.PasswordChar == '\0') ? string.Empty : "\tPasswordChar = \'" + txt.PasswordChar + "\',\n";
                    extra += "\tFontSize = " + txt.FontSize;
                    break;
                case "checkbox":
                    var chk = (Objects.CheckBox)obj;
                    type = "Objects.Checkbox";
                    extra += "SurfaceUnchecked = GetSurface(\"" + chk.SurfaceUnchecked.Tag + "\"),\n";
                    extra += "\tCaption = \"" + chk.Caption + "\",\n";
                    extra += "\tTextColor = new SFML.Graphics.Color(" + ColorToString(chk.TextColor) + "),\n";
                    extra += "\tFontSize = " + chk.FontSize;
                    break;
                case "image":
                    type = "Objects.Image";
                    break;
            }


            return string.Format(@"var {0} = new {1}() {{
    Name = ""{0}"",
    Width = {2},
    Height = {3},
    Top = {4},
    Left = {5},
    Surface = GetSurface(""{6}""),
    {7}{8}
}};",
                obj.Name, type,
                obj.Width, _obj.Height,
                obj.Top, _obj.Left,
                obj.Surface != null ? obj.Surface.Tag : string.Empty,
                dragable, extra);
        }

        private void cmdExport_Click(object sender, System.EventArgs e) {
            StringToClipboard(GetObjectFormat(_obj));
        }

        private void cmdExportAll_Click(object sender, System.EventArgs e) {
            var scene = ((Sfml)GraphicsManager.Graphics).SceneSystem;
            string output = string.Empty;

            foreach (var obj in scene._UIObject[(int)Program.State]) {
                output += GetObjectFormat(obj) + "\n";
            }

            StringToClipboard(output);
        }

        private void StringToClipboard(string value) {
            Thread thread = new Thread(() => Clipboard.SetText(value));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            MessageBox.Show("Coppied to clipboard");
        }

        private string ColorToString(SFML.Graphics.Color color) {
            string value = color.R + "," + color.G + "," + color.B;
            value += (color.A != 0) ? "," + color.A : string.Empty;
            return value;
        }

        private SFML.Graphics.Color StringToColor(string color) {
            byte[] colors = new byte[4];
            colors[3] = 255;
            var splitString = color.Split(',');

            if (splitString.Length != 3 && splitString.Length != 4) {
                return SFML.Graphics.Color.Transparent;
            }

            for (int i = 0; i < splitString.Length; i++) {
                if (!byte.TryParse(splitString[i], out colors[i])) {
                    return SFML.Graphics.Color.Transparent;
                }
            }

            return new SFML.Graphics.Color(colors[0], colors[1], colors[2], colors[3]);
        }
    }
}
