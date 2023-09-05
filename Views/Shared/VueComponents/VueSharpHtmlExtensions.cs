using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesWithVue.Views.Shared.VueComponents;

public static class HtmlExtensions
{
  //拓展public interface IHtmlHelper<TModel> : IHtmlHelper, 让用户可以直接调用 @Html.VueComponent(string)来输出
  /// <summary>
  /// 输出vue组件
  /// </summary>
  /// <param name="htmlHelper"></param>
  /// <param name="componentName">存储在View/Shared/VueComponents/的.vue文件的名字.必须要携带.vue   如存在header.vue传递值为: header</param>
  /// <returns></returns>
  public static async Task<IHtmlContent> VueComponent(this IHtmlHelper htmlHelper, string componentName)
  {
    //读取cm.vue文件
    var content = await System.IO.File.ReadAllTextAsync(Path.Combine("Views", "Shared", "VueComponents", $"{componentName}.vue"));
    //替换掉template标签
    content = content.Replace("<template ", "<script ");
    //替换掉template标签结尾
    content = content.Replace("</template>", "</script>");
    //去掉 export default xxx;
    content = System.Text.RegularExpressions.Regex.Replace(content, @"export default \w+;", "");
    //Console.WriteLine(content);
    //输出
    return new HtmlString(content);
  }
}
