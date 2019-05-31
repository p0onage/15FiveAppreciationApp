<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <HelloWorld :msg='msg'/>
    <h1>{{highFives[0].message}}</h1>
    <button @click="removeItem()">Next</button>
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

   public removeItem(){
     this.highFives.shift();
   }

  public async created() {
    this.highFives = await container.get<IAppreciationAPIService>(AppreciationAPIService).returnHighFives();
  }
}
</script>
