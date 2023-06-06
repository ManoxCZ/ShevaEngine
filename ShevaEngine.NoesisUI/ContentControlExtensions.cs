namespace Noesis;

public static class ContentControlExtensions
{
    public static void InitializeComponent(this UserControl contentControl)
    {
        if (contentControl.GetType().FullName is string fullName)
        {
            string xamlName = fullName.Substring(fullName.IndexOf('.') + 1) + ".xaml";

            GUI.LoadComponent(contentControl, xamlName);
        }
    }
}
