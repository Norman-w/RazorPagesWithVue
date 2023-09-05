<template type="text/html" id="light">
  <div class="light" :class="{on: lightOn}" style="border: 1px solid red" @click="toggleLight">
    All on? {{ allOn }}
  </div>
</template>


<script>
let light = {
  template: "#" + "light",
  props: ["allOn", "on-light-click"],
  data() {
    return {
      lightOn: false
    }
  },
  methods: {
    toggleLight() {
      this.lightOn = !this.lightOn;
      this.$emit("on-light-click");
    }
  }
}
Vue.component("light", light);
export default light;
</script>


<style scoped>
.light {
  width: 100px;
  height: 100px;
  background-color: darkgray;
  border-radius: 50%;
  border: 1px solid black;
  margin: 10px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-content: center;
  text-align: center;
}

.light.on {
  background-color: lightblue;
}
</style>