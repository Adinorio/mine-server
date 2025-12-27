using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MineServerGUI.Core;

namespace MineServerGUI.Forms
{
    public partial class SetupWizardForm : Form
    {
        private readonly ServerDownloader _downloader;
        private Label? _lblStatus;
        private ProgressBar? _progressBar;
        private Button? _btnDownload;
        private Button? _btnSelectFile;
        private Button? _btnCancel;
        private Button? _btnNext;
        private ComboBox? _cmbVersion;
        private Label? _lblVersion;
        private Label? _lblSelectedFile;
        private bool _isDownloading = false;
        private List<string> _availableVersions = new List<string>();

        public bool SetupComplete { get; private set; }
        public string? SelectedServerJarPath { get; private set; }
        public string? NewProfileName { get; private set; }
        public string? NewProfileDescription { get; private set; }
        public string? DetectedVersion { get; private set; }
        public string? RequestedVersion { get; private set; }  // User's explicit selection
        private readonly bool _isCreatingProfile;

        public SetupWizardForm(bool isCreatingProfile = false)
        {
            _isCreatingProfile = isCreatingProfile;
            _downloader = new ServerDownloader();
            InitializeComponent();
            
            if (_isCreatingProfile)
            {
                this.Text = "Create New Server Profile";
            }
        }

        private void InitializeComponent()
        {
            this.Text = "MineServer Setup Wizard";
            this.Size = new Size(550, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Padding = new Padding(0, 0, 0, 0);
            
            // Make form border more visible
            this.Paint += (s, e) =>
            {
                var rect = this.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(120, 120, 120), 2))
                {
                    e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }
            };

            // Main Content Panel
            var contentPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(550, 380),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(contentPanel);

            // Welcome Text (moved from top panel)
            // Show different text when creating a profile vs initial setup
            var lblTitle = new Label
            {
                Text = _isCreatingProfile ? "Create New Server Profile" : "Welcome to MineServer GUI",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblTitle);

            var lblSubtitle = new Label
            {
                Text = _isCreatingProfile 
                    ? "Select or download a Minecraft Server JAR file for this profile"
                    : "Minecraft Server JAR file is required to run the server",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(25, 50),
                AutoSize = true,
                UseMnemonic = false
            };
            contentPanel.Controls.Add(lblSubtitle);

            // Option 1: Select File Section
            var selectFilePanel = new Panel
            {
                Location = new Point(25, 80),
                Size = new Size(500, 90),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(selectFilePanel);

            var lblSelectFile = new Label
            {
                Text = "Use Existing Server.jar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 10),
                Size = new Size(300, 20),
                AutoSize = false
            };
            selectFilePanel.Controls.Add(lblSelectFile);

            var lblSelectFileDesc = new Label
            {
                Text = "If you already have a server.jar file on your computer:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(15, 32),
                Size = new Size(320, 15),
                AutoSize = true
            };
            selectFilePanel.Controls.Add(lblSelectFileDesc);

            // Selected file label (initially hidden)
            _lblSelectedFile = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(15, 55),
                Size = new Size(320, 20),
                AutoSize = false,
                Visible = false
            };
            selectFilePanel.Controls.Add(_lblSelectedFile);

