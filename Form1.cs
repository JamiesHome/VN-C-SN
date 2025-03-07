using Timer = System.Windows.Forms.Timer;

namespace WinFormsApp;

public partial class MainForm : Form
{
    private Label dateLabel;
    private ComboBox comboBox;
    private TableLayoutPanel tableLayout;
    private Timer timer;
    public MainForm()
    {
        InitializeComponent();
        SetupForm();
        CreateDateLabel();
        CreateComboBox();
        CreateInputFields();
        SetupTimer();
    }
}