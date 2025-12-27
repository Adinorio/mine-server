using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MineServerGUI.Core;

namespace MineServerGUI.Forms
{
    public partial class ServerSetupForm : Form
    {
        private CheckBox? _chkAcceptEULA;
        private TextBox? _txtMOTD;
        private NumericUpDown? _numMaxPlayers;
        private ComboBox? _cmbDifficulty;
        private ComboBox? _cmbGamemode;
        private Button? _btnCreateServer;
        private Button? _btnSkipSetup;
        private Label? _lblEULALink;

        public bool SetupComplete { get; private set; }
        public string? ServerMOTD { get; private set; }
        public int MaxPlayers { get; private set; } = 8;
        public string Difficulty { get; private set; } = "normal";
        public string Gamemode { get; private set; } = "survival";

        public ServerSetupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Server Setup";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title
            var lblTitle = new Label
            {
                Text = "Configure Your Server",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Set up your Minecraft server (optional settings can be changed later)",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(25, 50),
                AutoSize = true
            };
            this.Controls.Add(lblSubtitle);

            // EULA Section
            var eulaPanel = new Panel
            {
                Location = new Point(25, 85),
                Size = new Size(450, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(eulaPanel);

            _chkAcceptEULA = new CheckBox
            {
                Text = "I accept the Minecraft End User License Agreement (EULA)",
                Location = new Point(15, 15),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _chkAcceptEULA.CheckedChanged += ChkAcceptEULA_CheckedChanged;
            eulaPanel.Controls.Add(_chkAcceptEULA);

            _lblEULALink = new Label
            {
                Text = "Read the EULA: https://aka.ms/MinecraftEULA",
                Location = new Point(15, 45),
                Size = new Size(420, 20),
                ForeColor = Color.FromArgb(33, 150, 243),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8F)
            };
            _lblEULALink.Click += (s, e) => 
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://aka.ms/MinecraftEULA",
                    UseShellExecute = true
                });
            };
            eulaPanel.Controls.Add(_lblEULALink);

