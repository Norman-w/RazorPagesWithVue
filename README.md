# 在razor的cshtml中使用vue

### 添加一个组件的方式:
1. 在View/Shared/VueComponent下创建Header.vue文件
2. 编写Header.vue组件,在头部通过正常import引入vue,并且在尾部通过export default导出组件
3. 在vue中编写vue代码.但是要注意格式:
    ##### 必须指定一个 ```<template type="text/html" id="Header">``` 标签,除了id值以外其他的不要修改,在这里面写vue的模板代码
    ##### 必须指定一个 ```<script>```标签且不要设置type为module,在这里面写vue的js代码
    #####  为了保证vue的js代码能够正常运行,里面的 
    `let comp = {
    template: "#" + "Header",
    data() {
    return {
    xxx: false
    }
    },
    methods: {
    updateXxx() {
    this.xxx = !this.xxx;
    }
    },
    computed: {
    zzz() {
    return this.xxx;
    }
    }
    }
    Vue.component("Header", comp);
    export default comp;`
    这些内容尽量不要改格式.但是Header,comp,xxx,zzz这些自己根据情况修改.但是要注意comp不能重命,且最好是组件的名字,否则有加载不出来的情况
    ##### 必须指定一个 ```<style>```标签,可以有附加的参数如scoped,lang等,在这里面写vue的css代码,但是请注意,??一定不要再style中使用双斜杠注释,否则会导致vue的css代码无法正常运行.如果需要注释,请使用/* */注释(有的编译器不会提示//注释的错误,所以请注意)


在cshtml中使用vue组件的步骤.

引入组件,如  @(await Html.UseVueComponent("lights-container"))
定义一个锚点,如  <div id="app"></div>
在锚点内使用组件,如 <lights-container></lights-container>
使用 @Html.EnableVueContainers("app") 以使能vue组件的挂载
或者使用 @Html.EnableVueContainer("app", parametersDic)并传入C#参数
三步的顺序不能差,即先声明后使用,最后使能

cshtml向vue传递参数见Index.html里的示例
