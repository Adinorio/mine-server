using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineServerGUI.Core;
using MineServerGUI.Models;
using MineServerGUI.Utilities;

namespace MineServerGUI.Forms
{
    public partial class MainForm : Form
    {
        private readonly ServerManager _serverManager;
        private readonly ConfigManager _configManager;
        private readonly ServerDownloader _downloader;
        private readonly ServerProfileManager _profileManager;
        private System.Windows.Forms.Timer? _updateTimer;

        // UI Controls
        private Label? _lblStatus;
        private Label? _lblCurrentProfile;
        private Button? _btnManageProfiles;
        private Button? _btnStart;
        private Button? _btnStop;
        private Button? _btnRestart;
        private Button? _btnChangeVersion;
        private Label? _lblLocalIP;
        private Label? _lblPublicIP;
        private Button? _btnCopyLocalIP;
        private Button? _btnCopyPublicIP;
        private Label? _lblPlayers;
        private ListBox? _lstWhitelist;
        private TextBox? _txtAddPlayer;
        private Button? _btnAddPlayer;
        private Button? _btnRemovePlayer;
        private CheckBox? _chkWhitelistEnabled;
        private Label? _lblMaxPlayers;
        private NumericUpDown? _numMaxPlayers;
        private ComboBox? _cmbDifficulty;
        private ComboBox? _cmbGamemode;
        private TextBox? _txtMOTD;

        public MainForm()
        {
            _serverManager = new ServerManager();
            _configManager = new ConfigManager();
            _downloader = new ServerDownloader();
            _profileManager = new ServerProfileManager();
            InitializeComponent();
            
            // Check if there are any valid profiles with existing JAR files
            bool hasValidProfile = false;
            if (_profileManager.Profiles.Count > 0)
            {
                // Find the best profile (prefer meaningful names, valid JARs, recent modifications)
                ServerProfile? bestProfile = null;
                foreach (var profile in _profileManager.Profiles)
                {
                    if (File.Exists(profile.ServerJarPath))
                    {
                        hasValidProfile = true;
                        
                        // Prefer profiles with meaningful names (not single characters like "t")
                        if (profile.Name.Length > 1)
                        {
                            if (bestProfile == null || 
                                profile.LastModifiedDate > bestProfile.LastModifiedDate)
                            {
                                bestProfile = profile;
                            }
                        }
                        else if (bestProfile == null || bestProfile.Name.Length <= 1)
                        {
                            // Only use single-char names if no better option exists
                            if (bestProfile == null || 
                                profile.LastModifiedDate > bestProfile.LastModifiedDate)
                            {
                                bestProfile = profile;
                            }
                        }
                    }
                }
                
                // Set the best profile as current if found
                if (bestProfile != null)
                {
                    if (_profileManager.CurrentProfile == null || 
                        _profileManager.CurrentProfile.Id != bestProfile.Id)
                    {
                        _profileManager.SetCurrentProfile(bestProfile.Id);
                    }
                }
                else if (hasValidProfile)
                {
                    // Fallback: use first valid profile
                    var firstValid = _profileManager.Profiles.FirstOrDefault(p => File.Exists(p.ServerJarPath));
                    if (firstValid != null && 
                        (_profileManager.CurrentProfile == null || 
                         _profileManager.CurrentProfile.Id != firstValid.Id))
                    {
                        _profileManager.SetCurrentProfile(firstValid.Id);
                    }
                }
            }
            
            // Initialize with current profile if it exists and is valid
            if (_profileManager.CurrentProfile != null && File.Exists(_profileManager.CurrentProfile.ServerJarPath))
            {
                _serverManager.SetProfile(_profileManager.CurrentProfile);
                _configManager.SetProfile(_profileManager.CurrentProfile);
            }
            
            // Show setup wizard if:
            // 1. No profiles exist at all, OR
            // 2. Current profile is null, OR
            // 3. Current profile exists but has no valid JAR file
            bool needsSetup = _profileManager.Profiles.Count == 0 || 
                             _profileManager.CurrentProfile == null ||
                             !File.Exists(_profileManager.CurrentProfile.ServerJarPath);
            
            if (needsSetup)
            {
                CheckServerSetup();
                
                // If setup was cancelled or no valid profile was created, exit application
                if (_profileManager.CurrentProfile == null || !File.Exists(_profileManager.CurrentProfile.ServerJarPath))
                {
                    // Setup was cancelled, exit application
                    Application.Exit();
                    return;
                }
                
                // Setup completed, initialize with the new profile
                _serverManager.SetProfile(_profileManager.CurrentProfile);
                _configManager.SetProfile(_profileManager.CurrentProfile);
            }
            else
            {
                // We have valid profiles, ensure current profile is set
                if (_profileManager.CurrentProfile != null)
                {
                    _serverManager.SetProfile(_profileManager.CurrentProfile);
                    _configManager.SetProfile(_profileManager.CurrentProfile);
                }
            }
            
            SetupUpdateTimer();
            LoadConfiguration();
            UpdateUI();
            UpdateProfileLabel(); // Update profile label after initialization
        }