            // Server Settings Section
            var settingsPanel = new Panel
            {
                Location = new Point(25, 180),
                Size = new Size(450, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(settingsPanel);

            var lblSettings = new Label
            {
                Text = "Server Settings (Optional)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            settingsPanel.Controls.Add(lblSettings);

            // MOTD
            var lblMOTD = new Label
            {
                Text = "Server Name:",
                Location = new Point(15, 40),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            settingsPanel.Controls.Add(lblMOTD);

            _txtMOTD = new TextBox
            {
                Location = new Point(120, 40),
                Size = new Size(310, 23),
                Text = "A Minecraft Server"
            };
            settingsPanel.Controls.Add(_txtMOTD);

            // Max Players
            var lblMaxPlayers = new Label
            {
                Text = "Max Players:",
                Location = new Point(15, 70),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            settingsPanel.Controls.Add(lblMaxPlayers);

            _numMaxPlayers = new NumericUpDown
            {
                Location = new Point(120, 70),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 100,
                Value = 8
            };
            settingsPanel.Controls.Add(_numMaxPlayers);

            // Difficulty
            var lblDifficulty = new Label
            {
                Text = "Difficulty:",
                Location = new Point(220, 70),
                Size = new Size(70, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            settingsPanel.Controls.Add(lblDifficulty);

            _cmbDifficulty = new ComboBox
            {
                Location = new Point(295, 70),
                Size = new Size(135, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbDifficulty.Items.AddRange(new[] { "peaceful", "easy", "normal", "hard" });
            _cmbDifficulty.SelectedIndex = 2; // normal
            settingsPanel.Controls.Add(_cmbDifficulty);

            // Gamemode
            var lblGamemode = new Label
            {
                Text = "Gamemode:",
                Location = new Point(15, 100),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            settingsPanel.Controls.Add(lblGamemode);

            _cmbGamemode = new ComboBox
            {
                Location = new Point(120, 100),
                Size = new Size(135, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbGamemode.Items.AddRange(new[] { "survival", "creative", "adventure", "spectator" });
            _cmbGamemode.SelectedIndex = 0; // survival
            settingsPanel.Controls.Add(_cmbGamemode);

            // Buttons
            _btnSkipSetup = new Button
            {
                Text = "Skip Setup",
                Location = new Point(300, 330),
                Size = new Size(85, 32),
                FlatStyle = FlatStyle.Flat
            };
            _btnSkipSetup.Click += BtnSkipSetup_Click;
            this.Controls.Add(_btnSkipSetup);

            _btnCreateServer = new Button
            {
                Text = "Create Server",
                Location = new Point(390, 330),
                Size = new Size(85, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            _btnCreateServer.FlatAppearance.BorderSize = 0;
            _btnCreateServer.Click += BtnCreateServer_Click;
            this.Controls.Add(_btnCreateServer);
        }

        private void ChkAcceptEULA_CheckedChanged(object? sender, EventArgs e)
        {
            _btnCreateServer!.Enabled = _chkAcceptEULA!.Checked;
        }

        private void BtnCreateServer_Click(object? sender, EventArgs e)
        {
            if (!_chkAcceptEULA!.Checked)
            {
                MessageBox.Show(
                    "You must accept the Minecraft EULA to continue.",
                    "EULA Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var serverDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
                if (!Directory.Exists(serverDir))
                {
                    Directory.CreateDirectory(serverDir);
                }

                // Create eula.txt
                var eulaPath = Path.Combine(serverDir, "eula.txt");
                File.WriteAllText(eulaPath, "eula=true\n");

                // Create server.properties with user settings
                ServerMOTD = _txtMOTD!.Text;
                MaxPlayers = (int)_numMaxPlayers!.Value;
                Difficulty = _cmbDifficulty!.SelectedItem?.ToString() ?? "normal";
                Gamemode = _cmbGamemode!.SelectedItem?.ToString() ?? "survival";

                CreateServerProperties(serverDir);

                SetupComplete = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating server files:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnSkipSetup_Click(object? sender, EventArgs e)
        {
            if (!_chkAcceptEULA!.Checked)
            {
                MessageBox.Show(
                    "You must accept the Minecraft EULA to continue.",
                    "EULA Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var serverDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
                if (!Directory.Exists(serverDir))
                {
                    Directory.CreateDirectory(serverDir);
                }

                // Create eula.txt only
                var eulaPath = Path.Combine(serverDir, "eula.txt");
                File.WriteAllText(eulaPath, "eula=true\n");

                // Use default values
                ServerMOTD = "A Minecraft Server";
                MaxPlayers = 8;
                Difficulty = "normal";
                Gamemode = "survival";

                // Create default server.properties if it doesn't exist
                var propertiesPath = Path.Combine(serverDir, "server.properties");
                if (!File.Exists(propertiesPath))
                {
                    CreateServerProperties(serverDir);
                }

                SetupComplete = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating server files:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CreateServerProperties(string serverDir)
        {
            var propertiesPath = Path.Combine(serverDir, "server.properties");
            var properties = new System.Text.StringBuilder();

            properties.AppendLine("#Minecraft server properties");
            properties.AppendLine($"motd={ServerMOTD}");
            properties.AppendLine($"max-players={MaxPlayers}");
            properties.AppendLine("online-mode=false");
            properties.AppendLine("white-list=false");
            properties.AppendLine("pvp=true");
            properties.AppendLine($"difficulty={Difficulty}");
            properties.AppendLine($"gamemode={Gamemode}");
            properties.AppendLine("force-gamemode=false");
            properties.AppendLine("hardcore=false");
            properties.AppendLine("enable-command-block=true");
            properties.AppendLine("max-world-size=29999984");
            properties.AppendLine("server-port=25565");
            properties.AppendLine("server-ip=");
            properties.AppendLine("network-compression-threshold=256");
            properties.AppendLine("max-tick-time=60000");
            properties.AppendLine("use-native-transport=true");
            properties.AppendLine("enable-status=true");
            properties.AppendLine("enforce-whitelist=false");
            properties.AppendLine("level-name=world");
            properties.AppendLine("level-seed=");
            properties.AppendLine("level-type=minecraft\\:normal");
            properties.AppendLine("generate-structures=true");
            properties.AppendLine("generator-settings={}");
            properties.AppendLine("allow-flight=false");
            properties.AppendLine("spawn-monsters=true");
            properties.AppendLine("spawn-animals=true");
            properties.AppendLine("spawn-npcs=true");
            properties.AppendLine("spawn-protection=16");
            properties.AppendLine("enforce-secure-profile=false");
            properties.AppendLine("log-ips=true");
            properties.AppendLine("function-permission-level=2");
            properties.AppendLine("op-permission-level=4");
            properties.AppendLine("max-build-height=384");
            properties.AppendLine("view-distance=10");
            properties.AppendLine("simulation-distance=10");
            properties.AppendLine("max-chained-neighbor-updates=1000000");
            properties.AppendLine("entity-broadcast-range-percentage=100");
            properties.AppendLine("player-idle-timeout=0");

            File.WriteAllText(propertiesPath, properties.ToString());
        }
    }
}

