namespace VAT_Back
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // This sets the starting page to our new Role Selection hub
            MainPage = new NavigationPage(new RoleSelectionPage());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // We return a window using the MainPage we defined in the constructor
            return new Window(MainPage);
        }
    }
}