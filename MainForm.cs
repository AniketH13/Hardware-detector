using System;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using System.Threading.Tasks;

namespace HardwareDetector
{
    public class MainForm : Form
    {
        private TreeView hardwareTree;
        private RichTextBox detailsBox;
        private Button refreshButton;
        private Button exportButton;
        private ProgressBar progressBar;
        private Label statusLabel;

        public MainForm()
        {
            InitializeComponents();
            CheckAdminPrivileges();
            Shown += async (s, e) => await LoadHardwareInfo();
        }

        private void CheckAdminPrivileges()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                if (!principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {
                    statusLabel.Text = "Note: Run as Administrator for full hardware access";
                    statusLabel.ForeColor = Color.FromArgb(255, 200, 100);
                }
            }
        }

        private void InitializeComponents()
        {
            // Main form setup
            this.Text = "DetectIt - Hardware Detector";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // Header panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(41, 128, 185),
                Padding = new Padding(20, 10, 20, 10)
            };

            Label titleLabel = new Label
            {
                Text = "DetectIt",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            statusLabel = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 45)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statusLabel);

            // Button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            refreshButton = new Button
            {
                Text = "Refresh",
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += async (s, e) => await LoadHardwareInfo();

            exportButton = new Button
            {
                Text = "Export",
                Size = new Size(120, 30),
                Location = new Point(140, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            exportButton.FlatAppearance.BorderSize = 0;
            exportButton.Click += ExportToText;

            progressBar = new ProgressBar
            {
                Size = new Size(200, 25),
                Location = new Point(280, 12),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            buttonPanel.Controls.Add(refreshButton);
            buttonPanel.Controls.Add(exportButton);
            buttonPanel.Controls.Add(progressBar);

            // TreeView for categories
            hardwareTree = new TreeView
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ItemHeight = 30,
                ShowLines = false,
                FullRowSelect = true,
                HideSelection = false,
                DrawMode = TreeViewDrawMode.OwnerDrawText
            };
            hardwareTree.DrawNode += TreeViewDrawNode;
            hardwareTree.AfterSelect += TreeViewSelected;

            // Details panel
            Panel detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            detailsBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = new Font("Consolas", 11),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                DetectUrls = true
            };

            detailsPanel.Controls.Add(detailsBox);

            // Splitter
            Splitter splitter = new Splitter
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = Color.FromArgb(220, 220, 220)
            };

            // Add controls to form
            this.Controls.Add(detailsPanel);
            this.Controls.Add(splitter);
            this.Controls.Add(hardwareTree);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(headerPanel);
        }

        private void TreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            bool selected = (e.State & TreeNodeStates.Selected) != 0;
            Color backColor = selected ? Color.FromArgb(52, 152, 219) : Color.White;
            Color foreColor = selected ? Color.White : Color.FromArgb(50, 50, 50);

            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);

            using (StringFormat sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(e.Node.Text, hardwareTree.Font, new SolidBrush(foreColor), e.Bounds, sf);
            }
        }

        private void TreeViewSelected(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                detailsBox.Text = e.Node.Tag.ToString();
            }
        }

        private async Task LoadHardwareInfo()
        {
            try
            {
                refreshButton.Enabled = false;
                progressBar.Visible = true;
                statusLabel.Text = "Scanning hardware...";

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
                        AddTreeNode(cpuData);
                        AddTreeNode(memoryData);
                        AddTreeNode(motherboardData);
                        AddTreeNode(gpuData);
                        AddTreeNode(diskData);
                        AddTreeNode(networkData);
                        AddTreeNode(osData);
                    });
                });

                statusLabel.Text = "Hardware scan complete";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning hardware: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error during scan";
            }
            finally
            {
                refreshButton.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void AddTreeNode((string name, TreeNode[] nodes, string details) data)
        {
            if (data.nodes == null || data.nodes.Length == 0)
            {
                // Add "No devices found" message
                TreeNode emptyNode = new TreeNode("No devices found")
                {
                    ForeColor = Color.Gray,
                    Tag = "No devices were detected in this category."
                };
                data.nodes = new TreeNode[] { emptyNode };
            }

            TreeNode node = new TreeNode(data.name)
            {
                BackColor = Color.FromArgb(230, 240, 250)
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
            var nodes = new System.Collections.Generic.List<TreeNode>();
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

                    details = $"CPU Information\n{new string('═', 50)}\n\n";
                    details += $"Name: {name}\n";
                    details += $"Manufacturer: {manufacturer}\n";
                    details += $"Cores: {cores}\n";
                    details += $"Logical Processors: {logicalProcessors}\n";
                    details += $"Max Clock Speed: {clockSpeed} MHz\n";
                    details += $"Architecture: {architecture}\n";

                    TreeNode item = new TreeNode(name)
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("CPU Information", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectMemory()
        {
            var nodes = new System.Collections.Generic.List<TreeNode>();
            long totalMemory = 0;
            int moduleCount = 0;
            string details = $"Memory Information\n{new string('═', 50)}\n\n";

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

                    TreeNode item = new TreeNode($"Module {moduleCount}: {capacityGB} GB")
                    {
                        Tag = $"Module {moduleCount}\n{new string('─', 30)}\n" +
                              $"Capacity: {capacityGB} GB\n" +
                              $"Speed: {speed} MHz\n" +
                              $"Manufacturer: {manufacturer}\n" +
                              $"Part Number: {partNumber}"
                    };
                    nodes.Add(item);
                }
            }

            details += $"\nTotal RAM: {totalMemory / (1024 * 1024 * 1024)} GB";
            return ("Memory Information", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectMotherboard()
        {
            var nodes = new System.Collections.Generic.List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string manufacturer = SafeGetProperty(obj, "Manufacturer", "Unknown");
                    string product = SafeGetProperty(obj, "Product", "Unknown");
                    string serialNumber = SafeGetProperty(obj, "SerialNumber", "Unknown");
                    string version = SafeGetProperty(obj, "Version", "Unknown");

                    details = $"Motherboard Information\n{new string('═', 50)}\n\n";
                    details += $"Manufacturer: {manufacturer}\n";
                    details += $"Product: {product}\n";
                    details += $"Serial Number: {serialNumber}\n";
                    details += $"Version: {version}\n";

                    TreeNode item = new TreeNode($"{manufacturer} {product}")
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("Motherboard Information", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectGPU()
        {
            var nodes = new System.Collections.Generic.List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = SafeGetProperty(obj, "Name", "Unknown GPU");
                    string driverVersion = SafeGetProperty(obj, "DriverVersion", "Unknown");
                    string videoProcessor = SafeGetProperty(obj, "VideoProcessor", "Unknown");

                    details = $"GPU Information\n{new string('═', 50)}\n\n";
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

                    TreeNode item = new TreeNode(name)
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("GPU Information", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectDisks()
        {
            var nodes = new System.Collections.Generic.List<TreeNode>();
            string details = "";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string model = SafeGetProperty(obj, "Model", "Unknown Disk");
                    string interfaceType = SafeGetProperty(obj, "InterfaceType", "Unknown");
                    string mediaType = SafeGetProperty(obj, "MediaType", "Unknown");
                    string partitions = SafeGetProperty(obj, "Partitions", "0");

                    details = $"Disk Information\n{new string('═', 50)}\n\n";
                    details += $"Model: {model}\n";
                    details += $"Interface: {interfaceType}\n";

                    long size = SafeGetInt64(obj, "Size", 0);
                    if (size > 0)
                    {
                        details += $"Size: {size / (1024 * 1024 * 1024)} GB\n";
                    }

                    details += $"Media Type: {mediaType}\n";
                    details += $"Partitions: {partitions}\n";

                    TreeNode item = new TreeNode(model)
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("Disk Information", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectNetworkAdapters()
        {
            var nodes = new System.Collections.Generic.List<TreeNode>();
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

                    details = $"Network Adapter\n{new string('═', 50)}\n\n";
                    details += $"Name: {name}\n";
                    details += $"Manufacturer: {manufacturer}\n";
                    details += $"MAC Address: {macAddress}\n";
                    details += $"Speed: {speed}\n";

                    TreeNode item = new TreeNode(name)
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("Network Adapters", nodes.ToArray(), details);
        }

        private (string name, TreeNode[] nodes, string details) DetectOperatingSystem()
        {
            var nodes = new System.Collections.Generic.List<TreeNode>();
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

                    details = $"Operating System\n{new string('═', 50)}\n\n";
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

                    TreeNode item = new TreeNode(caption)
                    {
                        Tag = details
                    };
                    nodes.Add(item);
                }
            }

            return ("Operating System", nodes.ToArray(), details);
        }

        private void ExportToText(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = $"HardwareInfo_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string content = $"Hardware Information Report\n{new string('═', 60)}\n";
                    content += $"Generated: {DateTime.Now}\n\n";

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

                    System.IO.File.WriteAllText(saveDialog.FileName, content);
                    MessageBox.Show("Hardware information exported successfully!", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting: {ex.Message}", "Export Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string SafeGetProperty(ManagementObject obj, string propertyName, string defaultValue)
        {
            try
            {
                object value = obj[propertyName];
                return value?.ToString() ?? defaultValue;
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

        private string GetArchitecture(object archCode)
        {
            if (archCode == null) return "Unknown";

            try
            {
                return Convert.ToInt32(archCode) switch
                {
                    0 => "x86",
                    1 => "MIPS",
                    2 => "Alpha",
                    3 => "PowerPC",
                    5 => "ARM",
                    6 => "Itanium",
                    9 => "x64",
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
