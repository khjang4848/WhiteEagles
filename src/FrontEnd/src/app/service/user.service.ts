import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {ConfigService} from "./config/config.service";
import {AccountInquiry} from "../models/AccountInquiry";
import {Observable} from "rxjs";
import {AccountResponse} from "../models/AccountResponse";
import {Withdrawal} from "../models/Withdrawal";
import {WithdrawalResponse} from "../models/WithdrawalResponse";

@Injectable()
export class UserService {
    private readonly _apiPrefix: string;

    constructor(private _http: HttpClient, private _config: ConfigService) {
        this._apiPrefix = _config.ApiServicePrefix + "/User";
    }

    selectAccountInquiry(data: AccountInquiry) : Observable<AccountResponse> {
        return this._http.post<AccountResponse>(`${this._apiPrefix}/AccountInquiry`, data);
    }

    withdrawal(data: Withdrawal) : Observable<WithdrawalResponse> {
        return this._http.post<WithdrawalResponse>(`${this._apiPrefix}/Withdrawal`, data);
    }
}