            _btnSelectFile = new Button
            {
                Text = "Browse",
                Location = new Point(410, 32),
                Size = new Size(75, 28),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnSelectFile.FlatAppearance.BorderSize = 0;
            _btnSelectFile.Click += BtnSelectFile_Click;
            selectFilePanel.Controls.Add(_btnSelectFile);

            // Option 2: Download Section
            var downloadPanel = new Panel
            {
                Location = new Point(25, 175),
                Size = new Size(500, 105),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(downloadPanel);

            var lblDownload = new Label
            {
                Text = "Download Server.jar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 10),
                Size = new Size(350, 20),
                AutoSize = true
            };
            downloadPanel.Controls.Add(lblDownload);

            var lblDownloadDesc = new Label
            {
                Text = "Download an official Minecraft server version:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(15, 35),
                Size = new Size(320, 15),
                AutoSize = true
            };
            downloadPanel.Controls.Add(lblDownloadDesc);

            _lblVersion = new Label
            {
                Text = "Version:",
                Location = new Point(15, 60),
                Size = new Size(55, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            downloadPanel.Controls.Add(_lblVersion);

            _cmbVersion = new ComboBox
            {
                Location = new Point(75, 60),
                Size = new Size(220, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            _cmbVersion.Items.Add("Loading versions...");
            _cmbVersion.SelectedIndex = 0;
            _cmbVersion.Enabled = false;
            downloadPanel.Controls.Add(_cmbVersion);

            _btnDownload = new Button
            {
                Text = "Download",
                Location = new Point(410, 60),
                Size = new Size(75, 28),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnDownload.FlatAppearance.BorderSize = 0;
            _btnDownload.Click += BtnDownload_Click;
            downloadPanel.Controls.Add(_btnDownload);

            // Status label (more visible)
            _lblStatus = new Label
            {
                Text = "Choose an option above to continue",
                Location = new Point(25, 290),
                Size = new Size(320, 20),
                AutoSize = false,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
            contentPanel.Controls.Add(_lblStatus);

            // Progress bar
            _progressBar = new ProgressBar
            {
                Location = new Point(25, 325),
                Size = new Size(500, 20),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };
            contentPanel.Controls.Add(_progressBar);

            // Cancel button (disabled initially - only enabled when file/download is selected)
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(440, 288),
                Size = new Size(70, 32),
                FlatStyle = FlatStyle.Flat,
                Enabled = false
                // Don't set DialogResult here - let CancelSetup handle it
            };
            _btnCancel.Click += (s, e) => { CancelSetup(); };
            contentPanel.Controls.Add(_btnCancel);

            // Next button (initially hidden)
            _btnNext = new Button
            {
                Text = "Next",
                Location = new Point(360, 288),
                Size = new Size(70, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Visible = false,
                Enabled = false
            };
            _btnNext.FlatAppearance.BorderSize = 0;
            _btnNext.Click += BtnNext_Click;
            contentPanel.Controls.Add(_btnNext);

            // Handle window close (X button)
            this.FormClosing += SetupWizardForm_FormClosing;

            // Load available versions
            LoadAvailableVersions();

            // Subscribe to download progress
            _downloader.DownloadProgress += Downloader_DownloadProgress;
        }

        private void SetupWizardForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // If user is closing without completing setup, exit entire application
            if (!SetupComplete && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "Setup is not complete. Exit the application?",
                    "Exit Setup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Exit entire application
                    Application.Exit();
                }
                else
                {
                    // Cancel closing
                    e.Cancel = true;
                }
            }
        }

        private void CancelSetup()
        {
            // Reset the form to allow user to select a different file or version
            SelectedServerJarPath = null;
            DetectedVersion = null;
            RequestedVersion = null;
            
            // Hide selected file label
            if (_lblSelectedFile != null)
            {
                _lblSelectedFile.Visible = false;
                _lblSelectedFile.Text = "";
            }
            
            // Reset status message
            if (_lblStatus != null)
            {
                _lblStatus.Text = "Choose an option above to continue";
                _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            }
            
            // Hide progress bar
            if (_progressBar != null)
            {
                _progressBar.Visible = false;
                _progressBar.Value = 0;
            }
            
            // Hide Next button
            if (_btnNext != null)
            {
                _btnNext.Visible = false;
                _btnNext.Enabled = false;
            }
            
            // Re-enable all buttons and controls
            if (_btnSelectFile != null)
                _btnSelectFile.Enabled = true;
            if (_btnDownload != null)
                _btnDownload.Enabled = true;
            if (_cmbVersion != null)
            {
                _cmbVersion.Enabled = true;
                // Reset version selection to first item if available
                if (_cmbVersion.Items.Count > 0 && _cmbVersion.Items[0]?.ToString() != "Loading versions...")
                {
                    _cmbVersion.SelectedIndex = 0;
                }
            }
            if (_btnCancel != null)
                _btnCancel.Enabled = false; // Disable Cancel after reset (nothing to cancel)
            
            // Reset download state
            _isDownloading = false;
            
            // Reset setup complete flag
            SetupComplete = false;
            
            // Force UI update
            Application.DoEvents();
        }

        private async void LoadAvailableVersions()
        {
            try
            {
                _lblStatus!.Text = "Loading available versions...";
                _availableVersions = await _downloader.GetAvailableVersionsAsync();

                if (InvokeRequired)
                {
                    BeginInvoke(new Action(UpdateVersionComboBox));
                }
                else
                {
                    UpdateVersionComboBox();
                }
            }
            catch
            {
                _availableVersions = new List<string> { "1.21.11" };
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(UpdateVersionComboBox));
                }
                else
                {
                    UpdateVersionComboBox();
                }
            }
        }

        private void UpdateVersionComboBox()
        {
            if (_cmbVersion == null) return;

            _cmbVersion.Items.Clear();
            
            // Add versions with "Release" prefix for clarity
            foreach (var version in _availableVersions)
            {
                _cmbVersion.Items.Add(version);
            }

            // Select latest version (first in list, usually latest)
            if (_cmbVersion.Items.Count > 0)
            {
                // Try to select 1.21.11 if available, otherwise select first (latest)
                int index = _cmbVersion.Items.IndexOf("1.21.11");
                _cmbVersion.SelectedIndex = index >= 0 ? index : 0;
            }

            _cmbVersion.Enabled = true;
            _btnDownload!.Enabled = true;
            _lblStatus!.Text = "Choose an option above to continue";
            
            // Limit dropdown height to show max 10 items at once
            _cmbVersion.IntegralHeight = false;
            _cmbVersion.DropDownHeight = 200; // Approx 10 items
        }

        private void BtnSelectFile_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Minecraft Server|server.jar|JAR Files|*.jar|All Files|*.*",
                Title = "Select Minecraft Server JAR File (You can change the server JAR at any time)",
                CheckFileExists = true
            };
            
            // Set initial directory to common server locations
            var commonPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Minecraft Server"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server"),
                AppDomain.CurrentDomain.BaseDirectory
            };
            
