using System.Text;
using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace WinFormsApp;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    private void CreateSaveButton()
        {
            saveButton = new Button
            {
                Text = "保 存",
                Size = new Size(120, 35),
                Location = new Point(250, 20),
                Font = new Font("Microsoft YaHei", 10),
                BackColor = Color.LightSkyBlue
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);
        }

        // 输入验证检查
        private bool ValidateInput()
        {
            var ly = (TextBox)tableLayout.Controls[4];
            var grade1 = (TextBox)tableLayout.Controls[5];
            var nf = (TextBox)tableLayout.Controls[6];
            var grade2 = (TextBox)tableLayout.Controls[7];

            return ly.TextLength == 12 &&
                   grade1.TextLength == 1 &&
                   nf.TextLength == 11 &&
                   grade2.TextLength == 1;
        }

        // 保存事件处理
        private void SaveButton_Click(object sender, EventArgs e) => SaveData();
        private void Input_TextChanged(object sender, EventArgs e) => TryAutoSave();

        private void TryAutoSave()
        {
            if (ValidateInput()) SaveData();
        }

        private void SaveData()
        {
            var lyBox = (TextBox)tableLayout.Controls[4];
            var grade1Box = (TextBox)tableLayout.Controls[5];
            var nfBox = (TextBox)tableLayout.Controls[6];
            var grade2Box = (TextBox)tableLayout.Controls[7];

            // 检查重复
            if (CheckDuplicate(lyBox.Text))
            {
                MessageBox.Show("发现重码，请检查", "数据冲突", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 生成新记录
            var newRecord = new StringBuilder();
            newRecord.Append($"{GetNextID()},");        // 序号
            newRecord.Append($"{lyBox.Text},");        // LY
            newRecord.Append($"{grade1Box.Text},");     // Grade1
            newRecord.Append($"{nfBox.Text},");         // NF
            newRecord.Append($"{grade2Box.Text},");     // Grade2
            newRecord.Append($"{comboBox.SelectedItem},"); // 型号
            newRecord.Append(dateLabel.Text);           // 时间

            // 写入文件
            try
            {
                bool isNewFile = !File.Exists("MGP003.csv");
                using (var sw = new StreamWriter("MGP003.csv", true, Encoding.UTF8))
                {
                    if (isNewFile) sw.WriteLine("序号,LY,Grade,NF,Grade,型号,时间");
                    sw.WriteLine(newRecord);
                }

                // 清空输入
                lyBox.Clear();
                grade1Box.Clear();
                nfBox.Clear();
                grade2Box.Clear();
                lyBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 获取下个序号
        private int GetNextID()
        {
            if (!File.Exists("MGP003.csv")) return 1;
            
            var lastLine = File.ReadLines("MGP003.csv").LastOrDefault();
            if (string.IsNullOrEmpty(lastLine)) return 1;
            
            return int.TryParse(lastLine.Split(',')[0], out int id) ? id + 1 : 1;
        }

        // 查重逻辑
        private bool CheckDuplicate(string lyValue)
        {
            if (!File.Exists("MGP003.csv")) return false;

            return File.ReadLines("MGP003.csv")
                .Skip(1) // 跳过标题行
                .Any(line => line.Split(',').Length > 1 && 
                           line.Split(',')[1] == lyValue);
        }
    private void SetupForm()
    {
        this.ClientSize = new Size(800, 800);
        this.Text = "LOGO Grade Input";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        //this.Load += MainForm_Load;
    }
    private void CreateDateLabel()
    {
        dateLabel = new Label
        {
            AutoSize = true,
            //Text = DateTime.Now.ToString("yyyy-MM-dd"),
            Font = new Font("Microsoft Sans Serif", 10),
            BackColor = Color.Transparent
        };
        this.Controls.Add(dateLabel);
        UpdateDateTime(); // 初始化时间显示
    }
    private void SetupTimer()
    {
        timer = new Timer
        {
            Interval = 1000 // 每秒更新一次
        };
        timer.Tick += (s, e) => UpdateDateTime();
        timer.Start();
    }
    
    private void UpdateDateTime()
    {
        dateLabel.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        // 保持右对齐
        dateLabel.Location = new Point(
            this.ClientSize.Width - dateLabel.Width - 20,
            20
        );
    }
    private void CreateComboBox()
    {
        comboBox = new ComboBox
        {
            Location = new Point(20, 20),
            Size = new Size(200, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        comboBox.Items.AddRange(new object[] { "ANT602", "ANT603", "ANT605" });
        comboBox.SelectedIndex = 0;
        this.Controls.Add(comboBox);
    }
    private void CreateInputFields()
    {
        tableLayout = new TableLayoutPanel
        {
            ColumnCount = 4,
            RowCount = 2,
            Location = new Point(20, 100),
            Size = new Size(760, 60),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // 设置列宽
        for (int i = 0; i < 4; i++)
        {
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        }

        // 设置行高
        tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // 标签行
        tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F)); // 输入框行

        // 添加标签
        string[] labels = { "LY", "Grade", "NF", "Grade" };
        foreach (string label in labels)
        {
            var lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            tableLayout.Controls.Add(lbl);
        }

        // 添加文本框
        for (int i = 0; i < 4; i++)
        {
            var txt = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 0),
                Font = new Font("Microsoft Sans Serif", 9)
            };
            switch (i)
            {
                case 0: // LY
                    txt.MaxLength = 12;
                    txt.KeyPress += LyNf_KeyPress;
                    txt.TextChanged += LyNf_TextChanged;
                    break;
                case 1: // Grade1
                case 3: // Grade2
                    txt.MaxLength = 1;
                    txt.KeyPress += Grade_KeyPress;
                    txt.TextChanged += Grade_TextChanged;
                    break;
                case 2: // NF
                    txt.MaxLength = 11;
                    txt.KeyPress += LyNf_KeyPress;
                    txt.TextChanged += LyNf_TextChanged;
                    break;
            }
            tableLayout.Controls.Add(txt);
        }

        this.Controls.Add(tableLayout);
    }
    // LY 和 NF 的输入验证
    private void LyNf_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
        {
            e.Handled = true;
        }
    }
    // Grade 的输入验证
    private void Grade_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
        {
            e.Handled = true;
        }
    }
    // LY 和 NF 的文本修正
    private void LyNf_TextChanged(object sender, EventArgs e)
    {
        var txt = sender as TextBox;
        string cleanText = Regex.Replace(txt.Text, "[^A-Z0-9]", "");
        if (txt.Text != cleanText)
        {
            int pos = txt.SelectionStart;
            txt.Text = cleanText;
            txt.SelectionStart = pos > cleanText.Length ? cleanText.Length : pos;
        }
    }

    // Grade 的文本修正
    private void Grade_TextChanged(object sender, EventArgs e)
    {
        var txt = sender as TextBox;
        string cleanText = Regex.Replace(txt.Text, "[^A-Z]", "");
        if (cleanText.Length > 1) cleanText = cleanText.Substring(0, 1);
        if (txt.Text != cleanText)
        {
            int pos = txt.SelectionStart;
            txt.Text = cleanText;
            txt.SelectionStart = pos > cleanText.Length ? cleanText.Length : pos;
        }
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        // 调整日期标签位置
        dateLabel.Location = new Point(
            this.ClientSize.Width - dateLabel.Width - 20,
            20
        );
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "MainForm";
    }

    #endregion
}