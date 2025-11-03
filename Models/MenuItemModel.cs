using System.Windows.Media;

namespace WpfApp.Models
{
    /// <summary>
    /// 메뉴 항목 데이터 모델
    /// </summary>
    public class MenuItemModel
    {
        public string IconPath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Color IconColor { get; set; } = Colors.DodgerBlue;

        public MenuItemModel(string iconPath, string title, string description, Color iconColor)
        {
            IconPath = iconPath;
            Title = title;
            Description = description;
            IconColor = iconColor;
        }
    }
}
