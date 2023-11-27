import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ConfigService} from './config/config.service';
import {MerchantInfo} from '../models/MerchantInfo';
import {Observable} from 'rxjs';
import {LimitRegister} from '../models/LimitRegister';

@Injectable()
export class MerchantService {
    private readonly _apiPrefix: string;

    constructor(private _http:HttpClient, private _config: ConfigService) {
        this._apiPrefix = _config.ApiServicePrefix + "/Merchant";
    }

    insertMerchant(data: MerchantInfo) : Observable<number> {
        return this._http.post<number>(`${this._apiPrefix}/Insert`, data);
    }

    update(data: MerchantInfo) : Observable<number> {
        return this._http.post<number>(`${this._apiPrefix}/Update`, data);
    }

    selectMerchant(mid: string) : Observable<MerchantInfo> {
        return this._http.get<MerchantInfo>(`${this._apiPrefix}/Select?mid=${mid}`);
    }

    registerLimit(data: LimitRegister) : Observable<any>{
        return this._http.post<any>(`${this._apiPrefix}/RegisterLimit`, data);
    }
 }
