// Author: Norman
// Create: Sep.05 2023
// Description:
// 为了让用户可以在cshtml中使用vue组件更方便且更易维护,构造此类

// 使用分为3步
// 引用: @Html.UseVueComponent("xxx")
// 锚点: 如 <div id="aaa"></div>
// 使能: @Html.EnableVueContainers("aaa")

// 创建vue组件
// 在Views/Shared/VueComponents中创建xxx.vue文件即可, xxx就是上面引用时的xxx

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
  /// 因在script标签中不能使用import,所以需要将import的内容提取出来判断引用,并将import的内容替换掉
  /// </summary>
  /// <param name="scriptContent"></param>
  /// <param name="callerPath"></param>
  /// <returns>返回tag的类型名称和绝对路径的字典</returns>
  private static Dictionary<string, string> GetComponentsUsingDic(ref string scriptContent, string callerPath)
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
      
      //tag的绝对路径
      var tagFullPath = Path.Combine(Path.GetDirectoryName(callerPath) ?? string.Empty, tagPath);
      //如果字典中不存在该标签,则添加
      tagTypeAndPathDic.TryAdd(tagType, tagFullPath);
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
    var usingComponentsFullPathDic = GetComponentsUsingDic(ref thisComponentContent, componentFullPath);
    //通过匹配到的tag类型,确认要使用Html.PartialAsync加载哪些文件
    foreach (var tagComponentTypeAndPath in usingComponentsFullPathDic)
    {
      #region vue文件的路径,通过路径从缓存中取出文件内容,如果缓存中不存在,则加载文件内容
      //根据componentFullPath的路径和 类似于 ./xxx.vue 的路径,拼接出vue文件的路径
      if (BufferedContentDic.TryGetValue(tagComponentTypeAndPath.Value, out var value))
      {
        contentBuilder.Append(value);
        continue;
      }
      #endregion
      #region 如果文件不存在,则抛出异常
      if (!File.Exists(tagComponentTypeAndPath.Value))
      {
        throw new Exception($"文件{tagComponentTypeAndPath.Value}不存在");
      }
      #endregion
      #region 加载对应的文件,并将文件内容存储到缓存中
      var partialContent = await htmlHelper.LoadVueComponent(tagComponentTypeAndPath.Value);
      BufferedContentDic.Add(tagComponentTypeAndPath.Value, partialContent.ToString());
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
  /// <param name="componentNameOrPath">component的名字,或者是路径.如果给定名字,则进行搜索</param>
  /// <returns></returns>
  public static async Task<IHtmlContent> UseVueComponent(this IHtmlHelper htmlHelper, string componentNameOrPath)
  {
    //判断给定的路径是否为相对或者绝对路径而非组件名
    if (componentNameOrPath.Contains('/') || componentNameOrPath.Contains('\\'))
    {
      //如果是路径,则直接加载
      return await htmlHelper.LoadVueComponent(componentNameOrPath);
    }
    //如果是组件名,则搜索
    var filePath = SearchVueFile(componentNameOrPath);
    return await htmlHelper.LoadVueComponent(filePath);
  }
  
  public static IHtmlContent EnableVueContainers(this IHtmlHelper htmlHelper, params string[] containersName)
  {
    if (containersName is null || containersName.Length == 0)
    {
      return HtmlString.Empty;
    }
    var contentBuilder = new StringBuilder();
    contentBuilder.AppendLine("<script>");
    foreach (var containerName in containersName)
    {
      // 类似于 new Vue({el: "#app"});
      contentBuilder.AppendLine($"new Vue({{el: \"#{containerName}\"}});");
    }
    contentBuilder.AppendLine("</script>");
    return new HtmlString(contentBuilder.ToString());
  }
}