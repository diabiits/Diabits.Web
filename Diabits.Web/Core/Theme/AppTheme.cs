using MudBlazor;

namespace Diabits.Web.Core.Theme;

public static class AppTheme
{
    public static MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#AC0000",
            Secondary = "#EF88AD",
            Tertiary = "#700507",

            AppbarBackground = "#AC0000",
            DrawerBackground = "#FFFFFF",
            Background = "#FFFFFF",
            Surface = "#FFFFFF",

            TextPrimary = "#1E1E1E",
            TextSecondary = "#6B6B6B"
        },

        //LayoutProperties = new LayoutProperties
        //{
        //    DefaultBorderRadius = "16px"
        //}
    };
}
