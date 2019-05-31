<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <HelloWorld :msg='msg'/>
    <button v-on:click="this.getHighFives">Call HighFives</button>
    <li v-for="highFive in highFives" :key="highFive">
      <ul>{{highFive.message}}</ul>
    </li>
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
  private url = "https://localhost:44343/api/appreciation"
  //private AppreciationAPIService = Axios.get<IAppreciationAPIService>(this.url).then(response => console.log(response));
  public async getHighFives() {
    this.highFives = await container.get<IAppreciationAPIService>(AppreciationAPIService).returnHighFives();
  };
}
</script>
