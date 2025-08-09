using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_dnc_se1.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlContent DisplayUploadOrText(this IHtmlHelper htmlHelper, object value)
        {
            if (value is string strValue && strValue.Contains("/uploads/"))
            {
                var html = new HtmlContentBuilder();
                var parts = strValue.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var imgPath in parts)
                {
                    var trimmed = imgPath.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        html.AppendHtml($"<img src=\"{trimmed}\" style=\"width:100px; height:50x; object-fit: cover; margin:3px\" />");
                    }
                }

                return html;
            }

            // Nếu không phải đường dẫn ảnh thì in chuỗi bình thường
            return new HtmlString($"<p>{value?.ToString()}</p>");
        }
    }
}
