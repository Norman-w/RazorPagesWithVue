<template type="text/html" id="lights-container">
<!--  动态使用props的border颜色-->
  <div class="light-group" :style="{borderColor:color}">
    <div>Title:{{ title }}</div>
    <div>Room:{{room}}</div>
    <div>User:{{user}}</div>
    <sun :color="allOn ? 'red' : 'yellow'" v-on:on-sun-click="onLightClick"></sun>
    <light v-for="i in 3" 
           :key="i" 
           :allOn="allOn" 
           v-on:on-light-click="onLightClick"
           style="height: 20px"
    ></light>
    <small-light></small-light>
    <button @click="toggleAll">Toggle All</button>
  </div>
</template>

<script>
import smallLight from "./smaller/light.vue";
import light from "./light.vue";
import sun from "./sun.vue";
let comp = {
  template: "#" + "lights-container",
  // 这种可以
  // props: ["color"],
  // 这种也可以
  props: {
    color: {
      type: String,
      default: "blue"
    },
    user: {
      type: Object,
      default: ()=>{
        let age = 18;
        let name = "default name";
        return {age,name};
      }
    },
    room: {
      type: Object,
      default: ()=> {}
    },
    title: {
      type: String,
      default: "DEFAULT TITLE"
    },
  },
  components: {
    smallLight:smallLight,
    light:light,
    sun:sun
  },
  data() {
    return {
      allOn: false,
    }
  },
  methods: {
    toggleAll() {
      this.allOn = !this.allOn;
    },
    onLightClick() {
      console.log("light clicked");
      this.allOn = false;
    }
  },
  mounted() {
    console.log("组件sun加载完成");
    console.log("props传递的颜色值:",this.color);
    console.log('传入的user是:', this.user);
    console.log('user的name是:', this.user.name);
    console.log('传入的room是:',this.room);
    console.log('因为user是在传入时,直接使用@输出的字符串,所以vue将其解析出object\n' +
        '而room则是挂载到了vue的容器组件上,然后vue的子组件使用时获取过去的');
  }
}
Vue.component("lights-container", comp);
export default comp;
</script>


<style scoped>
.light-group {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100%;
  border: 1px solid blue;
}
</style>