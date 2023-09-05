# 在razor的cshtml中使用vue

### 添加一个组件的方式:
1. 在Views/Shared/VueComponents中添加两个文件,一个是cshtml,一个是vue,两个文件名建议同名.比如:Header.cshtml,Header.vue
2. 在cshtml中添加类似如下的代码:
```@using RazorPagesWithVue.Views.Shared.VueComponents
    @{
        //引用组件,假设被引用的组件叫做Avatar
        @(await Html.PartialAsync("VueComponents/avatar"))//是avatar.cshtml的简写
        //渲染自身(使用扩展方法把vue组件渲染成html中的标签)
        @(await Html.VueComponent("Header"))
    }
``` 
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
    这些内容尽量不要改格式.但是Header,comp,xxx,zzz这些自己根据情况修改.
    ##### 必须指定一个 ```<style>```标签,可以有附加的参数如scoped,lang等,在这里面写vue的css代码