            foreach (var path in commonPaths)
            {
                if (Directory.Exists(path))
                {
                    openFileDialog.InitialDirectory = path;
                    break;
                }
            }
            
            // If there's an existing server.jar, pre-select it
            var existingServerJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
            if (File.Exists(existingServerJar))
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(existingServerJar);
                openFileDialog.FileName = Path.GetFileName(existingServerJar);
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var fileName = Path.GetFileName(openFileDialog.FileName);
                    
                    // Show selected file immediately
                    _lblSelectedFile!.Text = $"✓ Selected: {fileName}";
                    _lblSelectedFile.Visible = true;
                    _lblStatus!.Text = "Detecting server version...";
                    _lblStatus.ForeColor = Color.FromArgb(33, 150, 243);
                    _btnSelectFile!.Enabled = false;
                    _btnDownload!.Enabled = false;
                    Application.DoEvents();

                    // Detect version (with timeout to prevent hanging)
                    string? detectedVersion = null;
                    string? detectionError = null;
                    
                    try
                    {
                        var detectionTask = Task.Run(() => 
                            ServerVersionDetector.DetectVersion(openFileDialog.FileName)
                        );
                        
                        // Wait max 3 seconds for detection
                        if (detectionTask.Wait(TimeSpan.FromSeconds(3)))
                        {
                            detectedVersion = detectionTask.Result;
                        }
                        else
                        {
                            detectionError = "Detection timed out (Java may be slow or unavailable)";
                        }
                    }
                    catch (Exception ex)
                    {
                        detectionError = $"Detection error: {ex.Message}";
                    }
                    
                    if (string.IsNullOrEmpty(detectedVersion) && !string.IsNullOrEmpty(detectionError))
                    {
                        _lblStatus.Text = $"⚠ {detectionError}";
                        _lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
                    }
                    
