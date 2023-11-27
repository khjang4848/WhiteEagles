import { Component, OnInit } from '@angular/core';
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from "@angular/material/core";
import {
    MAT_MOMENT_DATE_ADAPTER_OPTIONS,
    MomentDateAdapter
} from "@angular/material-moment-adapter";
import {FormControl} from "@angular/forms";
import * as _moment from 'moment';
import {Moment} from "moment";
import {MatDatepicker} from "@angular/material/datepicker";
import {CalculateService} from "../../../service/calculate.service";
import {getFormatYearAndMonth} from "../../../../lib/util";
import {CalculateInfo} from "../../../models/CalculateInfo";

const moment = _moment;

export const MY_FORMATS = {
    parse: {
        dateInput: 'YYYY-MM',
    },
    display: {
        dateInput: 'YYYY-MM',
        monthYearLabel: 'MMM YYYY',
        dateA11yLabel: 'LL',
        monthYearA11yLabel: 'MMMM YYYY',
    }
};
@Component({
    selector: 'app-calculatelist',
    templateUrl: './calculatelist.component.html',
    styleUrls: ['./calculatelist.component.css'],
    providers: [
        {
            provide: DateAdapter,
            useClass: MomentDateAdapter,
            deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
        },
        {provide: MAT_DATE_FORMATS, useValue: MY_FORMATS},
    ]
})
export class CalculatelistComponent implements OnInit {
    startDate = new FormControl(moment());
    endDate = new FormControl(moment());
    startDateText: string;
    endDateText: string;
    mid: string;
    calculateList: CalculateInfo[] = [];
    selected: boolean[];


    constructor(private _calculateService: CalculateService) {
        console.log(this.startDate.value._d);
        this.startDateText = getFormatYearAndMonth(this.startDate.value._d);
        this.endDateText = getFormatYearAndMonth(this.endDate.value._d);
        this.mid = "";
    }

    ngOnInit(): void {
    }

    chosenStartYearHandler(normalizedYear: Moment) {
        const ctrlValue = this.startDate.value;
        ctrlValue.year(normalizedYear.year());
        this.startDate.setValue(ctrlValue);
        this.startDateText = getFormatYearAndMonth(this.startDate.value._d);
    }

    chosenStartMonthHandler(normalizedMonth: Moment, datepicker: MatDatepicker<Moment>) {
        const ctrlValue = this.startDate.value;
        ctrlValue.month(normalizedMonth.month());
        this.startDate.setValue(ctrlValue);
        this.startDateText = getFormatYearAndMonth(this.startDate.value._d);
        datepicker.close();
    }

    chosenEndYearHandler(normalizedYear: Moment) {
        const ctrlValue = this.endDate.value;
        ctrlValue.year(normalizedYear.year());
        this.endDate.setValue(ctrlValue);
        this.endDateText = getFormatYearAndMonth(this.endDate.value._d);
    }

    chosenEndMonthHandler(normalizedMonth: Moment, datepicker: MatDatepicker<Moment>) {
        const ctrlValue = this.endDate.value;
        ctrlValue.month(normalizedMonth.month());
        this.endDate.setValue(ctrlValue);
        this.endDateText = getFormatYearAndMonth(this.endDate.value._d);
        datepicker.close();
    }

    search() {
        this._calculateService.selectCalculateInfo(this.startDateText, this.endDateText, this.mid)
            .subscribe(data => {
                this.calculateList = data;
                this.selected = Array.from(new Array(data.length), (x, i) => false);
            });
    }

    excel() {
        console.log(this.selected);
    }

    onchangeMonth(number: number) {
        this.startDate =  new FormControl(moment().add(number, 'M'));
        this.startDateText = getFormatYearAndMonth(this.startDate.value._d);
        this.endDate = new FormControl(moment());
        this.endDateText = getFormatYearAndMonth(this.endDate.value._d);
    }
}
