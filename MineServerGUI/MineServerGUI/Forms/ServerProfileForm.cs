using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineServerGUI.Core;
using MineServerGUI.Models;

namespace MineServerGUI.Forms
{
    public partial class ServerProfileForm : Form
    {
        private readonly ServerProfileManager _profileManager;
        private ListBox? _lstProfiles;
        private TextBox? _txtProfileName;
        private TextBox? _txtDescription;
        private Label? _lblCurrentProfile;
        private Button? _btnCreate;
        private Button? _btnDelete;
        private Button? _btnSwitch;
        private Button? _btnClose;
        private Label? _lblVersion;
        private Label? _lblDirectory;

        public ServerProfile? SelectedProfile { get; private set; }

        public ServerProfileForm(ServerProfileManager profileManager)
        {
            _profileManager = profileManager;
            InitializeComponent();
            LoadProfiles();
        }

        private void InitializeComponent()
        {
            this.Text = "Server Profiles";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title
            var lblTitle = new Label
            {
                Text = "Manage Server Profiles",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Current Profile Label
            _lblCurrentProfile = new Label
            {
                Text = $"Current: {_profileManager.CurrentProfile?.Name ?? "None"}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(20, 50),
                AutoSize = true
            };
            this.Controls.Add(_lblCurrentProfile);

            // Profiles List
            var lblProfiles = new Label
            {
                Text = "Server Profiles:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 80),
                AutoSize = true
            };
            this.Controls.Add(lblProfiles);

            _lstProfiles = new ListBox
            {
                Location = new Point(20, 100),
                Size = new Size(250, 280),
                BorderStyle = BorderStyle.FixedSingle
            };
            _lstProfiles.SelectedIndexChanged += LstProfiles_SelectedIndexChanged;
            this.Controls.Add(_lstProfiles);

            // Profile Details Panel
            var detailsPanel = new Panel
            {
                Location = new Point(290, 100),
                Size = new Size(280, 280),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(detailsPanel);

            var lblDetails = new Label
            {
                Text = "Profile Details:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            detailsPanel.Controls.Add(lblDetails);

            var lblName = new Label
            {
                Text = "Name:",
                Location = new Point(10, 40),
                Size = new Size(80, 20),
                AutoSize = true
            };
            detailsPanel.Controls.Add(lblName);

            _txtProfileName = new TextBox
            {
                Location = new Point(10, 60),
                Size = new Size(260, 23),
                BorderStyle = BorderStyle.FixedSingle
            };
            detailsPanel.Controls.Add(_txtProfileName);

            var lblDesc = new Label
            {
                Text = "Description:",
                Location = new Point(10, 90),
                Size = new Size(80, 20),
                AutoSize = true
            };
            detailsPanel.Controls.Add(lblDesc);

            _txtDescription = new TextBox
            {
                Location = new Point(10, 110),
                Size = new Size(260, 60),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            detailsPanel.Controls.Add(_txtDescription);

            _lblVersion = new Label
            {
                Text = "Version: Unknown",
                Location = new Point(10, 180),
                AutoSize = true
            };
            detailsPanel.Controls.Add(_lblVersion);

            _lblDirectory = new Label
            {
                Text = "Directory:",
                Location = new Point(10, 200),
                Size = new Size(260, 40),
                AutoSize = false
            };
            detailsPanel.Controls.Add(_lblDirectory);

            // Buttons
            _btnCreate = new Button
            {
                Text = "Create New",
                Location = new Point(20, 400),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _btnCreate.FlatAppearance.BorderSize = 0;
            _btnCreate.Click += BtnCreate_Click;
            this.Controls.Add(_btnCreate);

            _btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(130, 400),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            _btnDelete.FlatAppearance.BorderSize = 0;
            _btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(_btnDelete);

            _btnSwitch = new Button
            {
                Text = "Switch To",
                Location = new Point(240, 400),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            _btnSwitch.FlatAppearance.BorderSize = 0;
            _btnSwitch.Click += BtnSwitch_Click;
            this.Controls.Add(_btnSwitch);

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(470, 400),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            _btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(_btnClose);
        }

        private void LoadProfiles()
        {
            _lstProfiles!.Items.Clear();
            foreach (var profile in _profileManager.Profiles)
            {
                var displayText = $"{profile.Name} (v{profile.Version})";
                if (profile.Id == _profileManager.CurrentProfile?.Id)
                {
                    displayText += " [Current]";
                }
                _lstProfiles.Items.Add(new ProfileListItem { Profile = profile, DisplayText = displayText });
            }

            if (_lstProfiles.Items.Count > 0)
            {
                // Select current profile
                for (int i = 0; i < _lstProfiles.Items.Count; i++)
                {
                    var item = (ProfileListItem)_lstProfiles.Items[i];
                    if (item.Profile.Id == _profileManager.CurrentProfile?.Id)
                    {
                        _lstProfiles.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void LstProfiles_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_lstProfiles!.SelectedItem == null)
            {
                _btnDelete!.Enabled = false;
                _btnSwitch!.Enabled = false;
                _txtProfileName!.Text = "";
                _txtDescription!.Text = "";
                _lblVersion!.Text = "Version: Unknown";
                _lblDirectory!.Text = "Directory:";
                return;
            }

            var item = (ProfileListItem)_lstProfiles.SelectedItem;
            var profile = item.Profile;

            _txtProfileName!.Text = profile.Name;
            _txtDescription!.Text = profile.Description ?? "";
            _lblVersion!.Text = $"Version: {profile.Version}";
            _lblDirectory!.Text = $"Directory:\n{profile.ServerDirectory}";

            // Enable delete if there's more than 1 profile (can delete any profile, even current)
            _btnDelete!.Enabled = _profileManager.Profiles.Count > 1;
            _btnSwitch!.Enabled = profile.Id != _profileManager.CurrentProfile?.Id;
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            // Use SetupWizardForm for creating new profiles (with isCreatingProfile flag)
            var setupWizard = new SetupWizardForm(isCreatingProfile: true);
            var result = setupWizard.ShowDialog(this);

            if (result == DialogResult.OK && setupWizard.SetupComplete && !string.IsNullOrEmpty(setupWizard.SelectedServerJarPath))
            {
                // Show dialog to get profile name
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
                        try
                        {
                            // Use RequestedVersion (what user selected) first, then detect as fallback
                            string? version = setupWizard.RequestedVersion;
                            if (string.IsNullOrEmpty(version))
                            {
                                // Fallback to detection if RequestedVersion not available
                                try
                                {
                                    version = ServerVersionDetector.DetectVersion(setupWizard.SelectedServerJarPath);
                                }
                                catch 
                                {
                                    version = "Unknown";
                                }
                            }

                            var profile = _profileManager.CreateProfile(
                                txtName.Text.Trim(),
                                version ?? "Unknown",
                                string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim()
                            );

                            // Copy server.jar to new profile directory
                            if (File.Exists(setupWizard.SelectedServerJarPath))
                            {
                                if (!Directory.Exists(profile.ServerDirectory))
                                {
                                    Directory.CreateDirectory(profile.ServerDirectory);
                                }
                                File.Copy(setupWizard.SelectedServerJarPath, profile.ServerJarPath, true);
                            }

                            LoadProfiles();
                            MessageBox.Show(
                                $"Profile '{profile.Name}' created successfully!",
                                "Profile Created",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"Error creating profile:\n\n{ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_lstProfiles!.SelectedItem == null)
                return;

            var item = (ProfileListItem)_lstProfiles.SelectedItem;
            var profile = item.Profile;

            // Check if trying to delete the last profile
            if (_profileManager.Profiles.Count <= 1)
            {
                MessageBox.Show(
                    "Cannot delete the last remaining profile. You must have at least one profile.",
                    "Cannot Delete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // If deleting current profile, warn and switch to another first
            bool isCurrentProfile = profile.Id == _profileManager.CurrentProfile?.Id;
            string message = $"Are you sure you want to delete the profile '{profile.Name}'?";
            
            if (isCurrentProfile)
            {
                message += "\n\n⚠ WARNING: This is the currently active profile. " +
                          "You will be switched to another profile after deletion.";
            }
            
            message += "\n\nNote: The server directory will not be deleted automatically.";

            var result = MessageBox.Show(
                message,
                "Delete Profile",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // If deleting current profile, switch to another first
                    if (isCurrentProfile)
                    {
                        var otherProfile = _profileManager.Profiles.FirstOrDefault(p => p.Id != profile.Id);
                        if (otherProfile != null)
                        {
                            _profileManager.SetCurrentProfile(otherProfile.Id);
                        }
                    }

                    _profileManager.DeleteProfile(profile.Id);
                    LoadProfiles();
                    
                    // Refresh the form to show updated current profile
                    if (isCurrentProfile && _profileManager.CurrentProfile != null)
                    {
                        _lblCurrentProfile!.Text = $"Current: {_profileManager.CurrentProfile.Name}";
                    }
                    
                    MessageBox.Show(
                        "Profile deleted successfully.",
                        "Deleted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error deleting profile:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSwitch_Click(object? sender, EventArgs e)
        {
            if (_lstProfiles!.SelectedItem == null)
                return;

            var item = (ProfileListItem)_lstProfiles.SelectedItem;
            var profile = item.Profile;

            if (profile.Id == _profileManager.CurrentProfile?.Id)
            {
                MessageBox.Show(
                    "This profile is already active.",
                    "Already Active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Switch to profile '{profile.Name}'?\n\n" +
                "Note: The current server must be stopped before switching profiles.",
                "Switch Profile",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _profileManager.SetCurrentProfile(profile.Id);
                    SelectedProfile = profile;
                    _lblCurrentProfile!.Text = $"Current: {profile.Name}";
                    LoadProfiles();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error switching profile:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private class ProfileListItem
        {
            public ServerProfile Profile { get; set; } = null!;
            public string DisplayText { get; set; } = "";
            public override string ToString() => DisplayText;
        }
    }
}

