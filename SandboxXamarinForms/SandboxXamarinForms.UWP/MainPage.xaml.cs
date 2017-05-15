using System;

namespace SandboxXamarinForms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new SandboxXamarinForms.App());
        }
    }
}
