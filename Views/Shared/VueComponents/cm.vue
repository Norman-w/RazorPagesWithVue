<template type="text/html" id="coffee-maker">
    <div>
        <Light></Light>
        <div class="row">
            <div class="col-sm">
                <img 
                    @click="startMakingCoffee"
                    src="coffee-machine.png" 
                    alt="Coffee Machine" 
                    class="coffee-machine"
                    >
                    <progress-bar :percent="percent"></progress-bar>
            </div>
        </div>
        <img 
            v-for="n in numberOfCoffeesMade"
            :key="n"
            src="coffee.png" 
            alt="Coffee" 
            class="coffee">
    </div>
</template>

<script type="module">
    import light from "./light.vue";

    let name = "coffee-maker";
    let comp = {
      template: "#" + name,
      components:{
         "light" : light,
      },
      data() {
        return {
          percent: 0,
          numberOfCoffeesMade: 0,
          interval: null
        }
      },
      computed: {
        progressBarWidth() {
          return `${this.progressBarValue}%`
        }
      },
      methods: {
        startMakingCoffee() {
          if (this.interval) {
            clearInterval(this.interval);
          }

          this.percent = 0;

          this.interval = setInterval(() => {
            if (this.percent >= 100) {
              this.numberOfCoffeesMade++;
              clearInterval(this.interval);
            }
            this.percent += 5;
          }, 25);

        }
      }
    }
    Vue.component(name, comp);
    export default comp;
</script>

<style>
    .coffee-machine,
    .progress {
        width: 150px;
    }

    .coffee {
        width: 50px;
    }
</style>