        private void CheckServerSetup()
        {
            // Step 1: JAR Selection/Download
            var setupWizard = new SetupWizardForm();
            var result = setupWizard.ShowDialog();
            
            // If setup was cancelled or not completed, exit application
            if (result == DialogResult.Cancel || !setupWizard.SetupComplete)
            {
                // User cancelled setup, exit entire application
                Application.Exit();
                return;
            }

            // Create default profile for initial setup
            string? version = setupWizard.RequestedVersion ?? setupWizard.DetectedVersion;
            if (string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(setupWizard.SelectedServerJarPath))
            {
                // Try to detect version if not available
                try
                {
                    version = ServerVersionDetector.DetectVersion(setupWizard.SelectedServerJarPath);
                }
                catch
                {
                    version = "Unknown";
                }
            }
            
            var defaultProfile = _profileManager.CreateProfile(
                "Default Server",
                version ?? "Unknown",
                "Default server profile"
            );

            // Copy server.jar to profile directory if it's in a different location
            if (File.Exists(setupWizard.SelectedServerJarPath))
            {
                if (!Directory.Exists(defaultProfile.ServerDirectory))
                {
                    Directory.CreateDirectory(defaultProfile.ServerDirectory);
                }
                
                // Only copy if the source is different from the target
                var sourcePath = Path.GetFullPath(setupWizard.SelectedServerJarPath);
                var targetPath = Path.GetFullPath(defaultProfile.ServerJarPath);
                
                if (!sourcePath.Equals(targetPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(setupWizard.SelectedServerJarPath, defaultProfile.ServerJarPath, true);
                }
            }

            // Step 2: Server Setup (EULA + Configuration) - Profile-aware
            var serverSetup = new ServerSetupForm(defaultProfile.ServerDirectory);
            var setupResult = serverSetup.ShowDialog();
            
            // If server setup was cancelled, exit application
            if (setupResult == DialogResult.Cancel || !serverSetup.SetupComplete)
            {
                Application.Exit();
                return;
            }
            
            // Profile is now set as current and has all files initialized
            // The profile was already saved by CreateProfile(), so it will persist
        }

        private void InitializeComponent()
        {
            this.Text = "MineServer GUI";
            this.Size = new Size(700, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 9F);

            // Current Profile Label
            // Current Profile Label - Will be updated after initialization
            _lblCurrentProfile = new Label
            {
                Text = "Profile: Loading...",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(_lblCurrentProfile);

            // Change Server Version button - beside profile
            _btnChangeVersion = new Button
            {
                Text = "Change Server Version",
                Location = new Point(200, 20),
                Size = new Size(150, 25),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };
            _btnChangeVersion.FlatAppearance.BorderSize = 0;
            _btnChangeVersion.Click += BtnChangeVersion_Click;
            this.Controls.Add(_btnChangeVersion);

            // Manage Profiles Button
            _btnManageProfiles = new Button
            {
                Text = "Manage Profiles",
                Location = new Point(360, 20),
                Size = new Size(120, 25),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };
            _btnManageProfiles.FlatAppearance.BorderSize = 0;
            _btnManageProfiles.Click += BtnManageProfiles_Click;
            this.Controls.Add(_btnManageProfiles);

            // Status Section
            _lblStatus = new Label
            {
                Text = "● Stopped",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 50),
                Size = new Size(200, 30),
                AutoSize = true
            };
            this.Controls.Add(_lblStatus);

            // Server Control Buttons - Fixed spacing to prevent overlap
            _btnStart = new Button
            {
                Text = "▶ Start",
                Location = new Point(20, 90),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _btnStart.FlatAppearance.BorderSize = 0;
            _btnStart.Click += BtnStart_Click;
            this.Controls.Add(_btnStart);

            _btnStop = new Button
            {
                Text = "■ Stop",
                Location = new Point(130, 90),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            _btnStop.FlatAppearance.BorderSize = 0;
            _btnStop.Click += BtnStop_Click;
            this.Controls.Add(_btnStop);

            _btnRestart = new Button
            {
                Text = "↻ Restart",
                Location = new Point(240, 90),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            _btnRestart.FlatAppearance.BorderSize = 0;
            _btnRestart.Click += BtnRestart_Click;
            this.Controls.Add(_btnRestart);


            // Connection Info Section with Panel
            var connectionPanel = new Panel
            {
                Location = new Point(20, 170),
                Size = new Size(320, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(connectionPanel);

            var lblConnectionInfo = new Label
            {
                Text = "Connection Info",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                AutoSize = true
            };
            connectionPanel.Controls.Add(lblConnectionInfo);

            _lblLocalIP = new Label
            {
                Text = "Local IP: Loading...",
                Location = new Point(10, 30),
                Size = new Size(200, 20),
                AutoSize = true
            };
            connectionPanel.Controls.Add(_lblLocalIP);

            _btnCopyLocalIP = new Button
            {
                Text = "📋 Copy",
                Location = new Point(220, 28),
                Size = new Size(80, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            _btnCopyLocalIP.FlatAppearance.BorderSize = 1;
            _btnCopyLocalIP.Click += BtnCopyLocalIP_Click;
            connectionPanel.Controls.Add(_btnCopyLocalIP);

            _lblPublicIP = new Label
            {
                Text = "Public IP: Loading...",
                Location = new Point(10, 55),
                Size = new Size(200, 20),
                AutoSize = true
            };
            connectionPanel.Controls.Add(_lblPublicIP);

            _btnCopyPublicIP = new Button
            {
                Text = "📋 Copy",
                Location = new Point(220, 53),
                Size = new Size(80, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            _btnCopyPublicIP.FlatAppearance.BorderSize = 1;
            _btnCopyPublicIP.Click += BtnCopyPublicIP_Click;
            connectionPanel.Controls.Add(_btnCopyPublicIP);

            // Players Section
            _lblPlayers = new Label
            {
                Text = "Players: 0/8",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 250),
                Size = new Size(200, 25),
                AutoSize = true
            };
            this.Controls.Add(_lblPlayers);

            // Whitelist Section with Panel
            var whitelistPanel = new Panel
            {
                Location = new Point(20, 280),
                Size = new Size(320, 220),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(whitelistPanel);

            var lblWhitelist = new Label
            {
                Text = "Whitelist Management",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                AutoSize = true
            };
            whitelistPanel.Controls.Add(lblWhitelist);

            _chkWhitelistEnabled = new CheckBox
            {
                Text = "✓ Enable Whitelist",
                Location = new Point(10, 30),
                Size = new Size(150, 25),
                Checked = true,
                Font = new Font("Segoe UI", 9F)
            };
            _chkWhitelistEnabled.CheckedChanged += ChkWhitelistEnabled_CheckedChanged;
            whitelistPanel.Controls.Add(_chkWhitelistEnabled);

            _lstWhitelist = new ListBox
            {
                Location = new Point(10, 60),
                Size = new Size(300, 120),
                BorderStyle = BorderStyle.FixedSingle
            };
            whitelistPanel.Controls.Add(_lstWhitelist);

            _txtAddPlayer = new TextBox
            {
                Location = new Point(10, 190),
                Size = new Size(180, 23),
                BorderStyle = BorderStyle.FixedSingle
            };
            whitelistPanel.Controls.Add(_txtAddPlayer);

            _btnAddPlayer = new Button
            {
                Text = "+ Add",
                Location = new Point(200, 189),
                Size = new Size(55, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F)
            };
            _btnAddPlayer.FlatAppearance.BorderSize = 0;
            _btnAddPlayer.Click += BtnAddPlayer_Click;
            whitelistPanel.Controls.Add(_btnAddPlayer);

            _btnRemovePlayer = new Button
            {
                Text = "− Remove",
                Location = new Point(260, 189),
                Size = new Size(50, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F)
            };
            _btnRemovePlayer.FlatAppearance.BorderSize = 0;
            _btnRemovePlayer.Click += BtnRemovePlayer_Click;
            whitelistPanel.Controls.Add(_btnRemovePlayer);

            // Settings Section with Panel
            var settingsPanel = new Panel
            {
                Location = new Point(360, 50),
                Size = new Size(320, 450),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(settingsPanel);

            var lblSettings = new Label
            {
                Text = "Server Settings",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                AutoSize = true
            };
            settingsPanel.Controls.Add(lblSettings);

            _lblMaxPlayers = new Label
            {
                Text = "Max Players:",
                Location = new Point(10, 35),
                Size = new Size(120, 20),
                AutoSize = true
            };
            settingsPanel.Controls.Add(_lblMaxPlayers);

            _numMaxPlayers = new NumericUpDown
            {
                Location = new Point(10, 55),
                Size = new Size(120, 23),
                Minimum = 1,
                Maximum = 20,
                Value = 8,
                BorderStyle = BorderStyle.FixedSingle
            };
            _numMaxPlayers.ValueChanged += NumMaxPlayers_ValueChanged;
            settingsPanel.Controls.Add(_numMaxPlayers);

            var lblDifficulty = new Label
            {
                Text = "Difficulty:",
                Location = new Point(10, 85),
                Size = new Size(120, 20),
                AutoSize = true
            };
            settingsPanel.Controls.Add(lblDifficulty);

            _cmbDifficulty = new ComboBox
            {
                Location = new Point(10, 105),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            _cmbDifficulty.Items.AddRange(new[] { "peaceful", "easy", "normal", "hard" });
            _cmbDifficulty.SelectedIndex = 2; // normal
            _cmbDifficulty.SelectedIndexChanged += CmbDifficulty_SelectedIndexChanged;
            settingsPanel.Controls.Add(_cmbDifficulty);

            var lblGamemode = new Label
            {
                Text = "Gamemode:",
                Location = new Point(10, 135),
                Size = new Size(120, 20),
                AutoSize = true
            };
            settingsPanel.Controls.Add(lblGamemode);

            _cmbGamemode = new ComboBox
            {
                Location = new Point(10, 155),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            _cmbGamemode.Items.AddRange(new[] { "survival", "creative", "adventure", "spectator" });
            _cmbGamemode.SelectedIndex = 0; // survival
            _cmbGamemode.SelectedIndexChanged += CmbGamemode_SelectedIndexChanged;
            settingsPanel.Controls.Add(_cmbGamemode);

            var lblMOTD = new Label
            {
                Text = "Server Message (MOTD):",
                Location = new Point(10, 185),
                Size = new Size(200, 20),
                AutoSize = true
            };
            settingsPanel.Controls.Add(lblMOTD);

            _txtMOTD = new TextBox
            {
                Location = new Point(10, 205),
                Size = new Size(300, 23),
                MaxLength = 59,
                BorderStyle = BorderStyle.FixedSingle
            };
            _txtMOTD.TextChanged += TxtMOTD_TextChanged;
            settingsPanel.Controls.Add(_txtMOTD);

            // Load network info
            LoadNetworkInfo();

            // Developer: Add keyboard shortcut to reset setup (Ctrl+Shift+S)
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // Developer shortcut: Ctrl+Shift+S to reset setup wizard
            if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                var result = MessageBox.Show(
                    "Developer Mode: Reset Setup Wizard?\n\nThis will delete server.jar and show the setup wizard again.",
                    "Reset Setup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var serverJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
                    if (File.Exists(serverJar))
                    {
                        try
                        {
                            File.Delete(serverJar);
                            MessageBox.Show("server.jar deleted. Restart the application to see setup wizard.", 
                                "Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting server.jar:\n{ex.Message}", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        CheckServerSetup();
                    }
                }
            }
        }

        private void SetupUpdateTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // Update every second
            };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        private void LoadNetworkInfo()
        {
            var localIP = NetworkHelper.GetLocalIPAddress();
            _lblLocalIP!.Text = localIP != null ? $"Local IP: {localIP}:25565" : "Local IP: Not available";

            // Get public IP asynchronously
            System.Threading.Tasks.Task.Run(() =>
            {
                var publicIP = NetworkHelper.GetPublicIPAddress();
                
                if (IsDisposed || !IsHandleCreated)
                    return;

                try
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke((MethodInvoker)delegate
                        {
                            if (!IsDisposed && IsHandleCreated && _lblPublicIP != null && !_lblPublicIP.IsDisposed)
                            {
                                _lblPublicIP.Text = publicIP != null ? $"Public IP: {publicIP}:25565" : "Public IP: Not available";
                            }
                        });
                    }
                    else
                    {
                        if (_lblPublicIP != null && !_lblPublicIP.IsDisposed)
                        {
                            _lblPublicIP.Text = publicIP != null ? $"Public IP: {publicIP}:25565" : "Public IP: Not available";
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // Handle not created yet, ignore
                }
            });
        }

        private void LoadConfiguration()
        {
            try
            {
                var props = _configManager.LoadServerProperties();
                _numMaxPlayers!.Value = props.MaxPlayers;
                _cmbDifficulty!.SelectedItem = props.Difficulty;
                _cmbGamemode!.SelectedItem = props.Gamemode;
                _txtMOTD!.Text = props.MOTD;
                _chkWhitelistEnabled!.Checked = props.WhitelistEnabled;

                var whitelist = _configManager.LoadWhitelist();
                _lstWhitelist!.Items.Clear();
                foreach (var player in whitelist)
                {
                    _lstWhitelist.Items.Add(player);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateUI()
        {
            bool isRunning = _serverManager.IsRunning;

            _lblStatus!.Text = isRunning ? "● Running" : "● Stopped";
            _lblStatus.ForeColor = isRunning ? Color.Green : Color.Red;

            _btnStart!.Enabled = !isRunning;
            _btnStop!.Enabled = isRunning;
            _btnRestart!.Enabled = isRunning;
        }

        private async void BtnStart_Click(object? sender, EventArgs e)
        {
            try
            {
                // Check if server JAR exists (use current profile's path)
                var currentJarPath = _profileManager.CurrentProfile?.ServerJarPath 
                    ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
                
                // Detect MC version for Java requirement
                string? mcVersion = null;
                int requiredJavaVersion = 21; // Default
                if (File.Exists(currentJarPath))
                {
                    try
                    {
                        mcVersion = ServerVersionDetector.DetectVersion(currentJarPath);
                        if (!string.IsNullOrEmpty(mcVersion))
                        {
                            requiredJavaVersion = VersionHelper.GetRequiredJavaMajorVersion(mcVersion);
                        }
                    }
                    catch { }
                }

                // Check Java compatibility
                if (_serverManager.JavaPath == null)
                {
                    var result = MessageBox.Show(
                        $"Java is not installed!\n\n" +
                        (mcVersion != null ? $"Minecraft {mcVersion} requires Java {requiredJavaVersion}\n\n" : "") +
                        "Would you like to download and install Java automatically?\n\n" +
                        "(This will download ~200MB and install Java in the application directory)",
                        "Java Not Found",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        await DownloadJavaAsync(requiredJavaVersion, mcVersion);
                        // Refresh Java path after installation
                        _serverManager.RefreshJava();
                        if (_serverManager.JavaPath == null)
                        {
                            MessageBox.Show("Java installation completed, but could not find Java path. Please restart the application.",
                                "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                    }
                    else
                    {
                        return; // User cancelled
                    }
                }
                else
                {
                    // Check if Java version is compatible
                    if (mcVersion != null)
                    {
                        var javaCheck = _serverManager.CheckJavaCompatibility(mcVersion);
                        if (!javaCheck.IsCompatible)
                        {
                            var result = MessageBox.Show(
                                $"Java version mismatch!\n\n" +
                                $"Minecraft {mcVersion} requires Java {requiredJavaVersion}\n" +
                                $"Current Java version: {javaCheck.CurrentVersion}\n\n" +
                                "Would you like to download and install Java {requiredJavaVersion} automatically?\n\n" +
                                "(This will download ~200MB and install Java in the application directory)",
                                "Java Version Mismatch",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (result == DialogResult.Yes)
                            {
                                await DownloadJavaAsync(requiredJavaVersion, mcVersion);
                                // Refresh Java path after installation
                                _serverManager.RefreshJava();
                                if (_serverManager.JavaPath == null)
                                {
                                    MessageBox.Show("Java installation completed, but could not find Java path. Please restart the application.",
                                        "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }
                            else
                            {
                                return; // User cancelled
                            }
                        }
                    }
                }

                if (!File.Exists(currentJarPath))
                {
                    var result = MessageBox.Show(
                        "Server JAR file not found!\n\nWould you like to download Minecraft Server now?",
                        "Server JAR Missing",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        var setupWizard = new SetupWizardForm();
                        if (setupWizard.ShowDialog() == DialogResult.OK && setupWizard.SetupComplete)
                        {
                            // Reload profile if it was updated
                            if (_profileManager.CurrentProfile != null)
                            {
                                _serverManager.SetProfile(_profileManager.CurrentProfile);
                                _configManager.SetProfile(_profileManager.CurrentProfile);
                            }
                            // Continue to start server
                        }
                        else
                        {
                            return; // User cancelled download
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Server JAR file is required to run the server.\n\n" +
                            "Please download Minecraft Server from:\n" +
                            "https://www.minecraft.net/en-us/download/server",
                            "Server JAR Required",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }

                _serverManager.StartServer();
                MessageBox.Show("Server started successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            try
            {
                _serverManager.StopServer();
                MessageBox.Show("Server stopped.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping server:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRestart_Click(object? sender, EventArgs e)
        {
            if (!_serverManager.IsRunning)
            {
                MessageBox.Show("Server is not running. Please start the server first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to restart the server?\n\nAll players will be disconnected.",
                "Restart Server",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Disable buttons during restart
                _btnStart!.Enabled = false;
                _btnStop!.Enabled = false;
                _btnRestart!.Enabled = false;
                _lblStatus!.Text = "● Restarting...";
                _lblStatus.ForeColor = Color.Orange;
                Application.DoEvents();

                _serverManager.RestartServer();
                
                _lblStatus.Text = "● Running";
                _lblStatus.ForeColor = Color.Green;
                MessageBox.Show("Server restarted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restarting server:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI();
            }
        }

        private void BtnCopyLocalIP_Click(object? sender, EventArgs e)
        {
            var localIP = NetworkHelper.GetLocalIPAddress();
            if (localIP != null)
            {
                Clipboard.SetText($"{localIP}:25565");
                MessageBox.Show("Local IP copied to clipboard!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnCopyPublicIP_Click(object? sender, EventArgs e)
        {
            var publicIP = NetworkHelper.GetPublicIPAddress();
            if (publicIP != null)
            {
                Clipboard.SetText($"{publicIP}:25565");
                MessageBox.Show("Public IP copied to clipboard!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Could not retrieve public IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAddPlayer_Click(object? sender, EventArgs e)
        {
            var playerName = _txtAddPlayer!.Text.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Please enter a player name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _configManager.AddToWhitelist(playerName);
                _txtAddPlayer.Clear();
                LoadConfiguration();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding player:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemovePlayer_Click(object? sender, EventArgs e)
        {
            if (_lstWhitelist!.SelectedItem == null)
            {
                MessageBox.Show("Please select a player to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var playerName = _lstWhitelist.SelectedItem.ToString()!;
                _configManager.RemoveFromWhitelist(playerName);
                LoadConfiguration();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing player:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkWhitelistEnabled_CheckedChanged(object? sender, EventArgs e)
        {
            try
            {
                var props = _configManager.LoadServerProperties();
                props.WhitelistEnabled = _chkWhitelistEnabled!.Checked;
                props.EnforceWhitelist = _chkWhitelistEnabled.Checked;
                _configManager.SaveServerProperties(props);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating whitelist:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NumMaxPlayers_ValueChanged(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        private void CmbDifficulty_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        private void CmbGamemode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        private void TxtMOTD_TextChanged(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            try
            {
                var props = _configManager.LoadServerProperties();
                props.MaxPlayers = (int)_numMaxPlayers!.Value;
                props.Difficulty = _cmbDifficulty!.SelectedItem?.ToString() ?? "normal";
                props.Gamemode = _cmbGamemode!.SelectedItem?.ToString() ?? "survival";
                props.MOTD = _txtMOTD!.Text;
                _configManager.SaveServerProperties(props);
            }
            catch (Exception ex)
            {
                // Silently fail for auto-save
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_serverManager.IsRunning)
            {
                var result = MessageBox.Show("Server is still running. Do you want to stop it before closing?", 
                    "Server Running", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _serverManager.StopServer();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            base.OnFormClosing(e);
        }

        private void BtnChangeVersion_Click(object? sender, EventArgs e)
        {
            // Check if server is running
            if (_serverManager.IsRunning)
            {
                var result = MessageBox.Show(
                    "Server is currently running. You must stop the server before changing the version.\n\nDo you want to stop the server now?",
                    "Server Running",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _serverManager.StopServer();
                        
                        // Wait a moment for the process to fully release file handles
                        System.Threading.Thread.Sleep(1000);
                        
                        // Verify server is actually stopped
                        int waitCount = 0;
                        while (_serverManager.IsRunning && waitCount < 10)
                        {
                            System.Threading.Thread.Sleep(500);
                            waitCount++;
                            Application.DoEvents(); // Keep UI responsive
                        }
                        
                        if (_serverManager.IsRunning)
                        {
                            MessageBox.Show(
                                "Server process is still running. Please wait a moment and try again, or manually stop the server process.",
                                "Server Still Running",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                        
                        UpdateUI();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error stopping server:\n\n{ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return; // User cancelled
                }
            }

            // Get current server JAR path from profile
            var currentProfile = _profileManager.CurrentProfile;
            var currentJarPath = currentProfile?.ServerJarPath 
                ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
            var serverDirectory = currentProfile?.ServerDirectory 
                ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
            var profileName = currentProfile?.Name ?? "Default Server";

            // Show change version dialog
            var changeVersionForm = new ChangeVersionForm(currentJarPath, profileName, serverDirectory);
            var result2 = changeVersionForm.ShowDialog();

            if (result2 == DialogResult.OK && changeVersionForm.VersionChanged && !string.IsNullOrEmpty(changeVersionForm.NewServerJarPath))
            {
                try
                {
                    if (changeVersionForm.CreateNewProfile)
                    {
                        // Create new profile with the new version
                        using (var nameDialog = new Form())
                        {
                            nameDialog.Text = "Create New Profile";
                            nameDialog.Size = new Size(400, 200);
                            nameDialog.StartPosition = FormStartPosition.CenterParent;
                            nameDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                            nameDialog.MaximizeBox = false;
                            nameDialog.MinimizeBox = false;

                            var lblName = new Label
                            {
                                Text = "Profile Name:",
                                Location = new Point(20, 20),
                                AutoSize = true
                            };
                            nameDialog.Controls.Add(lblName);

                            var txtName = new TextBox
                            {
                                Location = new Point(20, 45),
                                Size = new Size(340, 23)
                            };
                            nameDialog.Controls.Add(txtName);

                            var lblDesc = new Label
                            {
                                Text = "Description (optional):",
                                Location = new Point(20, 75),
                                AutoSize = true
                            };
                            nameDialog.Controls.Add(lblDesc);

                            var txtDesc = new TextBox
                            {
                                Location = new Point(20, 100),
                                Size = new Size(340, 23)
                            };
                            nameDialog.Controls.Add(txtDesc);

                            var btnOk = new Button
                            {
                                Text = "Create",
                                DialogResult = DialogResult.OK,
                                Location = new Point(200, 130),
                                Size = new Size(75, 30)
                            };
                            nameDialog.Controls.Add(btnOk);
                            nameDialog.AcceptButton = btnOk;

                            var btnCancel = new Button
                            {
                                Text = "Cancel",
                                DialogResult = DialogResult.Cancel,
                                Location = new Point(285, 130),
                                Size = new Size(75, 30)
                            };
                            nameDialog.Controls.Add(btnCancel);
                            nameDialog.CancelButton = btnCancel;

                            if (nameDialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
                            {
                                // Use RequestedVersion (what user selected) first, then DetectedVersion as fallback
                                var newVersion = changeVersionForm.RequestedVersion 
                                    ?? changeVersionForm.DetectedVersion 
                                    ?? "Unknown";
                                var profile = _profileManager.CreateProfile(
                                    txtName.Text.Trim(),
                                    newVersion,
                                    string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim()
                                );

                                // Copy server.jar to new profile directory
                                if (File.Exists(changeVersionForm.NewServerJarPath))
                                {
                                    if (!Directory.Exists(profile.ServerDirectory))
                                    {
                                        Directory.CreateDirectory(profile.ServerDirectory);
                                    }
                                    File.Copy(changeVersionForm.NewServerJarPath, profile.ServerJarPath, true);
                                }

                                MessageBox.Show(
                                    $"New profile '{profile.Name}' created successfully with version {newVersion}!\n\nYou can switch to it from 'Manage Profiles'.",
                                    "Profile Created",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        // Update current profile with new server JAR
                        if (_profileManager.CurrentProfile != null)
                        {
                            var profile = _profileManager.CurrentProfile;
                            
                            // Handle file reset for downgrades
                            if (changeVersionForm.ResetServerFiles && Directory.Exists(profile.ServerDirectory))
                            {
                                try
                                {
                                    // Delete world directories but keep config files
                                    var worldDirs = Directory.GetDirectories(profile.ServerDirectory)
                                        .Where(d => Path.GetFileName(d).StartsWith("world", StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                    
                                    foreach (var worldDir in worldDirs)
                                    {
                                        Directory.Delete(worldDir, true);
                                    }
                                    
                                    // Delete other generated files that might cause issues
                                    var filesToDelete = new[] { "eula.txt", "usercache.json" };
                                    foreach (var fileName in filesToDelete)
                                    {
                                        var filePath = Path.Combine(profile.ServerDirectory, fileName);
                                        if (File.Exists(filePath))
                                        {
                                            File.Delete(filePath);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(
                                        $"Warning: Failed to reset some files:\n\n{ex.Message}\n\nContinuing with version change...",
                                        "Reset Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                }
                            }
                            
                            // Copy new JAR to profile directory
                            if (!Directory.Exists(profile.ServerDirectory))
                            {
                                Directory.CreateDirectory(profile.ServerDirectory);
                            }
                            
                            // Check if target file is locked and wait for it to be released
                            if (File.Exists(profile.ServerJarPath))
                            {
                                // Try to delete the old file with retries
                                int retries = 5;
                                bool deleted = false;
                                while (retries > 0 && !deleted)
                                {
                                    try
                                    {
                                        File.Delete(profile.ServerJarPath);
                                        deleted = true;
                                    }
                                    catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                                    {
                                        retries--;
                                        if (retries > 0)
                                        {
                                            System.Threading.Thread.Sleep(500); // Wait 500ms before retry
                                        }
                                        else
                                        {
                                            throw new Exception(
                                                $"Cannot replace server.jar because it is being used by another process.\n\n" +
                                                $"Please ensure:\n" +
                                                $"1. The server is completely stopped\n" +
                                                $"2. No other programs are using the file\n" +
                                                $"3. Try closing and reopening the application\n\n" +
                                                $"Original error: {ex.Message}");
                                        }
                                    }
                                }
                            }
                            
                            // Validate source file exists before attempting copy
                            // Use a local variable to handle fallback paths
                            string sourceJarPath = changeVersionForm.NewServerJarPath ?? string.Empty;
                            
                            if (string.IsNullOrEmpty(sourceJarPath) || !File.Exists(sourceJarPath))
                            {
                                // Try fallback to global path
                                var globalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
                                if (File.Exists(globalPath))
                                {
                                    System.Diagnostics.Debug.WriteLine($"[MainForm] NewServerJarPath doesn't exist, using global fallback: {globalPath}");
                                    sourceJarPath = globalPath;
                                }
                                else
                                {
                                    throw new Exception(
                                        $"Source file not found.\n\n" +
                                        $"Expected at: {changeVersionForm.NewServerJarPath ?? "null"}\n" +
                                        $"Global path: {globalPath}\n\n" +
                                        $"Please try downloading the version again.");
                                }
                            }
                            
                            // Copy the new file with retry logic
                            int copyRetries = 3;
                            bool copied = false;
                            while (copyRetries > 0 && !copied)
                            {
                                try
                                {
                                    // Ensure target directory exists
                                    var targetDir = Path.GetDirectoryName(profile.ServerJarPath);
                                    if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                                    {
                                        Directory.CreateDirectory(targetDir);
                                    }
                                    
                                    File.Copy(sourceJarPath, profile.ServerJarPath, true);
                                    copied = true;
                                    
                                    // Verify copy succeeded
                                    if (!File.Exists(profile.ServerJarPath))
                                    {
                                        throw new Exception("File copy completed but target file not found");
                                    }
                                }
                                catch (FileNotFoundException ex)
                                {
                                    throw new Exception(
                                        $"Source file not found: {changeVersionForm.NewServerJarPath}\n\n" +
                                        $"Please try downloading the version again.\n\n" +
                                        $"Original error: {ex.Message}");
                                }
                                catch (DirectoryNotFoundException ex)
                                {
                                    throw new Exception(
                                        $"Target directory not found: {Path.GetDirectoryName(profile.ServerJarPath)}\n\n" +
                                        $"Please check your profile configuration.\n\n" +
                                        $"Original error: {ex.Message}");
                                }
                                catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                                {
                                    copyRetries--;
                                    if (copyRetries > 0)
                                    {
                                        System.Threading.Thread.Sleep(500); // Wait 500ms before retry
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            $"Cannot copy server.jar because the target file is locked.\n\n" +
                                            $"Please ensure the server is completely stopped and try again.\n\n" +
                                            $"Original error: {ex.Message}");
                                    }
                                }
                            }
                            
                            // Update profile version - use RequestedVersion (what user selected) first
                            profile.Version = changeVersionForm.RequestedVersion 
                                ?? changeVersionForm.DetectedVersion 
                                ?? "Unknown";
                            profile.LastModifiedDate = DateTime.Now;
                            _profileManager.UpdateProfile(profile);
                            
                            // Update ServerManager and ConfigManager
                            _serverManager.SetProfile(profile);
                            _configManager.SetProfile(profile);
                            
                            var successMessage = $"Server version changed successfully to {profile.Version}!";
                            if (!string.IsNullOrEmpty(changeVersionForm.BackupPath))
                            {
                                successMessage += $"\n\n✓ Backup created at: {changeVersionForm.BackupPath}";
                            }
                            if (changeVersionForm.ResetServerFiles)
                            {
                                successMessage += "\n\n⚠ Server files have been reset. Your worlds have been deleted to prevent corruption.";
                            }
                            else
                            {
                                successMessage += "\n\n⚠ WARNING: Your existing world may have compatibility issues.";
                            }
                            successMessage += "\n\nYou must restart the server for changes to take effect.";
                            
                            MessageBox.Show(
                                successMessage,
                                "Version Changed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Fallback for legacy mode
                            var serverDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
                            if (!Directory.Exists(serverDir))
                            {
                                Directory.CreateDirectory(serverDir);
                            }
                            File.Copy(changeVersionForm.NewServerJarPath, Path.Combine(serverDir, "server.jar"), true);
                            
                            var legacyMessage = "Server version changed successfully!";
                            if (!string.IsNullOrEmpty(changeVersionForm.BackupPath))
                            {
                                legacyMessage += $"\n\n✓ Backup created at: {changeVersionForm.BackupPath}";
                            }
                            legacyMessage += "\n\nYou must restart the server for changes to take effect.";
                            
                            MessageBox.Show(
                                legacyMessage,
                                "Version Changed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error changing server version:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void BtnManageProfiles_Click(object? sender, EventArgs e)
        {
            // Check if server is running
            if (_serverManager.IsRunning)
            {
                var result = MessageBox.Show(
                    "Server is currently running. You must stop the server before managing profiles.\n\nDo you want to stop the server now?",
                    "Server Running",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _serverManager.StopServer();
                        UpdateUI();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error stopping server:\n\n{ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return; // User cancelled
                }
            }

            // Show profile management form
            var profileForm = new ServerProfileForm(_profileManager);
            var result2 = profileForm.ShowDialog();

            if (result2 == DialogResult.OK && profileForm.SelectedProfile != null)
            {
                // Profile was switched - update everything
                var newProfile = profileForm.SelectedProfile;
                _profileManager.SetCurrentProfile(newProfile.Id);
                _serverManager.SetProfile(newProfile);
                _configManager.SetProfile(newProfile);
                _lblCurrentProfile!.Text = $"Profile: {newProfile.Name} (v{newProfile.Version})";
                LoadConfiguration();
                UpdateUI();
                
                MessageBox.Show(
                    $"Switched to profile: {newProfile.Name}\n\nVersion: {newProfile.Version}",
                    "Profile Switched",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                // Profile list may have changed, refresh current profile display
                UpdateProfileLabel();
            }
        }

        private void UpdateProfileLabel()
        {
            if (_lblCurrentProfile != null && _profileManager.CurrentProfile != null)
            {
                var profile = _profileManager.CurrentProfile;
                _lblCurrentProfile.Text = $"Profile: {profile.Name} (v{profile.Version})";
            }
        }

        private async Task DownloadJavaAsync(int version, string? mcVersion = null)
        {
            var downloadForm = new Form
            {
                Text = "Downloading Java",
                Size = new Size(500, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblStatus = new Label
            {
                Text = $"Downloading Java {version}...",
                Location = new Point(20, 20),
                Size = new Size(460, 20),
                AutoSize = false
            };
            downloadForm.Controls.Add(lblStatus);

            var progressBar = new ProgressBar
            {
                Location = new Point(20, 50),
                Size = new Size(460, 23),
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100
            };
            downloadForm.Controls.Add(progressBar);

            var javaManager = new JavaManager();
            bool downloadComplete = false;
            Exception? downloadError = null;

            var progress = new Progress<DownloadProgress>(p =>
            {
                if (downloadForm.IsDisposed || !downloadForm.IsHandleCreated)
                    return;

                lblStatus.Text = p.Status;
                progressBar.Value = p.Progress;
                Application.DoEvents();
            });

            // Start download in background
            _ = Task.Run(async () =>
            {
                try
                {
                    await javaManager.DownloadAndInstallJavaAsync(version, progress);
                    downloadComplete = true;
                }
                catch (Exception ex)
                {
                    downloadError = ex;
                }
                finally
                {
                    if (downloadForm.IsHandleCreated)
                    {
                        downloadForm.Invoke(new Action(() => downloadForm.Close()));
                    }
                }
            });

            downloadForm.ShowDialog(this);

            if (downloadError != null)
            {
                MessageBox.Show(
                    $"Error downloading Java:\n\n{downloadError.Message}\n\n" +
                    "Please install Java manually from:\nhttps://adoptium.net/",
                    "Download Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (downloadComplete)
            {
                MessageBox.Show(
                    $"Java {version} has been installed successfully!\n\n" +
                    "The application will now use the bundled Java runtime.",
                    "Installation Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}

