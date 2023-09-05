<template type="text/html" id="lights-container">
<!--  动态使用props的border颜色-->
  <div class="light-group" :style="{borderColor:color}">
    <sun :color="allOn ? 'red' : 'yellow'" v-on:on-sun-click="onLightClick"></sun>
    <light v-for="i in 3" :key="i" :allOn="allOn" v-on:on-light-click="onLightClick" ></light>
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
    }
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