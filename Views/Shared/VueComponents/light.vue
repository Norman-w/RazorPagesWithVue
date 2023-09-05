<template type="text/html" id="light">
    <div class="light" :class="{on: lightOn}" style="border: 1px solid red" @click="toggleLight"></div>
</template>

<script>
let lightName = "light";
let lightComp = {
  template: "#" + lightName,
  data() {
    return {
      lightOn: false
    }
  },
  methods: {
    toggleLight() {
      this.lightOn = !this.lightOn;
    }
  }
}
Vue.component(lightName, lightComp);
export default lightComp;
</script>

<style scoped>
.light {
    width: 100px;
    height: 100px;
    background-color: darkgray;
    border-radius: 50%;
    border: 1px solid black;
    margin: 10px;
}
.light.on {
    background-color: lightblue;
}
</style>