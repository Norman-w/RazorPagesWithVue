using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesWithVue.Views.Shared.VueComponents;

public static class HtmlExtensions
{

  /// <summary>
  /// 可能的vue存储路径
  /// </summary>
  /// <returns></returns>
  private static readonly List<string> VueFilePathList = new()
  {
    Path.Combine("Views", "Shared", "VueComponents"), Path.Combine("Views", "Shared"),"Views",
  };

  /// <summary>
  /// 缓存的vue组件内容,避免重复加载
  /// </summary>
  private static readonly Dictionary<string, string> BufferedContentDic = new();

  /// <summary>
  /// 搜索vue文件
  /// </summary>
  /// <param name="componentName">组件名称,可以带扩展名</param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException">当找遍了所有可能的位置都没有找到这个vue文件时发生</exception>
  private static string SearchVueFile(string componentName)
  {
    if (string.IsNullOrWhiteSpace(componentName)) 
      throw new ArgumentNullException(nameof(componentName));
    componentName = componentName.Replace(".vue", "");
    //在可能的路径中搜索
    foreach (var path in VueFilePathList)
    { 
      var vueFilePath = Path.Combine(path, $"{componentName}.vue");
      if (File.Exists(vueFilePath))
      {
        return vueFilePath;
      }
    }
    return null;
  }

  /// <summary>
  /// 获取引用的组件和组件的路径
  /// </summary>
  /// <param name="scriptContent"></param>
  /// <returns></returns>
  private static Dictionary<string, string> GetComponentsUsingDic(ref string scriptContent)
  {
    Dictionary<string,string> tagTypeAndPathDic = new();
    //找到这个文件中所有的import xxx from 'xxx';
    var matchCollection = System.Text.RegularExpressions.Regex.Matches(scriptContent, @"import \w+ from \"".+?\"";");
    //遍历匹配结果
    foreach (System.Text.RegularExpressions.Match match in matchCollection)
    {
      //从value中提取 import 和 from 之间的内容
      var tagType = match.Value.Substring(match.Value.IndexOf("import ", StringComparison.Ordinal) + 7,
        match.Value.IndexOf("from", StringComparison.Ordinal) - 8);
      var tagPath = match.Value.Substring(match.Value.IndexOf("from", StringComparison.Ordinal) + 5,
        match.Value.Length - match.Value.IndexOf("from", StringComparison.Ordinal) - 6);
      tagPath = tagPath.Trim('\"');
      //如果字典中不存在该标签,则添加
      tagTypeAndPathDic.TryAdd(tagType, tagPath);
      scriptContent = scriptContent.Replace(match.Value, "");
    }
    return tagTypeAndPathDic;
  }

  
  private static async Task<IHtmlContent> LoadVueComponent(this IHtmlHelper htmlHelper, string componentFullPath)
  {
    if(string.IsNullOrWhiteSpace(componentFullPath)) throw new ArgumentNullException(nameof(componentFullPath));
    
    var contentBuilder = new StringBuilder();
    var thisComponentContent =
      await File.ReadAllTextAsync(componentFullPath);
    //替换掉template标签
    thisComponentContent = thisComponentContent.Replace("<template ", "<script ");
    //替换掉template标签结尾
    thisComponentContent = thisComponentContent.Replace("</template>", "</script>");
    //去掉 export default xxx;
    thisComponentContent =
      System.Text.RegularExpressions.Regex.Replace(thisComponentContent, @"export default \w+;", "");
    thisComponentContent = thisComponentContent.Replace("export default", "");
    var usingComponentsDic = GetComponentsUsingDic(ref thisComponentContent);
    //通过匹配到的tag类型,确认要使用Html.PartialAsync加载哪些文件
    foreach (var tagType in usingComponentsDic)
    {
      #region vue文件的路径,通过路径从缓存中取出文件内容,如果缓存中不存在,则加载文件内容
      //根据componentFullPath的路径和 类似于 ./xxx.vue 的路径,拼接出vue文件的路径
      var vueFilePath = Path.Combine(Path.GetDirectoryName(componentFullPath) ?? string.Empty, tagType.Value);
      if (BufferedContentDic.TryGetValue(vueFilePath, out var value))
      {
        contentBuilder.Append(value);
        continue;
      }
      #endregion
      #region 如果文件不存在,则抛出异常
      if (!File.Exists(vueFilePath))
      {
        throw new Exception($"文件{vueFilePath}不存在");
      }
      #endregion
      #region 加载对应的文件,并将文件内容存储到缓存中
      var partialContent = await htmlHelper.LoadVueComponent(vueFilePath);
      BufferedContentDic.Add(vueFilePath, partialContent.ToString());
      #endregion
      //将加载到的文件添加到contentBuilder中
      contentBuilder.Append(partialContent);
    }

    //将当前组件的内容添加到contentBuilder中
    contentBuilder.Append(thisComponentContent);

    //Console.WriteLine(content);
    //输出
    return new HtmlString(contentBuilder.ToString());
  }

  
  //拓展public interface IHtmlHelper<TModel> : IHtmlHelper, 让用户可以直接调用 @Html.VueComponent(string)来输出
  /// <summary>
  /// 输出vue组件
  /// </summary>
  /// <param name="htmlHelper"></param>
  /// <param name="componentName">component的名字</param>
  /// <returns></returns>
  public static async Task<IHtmlContent> UseVueComponent(this IHtmlHelper htmlHelper, string componentName)
  {
    var filePath = SearchVueFile(componentName);
    return await htmlHelper.LoadVueComponent(filePath);
  }
}