<template type="text/html" id="lights-container">
  <div class="light-group">
    <light v-for="i in 3" :key="i" :allOn="allOn" v-on:on-light-click="onLightClick" ></light>
    <button @click="toggleAll">Toggle All</button>
  </div>
</template>


<script>
let comp = {
  template: "#" + "lights-container",
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