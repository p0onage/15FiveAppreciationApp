import IAppreciation from './interfaces/IAppreciation';


export default class Appreciation implements IAppreciation {
    public Message: string = "";
    public Username: string = "";
    public Index: number = 0;
}