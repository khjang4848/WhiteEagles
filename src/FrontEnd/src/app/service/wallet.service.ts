import {Injectable} from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {ConfigService} from './config/config.service';
import {WalletHistoryRequest} from '../models/WalletHistoryRequest';
import {Observable} from 'rxjs';
import {WalletHistoryResult} from '../models/WalletHistoryResult';
import {LimitList} from "../models/LimitList";
import {LimitRegisterViewModel} from "../models/LimitRegisterViewModel";
import {LimitRegister} from "../models/LimitRegister";

@Injectable()
export class WalletService {
    private readonly _apiPrefix: string;
    private readonly _apiPrefix2: string;

    constructor(private _http: HttpClient, private _config: ConfigService) {
        this._apiPrefix = _config.ApiServicePrefix + "/Transfer";
        this._apiPrefix2 = _config.ApiServicePrefix + "/Wallet"
    }

    selectWalletHistory(data: WalletHistoryRequest) : Observable<Array<WalletHistoryResult>> {
        return this._http.post<Array<WalletHistoryResult>>(`${this._apiPrefix}/WalletHistory`, data);
    }

    selectWalletHistoryExcel(data: WalletHistoryRequest) : Observable<any> {
        return this._http.get(`${this._apiPrefix}/WalletHistoryExcel?startDate=${data.StartDate}&endDate=${data.EndDate}&mid=${data.Mid}&remiName=${data.RemiName}&inAccountNo=${data.InAccountNo}&bankCode=${data.BankCode}&transferStatus=${data.TransferStatus}`, {responseType:'blob'});
    }

    selectWalletHistoryCount(data: WalletHistoryRequest) : Observable<number> {
        return this._http.post<number>(`${this._apiPrefix}/WalletHistoryCount`, data);
    }

    selectAvailableLimit(mid: string) : Observable<LimitRegisterViewModel> {
        return this._http.get<LimitRegisterViewModel>(`${this._apiPrefix2}/SelectAvailableLimit?mid=${mid}`);
    }

    selectAvailableLimitAll(mid: string) : Observable<Array<LimitList>> {
        return this._http.get<Array<LimitList>>(`${this._apiPrefix2}/SelectAvailableLimitAll?mid=${mid}`);
    }

    registerLimit(data: LimitRegister) : Observable<void> {
        return this._http.post<void>(`${this._apiPrefix2}/LimitRegister`, data);
    }
}
