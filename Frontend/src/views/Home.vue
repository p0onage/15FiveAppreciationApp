<template>
<div>
  <link href="https://fonts.googleapis.com/css?family=Lato" rel="stylesheet">
    <header class="header">
    <div class="header__column">
      <h2 class="header__header">Most High Fives Given</h2>
      <h2>Mark Boyle</h2>
      <h2>5</h2>
    </div>
    <div class="header__column header__logo"><img width="238" src="../assets/tla-logo.png"></div>
    <div class="header__column">
      <h2 class="header__header">Most High Fives Recieved</h2>
      <h2>Alan How</h2>
      <h2>7</h2>
    </div>
</header>
  <div class="container">
    <img alt="Vue logo" src="../assets/logo.png">
    <h1>{{highFives[0].message}}</h1><br/>
    <h1>From: {{highFives[0].username}}</h1>
    <button class="button" @click="removeItem()">Good job!</button>
    <div class="total-high-fives">
      <h2 class="header__header">Total High Fives</h2>
      <h2>{{hightFiveAmount}}</h2>
    </div>
  </div>
</div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import HelloWorld from '@/components/HelloWorld.vue'; // @ is an alias to /src
import Axios from 'axios';
import IAppreciationAPIService from '../services/Interfaces/IAppreciationAPIService';
import AppreciationAPIService from '../services/AppreciationAPIService';
import container from '../inversify.config';
import Appreciation from '@/models/Appreciation';

@Component
export default class Home extends Vue {
  private highFives: Appreciation[] = [];
  private hightFiveAmount: number = 0;
  private url = "https://localhost:44343/api/appreciation"
  //private AppreciationAPIService = Axios.get<IAppreciationAPIService>(this.url).then(response => console.log(response));

  public removeItem(){
    this.highFives.shift();
  }

  public shuffleHighFives() {
  for (let i = this.highFives.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [this.highFives[i], this.highFives[j]] = [this.highFives[j], this.highFives[i]];
    }

  return this.highFives;
}

  public async created() {
    this.highFives = await container.get<IAppreciationAPIService>(AppreciationAPIService).returnHighFives();
    this.shuffleHighFives();
    this.hightFiveAmount = this.highFives.length;
  }
}
</script>

<style lang="scss" scoped>
  @import '../styles/main.scss';
</style>
