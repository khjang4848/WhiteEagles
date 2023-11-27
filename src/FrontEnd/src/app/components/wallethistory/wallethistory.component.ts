import {Component, OnInit, ViewChild} from '@angular/core';
import {WalletHistoryResult} from "../../models/WalletHistoryResult";
import {WalletHistoryRequest} from "../../models/WalletHistoryRequest";
import {WalletService} from "../../service/wallet.service";
import {getFormatDate, getFormatYearAndMonth} from "../../../lib/util";
import {MatTableDataSource} from "@angular/material/table";
import {MatSort} from "@angular/material/sort";
import {MatPaginator} from "@angular/material/paginator";
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from "@angular/material/core";
import {
    MAT_MOMENT_DATE_ADAPTER_OPTIONS,
    MomentDateAdapter
} from "@angular/material-moment-adapter";
import {FormControl} from "@angular/forms";
import * as _moment from 'moment';
import {Moment} from "moment";

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
    selector: 'app-wallethistory',
    templateUrl: './wallethistory.component.html',
    styleUrls: ['./wallethistory.component.css'],
    providers: [
        {provide: MAT_DATE_FORMATS, useValue: MY_FORMATS},
        {
            provide: DateAdapter,
            useClass: MomentDateAdapter,
            deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
        }
    ]
})
export class WallethistoryComponent implements OnInit {
    walletHistory: WalletHistoryResult[] = [];
    walletRequest: WalletHistoryRequest;
    rowCount: number;
    start = new FormControl(moment());
    end = new FormControl(moment());
    startDateText: string;
    endDateText: string;

    dataSource = new MatTableDataSource<WalletHistoryResult>(this.walletHistory);
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild("myTable") table: any;

    constructor(private _walletService: WalletService) {
        this.walletRequest = {
            Mid: "",
            StartDate: "",
            EndDate: "",
            RemiName: "",
            InAccountNo: "",
            BankCode: "",
            TransferStatus: "",
            Page: 0,
            PageCount: 20
        };

        this.startDateText = getFormatDate(this.start.value._d);
        this.endDateText = getFormatDate(this.end.value._d);
    }

    ngOnInit(): void {
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
    }

    changeDay(day: number) {
        this.start.setValue(moment().add(day, 'days'));
        this.end.setValue(moment());
        this.startDateText = getFormatDate(this.start.value._d);
        this.endDateText = getFormatDate(this.end.value._d);
    }


    searchData() {
        console.log(this.start);

        this.walletRequest.StartDate = getFormatDate(this.start.value._d);
        this.walletRequest.EndDate = getFormatDate(this.end.value._d);
        this.walletRequest.Page = 0;
        this.paginator.pageIndex = 0;

        this._walletService.selectWalletHistory(this.walletRequest)
            .subscribe(data => this.walletHistory = data);

        this._walletService.selectWalletHistoryCount(this.walletRequest)
            .subscribe(data => this.rowCount = data);
    }

    searchExcel() {
        this._walletService.selectWalletHistoryExcel(this.walletRequest)
            .subscribe(data => {
                window.open(window.URL.createObjectURL(data));
            });
    }

    onPageChanged(event) {
        this.walletRequest.Page = event.pageIndex;
        this._walletService.selectWalletHistory(this.walletRequest)
            .subscribe(data => this.walletHistory = data);

    }
}
