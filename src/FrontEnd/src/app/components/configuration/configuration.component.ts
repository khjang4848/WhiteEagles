import { Component, OnInit } from '@angular/core';
import {MerchantService} from "../../service/merchant.service";
import {ActivatedRoute} from "@angular/router";
import {MerchantInfo} from "../../models/MerchantInfo";
import {getConvertDate, getFormatDate, getFormatDateDash} from "../../../lib/util";
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from "@angular/material/core";
import {
    MAT_MOMENT_DATE_ADAPTER_OPTIONS,
    MomentDateAdapter
} from "@angular/material-moment-adapter";
import {FormControl} from "@angular/forms";
import * as _moment from 'moment';

const moment = _moment;

export const MY_FORMATS = {
    display: {
        dateInput: 'YYYY-MM-DD',
        monthYearLabel: 'MMM YYYY',
        dateA11yLabel: 'LL',
        monthYearA11yLabel: 'MMMM YYYY'
    }
}

@Component({
    selector: 'app-configuration',
    templateUrl: './configuration.component.html',
    styleUrls: ['./configuration.component.css'],
    providers: [
        {provide: MAT_DATE_FORMATS, useValue: MY_FORMATS},
        {
            provide: DateAdapter,
            useClass: MomentDateAdapter,
            deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
        }
    ]
})
export class ConfigurationComponent implements OnInit {
    start = new FormControl(moment());
    mid : string;
    merchantInfo : MerchantInfo;
    rowCount: number = 0;

    constructor(private _router: ActivatedRoute,
        private _merchantService: MerchantService) {
        this.mid = "A21012001m";
        this.merchantInfo = {
            MerchantId: this.mid,
            MerchantCode: this.mid,
            ServiceUse: "N",
            ServiceStartDate: "",
            SuspenseReceipts: 0,
            FeeType: "1",
            MinFee: 0,
            MinLimit: 0,
            FeeByCase: 0,
            MonthBaseFee: 0,
            CalculateDay: 1
        };
        this.searchMerchantInfo();
    }

    ngOnInit(): void {
    }

    searchMerchantInfo() {
        if(!this.mid) return;
        this._merchantService.selectMerchant(this.mid)
            .subscribe(data => {
                if(data) {
                    this.merchantInfo = data;
                    this.rowCount = 1;
                    this.start.setValue(moment(this.merchantInfo.ServiceStartDate));
                }
                else {
                    this.rowCount = 0;
                }
            });
    }

    save() {

        this.merchantInfo.ServiceStartDate = getFormatDateDash(this.start.value._d) + " 00:00:00";

        if(!this.merchantInfo.SuspenseReceipts) {
            alert("선수금 금액은 최소 0원입니다");
            return;
        }
        if(this.merchantInfo.SuspenseReceipts < 0) {
            alert("선수금 금액은 0원보다 커야 됩니다");
            return;
        }

        if(this.merchantInfo.SuspenseReceipts < 0) {
            alert("선수금 금액은 0원보다 커야 됩니다");
            return;
        }

        if (!confirm("변경된 정보를 저장하시겠습니다"))
            return;

        if(this.rowCount == 0) {
            this._merchantService.insertMerchant(this.merchantInfo)
                .subscribe(() => alert("저장되었습니다"));
        }
        else {
            this._merchantService.update(this.merchantInfo)
                .subscribe(() => alert("저장되었습니다"));
        }
    }

}
