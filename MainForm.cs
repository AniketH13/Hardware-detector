#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DetectIt
{
    public class MainForm : Form
    {
        private TreeView hardwareTree = null!;
        private RichTextBox detailsBox = null!;
        private Button refreshButton = null!;
        private Button exportButton = null!;
        private ProgressBar progressBar = null!;
        private Label statusLabel = null!;
        private Label inspectorTitleLabel = null!;
        private Label wmiBadgeLabel = null!;
        private Panel summaryContainerPanel = null!;
        private Label cpuStatValue = null!;
        private Label ramStatValue = null!;
        private Label gpuStatValue = null!;
        private Label osStatValue = null!;

        // Theme colors
        private readonly Color bgDark = Color.FromArgb(15, 16, 20);           // Deep background
        private readonly Color bgSecondary = Color.FromArgb(24, 25, 32);      // Sidebars & Headers
        private readonly Color bgTertiary = Color.FromArgb(32, 34, 44);       // Elevated cards
        private readonly Color accentPrimary = Color.FromArgb(10, 132, 255);   // iOS/Fluent Blue
        private readonly Color accentSuccess = Color.FromArgb(48, 209, 88);   // Emerald Green
        private readonly Color accentPurple = Color.FromArgb(191, 90, 242);   // Purple accent
        private readonly Color accentAmber = Color.FromArgb(255, 159, 10);    // Amber warning
        private readonly Color textPrimary = Color.FromArgb(242, 242, 247);   // Crisp text
        private readonly Color textSecondary = Color.FromArgb(142, 142, 147); // Subtitle text
        private readonly Color borderSubtle = Color.FromArgb(44, 46, 58);     // Subtle borders

        public MainForm()
        {
            InitializeComponents();
            CheckAdminPrivileges();
            Shown += async (s, e) => await LoadHardwareInfo();
        }

        private void CheckAdminPrivileges()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    statusLabel.Text = "⚠️ Run as Administrator for full hardware access";
                    statusLabel.ForeColor = accentAmber;
                }
            }
        }

        private void InitializeComponents()
        {
            this.Text = "DetectIt - Hardware Detector & Diagnostics";
            this.Size = new Size(1100, 720);
            this.MinimumSize = new Size(880, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgDark;
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // --- Top App Header ---
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = bgSecondary,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Custom border line under header
            Panel headerBorder = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = borderSubtle
            };
            headerPanel.Controls.Add(headerBorder);

            // App Brand Icon Badge
            Panel iconBadge = new Panel
            {
                Size = new Size(38, 38),
                Location = new Point(20, 15),
                BackColor = accentPrimary
            };
            Label iconLabel = new Label
            {
                Text = "⚡",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            iconBadge.Controls.Add(iconLabel);

            Label titleLabel = new Label
            {
                Text = "DetectIt",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = textPrimary,
                AutoSize = true,
                Location = new Point(68, 12)
            };

            Label versionLabel = new Label
            {
                Text = "v1.0 Pro",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = accentPrimary,
                BackColor = Color.FromArgb(20, 10, 132, 255),
                AutoSize = true,
                Location = new Point(175, 18),
                Padding = new Padding(4, 2, 4, 2)
            };

            Label subtitleLabel = new Label
            {
                Text = "Hardware Detection & System Diagnostics",
                Font = new Font("Segoe UI", 9),
                ForeColor = textSecondary,
                AutoSize = true,
                Location = new Point(69, 40)
            };

            headerPanel.Controls.Add(iconBadge);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(versionLabel);
            headerPanel.Controls.Add(subtitleLabel);

            // Top Action Controls (Refresh & Export)
            Panel actionsPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 320,
                BackColor = Color.Transparent
            };

            refreshButton = new Button
            {
                Text = "🔄 Refresh",
                Size = new Size(125, 36),
                Location = new Point(35, 16),
                FlatStyle = FlatStyle.Flat,
                BackColor = accentPrimary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 150, 255);
            refreshButton.Click += async (s, e) => await LoadHardwareInfo();

            exportButton = new Button
            {
                Text = "📁 Export Report",
                Size = new Size(135, 36),
                Location = new Point(170, 16),
                FlatStyle = FlatStyle.Flat,
                BackColor = accentSuccess,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            exportButton.FlatAppearance.BorderSize = 0;
            exportButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(68, 220, 108);
            exportButton.Click += ExportToText;

            progressBar = new ProgressBar
            {
                Size = new Size(270, 4),
                Location = new Point(35, 56),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            actionsPanel.Controls.Add(refreshButton);
            actionsPanel.Controls.Add(exportButton);
            actionsPanel.Controls.Add(progressBar);
            headerPanel.Controls.Add(actionsPanel);

            // --- Left Sidebar Navigation ---
            Panel sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 270,
                BackColor = bgSecondary,
                Padding = new Padding(10)
            };

            Panel sidebarBorder = new Panel
            {
                Dock = DockStyle.Right,
                Width = 1,
                BackColor = borderSubtle
            };
            sidebarPanel.Controls.Add(sidebarBorder);

            Label categoryHeaderLabel = new Label
            {
                Text = "HARDWARE CATEGORIES",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = textSecondary,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 0, 0)
            };
            sidebarPanel.Controls.Add(categoryHeaderLabel);

            // TreeView Setup
            hardwareTree = new TreeView
            {
                Dock = DockStyle.Fill,
                BackColor = bgSecondary,
                ForeColor = textPrimary,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ItemHeight = 38,
                ShowLines = false,
                FullRowSelect = true,
                HideSelection = false,
                DrawMode = TreeViewDrawMode.OwnerDrawText
            };
            hardwareTree.DrawNode += TreeViewDrawNode;
            hardwareTree.AfterSelect += TreeViewSelected;
            sidebarPanel.Controls.Add(hardwareTree);

            // Bottom Status Card inside Sidebar
            Panel statusCard = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = bgDark,
                Padding = new Padding(12),
                Margin = new Padding(10)
            };

            statusLabel = new Label
            {
                Text = "Status: Initializing...",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = textSecondary,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            statusCard.Controls.Add(statusLabel);
            sidebarPanel.Controls.Add(statusCard);

            // --- Main Content Details Panel ---
            Panel mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bgDark,
                Padding = new Padding(16)
            };

            // Inspector Sub-header
            Panel inspectorHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = bgSecondary,
                Padding = new Padding(15, 8, 15, 8)
            };

            inspectorTitleLabel = new Label
            {
                Text = "💻 System Summary",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = textPrimary,
                AutoSize = true,
                Location = new Point(12, 10)
            };

            wmiBadgeLabel = new Label
            {
                Text = "System Overview",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = accentPrimary,
                AutoSize = true,
                Location = new Point(300, 12)
            };

            inspectorHeader.Controls.Add(inspectorTitleLabel);
            inspectorHeader.Controls.Add(wmiBadgeLabel);

            // Top Quick Stat Metric Cards Panel
            summaryContainerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 10)
            };

            TableLayoutPanel statsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            statsGrid.Controls.Add(CreateStatCard("PROCESSOR", "Detecting...", accentPrimary, out cpuStatValue), 0, 0);
            statsGrid.Controls.Add(CreateStatCard("MEMORY", "Detecting...", accentSuccess, out ramStatValue), 1, 0);
            statsGrid.Controls.Add(CreateStatCard("GRAPHICS", "Detecting...", accentPurple, out gpuStatValue), 2, 0);
            statsGrid.Controls.Add(CreateStatCard("SYSTEM OS", "Detecting...", accentAmber, out osStatValue), 3, 0);

            summaryContainerPanel.Controls.Add(statsGrid);

            // Details RichTextBox Box Container
            Panel detailsBoxContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bgTertiary,
                Padding = new Padding(16)
            };

            detailsBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = bgTertiary,
                ForeColor = textPrimary,
                Font = new Font("Consolas", 10.5f),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                DetectUrls = true
            };
            detailsBoxContainer.Controls.Add(detailsBox);

            // Add all panels to main workspace
            mainContentPanel.Controls.Add(detailsBoxContainer);
            mainContentPanel.Controls.Add(summaryContainerPanel);
            mainContentPanel.Controls.Add(inspectorHeader);

            // Add top level components to form
            this.Controls.Add(mainContentPanel);
            this.Controls.Add(sidebarPanel);
            this.Controls.Add(headerPanel);
        }

        private Panel CreateStatCard(string title, string initialValue, Color accentColor, out Label valueLabel)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bgSecondary,
                Margin = new Padding(4),
                Padding = new Padding(12, 8, 12, 8)
            };

            Label titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = textSecondary,
                Dock = DockStyle.Top,
                Height = 16
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = accentColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLbl);
            return card;
        }

        private void TreeViewDrawNode(object? sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            bool selected = (e.State & TreeNodeStates.Selected) != 0;
            bool isParent = e.Node.Parent == null;

            Color backColor = selected ? Color.FromArgb(36, 40, 54) : bgSecondary;
            Color foreColor = selected ? accentPrimary : (isParent ? textPrimary : textSecondary);

            // Fill row background
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);

            // Draw modern left accent indicator bar for selected node
            if (selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(accentPrimary),
                    new Rectangle(e.Bounds.Left, e.Bounds.Top + 4, 3, e.Bounds.Height - 8));
            }

            // Text layout
            int leftIndent = isParent ? 14 : 32;
            Rectangle textBounds = new Rectangle(
                e.Bounds.Left + leftIndent,
                e.Bounds.Top,
                e.Bounds.Width - leftIndent - 5,
                e.Bounds.Height);

            Font font = isParent ? new Font(hardwareTree.Font, FontStyle.Bold) : hardwareTree.Font;

            TextRenderer.DrawText(e.Graphics, e.Node.Text, font, textBounds, foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            if (isParent) font.Dispose();
        }

        private void TreeViewSelected(object? sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            inspectorTitleLabel.Text = e.Node.Text;
            
            if (e.Node.Tag != null)
            {
                SetFormattedDetails(e.Node.Tag.ToString() ?? "");
            }

            // Update badge text according to category
            wmiBadgeLabel.Text = e.Node.Parent != null ? $"Category: {e.Node.Parent.Text}" : "Component Category";
        }

        private void SetFormattedDetails(string text)
        {
            detailsBox.Clear();
            if (string.IsNullOrEmpty(text)) return;

            string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.StartsWith("═") || line.StartsWith("─"))
                {
                    detailsBox.SelectionColor = borderSubtle;
                    detailsBox.AppendText(line + "\n");
                }
                else if (line.EndsWith("Information") || line.StartsWith("Module ") || line.StartsWith("Hardware"))
                {
                    detailsBox.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold);
                    detailsBox.SelectionColor = accentPrimary;
                    detailsBox.AppendText(line + "\n");
                }
                else if (line.Contains(":"))
                {
                    int colonIndex = line.IndexOf(':');
                    string key = line.Substring(0, colonIndex + 1);
                    string val = line.Substring(colonIndex + 1);

                    detailsBox.SelectionFont = new Font("Consolas", 10.5f, FontStyle.Bold);
                    detailsBox.SelectionColor = textSecondary;
                    detailsBox.AppendText(key);

                    detailsBox.SelectionFont = new Font("Consolas", 10.5f, FontStyle.Regular);
                    detailsBox.SelectionColor = textPrimary;
                    detailsBox.AppendText(val + "\n");
                }
                else
                {
                    detailsBox.SelectionFont = new Font("Consolas", 10.5f, FontStyle.Regular);
                    detailsBox.SelectionColor = textPrimary;
                    detailsBox.AppendText(line + "\n");
                }
            }
        }

        private async Task LoadHardwareInfo()
        {
            try
            {
                refreshButton.Enabled = false;
                progressBar.Visible = true;
                statusLabel.Text = "Scanning WMI hardware components...";

                hardwareTree.Nodes.Clear();
                detailsBox.Clear();

                await Task.Run(() =>
                {
                    var cpuData = DetectCPU();
                    var memoryData = DetectMemory();
                    var motherboardData = DetectMotherboard();
                    var gpuData = DetectGPU();
                    var diskData = DetectDisks();
                    var networkData = DetectNetworkAdapters();
                    var osData = DetectOperatingSystem();

                    this.Invoke((MethodInvoker)delegate
                    {
                        // System Summary Root Node
                        TreeNode summaryNode = new TreeNode("💻 System Overview")
                        {
                            Tag = BuildOverviewDetails(cpuData.details, memoryData.details, gpuData.details, diskData.details, osData.details)
                        };
                        hardwareTree.Nodes.Add(summaryNode);

                        AddTreeNode(cpuData);
                        AddTreeNode(memoryData);
                        AddTreeNode(motherboardData);
                        AddTreeNode(gpuData);
                        AddTreeNode(diskData);
                        AddTreeNode(networkData);
                        AddTreeNode(osData);

                        // Update Quick Stat Badges
                        cpuStatValue.Text = ExtractSummaryValue(cpuData.details, "Name:", "Unknown CPU");
                        ramStatValue.Text = ExtractSummaryValue(memoryData.details, "Total RAM:", "Unknown RAM");
                        gpuStatValue.Text = ExtractSummaryValue(gpuData.details, "Name:", "Unknown GPU");
                        osStatValue.Text = ExtractSummaryValue(osData.details, "OS:", "Windows OS");

                        // Select Summary node by default
                        hardwareTree.SelectedNode = summaryNode;
                    });
                });

                statusLabel.Text = "🟢 Hardware scan complete";
                statusLabel.ForeColor = accentSuccess;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning hardware: {ex.Message}", "Hardware Scan Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "🔴 Error during hardware scan";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                refreshButton.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private string ExtractSummaryValue(string details, string key, string fallback)
        {
            if (string.IsNullOrEmpty(details)) return fallback;
            foreach (var line in details.Split('\n'))
            {
                if (line.Trim().StartsWith(key))
                {
                    return line.Substring(line.IndexOf(key) + key.Length).Trim();
                }
            }
            return fallback;
        }

        private string BuildOverviewDetails(string cpu, string ram, string gpu, string disk, string os)
        {
            return $"SYSTEM HARDWARE OVERVIEW\n{new string('═', 55)}\n\n" +
                   $"Processor: {ExtractSummaryValue(cpu, "Name:", "N/A")}\n" +
                   $"System RAM: {ExtractSummaryValue(ram, "Total RAM:", "N/A")}\n" +
                   $"Graphics Card: {ExtractSummaryValue(gpu, "Name:", "N/A")}\n" +
                   $"Primary Storage: {ExtractSummaryValue(disk, "Model:", "N/A")}\n" +
                   $"Operating System: {ExtractSummaryValue(os, "OS:", "Windows")}\n\n" +
                   $"Status: All components verified & accessible via WMI\n";
        }

        private void AddTreeNode((string name, TreeNode[] nodes, string details) data)
        {
            if (data.nodes == null || data.nodes.Length == 0)
            {
                TreeNode emptyNode = new TreeNode("No devices found")
                {
                    ForeColor = textSecondary,
                    Tag = "No devices detected in this category."
                };
                data.nodes = new TreeNode[] { emptyNode };
            }

            TreeNode node = new TreeNode(data.name)
            {
                BackColor = bgSecondary,
                ForeColor = textPrimary
            };

            node.Nodes.AddRange(data.nodes);
            if (!string.IsNullOrEmpty(data.details))
            {
                node.Tag = data.details;
            }

            hardwareTree.Nodes.Add(node);
        }

        private (string name, TreeNode[] nodes, string details) DetectCPU()
        {
            var nodes = new List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = SafeGetProperty(obj, "Name", "Unknown CPU");
                    string manufacturer = SafeGetProperty(obj, "Manufacturer", "Unknown");
                    string cores = SafeGetProperty(obj, "NumberOfCores", "0");
                    string logicalProcessors = SafeGetProperty(obj, "NumberOfLogicalProcessors", "0");
                    string clockSpeed = SafeGetProperty(obj, "MaxClockSpeed", "0");
                    string architecture = GetArchitecture(obj["Architecture"]);

                    details = $"Processor (CPU) Information\n{new string('═', 50)}\n\n";
                    details += $"Name: {name}\n";
                    details += $"Manufacturer: {manufacturer}\n";
                    details += $"Cores: {cores}\n";
                    details += $"Logical Processors: {logicalProcessors}\n";
                    details += $"Max Clock Speed: {clockSpeed} MHz\n";
                    details += $"Architecture: {architecture}\n";

                    TreeNode item = new TreeNode($"🧠 {name}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("🧠 Processor (CPU)", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectMemory()
        {
            var nodes = new List<TreeNode>();
            long totalMemory = 0;
            int moduleCount = 0;
            string details = $"Memory (RAM) Information\n{new string('═', 50)}\n\n";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    moduleCount++;
                    long capacity = SafeGetInt64(obj, "Capacity", 0);
                    totalMemory += capacity;

                    string speed = SafeGetProperty(obj, "Speed", "Unknown");
                    string manufacturer = SafeGetProperty(obj, "Manufacturer", "Unknown");
                    string partNumber = SafeGetProperty(obj, "PartNumber", "Unknown");
                    long capacityGB = capacity / (1024 * 1024 * 1024);

                    details += $"Module {moduleCount}:\n";
                    details += $"  Capacity: {capacityGB} GB\n";
                    details += $"  Speed: {speed} MHz\n";
                    details += $"  Manufacturer: {manufacturer}\n";
                    details += $"  Part Number: {partNumber}\n\n";

                    TreeNode item = new TreeNode($"⚡ Module {moduleCount}: {capacityGB} GB")
                    {
                        Tag = $"Memory Module {moduleCount}\n{new string('─', 35)}\n" +
                              $"Capacity: {capacityGB} GB\n" +
                              $"Speed: {speed} MHz\n" +
                              $"Manufacturer: {manufacturer}\n" +
                              $"Part Number: {partNumber}"
                    };
                    nodes.Add(item);
                }
            }

            details += $"Total RAM: {totalMemory / (1024 * 1024 * 1024)} GB";
            return ("⚡ Memory (RAM)", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectMotherboard()
        {
            var nodes = new List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string manufacturer = SafeGetProperty(obj, "Manufacturer", "Unknown");
                    string product = SafeGetProperty(obj, "Product", "Unknown");
                    string serialNumber = SafeGetProperty(obj, "SerialNumber", "Unknown");
                    string version = SafeGetProperty(obj, "Version", "Unknown");

                    details = $"Motherboard & BIOS Information\n{new string('═', 50)}\n\n";
                    details += $"Manufacturer: {manufacturer}\n";
                    details += $"Product: {product}\n";
                    details += $"Serial Number: {serialNumber}\n";
                    details += $"Version: {version}\n";

                    TreeNode item = new TreeNode($"🖥️ {manufacturer} {product}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("🖥️ Motherboard & BIOS", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectGPU()
        {
            var nodes = new List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = SafeGetProperty(obj, "Name", "Unknown GPU");
                    string driverVersion = SafeGetProperty(obj, "DriverVersion", "Unknown");
                    string videoProcessor = SafeGetProperty(obj, "VideoProcessor", "Unknown");

                    details = $"Graphics (GPU) Information\n{new string('═', 50)}\n\n";
                    details += $"Name: {name}\n";
                    details += $"Driver Version: {driverVersion}\n";
                    details += $"Video Processor: {videoProcessor}\n";

                    long ram = SafeGetInt64(obj, "AdapterRAM", 0);
                    if (ram > 0)
                    {
                        details += $"Video RAM: {ram / (1024 * 1024)} MB\n";
                    }

                    string horizRes = SafeGetProperty(obj, "CurrentHorizontalResolution", "N/A");
                    string vertRes = SafeGetProperty(obj, "CurrentVerticalResolution", "N/A");
                    string refreshRate = SafeGetProperty(obj, "CurrentRefreshRate", "N/A");

                    if (horizRes != "N/A" && vertRes != "N/A")
                    {
                        details += $"Current Resolution: {horizRes}x{vertRes}\n";
                        details += $"Refresh Rate: {refreshRate} Hz\n";
                    }

                    TreeNode item = new TreeNode($"🎮 {name}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("🎮 Graphics (GPU)", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectDisks()
        {
            var nodes = new List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string model = SafeGetProperty(obj, "Model", "Unknown Disk");
                    string interfaceType = SafeGetProperty(obj, "InterfaceType", "Unknown");
                    string mediaType = SafeGetProperty(obj, "MediaType", "Unknown");
                    string partitions = SafeGetProperty(obj, "Partitions", "0");

                    details = $"Storage Drive Information\n{new string('═', 50)}\n\n";
                    details += $"Model: {model}\n";
                    details += $"Interface: {interfaceType}\n";

                    long size = SafeGetInt64(obj, "Size", 0);
                    if (size > 0)
                    {
                        details += $"Size: {size / (1024 * 1024 * 1024)} GB\n";
                    }

                    details += $"Media Type: {mediaType}\n";
                    details += $"Partitions: {partitions}\n";

                    TreeNode item = new TreeNode($"🖴 {model}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("🖴 Storage Drives", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectNetworkAdapters()
        {
            var nodes = new List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus=2"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = SafeGetProperty(obj, "Name", "Unknown Adapter");
                    string manufacturer = SafeGetProperty(obj, "Manufacturer", "Unknown");
                    string macAddress = SafeGetProperty(obj, "MACAddress", "N/A");
                    string speed = SafeGetProperty(obj, "Speed", "N/A");

                    details = $"Network Adapter Information\n{new string('═', 50)}\n\n";
                    details += $"Name: {name}\n";
                    details += $"Manufacturer: {manufacturer}\n";
                    details += $"MAC Address: {macAddress}\n";
                    details += $"Speed: {speed}\n";

                    TreeNode item = new TreeNode($"🌐 {name}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("🌐 Network Adapters", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectOperatingSystem()
        {
            var nodes = new List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string caption = SafeGetProperty(obj, "Caption", "Unknown OS");
                    string version = SafeGetProperty(obj, "Version", "Unknown");
                    string architecture = SafeGetProperty(obj, "OSArchitecture", "Unknown");
                    string buildNumber = SafeGetProperty(obj, "BuildNumber", "Unknown");
                    string systemDirectory = SafeGetProperty(obj, "SystemDirectory", "Unknown");

                    details = $"Operating System Information\n{new string('═', 50)}\n\n";
                    details += $"OS: {caption}\n";
                    details += $"Version: {version}\n";
                    details += $"Architecture: {architecture}\n";
                    details += $"Build Number: {buildNumber}\n";

                    string installDateStr = SafeGetProperty(obj, "InstallDate", "");
                    if (!string.IsNullOrEmpty(installDateStr))
                    {
                        try
                        {
                            DateTime installDate = ManagementDateTimeConverter.ToDateTime(installDateStr);
                            details += $"Install Date: {installDate}\n";
                        }
                        catch
                        {
                            details += $"Install Date: Unknown\n";
                        }
                    }

                    details += $"System Directory: {systemDirectory}\n";

                    TreeNode item = new TreeNode($"🪟 {caption}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("🪟 Operating System", nodes.ToArray(), details);
        }

        private void ExportToText(object? sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = $"DetectIt_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string content = $"DetectIt - Hardware Diagnostics Report\n{new string('═', 60)}\n";
                    content += $"Report Generated: {DateTime.Now}\n\n";

                    foreach (TreeNode category in hardwareTree.Nodes)
                    {
                        content += $"\n{category.Text}\n";
                        content += new string('─', 60) + "\n";

                        if (category.Tag != null)
                        {
                            content += category.Tag.ToString() + "\n";
                        }

                        foreach (TreeNode item in category.Nodes)
                        {
                            if (item.Tag != null)
                            {
                                content += item.Tag.ToString() + "\n";
                            }
                        }
                    }

                    File.WriteAllText(saveDialog.FileName, content);
                    MessageBox.Show("Hardware diagnostics report exported successfully!", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting report: {ex.Message}", "Export Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string SafeGetProperty(ManagementObject obj, string propertyName, string defaultValue)
        {
            try
            {
                object value = obj[propertyName];
                return value?.ToString()?.Trim() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private long SafeGetInt64(ManagementObject obj, string propertyName, long defaultValue)
        {
            try
            {
                object value = obj[propertyName];
                return value != null ? Convert.ToInt64(value) : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private string GetArchitecture(object? archCode)
        {
            if (archCode == null) return "Unknown";

            try
            {
                return Convert.ToInt32(archCode) switch
                {
                    0 => "x86 (32-bit)",
                    1 => "MIPS",
                    2 => "Alpha",
                    3 => "PowerPC",
                    5 => "ARM",
                    6 => "Itanium",
                    9 => "x64 (64-bit)",
                    12 => "ARM64",
                    _ => "Unknown"
                };
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
