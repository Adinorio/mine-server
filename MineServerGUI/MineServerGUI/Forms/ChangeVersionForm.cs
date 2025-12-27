using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MineServerGUI.Core;
using MineServerGUI.Utilities;

namespace MineServerGUI.Forms
{
    public partial class ChangeVersionForm : Form
    {
        private readonly string _currentServerJarPath;
        private Label? _lblStatus;
        private ProgressBar? _progressBar;
        private Button? _btnDownload;
        private Button? _btnSelectFile;
        private Button? _btnCancel;
        private Button? _btnApply;
        private ComboBox? _cmbVersion;
        private Label? _lblVersion;
        private Label? _lblCurrentVersion;
        private Label? _lblSelectedFile;
        private bool _isDownloading = false;
        private List<string> _availableVersions = new List<string>();
        private readonly ServerDownloader _downloader;
        private string? _requestedVersion;

        public bool VersionChanged { get; private set; }
        public bool CreateNewProfile { get; private set; }
        public bool ResetServerFiles { get; private set; }
        public string? NewServerJarPath { get; private set; }
        public string? DetectedVersion { get; private set; }
        public string? RequestedVersion { get; private set; }
        public string? BackupPath { get; private set; }

        private readonly string? _profileName;
        private readonly string? _serverDirectory;

        public ChangeVersionForm(string currentServerJarPath, string? profileName = null, string? serverDirectory = null)
        {
            _currentServerJarPath = currentServerJarPath;
            _profileName = profileName ?? "Default Server";
            _serverDirectory = serverDirectory;
            _downloader = new ServerDownloader();
            InitializeComponent();
            // Subscribe to events AFTER InitializeComponent to ensure all controls exist
            _downloader.DownloadProgress += Downloader_DownloadProgress;
            LoadCurrentVersion();
        }