                    if (!string.IsNullOrEmpty(detectedVersion))
                    {
                        _lblStatus.Text = $"✓ Detected version: {detectedVersion}";
                        _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                        _lblSelectedFile.Text = $"✓ Detected: {fileName} (v{detectedVersion})";
                        
                        // Re-enable buttons so user can choose
                        _btnSelectFile!.Enabled = true;
                        _btnDownload!.Enabled = true;
                        
                        // Show confirmation dialog with options
                        var message = $"Version detected: {detectedVersion}\n\n" +
                                     $"File: {fileName}\n\n" +
                                     $"Do you want to use this file?";
                        
                        var result = MessageBox.Show(
                            message,
                            "Version Detected",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question);
                        
                        if (result == DialogResult.Yes)
                        {
                            // User confirmed - copy file to server directory
                            var serverDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
                            if (!Directory.Exists(serverDir))
                            {
                                Directory.CreateDirectory(serverDir);
                            }

                            DetectedVersion = detectedVersion;
                            var targetPath = Path.Combine(serverDir, "server.jar");
                            File.Copy(openFileDialog.FileName, targetPath, true);
                            
                            SelectedServerJarPath = targetPath;
                            
                            // Update UI to show success and enable Next button
                            _lblSelectedFile.Text = $"✓ Ready: {fileName} (v{detectedVersion})";
                            _lblStatus.Text = "✓ Server JAR file ready! Click Next to continue.";
                            _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                            
                            // Show Next button
                            _btnNext!.Visible = true;
                            _btnNext.Enabled = true;
                            _btnSelectFile!.Enabled = false;
                            _btnDownload!.Enabled = false;
                            _btnCancel!.Enabled = true; // Enable Cancel when file is selected
                        }
                        else if (result == DialogResult.No)
                        {
                            // User wants to select a different file - reopen file dialog
                            _lblSelectedFile.Visible = false;
                            _lblStatus.Text = "Choose an option above to continue";
                            _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                            
                            // Recursively call to show file dialog again
                            BtnSelectFile_Click(sender, e);
                        }
                        else // Cancel
                        {
                            // User cancelled - reset UI
                            _lblSelectedFile.Visible = false;
                            _lblStatus.Text = "Choose an option above to continue";
                            _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                        }
                    }
                    else
                    {
                        _lblStatus.Text = "⚠ Could not detect version automatically";
                        _lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
                        
                        // Version detection failed, but still allow using the file
                        var result = MessageBox.Show(
                            "Could not detect server version automatically.\n\nDo you want to use this file anyway?",
                            "Version Detection Failed",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            var serverDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server");
                            if (!Directory.Exists(serverDir))
                            {
                                Directory.CreateDirectory(serverDir);
                            }

                            DetectedVersion = null; // Version detection failed
                            var targetPath = Path.Combine(serverDir, "server.jar");
                            File.Copy(openFileDialog.FileName, targetPath, true);
                            
                            SelectedServerJarPath = targetPath;
                            
                            // Update UI to show success and enable Next button
                            _lblSelectedFile.Text = $"✓ Ready: {fileName}";
                            _lblStatus.Text = "✓ Server JAR file ready! Click Next to continue.";
                            _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                            
                            // Show Next button
                            _btnNext!.Visible = true;
                            _btnNext.Enabled = true;
                            _btnSelectFile!.Enabled = false;
                            _btnDownload!.Enabled = false;
                            _btnCancel!.Enabled = true; // Enable Cancel when file is selected
                        }
                        else
                        {
                            _btnSelectFile.Enabled = true;
                            _btnDownload.Enabled = true;
                            _cmbVersion!.Enabled = true;
                            _btnCancel!.Enabled = false; // Keep Cancel disabled if user didn't confirm
                            _lblSelectedFile.Visible = false;
                            _lblStatus.Text = "Choose an option above to continue";
                            _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _lblSelectedFile!.Visible = false;
                    _lblStatus!.Text = $"✗ Error: {ex.Message}";
                    _lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
                    
                    MessageBox.Show(
                        $"Error copying server JAR file:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _btnSelectFile!.Enabled = true;
                    _btnDownload!.Enabled = true;
                    _cmbVersion!.Enabled = true;
                }
            }
        }

