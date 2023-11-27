import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {ConfigService} from "./config/config.service";
import {Observable} from "rxjs";
import {CalculateInfo} from "../models/CalculateInfo";

@Injectable()
export class CalculateService {
    private readonly _apiPrefix: string;

    constructor(private _http: HttpClient, private _config: ConfigService) {
        this._apiPrefix = _config.ApiServicePrefix + "/Calculate";
    }

    selectCalculateInfo(startDate: string, endDate: string, mid: string) : Observable<Array<CalculateInfo>> {
        return this._http.get<Array<CalculateInfo>>(`${this._apiPrefix}/SelectCalculateInfo?startDate=${startDate}&endDate=${endDate}&mid=${mid}`);
    }

    selectCalculateAllInfo(startDate: string, endDate: string, mid: string) : Observable<Array<CalculateInfo>> {
        return this._http.get<Array<CalculateInfo>>(`${this._apiPrefix}/SelectCalculateInfoAll?startDate=${startDate}&endDate=${endDate}&mid=${mid}`);
    }
}
