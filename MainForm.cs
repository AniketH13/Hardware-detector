#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DetectIt
{
    // ─────────────────────────────────────────────────────────────
    // Strongly-typed hardware record returned by each Detect*() method
    // ─────────────────────────────────────────────────────────────
    internal sealed record HardwareData(string Name, TreeNode[] Nodes, string Details);

    public class MainForm : Form
    {
        // ── UI controls ──────────────────────────────────────────
        private TreeView hardwareTree = null!;
        private RichTextBox detailsBox = null!;
        private Button refreshButton = null!;
        private Button exportButton = null!;
        private ProgressBar progressBar = null!;
        private Label statusLabel = null!;
        private Label inspectorTitleLabel = null!;
        private Label wmiBadgeLabel = null!;
        private Label cpuStatValue = null!;
        private Label ramStatValue = null!;
        private Label gpuStatValue = null!;
        private Label osStatValue = null!;

        // ── Theme palette (read-only, defined once) ───────────────
        private static readonly Color BgDark       = Color.FromArgb(15,  16,  20);
        private static readonly Color BgSecondary  = Color.FromArgb(24,  25,  32);
        private static readonly Color BgTertiary   = Color.FromArgb(32,  34,  44);
        private static readonly Color AccentBlue   = Color.FromArgb(10,  132, 255);
        private static readonly Color AccentGreen  = Color.FromArgb(48,  209, 88);
        private static readonly Color AccentPurple = Color.FromArgb(191, 90,  242);
        private static readonly Color AccentAmber  = Color.FromArgb(255, 159, 10);
        private static readonly Color TextPrimary  = Color.FromArgb(242, 242, 247);
        private static readonly Color TextMuted    = Color.FromArgb(142, 142, 147);
        private static readonly Color BorderColor  = Color.FromArgb(44,  46,  58);
        private static readonly Color SelectedBg   = Color.FromArgb(36,  40,  54);

        // ── Cached brushes (disposed in Dispose) ─────────────────
        private readonly SolidBrush _brushBgSecondary  = new(BgSecondary);
        private readonly SolidBrush _brushSelectedBg   = new(SelectedBg);
        private readonly SolidBrush _brushAccentBlue   = new(AccentBlue);

        // ── Cached fonts (disposed in Dispose) ───────────────────
        private readonly Font _fontTree       = new("Segoe UI", 10, FontStyle.Regular);
        private readonly Font _fontTreeBold   = new("Segoe UI", 10, FontStyle.Bold);
        private readonly Font _fontMono       = new("Consolas", 10.5f, FontStyle.Regular);
        private readonly Font _fontMonoBold   = new("Consolas", 10.5f, FontStyle.Bold);
        private readonly Font _fontHeading    = new("Segoe UI", 11,   FontStyle.Bold);

        // ─────────────────────────────────────────────────────────
        public MainForm()
        {
            InitializeLayout();
            CheckAdminPrivileges();
            Shown += async (_, _) => await LoadHardwareInfoAsync();
        }

        // ── Dispose cached GDI resources ─────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _brushBgSecondary.Dispose();
                _brushSelectedBg.Dispose();
                _brushAccentBlue.Dispose();
                _fontTree.Dispose();
                _fontTreeBold.Dispose();
                _fontMono.Dispose();
                _fontMonoBold.Dispose();
                _fontHeading.Dispose();
            }
            base.Dispose(disposing);
        }

        // ── Admin check ──────────────────────────────────────────
        private void CheckAdminPrivileges()
        {
            using var identity = WindowsIdentity.GetCurrent();
            if (!new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator))
            {
                statusLabel.Text      = "⚠️ Run as Administrator for full hardware access";
                statusLabel.ForeColor = AccentAmber;
            }
        }

        // ═════════════════════════════════════════════════════════
        // UI CONSTRUCTION — split into focused helper methods
        // ═════════════════════════════════════════════════════════
        private void InitializeLayout()
        {
            Text            = "DetectIt – Hardware Detector & Diagnostics";
            Size            = new Size(1200, 780);
            MinimumSize     = new Size(960, 660);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = BgDark;
            Font            = new Font("Segoe UI", 9.5f);

            var header       = BuildHeader();
            var sidebar      = BuildSidebar();
            var mainContent  = BuildMainContent();

            Controls.Add(mainContent);   // Fill first (behind sidebar)
            Controls.Add(sidebar);
            Controls.Add(header);
        }

        private Panel BuildHeader()
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 88,
                BackColor = BgSecondary,
                Padding   = new Padding(24, 12, 24, 12)
            };

            // Bottom border line
            panel.Controls.Add(new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = BorderColor
            });

            // Icon badge
            var badge = new Panel { Size = new Size(44, 44), Location = new Point(24, 20), BackColor = AccentBlue };
            badge.Controls.Add(new Label
            {
                Text      = "⚡",
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            panel.Controls.Add(badge);

            panel.Controls.Add(new Label
            {
                Text      = "DetectIt",
                Font      = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize  = true,
                Location  = new Point(80, 13)
            });

            panel.Controls.Add(new Label
            {
                Text      = "v1.0 Pro",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = AccentBlue,
                BackColor = Color.FromArgb(20, 10, 132, 255),
                AutoSize  = true,
                Location  = new Point(197, 20),
                Padding   = new Padding(5, 3, 5, 3)
            });

            panel.Controls.Add(new Label
            {
                Text      = "Hardware Detection & System Diagnostics",
                Font      = new Font("Segoe UI", 9.5f),
                ForeColor = TextMuted,
                AutoSize  = true,
                Location  = new Point(81, 50)
            });

            // Action buttons on the right
            var actions = BuildActionPanel();
            panel.Controls.Add(actions);

            return panel;
        }

        private Panel BuildActionPanel()
        {
            var panel = new Panel { Dock = DockStyle.Right, Width = 360, BackColor = Color.Transparent };

            refreshButton = CreateButton("🔄 Refresh", 20, 24, 140, AccentBlue, Color.FromArgb(40, 150, 255));
            refreshButton.Click += async (_, _) => await LoadHardwareInfoAsync();

            exportButton = CreateButton("📁 Export Report", 172, 24, 158, AccentGreen, Color.FromArgb(68, 220, 108));
            exportButton.Click += ExportToText;

            progressBar = new ProgressBar
            {
                Size     = new Size(320, 4),
                Location = new Point(20, 68),
                Style    = ProgressBarStyle.Marquee,
                Visible  = false
            };

            panel.Controls.Add(refreshButton);
            panel.Controls.Add(exportButton);
            panel.Controls.Add(progressBar);
            return panel;
        }

        private static Button CreateButton(string text, int x, int y, int width, Color bg, Color hover)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(width, 40),
                Location  = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = hover;
            return btn;
        }

        private Panel BuildSidebar()
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 320,
                BackColor = BgSecondary,
                Padding   = new Padding(12)
            };

            // Right border
            panel.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 1, BackColor = BorderColor });

            // Category header label
            panel.Controls.Add(new Label
            {
                Text      = "HARDWARE CATEGORIES",
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = TextMuted,
                Dock      = DockStyle.Top,
                Height    = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(12, 0, 0, 0)
            });

            // Status card (docked bottom, added before TreeView so TreeView fills remaining space)
            var statusCard = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 70,
                BackColor = BgDark,
                Padding   = new Padding(16, 12, 16, 12)
            };
            statusLabel = new Label
            {
                Text      = "Status: Initializing...",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = TextMuted,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            statusCard.Controls.Add(statusLabel);
            panel.Controls.Add(statusCard);

            // TreeView
            hardwareTree = new TreeView
            {
                Dock          = DockStyle.Fill,
                BackColor     = BgSecondary,
                ForeColor     = TextPrimary,
                Font          = _fontTree,
                BorderStyle   = BorderStyle.None,
                ItemHeight    = 44,
                ShowLines     = false,
                FullRowSelect = true,
                HideSelection = false,
                DrawMode      = TreeViewDrawMode.OwnerDrawText
            };
            hardwareTree.DrawNode    += OnDrawTreeNode;
            hardwareTree.AfterSelect += OnTreeNodeSelected;
            panel.Controls.Add(hardwareTree);

            return panel;
        }

        private Panel BuildMainContent()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = BgDark, Padding = new Padding(0) };

            // Inspector header bar
            var inspectorBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = BgSecondary,
                Padding   = new Padding(20, 0, 20, 0)
            };
            inspectorTitleLabel = new Label
            {
                Text      = "💻 System Summary",
                Font      = _fontHeading,
                ForeColor = TextPrimary,
                AutoSize  = true,
                Location  = new Point(20, 16)
            };
            wmiBadgeLabel = new Label
            {
                Text      = "System Overview",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = AccentBlue,
                AutoSize  = true,
                Location  = new Point(340, 19)
            };
            // Bottom border
            inspectorBar.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = BorderColor });
            inspectorBar.Controls.Add(inspectorTitleLabel);
            inspectorBar.Controls.Add(wmiBadgeLabel);

            // Stat cards row
            var statsRow = BuildStatsRow();

            // Details pane wrapper with gap from edges
            var detailsWrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgDark,
                Padding   = new Padding(16, 12, 16, 16)
            };
            var detailsContainer = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgTertiary,
                Padding   = new Padding(20, 16, 20, 16)
            };
            detailsBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = BgTertiary,
                ForeColor   = TextPrimary,
                Font        = _fontMono,
                BorderStyle = BorderStyle.None,
                ReadOnly    = true,
                DetectUrls  = true
            };
            detailsContainer.Controls.Add(detailsBox);
            detailsWrapper.Controls.Add(detailsContainer);

            // Order matters for DockStyle.Fill+Top stacking
            panel.Controls.Add(detailsWrapper);
            panel.Controls.Add(statsRow);
            panel.Controls.Add(inspectorBar);
            return panel;
        }

        private Panel BuildStatsRow()
        {
            var row = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 110,
                BackColor = BgDark,
                Padding   = new Padding(16, 12, 16, 0)
            };

            var grid = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 4,
                RowCount    = 1,
                BackColor   = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            for (int i = 0; i < 4; i++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            grid.Controls.Add(CreateStatCard("PROCESSOR", AccentBlue,   out cpuStatValue), 0, 0);
            grid.Controls.Add(CreateStatCard("MEMORY",    AccentGreen,  out ramStatValue), 1, 0);
            grid.Controls.Add(CreateStatCard("GRAPHICS",  AccentPurple, out gpuStatValue), 2, 0);
            grid.Controls.Add(CreateStatCard("SYSTEM OS", AccentAmber,  out osStatValue),  3, 0);

            row.Controls.Add(grid);
            return row;
        }

        private Panel CreateStatCard(string title, Color accentColor, out Label valueLabel)
        {
            var card = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgSecondary,
                Margin    = new Padding(0, 0, 8, 0),
                Padding   = new Padding(16, 12, 16, 12)
            };
            // Left accent stripe
            var stripe = new Panel { Dock = DockStyle.Left, Width = 3, BackColor = accentColor };
            var titleLbl = new Label
            {
                Text      = title,
                Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = TextMuted,
                Dock      = DockStyle.Top,
                Height    = 18
            };
            valueLabel = new Label
            {
                Text      = "Detecting…",
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = accentColor,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLbl);
            card.Controls.Add(stripe);
            return card;
        }

        // ═════════════════════════════════════════════════════════
        // TREE-VIEW RENDERING (no heap allocations per paint)
        // ═════════════════════════════════════════════════════════
        private void OnDrawTreeNode(object? sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            bool selected = (e.State & TreeNodeStates.Selected) != 0;
            bool isParent = e.Node.Parent == null;

            // Reuse cached brushes
            e.Graphics.FillRectangle(selected ? _brushSelectedBg : _brushBgSecondary, e.Bounds);

            if (selected)
            {
                // Taller accent bar (full height, 4px wide)
                e.Graphics.FillRectangle(_brushAccentBlue,
                    new Rectangle(e.Bounds.Left, e.Bounds.Top, 4, e.Bounds.Height));
            }

            Color foreColor  = selected ? AccentBlue : (isParent ? TextPrimary : TextMuted);
            Font  font       = isParent ? _fontTreeBold : _fontTree;
            int   leftIndent = isParent ? 18 : 40;

            var textRect = new Rectangle(
                e.Bounds.Left + leftIndent,
                e.Bounds.Top,
                e.Bounds.Width - leftIndent - 8,
                e.Bounds.Height);

            TextRenderer.DrawText(e.Graphics, e.Node.Text, font, textRect, foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        // ─────────────────────────────────────────────────────────
        private void OnTreeNodeSelected(object? sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            inspectorTitleLabel.Text = e.Node.Text;
            wmiBadgeLabel.Text = e.Node.Parent != null
                ? $"Category: {e.Node.Parent.Text}"
                : "Component Category";
            if (e.Node.Tag is string details)
                SetFormattedDetails(details);
        }

        // ── Syntax-highlighted detail pane ───────────────────────
        private void SetFormattedDetails(string text)
        {
            detailsBox.SuspendLayout();
            detailsBox.Clear();

            if (string.IsNullOrEmpty(text))
            {
                detailsBox.ResumeLayout();
                return;
            }

            foreach (var line in text.Split('\n'))
            {
                if (line.StartsWith('═') || line.StartsWith('─'))
                {
                    detailsBox.SelectionColor = BorderColor;
                    detailsBox.SelectionFont  = _fontMono;
                    detailsBox.AppendText(line + "\n");
                }
                else if (line.TrimEnd().EndsWith("Information") ||
                         line.StartsWith("Module ", StringComparison.Ordinal) ||
                         line.StartsWith("SYSTEM ", StringComparison.Ordinal))
                {
                    detailsBox.SelectionFont  = _fontHeading;
                    detailsBox.SelectionColor = AccentBlue;
                    detailsBox.AppendText(line + "\n");
                }
                else
                {
                    int colon = line.IndexOf(':');
                    if (colon > 0)
                    {
                        detailsBox.SelectionFont  = _fontMonoBold;
                        detailsBox.SelectionColor = TextMuted;
                        detailsBox.AppendText(line[..(colon + 1)]);

                        detailsBox.SelectionFont  = _fontMono;
                        detailsBox.SelectionColor = TextPrimary;
                        detailsBox.AppendText(line[(colon + 1)..] + "\n");
                    }
                    else
                    {
                        detailsBox.SelectionFont  = _fontMono;
                        detailsBox.SelectionColor = TextPrimary;
                        detailsBox.AppendText(line + "\n");
                    }
                }
            }

            detailsBox.ResumeLayout();
        }

        // ═════════════════════════════════════════════════════════
        // HARDWARE SCAN — parallel WMI queries
        // ═════════════════════════════════════════════════════════
        private async Task LoadHardwareInfoAsync()
        {
            refreshButton.Enabled = false;
            progressBar.Visible   = true;
            statusLabel.Text      = "Scanning WMI hardware components…";
            statusLabel.ForeColor = TextMuted;

            hardwareTree.BeginUpdate();
            hardwareTree.Nodes.Clear();
            detailsBox.Clear();

            try
            {
                // Run all WMI queries concurrently on the thread-pool
                var tasks = new Task<HardwareData>[]
                {
                    Task.Run(DetectCPU),
                    Task.Run(DetectMemory),
                    Task.Run(DetectMotherboard),
                    Task.Run(DetectGPU),
                    Task.Run(DetectDisks),
                    Task.Run(DetectNetworkAdapters),
                    Task.Run(DetectOperatingSystem)
                };
                var results = await Task.WhenAll(tasks);

                var (cpu, mem, mobo, gpu, disk, net, os) =
                    (results[0], results[1], results[2], results[3], results[4], results[5], results[6]);

                // Build overview node
                var summaryNode = new TreeNode("💻 System Overview")
                {
                    Tag = BuildOverview(cpu.Details, mem.Details, gpu.Details, disk.Details, os.Details)
                };
                hardwareTree.Nodes.Add(summaryNode);

                foreach (var data in results)
                    AddTreeNode(data);

                // Update stat cards
                cpuStatValue.Text = ExtractValue(cpu.Details, "Name:");
                ramStatValue.Text = ExtractValue(mem.Details, "Total RAM:");
                gpuStatValue.Text = ExtractValue(gpu.Details, "Name:");
                osStatValue.Text  = ExtractValue(os.Details,  "OS:");

                hardwareTree.SelectedNode = summaryNode;

                statusLabel.Text      = "🟢 Hardware scan complete";
                statusLabel.ForeColor = AccentGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning hardware:\n{ex.Message}",
                    "Hardware Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text      = "🔴 Error during hardware scan";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                hardwareTree.EndUpdate();
                refreshButton.Enabled = true;
                progressBar.Visible   = false;
            }
        }

        private void AddTreeNode(HardwareData data)
        {
            var nodes = data.Nodes.Length > 0
                ? data.Nodes
                : new[] { new TreeNode("No devices found") { ForeColor = TextMuted, Tag = "No devices detected." } };

            var node = new TreeNode(data.Name) { BackColor = BgSecondary, ForeColor = TextPrimary };
            node.Nodes.AddRange(nodes);
            if (!string.IsNullOrEmpty(data.Details))
                node.Tag = data.Details;

            hardwareTree.Nodes.Add(node);
        }

        // ═════════════════════════════════════════════════════════
        // WMI DETECTION — SELECT only needed columns, StringBuilder for strings
        // ═════════════════════════════════════════════════════════
        private static HardwareData DetectCPU()
        {
            var nodes = new List<TreeNode>();
            var sb    = new StringBuilder();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed, Architecture FROM Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                string name    = Get(obj, "Name",     "Unknown CPU");
                string mfg     = Get(obj, "Manufacturer", "Unknown");
                string cores   = Get(obj, "NumberOfCores", "0");
                string logical = Get(obj, "NumberOfLogicalProcessors", "0");
                string clock   = Get(obj, "MaxClockSpeed", "0");
                string arch    = ArchName(obj["Architecture"]);

                sb.Clear();
                sb.AppendLine($"Processor (CPU) Information\n{new string('═', 50)}\n");
                sb.AppendLine($"Name: {name}");
                sb.AppendLine($"Manufacturer: {mfg}");
                sb.AppendLine($"Cores: {cores}");
                sb.AppendLine($"Logical Processors: {logical}");
                sb.AppendLine($"Max Clock Speed: {clock} MHz");
                sb.AppendLine($"Architecture: {arch}");

                nodes.Add(new TreeNode($"🧠 {name}") { Tag = sb.ToString() });
            }

            return new HardwareData("🧠 Processor (CPU)", nodes.ToArray(), sb.ToString());
        }

        private static HardwareData DetectMemory()
        {
            var nodes      = new List<TreeNode>();
            var sb         = new StringBuilder();
            long totalMem  = 0;
            int  module    = 0;

            sb.AppendLine($"Memory (RAM) Information\n{new string('═', 50)}\n");

            using var searcher = new ManagementObjectSearcher(
                "SELECT Capacity, Speed, Manufacturer, PartNumber FROM Win32_PhysicalMemory");

            foreach (ManagementObject obj in searcher.Get())
            {
                module++;
                long cap   = GetLong(obj, "Capacity");
                totalMem  += cap;
                string spd = Get(obj, "Speed", "Unknown");
                string mfg = Get(obj, "Manufacturer", "Unknown");
                string pn  = Get(obj, "PartNumber", "Unknown");
                long   gb  = cap / (1024L * 1024 * 1024);

                sb.AppendLine($"Module {module}:");
                sb.AppendLine($"  Capacity: {gb} GB");
                sb.AppendLine($"  Speed: {spd} MHz");
                sb.AppendLine($"  Manufacturer: {mfg}");
                sb.AppendLine($"  Part Number: {pn}");
                sb.AppendLine();

                var tag = $"Memory Module {module}\n{new string('─', 35)}\nCapacity: {gb} GB\nSpeed: {spd} MHz\nManufacturer: {mfg}\nPart Number: {pn}";
                nodes.Add(new TreeNode($"⚡ Module {module}: {gb} GB") { Tag = tag });
            }

            sb.AppendLine($"Total RAM: {totalMem / (1024L * 1024 * 1024)} GB");
            return new HardwareData("⚡ Memory (RAM)", nodes.ToArray(), sb.ToString());
        }

        private static HardwareData DetectMotherboard()
        {
            var nodes = new List<TreeNode>();
            var sb    = new StringBuilder();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Manufacturer, Product, SerialNumber, Version FROM Win32_BaseBoard");

            foreach (ManagementObject obj in searcher.Get())
            {
                string mfg    = Get(obj, "Manufacturer",  "Unknown");
                string prod   = Get(obj, "Product",       "Unknown");
                string serial = Get(obj, "SerialNumber",  "Unknown");
                string ver    = Get(obj, "Version",       "Unknown");

                sb.Clear();
                sb.AppendLine($"Motherboard & BIOS Information\n{new string('═', 50)}\n");
                sb.AppendLine($"Manufacturer: {mfg}");
                sb.AppendLine($"Product: {prod}");
                sb.AppendLine($"Serial Number: {serial}");
                sb.AppendLine($"Version: {ver}");

                nodes.Add(new TreeNode($"🖥️ {mfg} {prod}") { Tag = sb.ToString() });
            }

            return new HardwareData("🖥️ Motherboard & BIOS", nodes.ToArray(), sb.ToString());
        }

        private static HardwareData DetectGPU()
        {
            var nodes = new List<TreeNode>();
            var sb    = new StringBuilder();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, DriverVersion, VideoProcessor, AdapterRAM, CurrentHorizontalResolution, CurrentVerticalResolution, CurrentRefreshRate FROM Win32_VideoController");

            foreach (ManagementObject obj in searcher.Get())
            {
                string name    = Get(obj, "Name",          "Unknown GPU");
                string driver  = Get(obj, "DriverVersion", "Unknown");
                string vproc   = Get(obj, "VideoProcessor","Unknown");
                long   vram    = GetLong(obj, "AdapterRAM");
                string hres    = Get(obj, "CurrentHorizontalResolution", "N/A");
                string vres    = Get(obj, "CurrentVerticalResolution",   "N/A");
                string refresh = Get(obj, "CurrentRefreshRate",          "N/A");

                sb.Clear();
                sb.AppendLine($"Graphics (GPU) Information\n{new string('═', 50)}\n");
                sb.AppendLine($"Name: {name}");
                sb.AppendLine($"Driver Version: {driver}");
                sb.AppendLine($"Video Processor: {vproc}");
                if (vram > 0) sb.AppendLine($"Video RAM: {vram / (1024 * 1024)} MB");
                if (hres != "N/A" && vres != "N/A")
                {
                    sb.AppendLine($"Current Resolution: {hres}x{vres}");
                    sb.AppendLine($"Refresh Rate: {refresh} Hz");
                }

                nodes.Add(new TreeNode($"🎮 {name}") { Tag = sb.ToString() });
            }

            return new HardwareData("🎮 Graphics (GPU)", nodes.ToArray(), sb.ToString());
        }

        private static HardwareData DetectDisks()
        {
            var nodes = new List<TreeNode>();
            var sb    = new StringBuilder();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Model, InterfaceType, MediaType, Partitions, Size FROM Win32_DiskDrive");

            foreach (ManagementObject obj in searcher.Get())
            {
                string model  = Get(obj, "Model",         "Unknown Disk");
                string iface  = Get(obj, "InterfaceType", "Unknown");
                string media  = Get(obj, "MediaType",     "Unknown");
                string parts  = Get(obj, "Partitions",    "0");
                long   size   = GetLong(obj, "Size");

                sb.Clear();
                sb.AppendLine($"Storage Drive Information\n{new string('═', 50)}\n");
                sb.AppendLine($"Model: {model}");
                sb.AppendLine($"Interface: {iface}");
                if (size > 0) sb.AppendLine($"Size: {size / (1024L * 1024 * 1024)} GB");
                sb.AppendLine($"Media Type: {media}");
                sb.AppendLine($"Partitions: {parts}");

                nodes.Add(new TreeNode($"🖴 {model}") { Tag = sb.ToString() });
            }

            return new HardwareData("🖴 Storage Drives", nodes.ToArray(), sb.ToString());
        }

        private static HardwareData DetectNetworkAdapters()
        {
            var nodes = new List<TreeNode>();
            var sb    = new StringBuilder();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, Manufacturer, MACAddress, Speed FROM Win32_NetworkAdapter WHERE NetConnectionStatus=2");

            foreach (ManagementObject obj in searcher.Get())
            {
                string name = Get(obj, "Name",         "Unknown Adapter");
                string mfg  = Get(obj, "Manufacturer", "Unknown");
                string mac  = Get(obj, "MACAddress",   "N/A");
                string spd  = Get(obj, "Speed",        "N/A");

                sb.Clear();
                sb.AppendLine($"Network Adapter Information\n{new string('═', 50)}\n");
                sb.AppendLine($"Name: {name}");
                sb.AppendLine($"Manufacturer: {mfg}");
                sb.AppendLine($"MAC Address: {mac}");
                sb.AppendLine($"Speed: {spd}");

                nodes.Add(new TreeNode($"🌐 {name}") { Tag = sb.ToString() });
            }

            return new HardwareData("🌐 Network Adapters", nodes.ToArray(), sb.ToString());
        }

        private static HardwareData DetectOperatingSystem()
        {
            var nodes = new List<TreeNode>();
            var sb    = new StringBuilder();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Caption, Version, OSArchitecture, BuildNumber, SystemDirectory, InstallDate FROM Win32_OperatingSystem");

            foreach (ManagementObject obj in searcher.Get())
            {
                string caption  = Get(obj, "Caption",        "Unknown OS");
                string version  = Get(obj, "Version",        "Unknown");
                string arch     = Get(obj, "OSArchitecture", "Unknown");
                string build    = Get(obj, "BuildNumber",    "Unknown");
                string sysDir   = Get(obj, "SystemDirectory","Unknown");
                string rawDate  = Get(obj, "InstallDate",    "");

                sb.Clear();
                sb.AppendLine($"Operating System Information\n{new string('═', 50)}\n");
                sb.AppendLine($"OS: {caption}");
                sb.AppendLine($"Version: {version}");
                sb.AppendLine($"Architecture: {arch}");
                sb.AppendLine($"Build Number: {build}");
                if (!string.IsNullOrEmpty(rawDate))
                {
                    try   { sb.AppendLine($"Install Date: {ManagementDateTimeConverter.ToDateTime(rawDate)}"); }
                    catch { sb.AppendLine("Install Date: Unknown"); }
                }
                sb.AppendLine($"System Directory: {sysDir}");

                nodes.Add(new TreeNode($"🪟 {caption}") { Tag = sb.ToString() });
            }

            return new HardwareData("🪟 Operating System", nodes.ToArray(), sb.ToString());
        }

        // ═════════════════════════════════════════════════════════
        // EXPORT — StringBuilder to avoid repeated concatenation
        // ═════════════════════════════════════════════════════════
        private void ExportToText(object? sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter      = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt  = "txt",
                FileName    = $"DetectIt_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"DetectIt – Hardware Diagnostics Report\n{new string('═', 60)}");
                sb.AppendLine($"Report Generated: {DateTime.Now}\n");

                foreach (TreeNode category in hardwareTree.Nodes)
                {
                    sb.AppendLine($"\n{category.Text}");
                    sb.AppendLine(new string('─', 60));
                    if (category.Tag is string catTag) sb.AppendLine(catTag);

                    foreach (TreeNode item in category.Nodes)
                        if (item.Tag is string itemTag) sb.AppendLine(itemTag);
                }

                File.WriteAllText(dialog.FileName, sb.ToString());
                MessageBox.Show("Report exported successfully!", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed:\n{ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═════════════════════════════════════════════════════════
        // HELPERS
        // ═════════════════════════════════════════════════════════
        private static string BuildOverview(string cpu, string mem, string gpu, string disk, string os)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SYSTEM HARDWARE OVERVIEW\n{new string('═', 55)}\n");
            sb.AppendLine($"Processor: {ExtractValue(cpu, "Name:")}");
            sb.AppendLine($"System RAM: {ExtractValue(mem, "Total RAM:")}");
            sb.AppendLine($"Graphics Card: {ExtractValue(gpu, "Name:")}");
            sb.AppendLine($"Primary Storage: {ExtractValue(disk, "Model:")}");
            sb.AppendLine($"Operating System: {ExtractValue(os, "OS:")}");
            sb.AppendLine("\nStatus: All components verified & accessible via WMI");
            return sb.ToString();
        }

        private static string ExtractValue(string details, string key, string fallback = "N/A")
        {
            if (string.IsNullOrEmpty(details)) return fallback;
            foreach (var line in details.Split('\n'))
            {
                var trimmed = line.TrimStart();
                if (!trimmed.StartsWith(key, StringComparison.Ordinal)) continue;
                int idx = trimmed.IndexOf(key, StringComparison.Ordinal);
                return trimmed[(idx + key.Length)..].Trim();
            }
            return fallback;
        }

        private static string Get(ManagementObject obj, string prop, string def = "")
        {
            try { return obj[prop]?.ToString()?.Trim() ?? def; }
            catch { return def; }
        }

        private static long GetLong(ManagementObject obj, string prop)
        {
            try { return obj[prop] is { } v ? Convert.ToInt64(v) : 0L; }
            catch { return 0L; }
        }

        private static string ArchName(object? code)
        {
            if (code == null) return "Unknown";
            try
            {
                return Convert.ToInt32(code) switch
                {
                    0  => "x86 (32-bit)",
                    5  => "ARM",
                    9  => "x64 (64-bit)",
                    12 => "ARM64",
                    _  => $"Code {code}"
                };
            }
            catch { return "Unknown"; }
        }
    }
}