        private void InitializeComponent()
        {
            this.Text = "Change Server Version";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Padding = new Padding(20);

            // Title
            var lblTitle = new Label
            {
                Text = "Change Server Version",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Current Version Label
            _lblCurrentVersion = new Label
            {
                Text = "Current Version: Detecting...",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 55),
                AutoSize = true
            };
            this.Controls.Add(_lblCurrentVersion);

            // Option 1: Select File Section
            var selectFilePanel = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(550, 90),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(selectFilePanel);

            var lblSelectFile = new Label
            {
                Text = "Use Existing Server.jar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            selectFilePanel.Controls.Add(lblSelectFile);

            var lblSelectFileDesc = new Label
            {
                Text = "If you already have a server.jar file on your computer:",
                Location = new Point(10, 30),
                Size = new Size(350, 20),
                AutoSize = true
            };
            selectFilePanel.Controls.Add(lblSelectFileDesc);

            _btnSelectFile = new Button
            {
                Text = "Browse",
                Location = new Point(450, 50),
                Size = new Size(85, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _btnSelectFile.FlatAppearance.BorderSize = 0;
            _btnSelectFile.Click += BtnSelectFile_Click;
            selectFilePanel.Controls.Add(_btnSelectFile);

            // Option 2: Download Section
            var downloadPanel = new Panel
            {
                Location = new Point(20, 195),
                Size = new Size(550, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(downloadPanel);

            var lblDownload = new Label
            {
                Text = "Download Server.jar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            downloadPanel.Controls.Add(lblDownload);

            var lblDownloadDesc = new Label
            {
                Text = "Download an official Minecraft server JAR file:",
                Location = new Point(10, 30),
                Size = new Size(350, 20),
                AutoSize = true
            };
            downloadPanel.Controls.Add(lblDownloadDesc);

            _lblVersion = new Label
            {
                Text = "Version:",
                Location = new Point(10, 55),
                Size = new Size(60, 20),
                AutoSize = true
            };
            downloadPanel.Controls.Add(_lblVersion);

            _cmbVersion = new ComboBox
            {
                Location = new Point(70, 53),
                Size = new Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            downloadPanel.Controls.Add(_cmbVersion);

            _btnDownload = new Button
            {
                Text = "Download",
                Location = new Point(450, 55),
                Size = new Size(85, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _btnDownload.FlatAppearance.BorderSize = 0;
            _btnDownload.Click += BtnDownload_Click;
            downloadPanel.Controls.Add(_btnDownload);

            _progressBar = new ProgressBar
            {
                Location = new Point(10, 95),
                Size = new Size(515, 25),
                Visible = false
            };
            downloadPanel.Controls.Add(_progressBar);

            // Status and Selected File Labels
            _lblSelectedFile = new Label
            {
                Text = "",
                Location = new Point(20, 340),
                Size = new Size(550, 20),
                AutoSize = true,
                Visible = false,
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(_lblSelectedFile);

            _lblStatus = new Label
            {
                Text = "Choose an option above to change the server version",
                Location = new Point(20, 365),
                Size = new Size(550, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(_lblStatus);

            // Buttons Section - Better organized
            var buttonsPanel = new Panel
            {
                Location = new Point(20, 400),
                Size = new Size(550, 60),
                BackColor = Color.Transparent
            };
            this.Controls.Add(buttonsPanel);

            // Primary action: Create New Profile (recommended) - Larger, more prominent
            var btnCreateProfile = new Button
            {
                Text = "Create New Profile",
                Location = new Point(0, 0),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false,
                Name = "btnCreateProfile" // Add name for easier finding
            };
            btnCreateProfile.FlatAppearance.BorderSize = 0;
            btnCreateProfile.Click += (s, e) =>
            {
                CreateNewProfile = true;
                BtnApply_Click(s, e);
            };
            buttonsPanel.Controls.Add(btnCreateProfile);

            // Secondary action: Apply to Current Profile (risky) - Smaller, less prominent
            _btnApply = new Button
            {
                Text = "Apply to Current Profile",
                Location = new Point(220, 0),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false
            };
            _btnApply.FlatAppearance.BorderSize = 0;
            _btnApply.Click += BtnApply_Click;
            buttonsPanel.Controls.Add(_btnApply);

            // Cancel button - Right aligned
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(470, 0),
                Size = new Size(80, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 9F)
            };
            _btnCancel.FlatAppearance.BorderSize = 1;
            _btnCancel.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            _btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            buttonsPanel.Controls.Add(_btnCancel);

            // Load available versions
            LoadAvailableVersions();
        }

        private async void LoadCurrentVersion()
        {
            if (string.IsNullOrEmpty(_currentServerJarPath) || !File.Exists(_currentServerJarPath))
            {
                _lblCurrentVersion!.Text = "Current Version: Unknown (No server.jar found)";
                return;
            }

            _lblCurrentVersion!.Text = "Current Version: Detecting...";
            
            try
            {
                var version = await Task.Run(() => ServerVersionDetector.DetectVersion(_currentServerJarPath));
                if (!string.IsNullOrEmpty(version))
                {
                    _lblCurrentVersion.Text = $"Current Version: {version}";
                }
                else
                {
                    _lblCurrentVersion.Text = "Current Version: Unknown";
                }
            }
            catch
            {
                _lblCurrentVersion.Text = "Current Version: Unknown";
            }
        }

        private void EnableCreateProfileButton()
        {
            var createProfileBtn = this.Controls.OfType<Panel>()
                .SelectMany(p => p.Controls.OfType<Button>())
                .FirstOrDefault(b => b.Text == "Create New Profile" || b.Name == "btnCreateProfile");
            if (createProfileBtn != null)
            {
                createProfileBtn.Enabled = true;
            }
        }

        private async void LoadAvailableVersions()
        {
            try
            {
                _availableVersions = await _downloader.GetAvailableVersionsAsync();
                _cmbVersion!.Items.Clear();
                foreach (var version in _availableVersions)
                {
                    _cmbVersion.Items.Add(version);
                }
                if (_availableVersions.Count > 0)
                {
                    _cmbVersion.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _lblStatus!.Text = $"Error loading versions: {ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }

        private async void BtnSelectFile_Click(object? sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Minecraft Server|server.jar|JAR Files|*.jar|All Files|*.*",
                Title = "Select Minecraft Server JAR File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = Path.GetFileName(openFileDialog.FileName);
                _lblSelectedFile!.Text = $"Selected: {fileName}";
                _lblSelectedFile.Visible = true;
                _lblStatus!.Text = "Detecting version...";
                _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                _btnSelectFile!.Enabled = false;

                string? detectedVersion = null;
                string? detectionError = null;

                try
                {
                    var detectionTask = Task.Run(() =>
                        ServerVersionDetector.DetectVersion(openFileDialog.FileName)
                    );

                    if (detectionTask.Wait(TimeSpan.FromSeconds(3)))
                    {
                        detectedVersion = detectionTask.Result;
                    }
                    else
                    {
                        detectionError = "Detection timed out";
                    }
                }
                catch (Exception ex)
                {
                    detectionError = $"Detection error: {ex.Message}";
                }

                if (!string.IsNullOrEmpty(detectedVersion))
                {
                    DetectedVersion = detectedVersion;
                    RequestedVersion = detectedVersion; // For manually selected files, use detected version
                    _lblSelectedFile.Text = $"✓ Selected: {fileName} (v{detectedVersion})";
                    _lblStatus.Text = $"✓ Version detected: {detectedVersion}. Click Apply to use this file.";
                    _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                    NewServerJarPath = openFileDialog.FileName;
                    _btnApply!.Enabled = true;
                    EnableCreateProfileButton();
                }
                else
                {
                    _lblStatus.Text = "⚠ Could not detect version automatically";
                    _lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
                    var result = MessageBox.Show(
                        "Could not detect server version automatically.\n\nDo you want to use this file anyway?",
                        "Version Detection Failed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        NewServerJarPath = openFileDialog.FileName;
                        // Try to detect version one more time, or use filename
                        try
                        {
                            var detected = ServerVersionDetector.DetectVersion(openFileDialog.FileName);
                            if (!string.IsNullOrEmpty(detected))
                            {
                                DetectedVersion = detected;
                                RequestedVersion = detected;
                            }
                        }
                        catch { }
                        _btnApply!.Enabled = true;
                        EnableCreateProfileButton();
                        _lblStatus.Text = "Ready to apply. Click Apply to use this file.";
                        _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                    }
                }

                _btnSelectFile.Enabled = true;
            }
        }

        private async void BtnDownload_Click(object? sender, EventArgs e)
        {
            if (_isDownloading || _cmbVersion!.SelectedItem == null)
                return;

            // Ensure form handle is created before starting download
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            _isDownloading = true;
            _btnDownload!.Enabled = false;
            _btnSelectFile!.Enabled = false;
            _cmbVersion!.Enabled = false;
            _btnCancel!.Enabled = false;
            
            // Make sure progress bar is visible and reset
            _progressBar!.Visible = true;
            _progressBar!.Value = 0;
            _progressBar!.Style = ProgressBarStyle.Continuous;
            _progressBar!.Minimum = 0;
            _progressBar!.Maximum = 100;
            
            // Force immediate UI update
            _lblStatus!.Text = "Preparing download...";
            _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            Application.DoEvents(); // Force UI update to show progress bar
            Refresh(); // Force form repaint

            try
            {
                var selectedVersion = _cmbVersion.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedVersion))
                {
                    throw new Exception("No version selected");
                }

                // Store the version BEFORE downloading - this is what the user explicitly selected
                _requestedVersion = selectedVersion.Trim();
                RequestedVersion = _requestedVersion; // Store for external access
                DetectedVersion = _requestedVersion; // Set to selected version (will be used for profile)
                
                // Debug: Verify version is being passed correctly
                System.Diagnostics.Debug.WriteLine($"[ChangeVersionForm] User selected version: '{_requestedVersion}'");
                
                _lblStatus!.Text = $"Downloading version {_requestedVersion}...";
                _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);

                // Always overwrite when downloading a specific version
                // Pass the exact version the user selected - no default fallback
                System.Diagnostics.Debug.WriteLine($"[ChangeVersionForm] Calling DownloadServerJarAsync with version='{_requestedVersion}', overwrite=true");
                await _downloader.DownloadServerJarAsync(_requestedVersion, overwrite: true);

                if (IsDisposed || !IsHandleCreated)
                    return;

                // Use profile-specific directory if provided, otherwise use global server directory
                var downloadedPath = !string.IsNullOrEmpty(_serverDirectory)
                    ? Path.Combine(_serverDirectory, "server.jar")  // Profile-specific path
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");  // Global fallback
                
                // If using profile directory, ensure it exists
                if (!string.IsNullOrEmpty(_serverDirectory) && !Directory.Exists(_serverDirectory))
                {
                    Directory.CreateDirectory(_serverDirectory);
                }
                
                // ServerDownloader always downloads to global path
                var globalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
                
                // Verify file exists at global location (where it was downloaded)
                if (!File.Exists(globalPath))
                {
                    throw new Exception($"Downloaded file not found at global location: {globalPath}\n\nPlease try downloading again.");
                }
                
                // If we need profile-specific path, copy from global to profile directory
                string? finalPath = null;
                if (downloadedPath != globalPath && !string.IsNullOrEmpty(_serverDirectory))
                {
                    // Ensure profile directory exists
                    if (!Directory.Exists(_serverDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(_serverDirectory);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to create profile directory: {_serverDirectory}\n\nError: {ex.Message}");
                        }
                    }
                    
                    // Copy from global to profile directory with retry logic
                    int copyRetries = 5;
                    bool copied = false;
                    while (copyRetries > 0 && !copied)
                    {
                        try
                        {
                            File.Copy(globalPath, downloadedPath, overwrite: true);
                            copied = true;
                            finalPath = downloadedPath;
                            System.Diagnostics.Debug.WriteLine($"[ChangeVersionForm] Successfully copied to profile directory: {downloadedPath}");
                        }
                        catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                        {
                            copyRetries--;
                            if (copyRetries > 0)
                            {
                                System.Threading.Thread.Sleep(500);
                            }
                            else
                            {
                                // Copy failed, use global path as fallback
                                System.Diagnostics.Debug.WriteLine($"[ChangeVersionForm] Failed to copy to profile directory after retries, using global path");
                                finalPath = globalPath;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ChangeVersionForm] Error copying to profile directory: {ex.Message}");
                            // Use global path as fallback
                            finalPath = globalPath;
                            break;
                        }
                    }
                }
                else
                {
                    // Using global path
                    finalPath = globalPath;
                }
                
                // Verify final path exists
                if (string.IsNullOrEmpty(finalPath) || !File.Exists(finalPath))
                {
                    throw new Exception($"File not found at final path: {finalPath ?? "null"}\n\nGlobal path: {globalPath}\nProfile path: {downloadedPath}");
                }
                
                // Detect version to verify it matches
                string? detectedVersion = null;
                try
                {
                    detectedVersion = ServerVersionDetector.DetectVersion(finalPath);
                }
                catch { }

                // Use the requested version (what user selected), not detected version
                DetectedVersion = _requestedVersion; // ✅ Use stored _requestedVersion, not local selectedVersion
                NewServerJarPath = finalPath; // Use the verified path
                
                // Check if this was from cache (status message will contain "cache" or "cached")
                var wasFromCache = _lblStatus!.Text.Contains("cache", StringComparison.OrdinalIgnoreCase) ||
                                   _lblStatus.Text.Contains("cached", StringComparison.OrdinalIgnoreCase);
                
                // Show both requested and detected (if different) for transparency
                if (!string.IsNullOrEmpty(detectedVersion) && detectedVersion != selectedVersion)
                {
                    if (wasFromCache)
                    {
                        _lblSelectedFile!.Text = $"✓ Using cached: server.jar (Requested: v{selectedVersion}, Detected: v{detectedVersion})";
                        _lblStatus!.Text = $"✓ Using cached version {selectedVersion}! Note: Detected version differs from requested. Click Apply to use.";
                    }
                    else
                    {
                        _lblSelectedFile!.Text = $"✓ Downloaded: server.jar (Requested: v{selectedVersion}, Detected: v{detectedVersion})";
                        _lblStatus!.Text = $"✓ Download complete! Note: Detected version differs from requested. Click Apply to use.";
                    }
                }
                else
                {
                    if (wasFromCache)
                    {
                        _lblSelectedFile!.Text = $"✓ Using cached: server.jar (v{selectedVersion})";
                        _lblStatus!.Text = $"✓ Using cached version {selectedVersion}! No download needed. Click Apply to use.";
                    }
                    else
                    {
                        _lblSelectedFile!.Text = $"✓ Downloaded: server.jar (v{selectedVersion})";
                        _lblStatus!.Text = $"✓ Server JAR downloaded successfully! Click Apply to use version {selectedVersion}.";
                    }
                }
                
                _lblSelectedFile.Visible = true;
                _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                _btnApply!.Enabled = true;
                EnableCreateProfileButton();
                _btnSelectFile.Enabled = false;
                _btnDownload.Enabled = false;
                _cmbVersion.Enabled = false;
                _btnCancel.Enabled = true;
                _progressBar.Visible = false;
            }
            catch (Exception ex)
            {
                _lblStatus!.Text = $"Error: {ex.Message}";
                _lblStatus.ForeColor = Color.Red;
                MessageBox.Show(
                    $"Error downloading server JAR:\n\n{ex.Message}",
                    "Download Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _btnDownload.Enabled = true;
                _btnSelectFile.Enabled = true;
                _cmbVersion!.Enabled = true;
                _btnCancel.Enabled = true;
                _progressBar!.Visible = false;
            }
            finally
            {
                _isDownloading = false;
            }
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NewServerJarPath) || !File.Exists(NewServerJarPath))
            {
                MessageBox.Show(
                    "No server JAR file selected or file not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Get current version from label
            var currentVersionText = _lblCurrentVersion?.Text ?? "Unknown";
            var currentVersion = currentVersionText.Replace("Current Version: ", "").Trim();
            var newVersion = _requestedVersion ?? DetectedVersion ?? "Unknown";

            // Check Java version compatibility
            var requiredJava = VersionHelper.GetRequiredJavaVersion(newVersion);
            var javaVersion = JavaVersionChecker.GetJavaMajorVersion();
            var javaCompatible = JavaVersionChecker.IsJavaVersionCompatible(javaVersion, requiredJava);

            if (!javaCompatible && javaVersion > 0)
            {
                var javaWarning = $"⚠ Java Version Incompatibility\n\n" +
                                $"Minecraft {newVersion} requires: {requiredJava}\n" +
                                $"Your Java version: {javaVersion}\n\n" +
                                $"The server may not start correctly with this Java version.\n\n" +
                                $"Do you want to continue anyway?";
                
                var javaResult = MessageBox.Show(
                    javaWarning,
                    "Java Version Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                
                if (javaResult == DialogResult.No)
                {
                    return;
                }
            }

            // Check for major version jumps
            var versionJump = VersionHelper.GetMajorVersionJump(currentVersion, newVersion);
            var isUpgrade = VersionHelper.IsUpgrade(currentVersion, newVersion);
            var isDowngrade = VersionHelper.IsDowngrade(currentVersion, newVersion);

            // If creating new profile, show backup prompt but no risky warnings
            if (CreateNewProfile)
            {
                // Offer backup even for new profiles (in case user wants to copy world)
                var backupPrompt = "Create backup before proceeding?\n\n" +
                                 "This will backup your current server files and worlds.\n" +
                                 "Recommended if you plan to copy worlds to the new profile.";
                
                var backupResult = MessageBox.Show(
                    backupPrompt,
                    "Create Backup?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (backupResult == DialogResult.Yes && !string.IsNullOrEmpty(_serverDirectory))
                {
                    try
                    {
                        var backupManager = new ServerBackupManager(_serverDirectory);
                        BackupPath = backupManager.CreateBackup(_serverDirectory, _profileName!, currentVersion);
                        MessageBox.Show(
                            $"Backup created successfully!\n\nLocation: {BackupPath}",
                            "Backup Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show(
                            $"Failed to create backup:\n\n{ex.Message}\n\nContinue anyway?",
                            "Backup Failed",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                VersionChanged = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // For current profile changes, show comprehensive warnings
            var warningMessage = "⚠ WARNING: Changing Server Version on Current Profile\n\n";

            // Add version jump warning
            if (versionJump >= 2)
            {
                warningMessage += $"⚠ MAJOR VERSION JUMP DETECTED\n" +
                                $"You are jumping {versionJump} version(s) from {currentVersion} to {newVersion}.\n" +
                                $"This is EXTREMELY RISKY and may cause severe world corruption!\n\n";
            }

            // Add upgrade/downgrade specific warnings
            if (isDowngrade)
            {
                warningMessage += "⚠ DOWNGRADE DETECTED\n" +
                                "Downgrading is VERY RISKY and often requires resetting server files.\n" +
                                "Your world will likely become incompatible and may corrupt.\n\n";
            }
            else if (isUpgrade)
            {
                warningMessage += "✓ Upgrade detected (safer than downgrade, but still risky)\n\n";
            }

            warningMessage += "⚠ RISKS:\n" +
                            "• Your existing world may become incompatible\n" +
                            "• World corruption is possible, especially when downgrading\n" +
                            "• Player data might be lost or corrupted\n" +
                            "• Plugins/mods may stop working\n\n";

            // Add Java version info
            if (!javaCompatible)
            {
                warningMessage += $"⚠ Java Version: {requiredJava} required, but you have Java {javaVersion}\n\n";
            }

            warningMessage += "💡 RECOMMENDED: Create a New Profile Instead\n" +
                            "• Keeps your current profile safe\n" +
                            "• Allows testing the new version separately\n" +
                            "• You can switch between versions easily\n\n" +
                            $"You are changing from version {currentVersion} to version {newVersion}.\n\n" +
                            "Options:\n" +
                            "• Yes = Continue with current profile (RISKY)\n" +
                            "• No = Create new profile instead (SAFER)\n" +
                            "• Cancel = Don't change anything";

            var result = MessageBox.Show(
                warningMessage,
                "Change Version Warning",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);
            
            if (result == DialogResult.Cancel)
            {
                return; // User cancelled
            }
            
            if (result == DialogResult.No)
            {
                // User wants to create new profile instead
                CreateNewProfile = true;
                VersionChanged = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // User confirmed - offer backup
            var backupPrompt2 = "⚠ IMPORTANT: Create Backup Before Changing Version?\n\n" +
                              "This will backup your server files and worlds.\n" +
                              "STRONGLY RECOMMENDED before changing versions!";
            
            var backupResult2 = MessageBox.Show(
                backupPrompt2,
                "Create Backup?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (backupResult2 == DialogResult.Yes && !string.IsNullOrEmpty(_serverDirectory))
            {
                try
                {
                    var backupManager = new ServerBackupManager(_serverDirectory);
                    BackupPath = backupManager.CreateBackup(_serverDirectory, _profileName!, currentVersion);
                    MessageBox.Show(
                        $"Backup created successfully!\n\nLocation: {BackupPath}",
                        "Backup Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    var continueResult = MessageBox.Show(
                        $"Failed to create backup:\n\n{ex.Message}\n\nContinue anyway?",
                        "Backup Failed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (continueResult == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            // For downgrades, offer to reset server files
            if (isDowngrade)
            {
                var resetPrompt = "⚠ DOWNGRADE DETECTED\n\n" +
                                "Downgrading usually requires resetting server files to prevent corruption.\n\n" +
                                "Would you like to reset server files?\n\n" +
                                "• Yes = Reset files (worlds will be deleted, but prevents corruption)\n" +
                                "• No = Keep files (RISKY - may cause corruption)\n" +
                                "• Cancel = Don't change version";
                
                var resetResult = MessageBox.Show(
                    resetPrompt,
                    "Reset Server Files?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                
                if (resetResult == DialogResult.Cancel)
                {
                    return;
                }
                
                ResetServerFiles = resetResult == DialogResult.Yes;
            }

            // User confirmed - proceed with changing current profile
            VersionChanged = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Downloader_DownloadProgress(object? sender, DownloadProgressEventArgs e)
        {
            // Early exit if form is disposed
            if (IsDisposed)
                return;

            // Marshal to UI thread if needed
            if (InvokeRequired)
            {
                try
                {
                    if (IsHandleCreated)
                    {
                        // Use BeginInvoke to queue update without blocking download thread
                        // The UI thread will process these messages asynchronously
                        BeginInvoke(new Action(() => Downloader_DownloadProgress(sender, e)));
                    }
                }
                catch (Exception)
                {
                    // Form is being disposed, handle not created, or other exception during invocation
                    // Silently ignore - form may be disposed
                    return;
                }
                return;
            }

            // Update UI on the UI thread
            try
            {
                if (_lblStatus != null && !_lblStatus.IsDisposed && IsHandleCreated)
                {
                    _lblStatus.Text = e.Status;
                    _lblStatus.Update(); // Force immediate repaint without invalidating parent
                }
                
                if (_progressBar != null && !_progressBar.IsDisposed && IsHandleCreated)
                {
                    var progressValue = Math.Min(100, Math.Max(0, e.Progress));
                    
                    // Only update if value actually changed to reduce unnecessary repaints
                    if (_progressBar.Value != progressValue)
                    {
                        _progressBar.Value = progressValue;
                        _progressBar.Update(); // Force immediate repaint without invalidating parent
                    }
                    
                    // Ensure progress bar stays visible during download
                    if (!_progressBar.Visible && progressValue > 0 && progressValue < 100)
                    {
                        _progressBar.Visible = true;
                    }
                }
            }
            catch (Exception)
            {
                // Control was disposed during update (InvalidOperationException, ObjectDisposedException, etc.)
                // Silently ignore - form/controls may be disposed
            }
        }
    }
}

