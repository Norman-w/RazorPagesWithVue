<template type="text/html" id="smallLight">
  <div class="light" :class="{on: lightOn}" style="border: 1px solid seagreen" @click="toggleLight">
    All on? {{ allOn }}
  </div>
</template>


<script itemscope>
let smallLight = {
  template: "#" + "smallLight",
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
// Vue.component("small-light", smallLight);
export default smallLight;
</script>


<style scoped>
.light {
  width: 100px;
  height: 100px;
  background-color: darkslategray;
  border-radius: 50%;
  border: 1px solid black;
  margin: 10px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-content: center;
  text-align: center;
  user-select: none;
  cursor: pointer;
}

.light.on {
  background-color:lightgreen;
}
</style>