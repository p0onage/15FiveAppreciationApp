import 'reflect-metadata';
import { Container } from 'inversify';
import AppreciationAPIService from './services/AppreciationAPIService';

const container = new Container();
// container.bind<StepManagerService>(StepManagerService).to(StepManagerService);
container
  .bind<AppreciationAPIService>(AppreciationAPIService)
  .to(AppreciationAPIService);

export default container;
