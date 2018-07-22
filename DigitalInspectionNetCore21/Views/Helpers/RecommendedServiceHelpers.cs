using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalInspectionNetCore21.Views.Helpers
{
	public static class RecommendedServiceHelpers
	{

		public static HtmlString Immediate(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "warning", "Immediate", "immediate");
		}

		public static HtmlString Moderate(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "alarm", "Moderate", "moderate");
		}

		public static HtmlString Watch(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "remove_red_eye", "Should watch", "watch");
		}

		public static HtmlString Maintenance(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "date_range", "Maintenance", "maintenance");
		}

		public static HtmlString Notes(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "description", "Notes", "notes");
		}

		public static HtmlString Unknown(bool hasIcon, bool hasLabel)
		{
			return CreateIndicator(hasIcon, hasLabel, "assignment_late", "Unknown", "unknown");
		}

		private static HtmlString CreateIndicator(
			bool hasIcon, bool hasLabel, string iconName, string labelText, string cssClass)
		{
			TagBuilder container = new TagBuilder("div");
			if (hasIcon)
			{
				var icon = CreateIcon(iconName);
				icon.AddCssClass(cssClass);
				// container.InnerHtml += icon.ToString();
			}

			if (hasLabel)
			{
				var label = CreateLabel(labelText);
				label.AddCssClass(cssClass);
				// container.InnerHtml += label.ToString();
			}

			return new HtmlString(container.ToString());
		}

		private static TagBuilder CreateIcon(string iconName)
		{
			var icon = new TagBuilder("i");
			icon.AddCssClass("material-icons recommended-service");
			// icon.InnerHtml = iconName;
			return icon;
		}

		private static TagBuilder CreateLabel(string labelText)
		{
			var label = new TagBuilder("span");
			label.AddCssClass("label recommended-service");
			// label.InnerHtml = labelText;
			return label;
		}
	}
}
