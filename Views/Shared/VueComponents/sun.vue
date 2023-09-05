<template type="text/html" id="sun">
  <div>
    <button @click="moveSun">Move Sun</button>
<!--    点一下移动一点-->
<!--    <div class="sun" :style="{transform: 'translateX(' + offset + 'px)'}"></div>-->
<!--    通过检测开关量,来检测是否移动-->
    
    
<!--    这样写有代码提示-->
    <div class="sun" :class="{sunMove:keepMove}" :style="{backgroundColor: color}" @click="moveSun">
      {{keepMove}}
    </div>
    
<!--    这样写是没有类的代码提示的-->
<!--    <div :class="keepMove === true ? 'sun sunMove' : 'sun'">-->
<!--      <div class="bigText">{{keepMove}}</div>-->
<!--    </div>-->
  </div>
</template>


<script>
let sun = {
  template: "#" + "sun",
  props: ["color", "on-sun-click"],
  data() {
    return {
      offset: 0,
      keepMove: false
    }
  },
  methods: {
    //点一下走一点
    // moveSun() {
    //   this.offset += 10;
    //   this.$emit("on-light-click", this.offset);
    // }
    
    //持续移动模式
    moveSun() {
      this.keepMove = !this.keepMove;
    }
  }
}
export default sun;
</script>


<style>
.sun {
  width: 50px;
  height: 50px;
  background-color: yellow;
  box-shadow: 10px 10px 10px 10px rgba(0, 0, 0, 0.5);
  border-radius: 50%;
  margin: 20px auto 0;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
}
/*不要再这里写两个斜杠的注释!!!!!!*/
.sunMove {
  box-shadow: 10px 10px 10px 10px rgba(0, 0, 0, 0.5);
  background-color: lightgreen;
  animation: rotate 2s infinite linear;
}
@keyframes rotate {
  from {transform: rotate(0deg);}
  to {transform: rotate(360deg);}
}
</style>