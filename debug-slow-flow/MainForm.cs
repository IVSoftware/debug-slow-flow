using System.Windows.Forms;

namespace debug_slow_flow
{
    public partial class MainForm : Form, IMessageFilter
    {
        public MainForm()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            Disposed += (sender, e) =>
            {
                Application.RemoveMessageFilter(this);
            };
            flowLayoutPanel1 = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.Azure,
                AutoScroll = true,
            };
            Controls.Add(flowLayoutPanel1);
            ClickAnywhere += (sender, e) =>
            {
                if(sender is Control control)
                {
                    var message = 
                        string.IsNullOrWhiteSpace(control.Text) ?
                            control.Name : control.Text;
                    // Better to not block the click event
                    // for a modal dialog so use BeginInvoke.
                    BeginInvoke(() => MessageBox.Show(message));
                }
            };
        }
        FlowLayoutPanel flowLayoutPanel1;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string[] panNames = { "John", "Doe", "Alice", "Lorem", "Ipsum", "Sudo", "Append", "Concat", "Java", "C#" };
            int[] ages = { 20, 50, 25, 55, 29, 36, 47, 23, 41, 21 };

            for (int i = 0; i < panNames.Length; i++)
            {
                Panel panChild = new Panel
                { 
                    Name = $"{nameof(panChild)}{i}",
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                };
                Label lblAge = new Label { Name = $"{nameof(lblAge)}{i}" };
                flowLayoutPanel1.Controls.Add(panChild);
                panChild.Controls.Add(new Label
                {
                    Name = $"lblAge{i}",
                    Text = $"Age {ages[i]}",
                    Location = new Point(4, 4),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(2),
                    AutoSize = true
                }); ;
                panChild.Controls.Add(new Label
                {
                    Name = $"lblName{i}",
                    Text = panNames[i],
                    Location = new Point(63, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(2),
                    AutoSize = true
                });
#if DEBUG
                // For debug, make it obvious where the label rectangles lie.
                foreach (var label in panChild.Controls.OfType<Label>())
                    label.BackColor = Color.LightCyan;
#endif
            }
            flowLayoutPanel1.VerticalScroll.Visible = true;
        }
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN && Control.FromHandle(m.HWnd) is Control control)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    ClickAnywhere?.Invoke(control, EventArgs.Empty);
                });
            }
            return false;
        }
        public event EventHandler ClickAnywhere;
        private const int WM_LBUTTONDOWN = 0x0201;
    }
}
