namespace ReelBites
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("dramadetails", typeof(Views.DramaDetailsPage));
            Routing.RegisterRoute("profile", typeof(Views.ProfilePage));
            Routing.RegisterRoute("explore", typeof(Views.ExplorePage));
            Routing.RegisterRoute("editprofile", typeof(Views.EditProfilePage));
            Routing.RegisterRoute("followers", typeof(Views.FollowersPage));
            Routing.RegisterRoute("following", typeof(Views.FollowingPage));
            Routing.RegisterRoute("settings", typeof(Views.SettingsPage));
            Routing.RegisterRoute("login", typeof(Views.LoginPage));
            Routing.RegisterRoute("register", typeof(Views.RegisterPage));
            Routing.RegisterRoute("main", typeof(Views.HomePage));
        }
    }
}