        private async void BtnDownload_Click(object? sender, EventArgs e)
        {
            if (_isDownloading)
                return;

            if (!IsHandleCreated)
            {
                // Wait for handle to be created
                HandleCreated += (s, args) => BtnDownload_Click(sender, e);
                return;
            }

            _isDownloading = true;
            _btnDownload!.Enabled = false;
            _btnSelectFile!.Enabled = false;
            _cmbVersion!.Enabled = false;
            _btnCancel!.Enabled = false;
            _progressBar!.Visible = true;
            _progressBar!.Value = 0;

            try
            {
                var selectedVersion = _cmbVersion!.SelectedItem?.ToString()?.Trim();
                if (string.IsNullOrEmpty(selectedVersion))
                {
                    selectedVersion = "1.21.11"; // Fallback to default
                }
                
                // Store requested version BEFORE downloading (user's explicit selection)
                RequestedVersion = selectedVersion;
                System.Diagnostics.Debug.WriteLine($"[SetupWizardForm] User selected version: '{RequestedVersion}'");
                
                // Set initial status - downloader will update via DownloadProgress events
                // ServerDownloader is the single source of truth for cache checking
                _lblStatus!.Text = "Checking cache and preparing download...";
                _lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                Application.DoEvents();
                
                // DownloadServerJarAsync will check cache internally and communicate status via DownloadProgress events
                // Forms should trust the event system, not pre-check cache
                await _downloader.DownloadServerJarAsync(selectedVersion, overwrite: true);
                
                if (IsDisposed || !IsHandleCreated)
                    return;

                // Check if cache was used by looking at the current status message
                // The DownloadProgress event should have updated it with cache info
                string? finalStatus = _lblStatus?.Text;
                bool wasFromCache = finalStatus != null && (
                    finalStatus.Contains("cache", StringComparison.OrdinalIgnoreCase) ||
                    finalStatus.Contains("cached", StringComparison.OrdinalIgnoreCase)
                );

                // Update UI to show success and enable Next button
                if (wasFromCache)
                {
                    _lblStatus!.Text = $"✓ Using cached version {selectedVersion} (no download needed)! Click Next to continue.";
                }
                else
                {
                    _lblStatus!.Text = $"✓ Server JAR downloaded successfully! Click Next to continue.";
                }
                _lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
                
                // Show Next button
                _btnNext!.Visible = true;
                _btnNext.Enabled = true;
                _btnSelectFile!.Enabled = false;
                _btnDownload!.Enabled = false;
                _cmbVersion!.Enabled = false;
                _btnCancel!.Enabled = true;
                _progressBar!.Visible = false;
                
                SelectedServerJarPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server", "server.jar");
            }
            catch (Exception ex)
            {
                if (IsDisposed || !IsHandleCreated)
                    return;

                MessageBox.Show(
                    $"Failed to download server JAR:\n\n{ex.Message}\n\nPlease download it manually from:\nhttps://www.minecraft.net/en-us/download/server",
                    "Download Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _btnDownload.Enabled = true;
                _btnSelectFile.Enabled = true;
                _cmbVersion!.Enabled = true;
                _btnCancel.Enabled = false; // Keep Cancel disabled on download error (nothing selected)
                _progressBar.Visible = false;
            }
            finally
            {
                _isDownloading = false;
            }
        }

        private void BtnNext_Click(object? sender, EventArgs e)
        {
            // Mark setup as complete and proceed to server setup
            SetupComplete = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Downloader_DownloadProgress(object? sender, DownloadProgressEventArgs e)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => Downloader_DownloadProgress(sender, e)));
                }
                catch (InvalidOperationException)
                {
                    // Form is being disposed or handle not created yet
                    return;
                }
                return;
            }

            if (_lblStatus != null && !_lblStatus.IsDisposed)
            {
                _lblStatus.Text = e.Status;
                // Update color based on status
                if (e.Status.Contains("cache", StringComparison.OrdinalIgnoreCase) || 
                    e.Status.Contains("cached", StringComparison.OrdinalIgnoreCase))
                {
                    _lblStatus.ForeColor = Color.FromArgb(76, 175, 80); // Green for cache
                }
                else if (e.Status.Contains("Error", StringComparison.OrdinalIgnoreCase))
                {
                    _lblStatus.ForeColor = Color.Red;
                }
                else
                {
                    _lblStatus.ForeColor = Color.FromArgb(100, 100, 100); // Gray for normal progress
                }
            }
            
            if (_progressBar != null && !_progressBar.IsDisposed)
            {
                _progressBar.Value = Math.Min(100, Math.Max(0, e.Progress));
                // Hide progress bar if using cache (100% progress means done)
                if (e.Progress >= 100 && (e.Status?.Contains("cache", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    _progressBar.Visible = false;
                }
            }
            
            // Force UI update
            Application.DoEvents();
            Update();
        }
    }
}

