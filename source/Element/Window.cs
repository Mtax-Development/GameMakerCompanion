using System.Windows.Forms;
using System.Diagnostics;

namespace GMS2_RPC.Element
{
    /// <summary> Class that contains configurations of customized windows shown by the application. </summary>
    internal static class Window
    {
        internal class About : Form
        {
            /// <summary> Structure containing all events that the windows can trigger. </summary>
            internal readonly struct Event
            {
                internal static void Label_Link_Repository_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
                {
                    Process.Start(Path.url_projectRepository);
                }
            }

            /// <summary> Configuration of the "About" window. </summary>
            /// <remarks> Displayed by the tray icon. </remarks>
            internal About()
            {
                System.Drawing.Size size = new System.Drawing.Size(440, 170);

                SuspendLayout();

                Label Label_HeadText = new Label()
                {
                    Name = "Label_HeadText",
                    Text = UserText.About.head_text,
                    Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                             | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right))),
                    AutoSize = true,
                    Location = new System.Drawing.Point(20, 10),
                    Size = new System.Drawing.Size(300, 15),
                    TabIndex = 0
                };

                LinkLabel Label_Link_Repository = new LinkLabel()
                {
                    Name = "Label_Link_Repository",
                    Text = UserText.About.repository_text,
                    AutoSize = true,
                    Location = new System.Drawing.Point(20, 35),
                    Size = new System.Drawing.Size(105, 15),
                    TabIndex = 1,
                    TabStop = true
                };

                Label_Link_Repository.LinkClicked += new LinkLabelLinkClickedEventHandler(Event.Label_Link_Repository_LinkClicked);

                Label Label_LegalClause = new Label()
                {
                    Name = "Label_LegalClause",
                    Text = UserText.About.legalClause_text,
                    AutoSize = true,
                    Location = new System.Drawing.Point(20, 70),
                    Size = new System.Drawing.Size(375, 45),
                    TabIndex = 2
                };

                Controls.Add(Label_LegalClause);
                Controls.Add(Label_Link_Repository);
                Controls.Add(Label_HeadText);

                Name = UserText.About.window_title;
                Text = UserText.About.window_title;
                Icon = Resource.projectIcon;

                TopMost = true;
                ShowInTaskbar = false;

                MinimizeBox = false;
                MaximizeBox = false;

                MinimumSize = size;
                MaximumSize = size;
                ClientSize = size;

                AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                FormBorderStyle = FormBorderStyle.FixedSingle;

                ResumeLayout(false);

                PerformLayout();
            }
        }
    }
}
